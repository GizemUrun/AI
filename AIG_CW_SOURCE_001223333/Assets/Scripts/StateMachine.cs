using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StateMachine : MonoBehaviour
{

    [SerializeField] GameObject cellPrefab;
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float cellSize = 1f;
    public Cell[,] grid;
    //public bool cOn; //coroutine
    //public bool isChangedState = false;
    //public List<GameObject> gridCells;
    

    public State currentState;

    public class Cell
    {
        public bool IsAlive;
        public GameObject indivCell;

        public Cell(bool isAlive, GameObject indivCell)
        {
            IsAlive = isAlive;
            this.indivCell = indivCell;
        }
    }

    

    // Start is called before the first frame update
    void Start()
    {
        currentState = new Idle();
        CreateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        currentState.ExecuteStateLogic(this);
        currentState.Transition(this);
    }


    public void CreateGrid()
    {
        grid = new Cell[gridWidth, gridHeight];
        
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                bool isAlive = true;
                Vector3 position = new Vector3(x * cellSize, 0, y * cellSize);
                GameObject cellObject = Instantiate(cellPrefab, position, Quaternion.identity);
                //gridCells.Add(cellObject);
                
                if (Random.Range(0, 2) == 0)
                {
                    isAlive = true;
                }
                else
                {
                    isAlive = false;
                }
                
                Cell newCell = new Cell(isAlive, cellObject);
                grid[x, y] = newCell;
        
                //Sets colour
                UpdateCellColour(newCell);

            }
        }
    }

  

    public void RandomiseGridCells()
    {
        /*
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                bool isAlive = true;

                foreach (Cell grid in grid)
                {
                    if (Random.Range(0, 2) == 0)
                    {
                        isAlive = true;
                    }
                    else
                    {
                        isAlive = false;
                    }

                    UpdateCellColour(cells);
                }

            }
        } */
    }


    /*
    public void ClearList()
    {
        Destroy(gridCells);
        gridCells.RemoveAll(GameObject);

    }
    */

    
    public void ConwaysGame()
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

        //StartCoroutine(TimeDelay(5.0f));

        //if (cOn)
        //{
            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    grid[x, y].IsAlive = nextGen[x, y];
                
                    UpdateCellColour(grid[x, y]);
                    //cOn = false;
                }
            }
        //}

    }



    public void DayNight()
    {
        bool[,] nextGen = new bool[gridWidth, gridHeight];

        // Game of life rules
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                int aliveNeighbours = CountAliveNeighbours(x, y);
                bool currentState = grid[x, y].IsAlive;

                // Day and Night's rules
                if (currentState && (aliveNeighbours == 3 || aliveNeighbours == 4 || (aliveNeighbours >= 6 && aliveNeighbours < 9)))
                {
                    nextGen[x, y] = true; // Cell remains alive 
                }
                else if (!currentState && (aliveNeighbours == 3 || (aliveNeighbours >= 6 && aliveNeighbours < 9)))
                {
                    nextGen[x, y] = true;  // Dead cell becomes alive
                }
                else if ((currentState && (aliveNeighbours < 3 || (aliveNeighbours > 4 && aliveNeighbours < 6))) || (!currentState && (aliveNeighbours < 3 || (aliveNeighbours > 3 && aliveNeighbours < 6))))
                {
                    nextGen[x, y] = false;  // Cell dies
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


    public void Seed()
    {
        bool[,] nextGen = new bool[gridWidth, gridHeight];

        // Game of life rules
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                int aliveNeighbours = CountAliveNeighbours(x, y);
                bool currentState = grid[x, y].IsAlive;

                // Seed's rules
                if (!currentState && aliveNeighbours == 2 )
                {
                    nextGen[x, y] = true; // Cell is born 
                }
                else if (currentState)
                {
                    nextGen[x, y] = false;  // Alive cells die
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




    // counts how many alive around cell
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

    /*
    public IEnumerator TimeDelay(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        cOn = true;

    }
    */

    // visual representation
    void UpdateCellColour(Cell cell)
    {
        Renderer cellRenderer = cell.indivCell.GetComponent<Renderer>();

        if (cell.IsAlive)
        {
            cellRenderer.material.color = Color.black; // Alive cell (black)
        }
        else
        {
            cellRenderer.material.color = Color.white; // Dead cell (white)
        }
    }


    public void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
