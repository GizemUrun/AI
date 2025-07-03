using UnityEngine;
using System.Collections.Generic;
using UnityEditor; // Required for Handles.Label

public enum SocketType
{
    Floor,
    WallBack,
    WallFront,
    Void
}

[System.Serializable]
public class Tile
{
    public string name;
    public GameObject prefab;
    public List<SocketType> sockets;
    public float weight = 1.0f;  // New attribute to define tile weight (default: 1)

    public Tile(string name, GameObject prefab, List<SocketType> sockets, float weight = 1.0f)
    {
        this.name = name;
        this.prefab = prefab;
        this.sockets = sockets;
        this.weight = weight;
    }
}

public class DungeonGenerator : MonoBehaviour
{
    // List of tiles to be defined in the inspector
    public List<Tile> tiles;

    // Grid dimensions
    public int gridWidthX = 5;
    public int gridWidthZ = 7;

    // Spacing between tiles
    public float tileSize = 5.0f;

    // Compatibility matrix
    public bool[,] socketCompatibilityMatrix;

    // 2D array representing the grid of cells
    private GridCell[,] grid;

    void Start()
    {
        InitializeCompatibilityMatrix();
        InitializeGrid();
        GenerateDungeon();
    }

    // Initialize the compatibility matrix with default values if not set
    public void InitializeCompatibilityMatrix()
    {
        int socketTypeCount = System.Enum.GetValues(typeof(SocketType)).Length;

        if (socketCompatibilityMatrix == null || socketCompatibilityMatrix.GetLength(0) != socketTypeCount)
        {
            socketCompatibilityMatrix = new bool[socketTypeCount, socketTypeCount];

            // Optionally, set default compatibility here
            // For example, make all sockets compatible with themselves
            for (int i = 0; i < socketTypeCount; i++)
            {
                socketCompatibilityMatrix[i, i] = true;
            }
        }
    }

    // Data structure for a cell in the grid
    public class GridCell
    {
        public List<Tile> possibleTiles;
        public int entropy;

        public GridCell(List<Tile> tiles)
        {
            this.possibleTiles = new List<Tile>(tiles);
            this.entropy = tiles.Count;
        }
    }

    void InitializeGrid()
    {
        grid = new GridCell[gridWidthX, gridWidthZ];

        for (int x = 0; x < gridWidthX; x++)
        {
            for (int z = 0; z < gridWidthZ; z++)
            {
                grid[x, z] = new GridCell(tiles);
            }
        }
    }

    void GenerateDungeon()
    {
        while (!AllCellsCollapsed())
        {
            CollapseCell();
        }
    }

    bool AllCellsCollapsed()
    {
        foreach (GridCell cell in grid)
        {
            if (cell.entropy != -1)
                return false;
        }
        return true;
    }

    // Weighted random selection function
    Tile WeightedRandomSelect(List<Tile> tiles)
    {
        // Calculate the total weight of all the tiles
        float totalWeight = 0f;
        foreach (Tile tile in tiles)
        {
            totalWeight += tile.weight;
        }

        // Generate a random number between 0 and totalWeight
        float randomValue = Random.Range(0f, totalWeight);

        // Select the tile based on the random number
        float cumulativeWeight = 0f;
        foreach (Tile tile in tiles)
        {
            cumulativeWeight += tile.weight;

            // If the random value is less than the cumulative weight, select the tile
            if (randomValue <= cumulativeWeight)
            {
                return tile;
            }
        }

        return tiles[tiles.Count - 1];  // Fallback to the last tile if something goes wrong
    }

