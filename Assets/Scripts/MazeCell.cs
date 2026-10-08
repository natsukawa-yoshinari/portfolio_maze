using UnityEngine;

public class MazeCell : MonoBehaviour
{
    private const int WALL = 0;
    private const int PATH = 1;

    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    public void SetMazeType(int type)
    {
        if (type == WALL) _spriteRenderer.color = Color.black;
        if (type == PATH) _spriteRenderer.color = Color.white;
    }
}
