using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MazePlayer : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed = 5f;

    private MazeCreater _mazeCreater;

    // 現在のグリッド座標
    private int _currentRow;
    private int _currentCol;

    private bool _isMoving = false;
    private bool _isInputDisabled = false;

    public System.Action<int, int> OnCellChanged;

    private void Update()
    {
        if (_isMoving || _isInputDisabled) return;

        if (Keyboard.current.wKey.wasPressedThisFrame) TryMove(-1, 0);
        else if (Keyboard.current.sKey.wasPressedThisFrame) TryMove(1, 0);
        else if (Keyboard.current.aKey.wasPressedThisFrame) TryMove(0, -1);
        else if (Keyboard.current.dKey.wasPressedThisFrame) TryMove(0, 1);
    }

    // 迷路データを受け取る
    public void Initialize(MazeCreater mazeCreater)
    {
        _mazeCreater = mazeCreater;
    }

    // プレイヤーをスタート位置 [1, 1] へ配置する
    public (int row, int col) PlaceAtStart()
    {
        _currentRow = 1;
        _currentCol = 1;
        transform.position = _mazeCreater.GridToWorld(_currentRow, _currentCol, transform.position.z);
        return (_currentRow, _currentCol);
    }

    public void SetInputEnabled(bool enabled)
    {
        _isInputDisabled = !enabled;
    }

    public void ResetState()
    {
        StopAllCoroutines();
        _isMoving = false;
        _isInputDisabled = false;
    }

    // 移動メソッド
    private void TryMove(int rowDelta, int colDelta)
    {
        int nextRow = _currentRow + rowDelta;
        int nextCol = _currentCol + colDelta;

        int width = _mazeCreater.mazeInfoArr.GetLength(0);
        if (nextRow < 0 || nextRow >= width || nextCol < 0 || nextCol >= width) return;
        if (_mazeCreater.mazeInfoArr[nextRow, nextCol] != (int)MazeCreater.MazeType.Path) return;

        _currentRow = nextRow;
        _currentCol = nextCol;
        StartCoroutine(SmoothMove(_mazeCreater.GridToWorld(_currentRow, _currentCol, transform.position.z)));
    }

    private IEnumerator SmoothMove(Vector3 targetPos)
    {
        _isMoving = true;

        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, targetPos, _moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
        _isMoving = false;

        // 移動完了後にGameManagerへ現在座標を通知
        OnCellChanged?.Invoke(_currentRow, _currentCol);
    }
}