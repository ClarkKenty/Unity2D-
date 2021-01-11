using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject
{
    public Text foodText;
    public Text treasureText;
    public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;
    int food = 100;
    int treasurep = 0;
    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;
    Animator animator;
    protected override void Start()
    {
        animator = GetComponent<Animator>();
        food = GameManager.instance.playerFoodPoints;
        foodText.text = "Food:" + food;
        treasureText.text = "Treasure:" + treasurep;
        base.Start();
    }

    public void LoseFood(int loss)
    {
        CheckIfGameOver();
    }
    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.tag == "Treasure")
        {
            treasurep += GameManager.instance.castle_fortune[GameManager.instance.currentloc];
            Debug.Log(GameManager.instance.castle_fortune[GameManager.instance.currentloc]);
            treasureText.text = "Treasure:" + treasurep;
            other.gameObject.SetActive(false);
        }
        if (other.tag == "Chest")
        {
            food += pointsPerFood;
            foodText.text = "Food:" + food;
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Soda")
        {
            food += pointsPerSoda;
            foodText.text = "Food:" + food;
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Exit")
        {
            if (other.name == "Exit" || other.name == "Exit2")
            {
                other.name = "0";
            }
            if (GameManager.instance.level % 2 == 0)
            {
                GameManager.instance.previousloc = GameManager.instance.currentloc;
                GameManager.instance.currentloc = int.Parse(other.name);
                GameManager.instance.currentname = GameManager.instance.castle_names[int.Parse(other.name)];
                for(int j = 0;j<GameManager.instance.path_num;j++)
                {
                    Debug.Log(GameManager.instance.currentloc);
                    Debug.Log(GameManager.instance.graph[2*j]);
                    if(GameManager.instance.currentloc==GameManager.instance.graph[2*j] && GameManager.instance.previousloc==GameManager.instance.graph[2*j+1] || GameManager.instance.currentloc==GameManager.instance.graph[2*j+1] && GameManager.instance.previousloc==GameManager.instance.graph[2*j])
                    {
                        Debug.Log("AA");
                        food-=GameManager.instance.path_danger[j];
                    }
                }
            }
            GameManager.instance.tempname = other.name;
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }
        else if (other.tag == "back")
        {
            GameManager.instance.tempname = other.name;
            GameManager.instance.currentloc = GameManager.instance.previousloc;
            GameManager.instance.currentname = GameManager.instance.castle_names[GameManager.instance.currentloc];
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }
    }

    void Restart()
    {
        SceneManager.LoadScene(0);
    }

    void OnDisable()
    {
        GameManager.instance.playerFoodPoints = food;
        GameManager.instance.playerTreasurePoints = treasurep;
    }

    protected override void AttempMove<T>(int xDir, int yDir)
    {
        LoseFood(1);
        foodText.text = "Food:" + food;
        base.AttempMove<T>(xDir, yDir);
        RaycastHit2D hit;
        if (Move(xDir, yDir, out hit))
        {
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        }
        CheckIfGameOver();
    }

    void CheckIfGameOver()
    {
        if (food <= 0)
        {
            SoundManager.instance.musicSource.Stop();
            SoundManager.instance.PlaySingle(gameOverSound);
            GameManager.instance.GameOver();
            return;
        }
        if(GameManager.instance.currentloc == GameManager.instance.castle_num-1 && GameManager.instance.level%2==0)
        {
            SoundManager.instance.musicSource.Stop();
            SoundManager.instance.PlaySingle(gameOverSound);
            GameManager.instance.GameOver();
        }
    }

    protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);
        animator.SetTrigger("playerChop");
    }
    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.gameoverid == 1)
        return;
        if (!GameManager.instance.playerTurn) return;//还在移动，拒绝指令
        int horizontal = 0;
        int vertical = 0;
        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");
        if (horizontal != 0)
            vertical = 0;
        if (horizontal != 0 || vertical != 0)
        {
            GameManager.instance.playerTurn = false;
            AttempMove<Wall>(horizontal, vertical);

        }
    }
}
