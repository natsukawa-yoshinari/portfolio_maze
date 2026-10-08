using UnityEngine;

public class MazeCreater : MonoBehaviour
{
    [SerializeField]
    private GameObject _mazeCell;
    [SerializeField]
    private int _width = 5; // 迷路のサイズ

    public const float CellSize = 0.5f;

    public int[,] mazeInfoArr; // 迷路の数値情報を持つ2次元配列
    public GameObject[,] mazeCellArr; // GameObjectを持つ

    public enum MazeType
    {
        Null = -1,
        Wall = 0,
        Path = 1,
    }

    // 迷路生成完了時のイベント（GameManagerが購読）
    public System.Action OnMazeCreated;

    public void CreateMaze()
    {
        mazeInfoArr = new int[_width, _width];
        mazeCellArr = new GameObject[_width, _width];

        CreateMazeInfo();
        CreateMazeObject();
    }

    // 迷路を再生成
    public void ReCreate()
    {
        CreateMazeInfo();
        SetCellColor();
        OnMazeCreated?.Invoke();
    }

    // グリッド座標をワールド座標へ変換する
    public Vector3 GridToWorld(int row, int col, float z = 0f)
    {
        float xOffset = -(_width / 2) * CellSize;
        float yOffset = (_width / 2) * CellSize;

        float x = xOffset + col * CellSize;
        float y = yOffset - row * CellSize;

        return new Vector3(x, y, z);
    }


    // 棒倒し法で作成
    private void CreateMazeInfo()
    {
        // 外周と中の柱を壁に初期化
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                // 外周をWallにする
                if (i == 0 || j == 0 || i == _width - 1 || j == _width - 1)
                    mazeInfoArr[i, j] = (int)MazeType.Wall;

                // 中の[偶数, 偶数]マスをWall（柱）にする
                else if (i % 2 == 0 && j % 2 == 0)
                    mazeInfoArr[i, j] = (int)MazeType.Wall;

                // それ以外は通路
                else mazeInfoArr[i, j] = (int)MazeType.Path;

            }
        }

        // 棒倒し
        for (int i = 2; i < _width - 1; i += 2)
        {
            for (int j = 2; j < _width - 1; j += 2)
            {
                while (true)
                {
                    // 最初だけは上も含める、それ以外は上を除いた3方向
                    int random;
                    if (i == 2) random = UnityEngine.Random.Range(1, 4);
                    else random = UnityEngine.Random.Range(2, 5);

                    int targetI = i;
                    int targetJ = j;

                    switch (random)
                    {
                        case 1: targetI++; break; // 上
                        case 2: targetJ++; break; // 右
                        case 3: targetJ--; break; // 左
                        case 4: targetI--; break; // 下
                    }

                    // 倒した先がすでにWallでなければ、壁を設置
                    if (mazeInfoArr[targetI, targetJ] != (int)MazeType.Wall)
                    {
                        mazeInfoArr[targetI, targetJ] = (int)MazeType.Wall;
                        break;
                    }
                }
            }
        }
    }

    private void CreateMazeObject()
    {
        // オフセットの計算
        float xOffset = -(_width / 2) * CellSize;
        float yOffset = (_width / 2) * CellSize;
        Vector2 cellPos = new Vector2(xOffset, yOffset);

        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                mazeCellArr[i, j] = Instantiate(_mazeCell, cellPos, Quaternion.identity);
                cellPos.x += CellSize;
            }
            cellPos.y -= CellSize;
            cellPos.x = xOffset;
        }

        SetCellColor();
    }

    private void SetCellColor()
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _width; j++)
            {
                mazeCellArr[i, j].GetComponent<MazeCell>().SetMazeType(mazeInfoArr[i, j]);
            }
        }
    }
}
