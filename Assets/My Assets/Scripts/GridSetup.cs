using UnityEngine;

public enum TileType
{
    Floor,
    Wall,
    Goal,
    Box,
    Player
}

public struct Tile
{
    public TileType Type;
    public GameObject Occupier; // Box or Player
}

public class GridSetup : MonoBehaviour
{

}
