using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ghost : BaseEnemy
{
    private bool isBegin = false;

    #region 初始化
    void Start()
    {
        BaseStart();
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

        if(isBegin)
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
                    walk();
                }
            }
        } else
        {
            Invoke("begin", 5);
        }
    }

    private void begin()
    {
        isBegin = true;
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
        Vector3 targetPosition = player.transform.position;

        // 获取物体当前位置
        Vector3 currentPosition = transform.position;

        Vector3 distance = targetPosition - currentPosition;

        if (Mathf.Abs(distance.x) >= 2.7f || Mathf.Abs(distance.y) > 1.5f || Mathf.Abs(distance.z) > 2f)
        {
            if (!isWalk)
            {
                isWalk = true;
                isJumpHit = false;
                isHitOnGround = false;
                animator.SetBool("isWalk", true);
            }
            //停止攻击
            if (isAttack)
            {
                stopAttack();
            }
            //敌人朝向
            spriteRenderer.flipX = targetPosition.x < currentPosition.x ? true : false;

            // 计算移动的方向和距离
            Vector3 directionToTarget = (targetPosition - currentPosition).normalized;
            Vector3 moveDirection = directionToTarget * moveSpeed * Time.deltaTime;

            //Debug.Log("移动物体 靠近Player");
            // 移动物体
            transform.position = Vector3.MoveTowards(currentPosition, targetPosition, moveDirection.magnitude);
        }
        else  //离player近，可以攻击
        {
            //停止移动
            stopWalk();
            //开始攻击
            attack();
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
    }
}
