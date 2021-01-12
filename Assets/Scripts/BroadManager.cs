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
    public Count foodCount = new Count(1, 4);
    public int rows = 8;
    public int columns = 8;
    public GameObject[] floorTiles;
    public GameObject[] OuterWallTiles;

    public GameObject[] exitTile;

    public GameObject[] wallTiles;

    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject background;
    public GameObject treasurechest;
    Text tribenum;
    Text tribemes;
    Text solu;
    Transform boardHolder;
    List<Vector3> gridPositions = new List<Vector3>();//所有可用位置数组
    //生成关卡
    public void SetUpScene(int level)
    {
        tribenum = GameObject.Find("tribeinfo").GetComponent<Text>();
        tribemes = GameObject.Find("numText").GetComponent<Text>();
        solu = GameObject.Find("Solution_Text").GetComponent<Text>();
        solu.text=GameManager.instance.solution;
        if (level % 2 == 0)
        {
            if(level!=2)
            {
                GameManager.instance.pathshow+=("->" + GameManager.instance.castle_names[GameManager.instance.currentloc]);
            }
            tribenum.text = "部落:" + GameManager.instance.castle_names[GameManager.instance.currentloc];
            rows = 15;
            columns = 22;
            BoardSetup();
            List<int> dest = new List<int>();
            for (int i = 0; i < GameManager.instance.graph.ToArray().Length; i += 2)
            {
                if (GameManager.instance.graph[i] == GameManager.instance.currentloc)
                    dest.Add(GameManager.instance.graph[i + 1]);
                if (GameManager.instance.graph[i + 1] == GameManager.instance.currentloc)
                    dest.Add(GameManager.instance.graph[i]);
            }
            int iii=1;
            for (int i = 0; i < dest.ToArray().Length; i++)
            {
                GameObject instance = Instantiate(exitTile[0], new Vector3(columns - 1, rows - 1 - i * 3, 0f), Quaternion.identity) as GameObject;
                instance.name = dest[i].ToString();
                tribemes.text += "出口"+iii++.ToString()+"：通往"+"部落：" + GameManager.instance.castle_names[dest[i]] + "\n\n\n";
            }
            InitialiseList();
            LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
            int enemyCount2 = (int)Mathf.Log(level, 2f);
            Instantiate(treasurechest, new Vector3(10f,7f, 0f), Quaternion.identity);
            return;
        }
        rows = 6;
        columns = 15;
        BoardSetup();
        GameObject instance2 = Instantiate(exitTile[0], new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity) as GameObject;
        instance2.name = GameManager.instance.tempname;
        if (level != 1)
        {
           GameObject exitback = Instantiate(exitTile[1], new Vector3(-1, 0, 0f), Quaternion.identity) as GameObject;
           exitback.name = GameManager .instance.currentname;
        }
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
                if(GameManager.instance.level%2==0 && x == columns -1 && y== -1) 
                {
                    Instantiate(wallTiles[0] ,new Vector3(x,y,0f), Quaternion.identity);
                    continue;
                }
                if(GameManager.instance.level%2==0 && x == columns -2 && y== -1) 
                {
                    Instantiate(wallTiles[0] ,new Vector3(x,y,0f), Quaternion.identity);
                    continue;
                }
                if (GameManager.instance.level % 2 == 0)
                {
                    GameObject instance2 = Instantiate(background, new Vector3(11f, 7.5f, 0f), Quaternion.identity) as GameObject;
                    Instantiate(OuterWallTiles[0], new Vector3(-1f,0f,0f), Quaternion.identity);
                    instance2.transform.SetParent(boardHolder);
                }
                if (GameManager.instance.level == 1)
                {
                    GameObject toInstantiate = OuterWallTiles[UnityEngine.Random.Range(0, OuterWallTiles.Length)];//外墙
                    GameObject instance = Instantiate(toInstantiate, new Vector3(-1, 0, 0f), Quaternion.identity) as GameObject;//实例化
                    instance.transform.SetParent(boardHolder);

                }
                if ((x == -1 && y != 0) || x == columns || y == -1 || y == rows)
                {
                    GameObject toInstantiate = OuterWallTiles[UnityEngine.Random.Range(0, OuterWallTiles.Length)];//外墙
                    GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;//实例化
                    instance.transform.SetParent(boardHolder);

                }
                else if (GameManager.instance.level % 2 != 0)
                {
                    GameObject toInstantiate = floorTiles[UnityEngine.Random.Range(0, floorTiles.Length)];//地板
                    GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;//实例化
                    instance.transform.SetParent(boardHolder);
                }
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
