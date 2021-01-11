using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Text;
using System;
public class GameManager : MonoBehaviour
{
    public int[] father;
    public List<string> list;
    public int castle_num;
    public int bugfix = 0;
    public int path_num;
    public int currentloc;
    public int previousloc;
    public string currentname;
    public List<string> castle_names;
    public List<int> castle_fortune;
    public List<int> path_danger;
    public string tempname;
    public List<int> graph;//1:城堡，2：城堡
    public bool doingSetup;
    GameObject levelImage;
    public int gameoverid;
    Text levelText;
    void findbestpath()
    {
        List<int> contains = new List<int>();
        int[] path = new int[castle_num];
        for (int i = 0; i < path.Length; i++)
        {
            path[i] = -1;
        }
        contains.Add(0);
        int[] distances = new int[castle_num];//顶点集到每个点的距离
        for (int i = 0; i < distances.Length; i++)
        {
            distances[i] = 9999;
        }
        distances[0] = -1;
        while (contains.ToArray().Length != castle_num)
        {
            int currentmin = 9999;
            int pos = 0;
            for (int i = 0; i < contains.ToArray().Length; i++)//找出当前顶点集合到其余点的最短距离
            {
                for (int j = 0; j < graph.ToArray().Length; j += 2)
                {
                    if (contains[i] == graph[j] && !contains.Contains(graph[j + 1]))//contains[i]到graph[j+1]有通路.
                    {
                        int dis = path_danger[j / 2];
                        if (distances[graph[j + 1]] > dis)//当前顶点集到graph[j+1]的距离
                        {
                            path[graph[j + 1]] = contains[i];
                            distances[graph[j + 1]] = dis;
                        }
                    }
                    else if (contains[i] == graph[j + 1] && !contains.Contains(graph[j]))//contains[i]集到graph[j]有通路.
                    {
                        int dis = path_danger[j / 2];
                        if (distances[graph[j]] > dis)
                        {
                            path[graph[j]] = contains[i];//入度
                            distances[graph[j]] = dis;
                        }
                    }
                }
            }

            for (int i = 0; i < distances.Length; i++)
            {
                if (distances[i] < currentmin)
                {
                    currentmin = distances[i];
                    pos = i;
                }
            }
            distances[pos] = -1;
            contains.Add(pos);
        }
    }


    public void GameOver()
    {
        if (playerFoodPoints > 0)
            levelText.text = "到达终点！";
        else
        {
            levelText.text = "GAME OVER!\nYOU LOSE!";
            gameoverid = 1;
        }
        levelImage.SetActive(true);
        enabled = false;
    }

    public int dijkstra(int kk)
    {   //从源点到目标点 
        int INF = 99999;
        father = new int[castle_num];
        father[0] = -1;
        int[,] cost = new int[castle_num, castle_num];//邻接矩阵
        for (int a = 0; a < castle_num; a++)
        {
            for (int b = 0; b < castle_num; b++)
            {
                cost[a, b] = INF;
            }
        }
        for (int k = 0; k < graph.ToArray().Length; k+=2)
        {
            cost[graph[k], graph[k + 1]] = path_danger[k / 2];
            cost[graph[k + 1], graph[k]] = path_danger[k / 2];
        }//ok
        bool [] used = new bool[castle_num];//节点是否被访问
        int[] d = new int[castle_num];      //表示源点到i这个点的距离
        for (int i = 0; i < castle_num; i++)
        {   //初始化 
            used[i] = false;   //一开始每个点都没被访问 
            d[i] = INF;  //先假设源点到其他点的距离 
        }
        d[0]=0;
        while(true)
        {
            int v = -1;
            for(int u = 0;u<castle_num;u++)//从尚未用过的顶点选一个最小的顶点
            {
                if(!used[u] && (v == -1 || d[u] < d[v])) v = u;
            }
            if(v==-1) break;
            used[v] = true;
            for(int u = 0;u<castle_num;u++)
            {
                if(d[u]>d[v]+cost[v,u])
                {
                    d[u] = d[v]+cost[v,u];
                    father[u] = v;
                }
            }
        }
        return d[kk];
    }

    public void ReadFileList()
    {
        StreamReader sr;
        sr = File.OpenText("D:\\qq" + "//" + "map.txt");
        list = new List<string>();
        string str;
        while ((str = sr.ReadLine()) != null)
            list.Add(str);
        sr.Close();
        sr.Dispose();
        string[] line = list[0].Split(' ');
        castle_num = int.Parse(line[0]);
        path_num = int.Parse(line[1]);
        for (int i = 1; i < 1 + castle_num; i++)
        {
            line = list[i].Split(' ');
            castle_names.Add(line[0]);
            castle_fortune.Add(int.Parse(line[1]));
        }
        for (int i = 1 + castle_num; i < list.ToArray().Length; i++)
        {
            line = list[i].Split(' ');
            graph.Add(int.Parse(line[0]));
            graph.Add(int.Parse(line[1]));
            path_danger.Add(int.Parse(line[2]));
        }
    }
    public float levelStartDelay = 0.2f;
    public float times = 0;
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.sceneLoaded += LevelWasLoaded;
    }

    void LevelWasLoaded(Scene scene, LoadSceneMode mode)
    {
        level++;
        InitGame();
    }
    public static GameManager instance = null;
    public int playerFoodPoints = 100;
    public int playerTreasurePoints = 0;
    public bool playerTurn = true;
    BroadManager broadManager;
    public int level;
    List<Enemy> enemies;
    public bool enemyMoving = false;
    public float turnDelay = 0.1f;

    void Update()
    {
        return;
        if (enemyMoving) return;
        enemyMoving = true;
        MoveEnemys();
    }

    void MoveEnemys()
    {
        enemyMoving = true;
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
        }
    }

    void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
        ReadFileList();
        dijkstra(castle_num-1);
        DontDestroyOnLoad(transform.gameObject);
        enemies = new List<Enemy>();
        broadManager = GetComponent<BroadManager>();
        InitGame();
    }

    void InitGame()
    {
        doingSetup = true;
        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        //levelText.text = "Day " +level;
        //levelImage.SetActive (true);
        Invoke("HideLevelImage", 0);
        enemies.Clear();
        broadManager.SetUpScene(level);
    }

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }
}
