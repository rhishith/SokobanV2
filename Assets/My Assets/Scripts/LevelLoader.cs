using TMPro;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private GridManager gridManager;

    [Header("Prefabs")]
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject boxPrefab;
    [SerializeField] private GameObject goalPrefab;
    [SerializeField] private GameObject playerPrefab;

    [Header("Level Data")]
    [SerializeField] private Transform levelRoot;
    [SerializeField] private TextAsset[] levelFiles;

    public void LoadLevelFromText(int index)
    {
        if (levelFiles == null || levelFiles.Length == 0) return;
        
        // Wrap index around
        if (index < 0 || index >= levelFiles.Length)
        {
            index = index % levelFiles.Length;
            if (index < 0) index += levelFiles.Length;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.CurrentLevelIndex = index;
        }

        string text = levelFiles[index].text.Replace("\r", "");
        string[] lines = text.Split('\n');
        LoadLevel(lines);
    }

    private void LoadLevel(string[] map)
    {
        int currentLevelIndex = GameManager.Instance != null ? GameManager.Instance.CurrentLevelIndex : 0;
        
        if (audioManager != null)
        {
            audioManager.SwitchBackgroundMusic(currentLevelIndex);
        }

        // Map dimensions
        int height = map.Length;
        int width = 0;
        foreach (var line in map) if (line.Length > width) width = line.Length;

        gridManager.InitGrid(width, height);

        // Clear previous
        if (levelRoot != null)
        {
            for (int i = levelRoot.childCount - 1; i >= 0; i--)
            {
                Destroy(levelRoot.GetChild(i).gameObject);
            }
        }

        // Build Level
        for (int y = 0; y < height; y++)
        {
            string row = map[height - 1 - y]; // Flip Y because array 0 is top, grid 0 is bottom
            for (int x = 0; x < width; x++)
            {
                if (x >= row.Length) break; // Handle uneven lines

                char c = row[x];
                Tile tile = new Tile();
                Vector2Int gridPos = new Vector2Int(x, y);
                Vector3 worldPos = gridManager.GridToWorld(gridPos);

                switch (c)
                {
                    case '#': // Wall
                        tile.Type = TileType.Wall;
                        InstantiateObject(wallPrefab, worldPos);
                        break;
                    case '.': // Floor
                        tile.Type = TileType.Floor;
                        InstantiateObject(floorPrefab, worldPos);
                        break;
                    case 'G': // Goal
                        tile.Type = TileType.Goal;
                        InstantiateObject(floorPrefab, worldPos);
                        InstantiateObject(goalPrefab, worldPos);
                        break;
                    case 'B': // Box
                        tile.Type = TileType.Box;
                        InstantiateObject(floorPrefab, worldPos);
                        GameObject box = InstantiateObject(boxPrefab, worldPos);
                        tile.Occupier = box;
                        break;
                    case 'P': // Player
                        tile.Type = TileType.Player;
                        InstantiateObject(floorPrefab, worldPos);
                        GameObject player = InstantiateObject(playerPrefab, worldPos);
                        
                        // Initialize Player
                        PlayerController pc = player.GetComponent<PlayerController>();
                        if (pc != null)
                        {
                            pc.Initialize(gridManager, audioManager, gridPos);
                        }
                        
                        tile.Occupier = player;
                        break;
                }
                gridManager.SetTile(gridPos, tile);
            }
        }
    }

    private GameObject InstantiateObject(GameObject prefab, Vector3 position)
    {
        if (prefab == null) return null;
        GameObject obj = Instantiate(prefab, position, Quaternion.identity);
        if (levelRoot != null) obj.transform.SetParent(levelRoot, true);
        return obj;
    }
}
