using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
    public LayerMask blockingLayer;
    public float moveTime = 1f;
    BoxCollider2D boxCollider2D;
    Rigidbody2D rb2D;
    float inverseMoveTime;

    //初始化
    protected virtual void Start()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        inverseMoveTime = 2f / moveTime;
    }

    protected abstract void OnCantMove<T>(T Component) where T : Component;

    protected IEnumerator SmoothMovement(Vector3 end)
    {//平滑移动
        float sqrRemainDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
            rb2D.MovePosition(newPosition);
            sqrRemainDistance = (transform.position - end).sqrMagnitude;
            yield return null;//下一帧继续执行
        }
        GameManager.instance.playerTurn = true;
    }

    //调用Linecast()进行线性投射，若存在障碍物则平滑移动，否则返回false
    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)//enemymove有问题
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir , yDir );
        boxCollider2D.enabled = false;//防止探测光线被自身碰撞器挡住
        bool b = Physics2D.Linecast(start, end, blockingLayer);//
        hit = Physics2D.Linecast(start, end, blockingLayer);//返回光线撞到的对象信息hit
        boxCollider2D.enabled = true;
        if (hit.transform == null)
        {
            StartCoroutine(SmoothMovement(end));//平滑移动
            return true;
        }
        else { GameManager.instance.playerTurn = true;  }
        return false;
    }

    protected virtual void AttempMove<T>(int xDir, int yDir) where T : Component
    {
        if(GameManager.instance.gameoverid == 1)
        return;
        RaycastHit2D hit;//存储线性投射检测到的障碍物
        bool canMove = Move(xDir, yDir, out hit);//接收方向信息，确定目的地并且判断该点是否存在障碍物
        if (hit.transform == null) return;
        T hitComponent = hit.transform.GetComponent<T>();
        if (!canMove && hitComponent != null)
            OnCantMove(hitComponent);
    }
}
