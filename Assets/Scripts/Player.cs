﻿//该脚本控制主角的行为
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject
{
    public Text foodText;
    public Text treasureText;
    public Text endText;

    public int wallDamage = 1;
    public int pointsPerFood = 5;
    public int pointsPerSoda = 5;
    public float restartLevelDelay = 1f;
    int food = 100;
    int treasurep = 0;
    public AudioClip moveSound1;//移动声音
    public AudioClip moveSound2;
    public AudioClip eatSound1;//吃东西的声音
    public AudioClip eatSound2;
    public AudioClip drinkSound1;//喝饮料的声音
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;//死亡声音
    public int EnemyDamage = 10;
    Animator animator;
    protected override void Start()
    {
        animator = GetComponent<Animator>();
        food = GameManager.instance.playerFoodPoints;
        treasurep = GameManager.instance.playerTreasurePoints;
        foodText.text = "Health:" + food;
        treasureText.text = "Treasure:" + treasurep;
        endText.text = "Destination:" + GameManager.instance.castle_names[GameManager.instance.castle_num - 1];
        base.Start();
    }


    public void LoseFood(int loss)
    {
        CheckIfGameOver();
    }
    void OnTriggerEnter2D(Collider2D other)//玩家触碰到其他实物
    {

        if (other.tag == "Treasure")//该实物为宝藏
        {
            GameManager.instance.taketreasure[GameManager.instance.currentloc] = true;
            treasurep += GameManager.instance.castle_fortune[GameManager.instance.currentloc];//加钱
            treasureText.text = "Treasure:" + treasurep;
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Enemy")//该实物为敌人
        {
            food -= EnemyDamage;
            foodText.text = "Health:" + food;
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Soda")//该实物为饮料
        {
            food += pointsPerSoda;
            foodText.text = "Health:" + food;//加血
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Food")//该实物为食物
        {
            food += pointsPerFood;
            foodText.text = "Health:" + food;//加血
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Exit")//该实物为出口
        {
            if (other.name == "Exit" || other.name == "Exit2")
            {
                other.name = "0";
            }
            if (GameManager.instance.level % 2 == 0)//玩家触碰的时部落中的出口
            {
                GameManager.instance.previousloc = GameManager.instance.currentloc;
                GameManager.instance.currentloc = int.Parse(other.name);
                GameManager.instance.currentname = GameManager.instance.castle_names[int.Parse(other.name)];
                for (int j = 0; j < GameManager.instance.path_num; j++)
                {
                    if (GameManager.instance.currentloc == GameManager.instance.graph[2 * j] && GameManager.instance.previousloc == GameManager.instance.graph[2 * j + 1] || GameManager.instance.currentloc == GameManager.instance.graph[2 * j + 1] && GameManager.instance.previousloc == GameManager.instance.graph[2 * j])
                    {
                        food -= GameManager.instance.path_danger[j];//进入道路，玩家扣除对应权值的血量
                    }
                }
            }
            GameManager.instance.tempname = other.name;
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }
        else if (other.tag == "back")//该实物为回退出口
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
        foodText.text = "Health:" + food;
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
        GameManager.instance.playerFoodPoints = food;
        if (food <= 0)
        {
            SoundManager.instance.musicSource.Stop();
            SoundManager.instance.PlaySingle(gameOverSound);
            GameManager.instance.GameOver();
            return;
        }
        if (GameManager.instance.currentloc == GameManager.instance.castle_num - 1 && GameManager.instance.level % 2 == 0)
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
    void Update()//接收玩家的键盘输入，角色进行移动
    {
        if (GameManager.instance.gameoverid == 1)
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
