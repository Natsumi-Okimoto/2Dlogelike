using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{

    public class Count
    {
        public int minmum;
        public int maximum;

        public Count(int min, int max)
        {
            minmum = min;
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

        LayoutObjectAtRandom(wall, Wallcount.minmum, Wallcount.maximum);

        LayoutObjectAtRandom(food, foodcount.minmum, foodcount.maximum);

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

                GameObject instance = Instantiate(toInsutantiate, new Vector3(x, y, 0), Quaternion.identity);

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
}
    // Update is called once per frame
   
