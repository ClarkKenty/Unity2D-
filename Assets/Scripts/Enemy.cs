//该脚本控制敌人的移动和攻击行为（暂未完善）
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject//Enemy继承自MovingObject对象
{

    private Animator animator;//动画控制器
    bool skipMove;
    public int playerDamage = 1;

    protected override void OnCantMove<T>(T Component)
    {
        Player hitPlayer = Component as Player;
        hitPlayer.LoseFood(playerDamage);
    }
    Transform target;
    // Start is called before the first frame update
    protected override void Start()
    {
        playerDamage = 0;
        GameManager.instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
    }

    // Update is called once per frame
    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;
        if(Mathf.Abs (target.position.x - transform.position.x) < float.Epsilon)

                //If the y coordinate of the target's (player) position is greater than the y coordinate of this enemy's position set y direction 1 (to move up). If not, set it to -1 (to move down).
                yDir = target.position.y > transform.position.y ? 1 : -1;

            //If the difference in positions is not approximately zero (Epsilon) do the following:
            else
                //Check if target x position is greater than enemy's x position, if so set x direction to 1 (move right), if not set to -1 (move left).
                xDir = target.position.x > transform.position.x ? 1 : -1;
        AttempMove<Player>(xDir, yDir);
    }

    protected override void AttempMove<T>(int xDir, int yDir)
    {
        base.AttempMove<T>(xDir, yDir);
    }
}
