using UnityEngine;

public class GoalKey : MonoBehaviour
{
    // キーを配置して表示する
    public void Show(Vector3 worldPos)
    {
        transform.position = worldPos;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}