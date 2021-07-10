using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{

    public class Count
    {
        public int minimum;
        public int maximum;

        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    public int columns = 8;
    public int rows = 8;

    public Count Wallcount = new Count(3, 9);
    public Count foodcount = new Count(1, 5);

    public GameObject exit;
    public GameObject floor;
    public GameObject wall;
    public GameObject OuterWall;
    public GameObject enemy;
    public GameObject food;

    private Transform boardHorder;

    public List<Vector3> gridPositions = new List<Vector3>();
    // Start is called before the first frame update
    void Start()
    {
        BoardSetup();

        InitialiseList();

        LayoutObjectAtRandom(wall, Wallcount.minimum, Wallcount.maximum);

        LayoutObjectAtRandom(food, foodcount.minimum, foodcount.maximum);

        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0), Quaternion.identity);
    }

    void BoardSetup()
    {
        boardHorder = new GameObject("Board").transform;

        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                GameObject toInsutantiate = floor;

                if (x == -1 || x == columns || y == -1 || y == rows)
                {
                    toInsutantiate = OuterWall;
                }

                GameObject instance = Instantiate(toInsutantiate, new Vector3(x, y, 0), Quaternion.identity) as GameObject;

                instance.transform.SetParent(boardHorder);
            }
        }
    }

    void InitialiseList()
    {

        gridPositions.Clear();

        for (int x = 1; x < columns - 1; x++)
        {
            for (int y = 1; y < rows - 1; y++)
            {

                gridPositions.Add(new Vector3(x, y, 0));
            }
        }
    }

    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);

        Vector3 randomPosition = gridPositions[randomIndex];

        gridPositions.RemoveAt(randomIndex);

        return randomPosition;
    }

    void LayoutObjectAtRandom(GameObject tile, int minimum, int maximum)
    {

        int objectCount = Random.Range(minimum, maximum);

        for (int i = 0; i < objectCount; i++)
        {

            Vector3 randomPosition = RandomPosition();

            Instantiate(tile, randomPosition, Quaternion.identity);
        }
    }
    public void SetupScene(int level)
    {
        //外壁と床を作成します
        BoardSetup();

        //グリッド位置のリストをリセットします
        InitialiseList();

        //ランダム化された位置で、最小値と最大値に基づいてランダムな数の壁タイルをインスタンス化します。
        LayoutObjectAtRandom(wall, Wallcount.minimum, Wallcount.maximum);

        //ランダム化された位置で、最小値と最大値に基づいてランダムな数の食品タイルをインスタンス化します。
        LayoutObjectAtRandom(food, foodcount.minimum, foodcount.maximum);

        //対数進行に基づいて、現在のレベル数に基づいて敵の数を決定します
        int enemyCount = (int)Mathf.Log(level, 2f);

        //ランダム化された位置で、最小値と最大値に基づいてランダムな数の敵をインスタンス化します。
        LayoutObjectAtRandom(enemy, enemyCount, enemyCount);

        //ゲームボードの右上隅に出口タイルをインスタンス化します
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
    }
}
    
   
