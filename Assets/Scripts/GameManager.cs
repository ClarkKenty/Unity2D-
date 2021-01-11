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
    public void GameOver()
    {
        if (playerFoodPoints > 0)
            levelText.text = "GAME OVER!\n到达终点！";
        else
        {
            levelText.text = "GAME OVER!\nYOU LOSE!";
            gameoverid =1;
        }
        levelImage.SetActive(true);
        enabled = false;
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
