using UnityEngine;

public enum TileType
{
    Floor,
    Wall,
    Goal,
    Box,
    Player
}

[System.Serializable]
public struct Tile
{
    public TileType Type;
    public GameObject Occupier; // Box or Player
}

public class GridSetup : MonoBehaviour
{
    // Implementation intentionally left empty
}
