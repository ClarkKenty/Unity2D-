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
    public int[,] cost;//cost为邻接矩阵,castle_num为城堡数量，即顶点数
    Text levelText;
    Text gameoverpath;
    public int[] fortunepath;
    public string pathshow;
    HashSet<int> set;
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
    public int dfs(int origin, int blood)//origin为起点，blood为初始血量100
    {
        set.Add(origin);//标记该点已经进入过递归
        if (origin == castle_num - 1)//最内层的递归结束标志，表示递归已经进行到了终点，该线程的递归结束。
        {
            return castle_fortune[castle_num - 1];//返回这个顶点的权值，即城堡的财富值
        }
        int value = castle_fortune[origin];//value为当前已获得的财富，初始化为该点权值。
        int valueori = castle_fortune[origin];//存储一下该点的权值
        int next = -1;
        for (int i = 0; i < castle_num; i++)//对其余符合条件的顶点进行dfs()
        {
            if (cost[origin, i] != INF && (blood - cost[origin, i]) > 0 && !set.Contains(i))//若origin到顶点i存在路径 && 进入通往顶点i的路径不会让角色死亡 && 该顶点没有在上层dfs递归过(防止路径含有环) ，则该点进入dfs()递归
            {
                int ret = dfs(i, blood - cost[origin, i]);//dfs(（顶点i）和（进入从origin到i的路径后所剩余的生命值） )，并将返回值赋给ret，该返回值为从 i 到终点（在不死情况下）的最长路径
                if (value < ret + valueori)//更新从origin通过顶点 i 到终点的最长路径，
                {
                    value = ret + valueori;
                    next = i;//若更新成功，则将 i 赋给next，表示在最长路径中，origin之后的下一个顶点为 i (用于以后的路径还原)
                }
            }
        }
        if (next != -1)//若next!=-1，则表示该点存在通过next到达终点的最长路径
        {
            fortunepath[origin] = next;//（用于路径还原）表示路径中origin 的下一个顶点为 next
        }
        set.Remove(origin);//该dfs运行结束，将该点从集合中取出
        if (next == -1) return -1;//如果next==-1，则表示没有进行过更新，不存在从该点到终点（在不死亡情况下）的路径
        return value;//返回值为从该点到终点所有已获得的财富
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
        }
    }
    public int dijkstra(int kk)
    {   //从源点到目标点 
        father = new int[castle_num];//路径还原数组
        father[0] = -1;
        bool[] used = new bool[castle_num];//节点是否被访问
        int[] d = new int[castle_num];      //当前顶点集到其余顶点的最短距离
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
            for (int u = 0; u < castle_num; u++)//更新到其余顶点的最短距离
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
    public bool[] taketreasure;

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
        taketreasure = new bool[castle_num];
        for(int i=0;i<castle_num;i++)
        {
            taketreasure[i] = false;
        }
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
