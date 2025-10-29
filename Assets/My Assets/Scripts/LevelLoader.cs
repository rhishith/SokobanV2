using TMPro;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public AudioManager audioManager;
    public GridManager gridManager;
    public GameObject floorPrefab, wallPrefab, boxPrefab, goalPrefab, playerPrefab;
    public Transform levelRoot;
    public TextAsset[] levelFiles;
    public TextMeshProUGUI currentLevelText;

    public void NextLevel()
    {
        if (levelFiles == null || levelFiles.Length == 0) return;

        GameManager.instance.currentLevelIndex = (GameManager.instance.currentLevelIndex + 1) % levelFiles.Length;
        LoadLevelFromText(GameManager.instance.currentLevelIndex);
    }

    public void LoadLevelFromText(int index)
    {
        if (levelFiles == null || levelFiles.Length == 0) return;
        if (index < 0 || index >= levelFiles.Length) index = 0;

        string text = levelFiles[index].text.Replace("\r", "");
        string[] lines = text.Split('\n');
        LoadLevel(lines);
    }

    private void LoadLevel(string[] map)
    {
        Debug.Log(GameManager.instance.currentLevelIndex);
        audioManager.SwitchBackgroundMusic(GameManager.instance.currentLevelIndex);

        int width = map[0].Length;
        int height = map.Length;
        gridManager.InitGrid(width, height);

        // Clear previous level
        if (levelRoot != null)
        {
            for (int i = levelRoot.childCount - 1; i >= 0; i--)
                Destroy(levelRoot.GetChild(i).gameObject);
        }

        // Instantiate level objects
        for (int y = 0; y < height; y++)
        {
            string row = map[height - 1 - y]; // Flip Y
            for (int x = 0; x < width; x++)
            {
                char c = row[x];
                Tile tile = new Tile();
                Vector3 pos = gridManager.GridToWorld(new Vector2Int(x, y));

                switch (c)
                {
                    case '#':
                        tile.Type = TileType.Wall;
                        var wall = Instantiate(wallPrefab, pos, Quaternion.identity);
                        if (levelRoot != null) wall.transform.SetParent(levelRoot, true);
                        break;
                    case '.':
                        tile.Type = TileType.Floor;
                        var floor = Instantiate(floorPrefab, pos, Quaternion.identity);
                        if (levelRoot != null) floor.transform.SetParent(levelRoot, true);
                        break;
                    case 'G':
                        tile.Type = TileType.Goal;
                        var floor2 = Instantiate(floorPrefab, pos, Quaternion.identity);
                        if (levelRoot != null) floor2.transform.SetParent(levelRoot, true);
                        var goal = Instantiate(goalPrefab, pos, Quaternion.identity);
                        if (levelRoot != null) goal.transform.SetParent(levelRoot, true);
                        break;
                    case 'B':
                        tile.Type = TileType.Box;
                        var floor3 = Instantiate(floorPrefab, pos, Quaternion.identity);
                        if (levelRoot != null) floor3.transform.SetParent(levelRoot, true);
                        GameObject box = Instantiate(boxPrefab, pos, Quaternion.identity);
                        if (levelRoot != null) box.transform.SetParent(levelRoot, true);
                        tile.Occupier = box;
                        break;
                    case 'P':
                        tile.Type = TileType.Player;
                        var floor4 = Instantiate(floorPrefab, pos, Quaternion.identity);
                        if (levelRoot != null) floor4.transform.SetParent(levelRoot, true);
                        GameObject player = Instantiate(playerPrefab, pos, Quaternion.identity);
                        if (levelRoot != null) player.transform.SetParent(levelRoot, true);
                        PlayerController pc = player.GetComponent<PlayerController>();
                        pc.playerPos = new Vector2Int(x, y);
                        pc.gridManager = gridManager;
                        pc.audiomanager = audioManager;
                        pc.levelLoader = this;
                        tile.Occupier = player;
                        break;
                }
                gridManager.SetTile(new Vector2Int(x, y), tile);
            }
        }
    }
}