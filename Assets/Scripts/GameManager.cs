using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private MazeCreater _mazeCreater;
    [SerializeField] private MazePlayer _mazePlayer;
    [SerializeField] private MazeGoal _mazeGoal;
    [SerializeField] private GoalKey _goalKey;
    [SerializeField] private GameObject _startCanvas;
    [SerializeField] private GameObject _endCanvas;

    // ゴール・キーのグリッド座標
    private int _goalRow, _goalCol;
    private int _keyRow, _keyCol;

    // キー取得済みフラグ
    private bool _hasKey = false;

    public void StartGame()
    {
        SetupGame();
        _startCanvas.SetActive(false);
    }

    // 迷路を再生成してゲームをリスタートする
    public void ReStart()
    {
        // 前の状態をリセット
        _mazePlayer.ResetState();
        _mazeGoal.Hide();
        _goalKey.Hide();
        _hasKey = false;

        // 迷路再生成
        _mazeCreater.ReCreate();

        _endCanvas.SetActive(false);
    }

    // 迷路生成後にプレイヤー・ゴール・キーを配置、イベントを設定する
    private void SetupGame()
    {
        _mazeCreater.CreateMaze();

        _mazePlayer.Initialize(_mazeCreater);
        _mazePlayer.SetInputEnabled(true);
        var (playerRow, playerCol) = _mazePlayer.PlaceAtStart();

        _mazePlayer.OnCellChanged -= OnPlayerCellChanged;
        _mazePlayer.OnCellChanged += OnPlayerCellChanged;

        _mazeCreater.OnMazeCreated -= SetupGame;
        _mazeCreater.OnMazeCreated += SetupGame;

        PlaceGoal(playerRow, playerCol);
        PlaceKey(playerRow, playerCol);
    }

    // プレイヤーが移動したセルでキー・ゴールの判定を行う
    private void OnPlayerCellChanged(int row, int col)
    {
        CheckKey(row, col);
        CheckGoal(row, col);
    }

    private void CheckKey(int row, int col)
    {
        if (_hasKey) return;
        if (row != _keyRow || col != _keyCol) return;

        _hasKey = true;
        _goalKey.Hide();
        _mazeGoal.SetLocked(false);
        Debug.Log("GameManager: GoalKeyを取得しました。");
    }

    private void CheckGoal(int row, int col)
    {
        if (row != _goalRow || col != _goalCol) return;

        if (!_hasKey)
        {
            Debug.Log("GameManager: ゴールにたどり着いたが、GoalKeyがない！");
            return;
        }

        // ゴール到達
        _mazePlayer.SetInputEnabled(false);
        Debug.Log("GameManager: ゴール到達！");
        _endCanvas.SetActive(true);
    }

    // ゴールを右下に固定配置する
    private void PlaceGoal(int excludeRow, int excludeCol)
    {
        int width = _mazeCreater.mazeInfoArr.GetLength(0);
        _goalRow = width - 2;
        _goalCol = width - 2;
        _mazeGoal.Show(_mazeCreater.GridToWorld(_goalRow, _goalCol));
    }

    // プレイヤー・ゴールと重ならないランダムなPathセルにキーを配置する
    private void PlaceKey(int excludeRow, int excludeCol)
    {
        // ゴール座標も除外する
        if (TryGetRandomPathCell(excludeRow, excludeCol, out int row, out int col,
                                 _goalRow, _goalCol))
        {
            _keyRow = row;
            _keyCol = col;
            _goalKey.Show(_mazeCreater.GridToWorld(row, col));
        }
        else
        {
            Debug.LogError("GameManager: キー配置できるPathセルが見つかりませんでした。");
        }
    }

    // 除外座標を避けてランダムなPathセルを返す
    private bool TryGetRandomPathCell(int exRow1, int exCol1,
                                      out int row, out int col,
                                      int exRow2 = -1, int exCol2 = -1)
    {
        int width = _mazeCreater.mazeInfoArr.GetLength(0);
        var candidates = new System.Collections.Generic.List<(int, int)>();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (_mazeCreater.mazeInfoArr[i, j] != (int)MazeCreater.MazeType.Path) continue;
                if (i == exRow1 && j == exCol1) continue;
                if (i == exRow2 && j == exCol2) continue;
                candidates.Add((i, j));
            }
        }

        if (candidates.Count == 0)
        {
            row = -1; col = -1;
            return false;
        }

        var pick = candidates[UnityEngine.Random.Range(0, candidates.Count)];
        row = pick.Item1;
        col = pick.Item2;
        return true;
    }
}
