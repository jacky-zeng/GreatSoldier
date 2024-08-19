using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ghost : BaseEnemy
{
    private bool isBegin = false;
    private bool isBeginReal = false;
    private float beginRealAfterTime = 3.5f;

    private Vector3 targetPosition;
    private float walkMaxDuringTime = 3f;  //移动持续最大时间
    private float walkDuringTime = 0;      //移动已持续时间

    private void Awake()
    {
        BaseAwake();
    }

    #region 初始化
    void Start()
    {
        player = GameObject.Find("Player");
        enemyAvatarSprite = Resources.Load<Sprite>("Images/Enemy/Ghost/enemyAvatar") as Sprite;
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        //if (isHit || isUpHit || isJumpHit || isAttackUp3 || isHitOnGround)
        //{
        //    //血条改变
        //    bloodBar.fillAmount = Mathf.Lerp(bloodBar.fillAmount, (maxBlood - hitDamage) / maxBlood, hitDamage * Time.deltaTime);
        //}

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
                    if(walkDuringTime == 0)
                    {
                        targetPosition = player.transform.position; //获取的是walkMaxDuringTime前的player所在位置
                        if (Random.Range(1, 99) > 50)
                        {
                            targetPosition.x += 5;
                        }
                    }

                    if ( walkDuringTime >= walkMaxDuringTime)
                    {
                        walkDuringTime = 0;
                    } else
                    {
                        walkDuringTime += Time.deltaTime;
                    }
                    
                    // 获取物体当前位置
                    Vector3 currentPosition = transform.position;

                    Vector3 distance = targetPosition - currentPosition;

                    if (Mathf.Abs(distance.x) >= 2.7f || Mathf.Abs(distance.y) > 1.5f || Mathf.Abs(distance.z) > 2f)
                    {
                        walk(currentPosition, targetPosition);
                    }
                    else  //离player近，可以攻击
                    {
                        //停止移动
                        stopWalk();
                        //开始攻击
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

    public void changeAnimatorStatus(int type)
    {
        switch(type)
        {
            case 1:
                //使用默认动画xx
                break;
            case 2:
                beginRealAfterTime = 0.1f;
                //使用walk动画
                animator.SetBool("isWalk", true);
                break;
            case 3:
                //使用land动画 （躲井盖）
                animator.SetBool("isLand", true);
                break;
            default:break;
        }
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
    private void walk(Vector3 currentPosition, Vector3 targetPosition)
    {
        //敌人朝向
        spriteRenderer.flipX = targetPosition.x < currentPosition.x ? true : false;
        //停止攻击
        if (isAttack)
        {
            stopAttack();
        }
        if (!isWalk)
        {
            isWalk = true;
            isJumpHit = false;
            isHitOnGround = false;
            animator.SetBool("isWalk", true);
        }

        AnimatorStateInfo tempAnimatorClipName = GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);

        if (tempAnimatorClipName.IsName("walk")) //不能用animator.GetBool("isWalk")判断，因为虽然设置了isWalk，但是上一个动画有exitTime
        {
            // 计算移动的方向和距离
            Vector3 directionToTarget = (targetPosition - currentPosition).normalized;
            Vector3 moveDirection = directionToTarget * moveSpeed * Time.deltaTime;

            //Debug.Log("移动物体 靠近Player");
            // 移动物体
            transform.position = Vector3.MoveTowards(currentPosition, targetPosition, moveDirection.magnitude);
        }
    }

    //攻击
    private void attack()
    {
        //开始攻击
        if (!isAttack)
        {
            //使攻击的朝向和主体保持一致
            emenyAttackObj.transform.localScale = new Vector3(spriteRenderer.flipX ? -1 : 1, 1, 1);
            isJumpHit = false;
            isHitOnGround = false;
            isAttack = true;
            animator.SetBool("isAttack", true);
            Invoke("stopAttack", 3.5f);
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

        CapCollider.enabled = false;
        playAudio("Audios/Tool/manHit");

        StartCoroutine(diedDelay(1));
    }

}
