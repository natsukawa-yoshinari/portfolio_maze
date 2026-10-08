using UnityEngine;

public class MazeGoal : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    [SerializeField]
    private Color _lockedColor = Color.red;

    [SerializeField]
    private Color _unlockedColor = Color.green;

    // ゴールを配置して表示する
    public void Show(Vector3 worldPos)
    {
        transform.position = worldPos;
        gameObject.SetActive(true);
        SetLocked(true);
    }

    // ゴールのロック状態を切り替える
    public void SetLocked(bool isLocked)
    {
        if (_spriteRenderer == null) return;
        _spriteRenderer.color = isLocked ? _lockedColor : _unlockedColor;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}