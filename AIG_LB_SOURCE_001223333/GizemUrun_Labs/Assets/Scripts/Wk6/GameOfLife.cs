using UnityEngine;
using UnityEngine.UI;

public class GameOfLife : MonoBehaviour
{
    public int gridWidth = 10;    
    public int gridHeight = 10;   
    public float cellSize = 1f;
    public GameObject cellPrefab;
    private Cell[,] grid;
     
    public class Cell
    {
        public bool IsAlive; 
        public GameObject cellObject;

        public Cell(bool isAlive, GameObject cellObject)
        {
            IsAlive = isAlive;
            this.cellObject = cellObject;
        }
    }

    void Start()
    {
        CreateGrid();
    }

    // Create a grid of cells
    void CreateGrid()
    {
        grid = new Cell[gridWidth, gridHeight];

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                bool isAlive;
                Vector3 position = new Vector3(x * cellSize, 0, y * cellSize);
                GameObject cellObject = Instantiate(cellPrefab, position, Quaternion.identity);
                
                if(Random.Range(0,2) == 0){
                    isAlive = true;
                }
                else {
                   isAlive = false;
                }

                Cell newCell = new Cell(isAlive, cellObject);
                grid[x, y] = newCell;

                //Sets colour
                UpdateCellColour(newCell);
            }
        }
    }

    public void NextGen()
    {
        bool[,] nextGen = new bool[gridWidth, gridHeight];

        // Game of life rules
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                int aliveNeighbours = CountAliveNeighbours(x, y);
                bool currentState = grid[x, y].IsAlive;

                // Conway's rules
                if (currentState && (aliveNeighbours < 2 || aliveNeighbours > 3))
                {
                    nextGen[x, y] = false; // Cell dies 
                }
                else if (!currentState && aliveNeighbours == 3)
                {
                    nextGen[x, y] = true;  // Dead cell becomes alive
                }
                else
                {
                    nextGen[x, y] = currentState; // Cell survives or stays dead
                }
            }
        }

        
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                grid[x, y].IsAlive = nextGen[x, y];
                UpdateCellColour(grid[x, y]);
            }
        }
    }

    // Count amount of alive cells around cell
    int CountAliveNeighbours(int x, int y)
    {
        int aliveCount = 0;

        // Checks cells around the target cell
        for (int dy = -1; dy <= 1; dy++)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                if (dx == 0 && dy == 0) continue;

                int nx = x + dx;
                int ny = y + dy;

                // Checks if neighbour in grid
                if (nx >= 0 && nx < gridWidth && ny >= 0 && ny < gridHeight)
                {
                    if (grid[nx, ny].IsAlive)
                    {
                        aliveCount++;
                    }
                }
            }
        }
        return aliveCount;
    }
    // Update the visual if alive or not
    void UpdateCellColour(Cell cell)
    {
        Renderer cellRenderer = cell.cellObject.GetComponent<Renderer>();

        if (cell.IsAlive)
        {
            cellRenderer.material.color = Color.black; // Alive cell (black)
        }
        else
        {
            cellRenderer.material.color = Color.white; // Dead cell (white)
        }
    }


}
