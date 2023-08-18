using System.Collections;using System.Collections.Generic;using UnityEngine;using UnityEngine.Events;


/// <summary>
/// This class is responsible for managing the game state and providing access to game data.
/// </summary>
public class GameManager : MonoBehaviour {		public static GameManager instance;		[SerializeField] GameObject cellPrefab;		[SerializeField] float simulationRate = 1f;		[SerializeField] int maxVineIteration = 5;		Cell[,] cells;		LSystemVine vineGenerator = new LSystemVine();		int currentVineIteration = 0;		public int width = 10;		public int height = 10;		float timeSinceLastUpdate = 0f;

		/// <summary>
		/// Sets the instance of the VineGrowthManager and adds two 
		/// UnityEvents for when the vine growth starts and ends.
		/// </summary>
		private void Awake() {				if (instance != null && instance != this) {						Destroy(this.gameObject);						return;				}				instance = this;				DontDestroyOnLoad(this.gameObject);		}

		/// <summary>
		/// Initializes the grid and sets up the game loop.
		/// </summary>
		private void Start() {				InitializeGrid();				StartCoroutine(GameLoop());		}

		/// <summary>
		/// Coroutine that runs the game loop, updating the game and animating the vine growth.
		/// </summary>
		/// <returns>
		/// IEnumerator object for the game loop.
		/// </returns>
		IEnumerator GameLoop() {				while (true) {						UpdateGame();						if (currentVineIteration < maxVineIteration) {								string pattern = vineGenerator.GeneratePattern(currentVineIteration);								currentVineIteration++;						}						yield return new WaitUntil(() => timeSinceLastUpdate >= simulationRate);						timeSinceLastUpdate = 0f;				}		}

		/// <summary>
		/// Updates the game and checks if the left mouse button is being held down. 
		/// </summary>
		private void Update() {				timeSinceLastUpdate += Time.deltaTime;

				// Check if the left mouse button is being held down
				if (Input.GetMouseButton(0)) {						ActivateCell();				}		}


		/// <summary>
		/// Activates a cell in the grid based on the mouse position.
		/// </summary>
		private void ActivateCell() {				Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y));

				// Bottom left cell's center is at 0,0 and each cell is 1x1. 
				int cellX = Mathf.FloorToInt(mouseWorldPosition.x);				int cellY = Mathf.FloorToInt(mouseWorldPosition.z);

				// validate the cell coords
				// thinking: have this be a bool that returns whether or not this particular cell is true/alive.
						// if it is:
						// Activation also means animation. 
				if (cellX >= 0 && cellX < width && cellY >= 0 && cellY < height) {						Debug.Log($"{cells[cellX, cellY].name}");						cells[cellX, cellY].SetAlive(true);				}		}

		/// <summary>
		/// Initializes the grid by creating a 2D array of Cell objects and setting them to be dead. 
		/// Also calls the Glider() method.
		/// </summary>
		private void InitializeGrid() {				cells = new Cell[width, height];				for (int x = 0; x < width; x++) {						for (int y = 0; y < height; y++) {								GameObject cellObject = Instantiate(cellPrefab, new Vector3(x, 0, y), Quaternion.Euler(90, 0, 0));								Cell cell = cellObject.GetComponent<Cell>();								cells[x, y] = cell;
								cell.SetAlive(false);						}				}				Glider();		}

		/// <summary>
		/// Sets the cells in the center of the grid to alive.
		/// </summary>
		void CenterSquare() {				cells[4, 4].SetAlive(true);				cells[4, 5].SetAlive(true);				cells[5, 4].SetAlive(true);		}

		/// <summary>
		/// Sets up a 3x3 glider pattern in the cell grid.
		/// </summary>
		void Glider() {
				// This is just an example for a 3x3 glider; adjust based on your grid size
				cells[2, 2].SetAlive(true);				cells[3, 2].SetAlive(true);				cells[4, 2].SetAlive(true);				cells[4, 1].SetAlive(true);				cells[3, 0].SetAlive(true);		}


		/// <summary>
		/// Counts the number of alive neighbors for a given cell.
		/// </summary>
		/// <param name="x">The x coordinate of the cell.</param>
		/// <param name="y">The y coordinate of the cell.</param>
		/// <returns>The number of alive neighbors.</returns>
		int CountAliveNeighbors(int x, int y) {				int count = 0;				for (int i = -1; i <= 1; i++) {						for (int j = -1; j <= 1; j++) {								if (i == 0 && j == 0) continue; // skip the current cell
								
								int neighborX = x + i;
								int neighborY = y + j;
								
								if (neighborX >= 0 && neighborX < width && 
										neighborY >= 0 && neighborY < height) {
												if (cells[neighborX, neighborY].IsAlive){
														count++;
												}
										}						}				}				Debug.Log($"NCount: {count}");				return count;		}

		/// <summary>
		/// Updates the game by calculating the number of alive neighbors for each cell, 
		/// and then setting the alive/dead state of each cell accordingly.
		/// </summary>
		private void UpdateGame() {				List<Vector2Int> evolveCells = new List<Vector2Int>();				List<Vector2Int> extinctCells = new List<Vector2Int>();

				// Calculate neighbors
				for (int x = 0; x < width; x++) {						for (int y = 0; y < height; y++) {								int aliveNeighbors = CountAliveNeighbors(x, y);								Debug.Log($"Living Neghbors to {x}, {y} = {aliveNeighbors}");								bool survives;								if (cells[x, y].IsAlive) {										survives = aliveNeighbors == 2 || aliveNeighbors == 3;								} else {										survives = aliveNeighbors == 3;								}								if (survives && !cells[x, y].IsAlive) {										evolveCells.Add(new Vector2Int(x, y));								} else if (!survives && cells[x, y].IsAlive) {										extinctCells.Add(new Vector2Int(x, y));								}								cells[x, y].Neighbors = aliveNeighbors;						}				}				// Handle evolving cells (future method)				foreach (var pos in evolveCells) {						cells[pos.x, pos.y].SetAlive(true);						var pattern = vineGenerator.GeneratePattern(1);												var vr = cells[pos.x, pos.y].GetComponent<VineRenderer>();						StartCoroutine(vr.SimulateVine(pattern, VineState.Growing));				}				// Handle extinct cells 				foreach (var pos in extinctCells) {						cells[pos.x, pos.y].SetAlive(false);						var pattern = vineGenerator.GeneratePattern(1);

						var vr = cells[pos.x, pos.y].GetComponent<VineRenderer>();						StartCoroutine(vr.SimulateVine(pattern, VineState.Withering));				}		}}