    void CollapseCell()
    {
        int minEntropy = int.MaxValue;
        List<Vector2Int> minEntropyCells = new List<Vector2Int>();

        // Find cells with the minimum entropy
        for (int x = 0; x < gridWidthX; x++)
        {
            for (int z = 0; z < gridWidthZ; z++)
            {
                GridCell cell = grid[x, z];
                int currentEntropy = cell.entropy;

                if (currentEntropy == -1)
                    continue; // Skip already collapsed cells

                if (currentEntropy < minEntropy)
                {
                    minEntropy = currentEntropy;
                    minEntropyCells.Clear();
                    minEntropyCells.Add(new Vector2Int(x, z));
                }
                else if (currentEntropy == minEntropy)
                {
                    minEntropyCells.Add(new Vector2Int(x, z));
                }
            }
        }

        if (minEntropyCells.Count == 0)
        {
            Debug.LogError("No cells to collapse.");
            return;
        }

        // Randomly select one of the cells with minimum entropy
        Vector2Int selectedCellCoords = minEntropyCells[Random.Range(0, minEntropyCells.Count)];
        GridCell selectedCell = grid[selectedCellCoords.x, selectedCellCoords.y];

        // Update possible tiles for this cell based on already collapsed neighbors
        List<Tile> compatibleTiles = new List<Tile>(selectedCell.possibleTiles);

        // Directions for adjacent cells (North, East, South, West)
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0, -1),  // North (-z)
            new Vector2Int(-1, 0),  // East (-x)
            new Vector2Int(0, 1),   // South (+z)
            new Vector2Int(1, 0)    // West (+x)
        };

        // Corresponding opposite directions
        int[] oppositeDirections = new int[] { 2, 3, 0, 1 };

        for (int i = 0; i < directions.Length; i++)
        {
            Vector2Int dir = directions[i];
            int neighborX = selectedCellCoords.x + dir.x;
            int neighborY = selectedCellCoords.y + dir.y;

            // Skip out-of-bounds cells
            if (neighborX < 0 || neighborX >= gridWidthX || neighborY < 0 || neighborY >= gridWidthZ)
                continue;

            GridCell neighborCell = grid[neighborX, neighborY];

            // Only consider collapsed neighboring cells
            if (neighborCell.entropy != -1)
                continue;

            Tile neighborTile = neighborCell.possibleTiles[0]; // Since it's collapsed, it has only one tile

            // Filter compatible tiles
            List<Tile> newCompatibleTiles = new List<Tile>();
            foreach (Tile tile in compatibleTiles)
            {
                SocketType mySocket = tile.sockets[i]; // My socket in direction i
                SocketType neighborSocket = neighborTile.sockets[oppositeDirections[i]]; // Neighbor's socket in opposite direction

                if (AreSocketsCompatible(mySocket, neighborSocket))
                {
                    newCompatibleTiles.Add(tile);
                }
            }

            compatibleTiles = newCompatibleTiles;

            // If no compatible tiles are left, we have a contradiction
            if (compatibleTiles.Count == 0)
            {
                Debug.LogError($"Contradiction found at cell ({selectedCellCoords.x}, {selectedCellCoords.y})");
                return;
            }
        }

        // Randomly select one of the compatible tiles for this cell using weighted random selection
        Tile selectedTile = WeightedRandomSelect(compatibleTiles);

        // Collapse the cell
        selectedCell.possibleTiles = new List<Tile> { selectedTile };
        selectedCell.entropy = -1; // Mark as collapsed

        // Place the tile in the scene
        Vector3 position = new Vector3(selectedCellCoords.x * tileSize, 0, selectedCellCoords.y * tileSize);
        Instantiate(selectedTile.prefab, position, Quaternion.identity);

        // Start constraint propagation
        PropagateConstraints(selectedCellCoords.x, selectedCellCoords.y);
    }
    // New Method for Constraint Propagation
    void PropagateConstraints(int x, int y)
    {
        Queue<Vector2Int> cellsToProcess = new Queue<Vector2Int>();
        cellsToProcess.Enqueue(new Vector2Int(x, y));

        // Directions for adjacent cells (North, East, South, West)
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0, -1),  // North (-z), sockets[0]
            new Vector2Int(-1, 0),  // East (-x), sockets[1]
            new Vector2Int(0, 1),   // South (+z), sockets[2]
            new Vector2Int(1, 0)    // West (+x), sockets[3]
        };

        // Opposite directions
        int[] oppositeDirections = new int[] { 2, 3, 0, 1 };

        while (cellsToProcess.Count > 0)
        {
            Vector2Int cellCoords = cellsToProcess.Dequeue();
            GridCell cell = grid[cellCoords.x, cellCoords.y];

            // For each direction
            for (int i = 0; i < directions.Length; i++)
            {
                Vector2Int dir = directions[i];
                int neighborX = cellCoords.x + dir.x;
                int neighborY = cellCoords.y + dir.y;

                // Skip out-of-bounds cells
                if (neighborX < 0 || neighborX >= gridWidthX || neighborY < 0 || neighborY >= gridWidthZ)
                    continue;

                GridCell neighborCell = grid[neighborX, neighborY];

                // Skip collapsed cells
                if (neighborCell.entropy == -1)
                    continue;

                bool changed = false;
                List<Tile> tilesToRemove = new List<Tile>();

                foreach (Tile neighborPossibleTile in neighborCell.possibleTiles)
                {
                    bool hasCompatibleTile = false;

                    foreach (Tile myPossibleTile in cell.possibleTiles)
                    {
                        SocketType mySocket = myPossibleTile.sockets[i]; // My socket in direction i
                        SocketType neighborSocket = neighborPossibleTile.sockets[oppositeDirections[i]]; // Neighbor's socket facing us

                        if (AreSocketsCompatible(mySocket, neighborSocket))
                        {
                            hasCompatibleTile = true;
                            break;
                        }
                    }

                    if (!hasCompatibleTile)
                    {
                        tilesToRemove.Add(neighborPossibleTile);
                    }
                }

                if (tilesToRemove.Count > 0)
                {
                    changed = true;
                    foreach (Tile tileToRemove in tilesToRemove)
                    {
                        Debug.Log($"Removing tile at ({neighborX}, {neighborY}) with sockets {string.Join(", ", tileToRemove.sockets)} because it's incompatible with cell at ({cellCoords.x}, {cellCoords.y})");
                        neighborCell.possibleTiles.Remove(tileToRemove);
                    }
                    neighborCell.entropy = neighborCell.possibleTiles.Count;

                    if (neighborCell.entropy == 0)
                    {
                        Debug.LogError($"No possible tiles left for cell at ({neighborX}, {neighborY})");
                        return;
                    }
                }

                if (changed)
                {
                    // Enqueue neighbor cell to propagate constraints
                    cellsToProcess.Enqueue(new Vector2Int(neighborX, neighborY));
                }
            }
        }
    }


    bool AreSocketsCompatible(SocketType a, SocketType b)
    {
        int indexA = (int)a;
        int indexB = (int)b;

        // Ensure indices are within bounds
        if (indexA < 0 || indexA >= socketCompatibilityMatrix.GetLength(0) ||
            indexB < 0 || indexB >= socketCompatibilityMatrix.GetLength(1))
        {
            return false;
        }

        return socketCompatibilityMatrix[indexA, indexB];
    }

    void OnDrawGizmos()
    {
        if (grid == null)
            return;

        // Iterate over the grid
        for (int x = 0; x < gridWidthX; x++)
        {
            for (int z = 0; z < gridWidthZ; z++)
            {
                GridCell cell = grid[x, z];
                Vector3 position = new Vector3(x * tileSize, 0, z * tileSize);

                // Draw a cube to represent the cell
                Gizmos.color = Color.gray;
                Gizmos.DrawWireCube(position, Vector3.one * tileSize * 0.9f);


                if (cell.entropy == -1)
                Gizmos.color = Color.green; // Collapsed cells
            else if (cell.entropy > 0)
                Gizmos.color = Color.yellow; // Active cells
            else
                Gizmos.color = Color.red; // Contradicted cells

            Gizmos.DrawCube(position, Vector3.one * tileSize * 0.5f);
                // Adjust the label position to be above the tile
                Vector3 labelPosition = position + Vector3.up * (tileSize * 0.6f); // Adjust the height as needed

                // Prepare the label content
                string labelText = $"({x}, {z})\n";

                if (cell.entropy == -1)
                {
                    // The cell is collapsed, display the sockets of the selected tile
                    Tile tile = cell.possibleTiles[0]; // Since it's collapsed, only one tile

                    labelText += $"N: {tile.sockets[0]}\n";
                    labelText += $"E: {tile.sockets[1]}\n";
                    labelText += $"S: {tile.sockets[2]}\n";
                    labelText += $"W: {tile.sockets[3]}";
                }
                else
                {
                    // The cell is not collapsed, display the entropy
                    labelText += $"Entropy: {cell.entropy}";
                }

                // Set up the GUI style
                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.black;
                style.alignment = TextAnchor.MiddleCenter;
                style.fontSize = 12;
                style.fontStyle = FontStyle.Bold;

                // Draw the label
                Handles.Label(labelPosition, labelText, style);
            }
        }
    }

}

