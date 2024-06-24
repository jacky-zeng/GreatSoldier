using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeiShen : BaseEnemy
{
    private bool isBegin = false;

    private void Awake()
    {
        BaseAwake();
    }

    #region 初始化
    void Start()
    {
        enemyAvatarSprite = Resources.Load<Sprite>("Images/Enemy/LeiShen/enemyAvatar") as Sprite;
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



        if (isBegin)
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
        }
        else if (hitDamage > 0)  //leishen受伤后，才会开始攻击
        {
            isBegin = true;
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

    public void animatorAttackEvent()
    {
        playAudio("Audios/Enemy/LeiShen/knifeAttack");
    }

    public void animatorAttack1Event()
    {
        playAudio("Audios/Enemy/LeiShen/girlKnifeAttack");
    }

    public void animatorJumpAttackEvent()
    {
        playAudio("Audios/Enemy/LeiShen/girlJumpAttack");
    }

    public void animatorJumpAttackGroundEvent()
    {
        playAudio("Audios/Enemy/LeiShen/knifeHitGround");
    }

    //移动
    private void walk()
    {
        Vector3 targetPosition = player.transform.position;

        // 获取物体当前位置
        Vector3 currentPosition = transform.position;

        Vector3 distance = targetPosition - currentPosition;

        if (Mathf.Abs(distance.x) >= 5f /*|| Mathf.Abs(distance.y) > 1.5f*/|| Mathf.Abs(distance.z) > 2f)
        {
            if(Random.Range(1, 19) % 3 == 0 && Mathf.Abs(distance.x) >= 12f && Mathf.Abs(distance.x) <= 20f)
            {
                jumpAttack();
            } else if(isJumpAttack == false)
            {
                //敌人朝向
                spriteRenderer.flipX = targetPosition.x < currentPosition.x ? true : false;
                //停止攻击
                if (isAttack)
                {
                    stopAttack();
                }
                if (isAttackHeavy)
                {
                    stopAttackHeavy();
                }
                if (isJumpAttack)
                {
                    stopJumpAttack();
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
        }
        else  //离player近，可以攻击
        {
            //停止移动
            stopWalk();

            //开始攻击
            int randomInt = Random.Range(1, 19);
            if (randomInt % 2 == 0 || randomInt % 3 == 0)
            {
                //Debug.Log("normal attack isAttack = " + isAttack);
                attack();  //普通攻击
            }
            else
            {
                //Debug.Log("attackHeavy isAttackHeavy = " + isAttackHeavy);
                attackHeavy(); //重击
            }
        }
    }

    //攻击
    private void attack()
    {
        //开始攻击
        if (!isAttack && !isAttackHeavy && !isJumpAttack)
        {
            isAttack = true;

            //使攻击的朝向和主体保持一致
            emenyAttackObj.transform.localScale = new Vector3(spriteRenderer.flipX ? -1 : 1, 1, 1);
            isJumpHit = false;
            isHitOnGround = false;
            animator.SetBool("isAttack", true);
            Invoke("stopAttack", 3.5f);
        }
    }

    private void attackHeavy()
    {
        //开始攻击
        if (!isAttackHeavy && !isAttack && !isJumpAttack)
        {
            isAttackHeavy = true;

            //使攻击的朝向和主体保持一致
            emenyAttackObj.transform.localScale = new Vector3(spriteRenderer.flipX ? -1 : 1, 1, 1);
            isJumpHit = false;
            isHitOnGround = false;
            animator.SetBool("isAttackHeavy", true);
            Invoke("stopAttackHeavy", 3.5f);
        }
    }

    //起跳攻击
    private void jumpAttack()
    {
        //开始攻击
        if (!isAttackHeavy && !isAttack && !isJumpAttack)
        {
            isJumpAttack = true;

            //使攻击的朝向和主体保持一致
            emenyAttackObj.transform.localScale = new Vector3(spriteRenderer.flipX ? -1 : 1, 1, 1);

            //给一个瞬间当前朝向和向上的力
            rigi.AddForce(new Vector3(spriteRenderer.flipX ? -60 : 60, 260, 0), ForceMode.Impulse);

            isJumpHit = false;
            isHitOnGround = false;
            animator.SetBool("isJumpAttack", true);
            Invoke("stopJumpAttack", 3.5f);
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
        dieDestroy();
    }
}
