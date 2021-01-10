using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool doingSetup;
    GameObject levelImage;
    Text levelText;
    public void GameOver()
    {
        levelText.text = "YOU DIED";
        levelImage.SetActive(true);
        enabled = false;
    }

    public float levelStartDelay = 0.2f;
    public float times = 0;
    // Start is called before the first frame update
    void Start(){
        SceneManager.sceneLoaded += LevelWasLoaded;
    }

    void LevelWasLoaded(Scene scene,LoadSceneMode mode)
    {
        level++;
        InitGame();
    }
    public static GameManager instance = null;
    public int playerFoodPoints = 100;
    public bool playerTurn = true;
    BroadManager broadManager;
    public int level ;
    List<Enemy> enemies;
    public bool enemyMoving=false;
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
        Invoke("HideLevelImage",0);
        enemies.Clear();
        broadManager.SetUpScene(level);
    }

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }
}
