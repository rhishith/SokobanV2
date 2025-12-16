using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private float tileSize = 1f;
    [SerializeField] private Vector2 gridOrigin = Vector2.zero; // center or starting point of the grid

    public int Width { get; private set; }
    public int Height { get; private set; }
    public Tile[,] Grid { get; private set; }

    public void InitGrid(int width, int height)
    {
        Width = width;
        Height = height;
        Grid = new Tile[width, height];
    }

    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        Vector2 offset = new Vector2(Width * tileSize, Height * tileSize) / 2f;
        Vector3 world = new Vector3(gridPos.x * tileSize, gridPos.y * tileSize, 0)
            + (Vector3)gridOrigin - (Vector3)offset + Vector3.one * tileSize / 2f;
        return world;
    }

    public bool IsInsideGrid(Vector2Int pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < Width && pos.y < Height;
    }

    public Tile GetTile(Vector2Int pos)
    {
        if (Grid == null)
        {
            Debug.LogError("GridManager.Grid is null. Did you call InitGrid or LoadLevel?");
            return default;
        }
        if (!IsInsideGrid(pos)) return default;
        return Grid[pos.x, pos.y];
    }

    public void SetTile(Vector2Int pos, Tile tile)
    {
        if (Grid == null)
        {
            Debug.LogError("GridManager.Grid is null. Did you call InitGrid or LoadLevel?");
            return;
        }
        if (!IsInsideGrid(pos)) return;
        Grid[pos.x, pos.y] = tile;
    }

    public bool AreAllGoalsCovered()
    {
        if (Grid == null) return false;
        
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Tile t = Grid[x, y];
                if (t.Type == TileType.Goal)
                {
                    if (t.Occupier == null) return false;
                    if (!t.Occupier.CompareTag("Box")) return false;
                }
            }
        }
        return true;
    }

    private void OnDrawGizmos()
    {
        if (Width <= 0 || Height <= 0) return;

        Gizmos.color = Color.green;

        // offset to center the grid around gridOrigin
        Vector2 offset = new Vector2(Width * tileSize, Height * tileSize) / 2f;

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                Vector3 pos = new Vector3(x * tileSize, y * tileSize, 0) + (Vector3)gridOrigin - (Vector3)offset + Vector3.one * tileSize / 2f;
                Gizmos.DrawWireCube(pos, new Vector3(tileSize, tileSize, 0));
            }
        }
    }
}
