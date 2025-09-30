using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int width;
    public int height;
    public Tile[,] grid;
    public float tileSize = 1f;
    public Vector2 gridOrigin = Vector2.zero; // center or starting point of the grid

    public void InitGrid(int w, int h)
    {
        width = w;
        height = h;
        grid = new Tile[w, h];
    }

    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        Vector2 offset = new Vector2(width * tileSize, height * tileSize) / 2f;
        Vector3 world = new Vector3(gridPos.x * tileSize, gridPos.y * tileSize, 0)
            + (Vector3)gridOrigin - (Vector3)offset + Vector3.one * tileSize / 2f;
        return world;
    }

    public bool IsInsideGrid(Vector2Int pos)
    {
        return pos.x >= 0 && pos.y >= 0 && pos.x < width && pos.y < height;
    }

    public Tile GetTile(Vector2Int pos)
    {
        if (grid == null)
        {
            Debug.LogError("GridManager.grid is null. Did you call InitGrid or LoadLevel?");
            return default;
        }
        if (!IsInsideGrid(pos)) return default;
        return grid[pos.x, pos.y];
    }

    public void SetTile(Vector2Int pos, Tile tile)
    {
        if (grid == null)
        {
            Debug.LogError("GridManager.grid is null. Did you call InitGrid or LoadLevel?");
            return;
        }
        if (!IsInsideGrid(pos)) return;
        grid[pos.x, pos.y] = tile;
    }

    public bool AreAllGoalsCovered()
    {
        if (grid == null) return false;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile t = grid[x, y];
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
        if (width <= 0 || height <= 0) return;

        Gizmos.color = Color.green;

        // offset to center the grid around gridOrigin
        Vector2 offset = new Vector2(width * tileSize, height * tileSize) / 2f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 pos = new Vector3(x * tileSize, y * tileSize, 0) + (Vector3)gridOrigin - (Vector3)offset + Vector3.one * tileSize / 2f;
                Gizmos.DrawWireCube(pos, new Vector3(tileSize, tileSize, 0));
            }
        }
    }
}
