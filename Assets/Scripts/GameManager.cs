using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class GameManager : MonoBehaviour {
	public static GameManager instance;

  [SerializeField] GameObject cellPrefab; 
	[SerializeField] VineRenderer vineRenderer;
  [SerializeField] float simulationRate = 1f;
	[SerializeField] int maxVineIteration = 5;

	Cell[,] cells;

	LSystemVine vineGenerator = new LSystemVine();
  
	int currentVineIteration = 0;
	
	bool isVineGrowing = false;

	public int width = 10;
  public int height = 10;
   
	public UnityEvent onVineGrowthStart;
	public UnityEvent onVineGrowthEnd;

  
  float timeSinceLastUpdate = 0f;

		private void Awake() {
				if (instance != null && instance != this) { 
						Destroy(this.gameObject);
						return;
				}

				instance = this;
				DontDestroyOnLoad(this.gameObject);

				if (onVineGrowthStart == null) onVineGrowthStart = new UnityEvent();
				if (onVineGrowthEnd == null) onVineGrowthEnd = new UnityEvent();
		}

		private void Start() {
		  InitializeGrid();
		  onVineGrowthStart.AddListener(() => isVineGrowing = true);
		  onVineGrowthEnd.AddListener(() => isVineGrowing = false);

		  StartCoroutine(GameLoop());
	  }

		IEnumerator GameLoop() {
				while(true) {
						UpdateGame();

						if (currentVineIteration < maxVineIteration){
						string pattern = vineGenerator.GeneratePattern(currentVineIteration);
						StartCoroutine(vineRenderer.AnimateVineGrowth(pattern));
						currentVineIteration++;
						}
			
						yield return new WaitUntil(() => timeSinceLastUpdate >= simulationRate);
						timeSinceLastUpdate = 0f;
				}
		}

		private void Update() {
				timeSinceLastUpdate += Time.deltaTime;

				// Check if the left mouse button is being held down
				if (Input.GetMouseButton(0)) {
						Debug.Log("Click!");
						ActivateCell();
				}
		}

	private void ActivateCell() {
		Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y));

		// Bottom left cell's center is at 0,0 and each cell is 1x1. 
		int cellX = Mathf.FloorToInt(mouseWorldPosition.x);
		int cellY = Mathf.FloorToInt(mouseWorldPosition.z);

		// validate the cell coords
		if (cellX >= 0 && cellX < width && cellY >= 0 && cellY < height) {
			Debug.Log($"{cells[cellX, cellY].name}");
			cells[cellX, cellY].SetAlive(true);
		}
	}

	private void InitializeGrid() {
		cells = new Cell[width, height];
				
		for (int x=0; x<width; x++) {
			for (int y=0; y<height; y++){
				GameObject cellObject = Instantiate(cellPrefab, new Vector3(x, 0, y), Quaternion.Euler(90,0,0));
				Cell cell = cellObject.GetComponent<Cell>();
				cells[x,y] = cell;
				cell.InitializeCell();
				cell.SetAlive(false);
			}
		}
				
		Glider();
	}

	void CenterSquare(){
		cells[4, 4].SetAlive(true);
		cells[4, 5].SetAlive(true);
		cells[5, 4].SetAlive(true);
	}

	void Glider(){
		// This is just an example for a 3x3 glider; adjust based on your grid size
		cells[2, 2].SetAlive(true);
		cells[3, 2].SetAlive(true);
		cells[4, 2].SetAlive(true);
		cells[4, 1].SetAlive(true);
		cells[3, 0].SetAlive(true);
	}

	int CountAliveNeighbors(int x, int y) {

		int count = 0; 

		for (int i = -1; i <= 1; i++) {
			for (int j = -1; j <=1; j++) {
				if (i==0 && j==0) continue; // no neigbors. it's a dead cell
				if (x+i < 0 || x + i >= width || y + j < 0 || y + j >= height) continue; // out of range.
								
				if (cells[x + i, y + j].IsAlive) {
						count++;
				}
			}
		}
		return count;
	}

	private void UpdateGame() {
		List<Vector2Int> evolveCells = new List<Vector2Int>();
		List<Vector2Int> extinctCells = new List<Vector2Int>();

		// Calculate neighbors
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				int aliveNeighbors = CountAliveNeighbors(x, y);
				bool survives = false;

				if (cells[x,y].IsAlive){
						cells[x, y].IncrementAge();
						survives = aliveNeighbors == 2 || aliveNeighbors == 3;
				} else {
						survives = aliveNeighbors == 3;
				}

				if (survives && !cells[x,y].IsAlive) {
						evolveCells.Add(new Vector2Int(x,y));
				} else if (!survives && cells[x,y].IsAlive) {
						extinctCells.Add(new Vector2Int(x,y));
				}
				cells[x, y].Neighbors = aliveNeighbors;
			}
		}

		foreach(var pos in evolveCells) {
			cells[pos.x, pos.y].SetAlive(true);
		}

		foreach(var pos in extinctCells) {
			cells[pos.x, pos.y].SetAlive(false);
		}
	}

		private void OnDestroy() {
				onVineGrowthStart.RemoveAllListeners();
				onVineGrowthEnd.RemoveAllListeners();
		}
}
