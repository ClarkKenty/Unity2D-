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
    public string solution;
    public List<string> list;
    public int castle_num;
    public int bugfix = 0;
    public int path_num;
    public int currentloc;
    public int previousloc;
    public Text solu1;
    public string currentname;
    public List<string> castle_names;
    public List<int> castle_fortune;
    public List<int> path_danger;
    public string tempname;
    public List<int> graph;//1:城堡，2：城堡
    public bool doingSetup;
    int INF = 99999;
    GameObject levelImage;
    public int gameoverid;
    public int[,] cost;
    Text levelText;
    Text gameoverpath;
    public int[] fortunepath;
    public string pathshow;
    HashSet<int> set;
    int pathcount = 0;
    public void GameOver()
    {
        if (playerFoodPoints <= 0)
        {
            levelText.text = "GAME OVER!\nYOU LOSE!";
            gameoverid = 1;
        }
        else
        {
            levelText.text = "到达终点！";
        }
        levelImage.SetActive(true);
        gameoverpath.text = pathshow.ToString();
        enabled = false;
    }

    public int dfs(int origin, int blood)
    {
        set.Add(origin);
        if (origin == castle_num - 1)
        {
            return castle_fortune[castle_num - 1];
        }
        int value = castle_fortune[origin];
        int valueori = castle_fortune[origin];
        int next = -1;
        for (int i = 0; i < castle_num; i++)
        {
            if (cost[origin, i] != INF && (blood - cost[origin, i]) > 0 && !set.Contains(i))
            {
                int ret = dfs(i, blood - cost[origin, i]);
                if (value < ret + valueori)
                {
                    value = ret + valueori;
                    next = i;
                }
            }
        }
        pathcount++;
        if (next != -1)
        {
            fortunepath[origin] = next;
        }
        set.Remove(origin);
        if (next == -1) return -1;
        return value;
    }

    public void graphinit()
    {
        cost = new int[castle_num, castle_num];//邻接矩阵
        for (int a = 0; a < castle_num; a++)
        {
            for (int b = 0; b < castle_num; b++)
            {
                cost[a, b] = INF;
            }
        }
        for (int k = 0; k < graph.ToArray().Length; k += 2)
        {
            cost[graph[k], graph[k + 1]] = path_danger[k / 2];
            cost[graph[k + 1], graph[k]] = path_danger[k / 2];
        }//ok
    }
    public int dijkstra(int kk)
    {   //从源点到目标点 
        father = new int[castle_num];
        father[0] = -1;

        bool[] used = new bool[castle_num];//节点是否被访问
        int[] d = new int[castle_num];      //表示源点到i这个点的距离
        for (int i = 0; i < castle_num; i++)
        {   //初始化 
            used[i] = false;   //一开始每个点都没被访问 
            d[i] = INF;
        }
        d[0] = 0;
        while (true)
        {
            int v = -1;
            for (int u = 0; u < castle_num; u++)//从尚未用过的顶点选一个最小的顶点
            {
                if (!used[u] && (v == -1 || d[u] < d[v])) v = u;
            }
            if (v == -1) break;
            used[v] = true;
            for (int u = 0; u < castle_num; u++)
            {
                if (d[u] > d[v] + cost[v, u])
                {
                    d[u] = d[v] + cost[v, u];
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
    public int level = 1;
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
        graphinit();
        set = new HashSet<int>();
        pathshow += ("路线：" + castle_names[0]);
        fortunepath = new int[castle_num];
        for (int i = 0; i < castle_num; i++)
        {
            fortunepath[i] = -1;
        }
        dijkstra(castle_num - 1);
        dfs(0, 100);
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
        solu1 = GameObject.Find("Solution_Text").GetComponent<Text>();
        gameoverpath = GameObject.Find("gameovertext").GetComponent<Text>();
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
