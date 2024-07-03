using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KunBasketBall : BaseEnemy
{
    private bool isBegin = false;
    private bool isBeginReal = false;
    private float beginRealAfterTime = 3.5f;

    private float walkMaxDuringTime = 0.6f; //移动持续最大时间
    private float walkDuringTime = 0;       //移动已持续时间

    public GameObject basketball;

    private void Awake()
    {
        BaseAwake();
    }

    #region 初始化
    void Start()
    {
        player = GameObject.Find("Player");
        enemyAvatarSprite = Resources.Load<Sprite>("Images/Enemy/EnemyKunBasketBall/enemyAvatar") as Sprite;

        begin();
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        if(isBeginReal)
        {
            if (isDie)
            {
                //已死亡
            }
            else if (hitDamage >= maxBlood) //死亡
            {
                die();
            }
            else
            {
                if (isHit)
                {
                    byHit();
                }
                else if (isUpHit)
                {
                    byUpHit();
                }
                else if (!isHitOnGround && !isJumpHit)
                {
                    walkDuringTime += Time.deltaTime;
                    if ( walkDuringTime < walkMaxDuringTime)
                    {
                        walk();
                    } else
                    {
                        attack();
                    } 
                    
                }
            }
        }
    }

    public void begin()
    {
        if(!isBegin)
        {
            isBegin = true;
            Invoke("beginReal", beginRealAfterTime); //延时，为了player能够看到开始时的敌人动画
        }
    }

    private void beginReal()
    {
        if (player == null)
        {
            player = GameObject.Find("Player");
        }
        isBeginReal = true;
    }

    #region 碰撞
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Ground")
        {
            OnCollisionEnterGround();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        OnTriggerEnterOrStay(other, "Stay");
    }

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterOrStay(other, "Enter");
    }
    #endregion

    //移动
    private void walk()
    {
        if(!isWalk)
        {
            isWalk = true;
            isJumpHit = false;
            isHitOnGround = false;
            animator.SetBool("isWalk", true);
        }
        
    }

    //攻击
    private void attack()
    {
        //开始攻击
        if (!isAttack)
        {
            isAttack = true;
            isJumpHit = false;
            isHitOnGround = false;
            animator.SetBool("isAttack", true);
        }
    }

    public void animatorShotEvent()
    {
        if (isAttack)
        {
            //篮球位置
            Vector3 pos = transform.position;
            pos.x += 4.5f;
            //pos.y += 2.5f;
            GameObject bulletBasketball = ObjectPool.Instance.Get(basketball, pos, Vector3.one, true);

            //改变旋转角度
            bulletBasketball.GetComponent<BasketBall>().setRotation(0);

            stopAttack();
            stopWalk();
            walkMaxDuringTime = 2;
            walkDuringTime = 0;
        }
    }

    //死亡
    private void die()
    {
        //调用管理器，设置敌人死亡
        GameManager.instance.sectionEnemyDied(gameObject.name);

        isBegin = false;
        isDie = true;
        //rigi.isKinematic = true;
        //CapCollider.enabled = false;
        stopHit();
        stopUpHit();
        //隐藏血条对象
        enemyHealthObj.SetActive(false);
        ////敌人静止
        //rigi.velocity = new Vector3(0, 0, 0);
        ////敌人摆正
        //transform.rotation = Quaternion.Euler(0, 0, 0);
        animator.SetTrigger("triggerDie");

        dieDestroy();
    }
}
