using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
public class BroadManager : MonoBehaviour
{
    [Serializable]
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

    public Count wallCount = new Count(5, 9);
    public Count foodCount = new Count(1, 5);
    public int rows = 8;
    public int columns = 8;
    public GameObject[] floorTiles;
    public GameObject[] OuterWallTiles;

    public GameObject[] exitTile;

    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    Transform boardHolder;
    List<Vector3> gridPositions = new List<Vector3>();//所有可用位置数组
    //生成关卡
    public void SetUpScene(int level)
    {
        if (level % 2 == 0)
        {
            rows = 8;
            columns = 8;
            BoardSetup();
            Instantiate(exitTile[0], new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
            Instantiate(exitTile[1], new Vector3(0, rows - 1, 0f), Quaternion.identity);
            InitialiseList();
            LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
            LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
            int enemyCount2 = (int)Mathf.Log(level, 2f);
            LayoutObjectAtRandom(enemyTiles, enemyCount2, enemyCount2);
            return;
        }
        rows = 8;
        columns = 15;
        BoardSetup();
        Instantiate(exitTile[0], new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
        Instantiate(exitTile[1], new Vector3(0, rows - 1, 0f), Quaternion.identity);
        InitialiseList();
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
        int enemyCount = (int)Mathf.Log(level, 2f);
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);

    }

    //生成所有可用位置数组
    void InitialiseList()
    {
        gridPositions.Clear();
        for (int x = 1; x < columns - 1; x++)
        {
            for (int y = 1; y < rows - 1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    //铺设地板floor,判断如果是四条边沿坐标则铺设外墙OuterWall.
    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;
        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                GameObject toInstantiate = floorTiles[UnityEngine.Random.Range(0, floorTiles.Length)];//地板
                if (x == -1 || x == columns || y == -1 || y == rows)
                    toInstantiate = OuterWallTiles[UnityEngine.Random.Range(0, OuterWallTiles.Length)];//外墙
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;//实例化
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    Vector3 RandomPosition()//从gridpositions数组取得随机坐标
    {
        int randomIndex = UnityEngine.Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);//避免重复抽到坐标
        return randomPosition;
    }

    //根据数量区间获取随机数，生成指定资源和数量在随机的位置
    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = UnityEngine.Random.Range(minimum, maximum + 1);
        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[UnityEngine.Random.Range(0, tileArray.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }
}
