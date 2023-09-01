using UnityEngine;

public class TileCell : MonoBehaviour
{
    public Vector2Int coordinate { get; set; }
    public TIle tile { get; set; }
    public bool empty => tile == null;
    public bool occupied => tile != null;

}
