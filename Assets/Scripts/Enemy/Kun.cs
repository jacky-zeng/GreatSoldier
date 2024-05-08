using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Kun : MonoBehaviour
{
    #region 变量定义
    private Animator animator;
    public GameObject player;
    public GameObject hitPoint;
    private AnimatorStateInfo animatorClipName = new AnimatorStateInfo();
    private Rigidbody rigi;
    private SpriteRenderer spriteRenderer;
    public GameObject hitObj;
    public GameObject hit2Obj;
    public GameObject emenyAttackObj;
    private float hitDuringTime = 0;
    private float hitMaxDuringTime = 0.6f;

    private float upHitDuringTime = 0;
    private float upHitMaxDuringTime = 0.05f;

    private float upHitAngleDuringTime = 0;
    private float upHitAngleMaxDuringTime = 1.2f;

    Vector2 direction = Vector2.one;

    private AudioSource audioSource;

    private CapsuleCollider CapCollider;

    //受到了几次伤害
    private float hitDamage = 0;
    //总血量
    public float maxBlood = 10;
    private bool isDie = false;
    //血条对象
    public GameObject enemyHealthObj;
    //血条头像
    public Image enemyAvatar;
    //血条
    public Image bloodBar;

    private bool isWalk = false;
    public float moveSpeed = 3;

    public int JumpHitForce = 260;

    private bool isAttack = false;
    private bool isHit = false;           //是否收到普通伤害
    private bool isUpHit = false;         //是否被上挑
    private bool isJumpHit = false;       //是否被跳起来踢飞
    private bool isAttackUp3 = false;     //是否被上挑挑飞
    public float upPosSpeed = 3;          //被上挑时的离心距离
    private bool isHitOnGround = false;   //是否被打趴在地上

    private float tempAngle = 0;
    private int angleCount = 0;           //已经旋转的次数
    private bool isOnGround = true;       //是否在地面上
    #endregion

    #region 初始化
    void Start()
    {
        animator = GetComponent<Animator>();
        rigi = GetComponent<Rigidbody>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        CapCollider = GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (isHit || isUpHit || isJumpHit || isAttackUp3 || isHitOnGround)
        //{
        //    //血条改变
        //    bloodBar.fillAmount = Mathf.Lerp(bloodBar.fillAmount, (maxBlood - hitDamage) / maxBlood, hitDamage * Time.deltaTime);
        //}

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
    #endregion

    #region 碰撞

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Ground")
        {
            if(!isOnGround)
            {
                isOnGround = true;
                isAttackUp3 = false;
                upHitDuringTime = 0;
                rigi.velocity = Vector2.zero;
                stopUpHit(true);
                isHitOnGround = true;
                Log("进入地面Ground OnCollisionEnter", true);
            }
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

    private void OnTriggerEnterOrStay(Collider other, string type)
    {
        //Debug.Log("EnterOrStay!!!" + other.gameObject.name + "isJumpHit = " + isJumpHit);
        if (other.gameObject.name == "Attack")
        {
            AnimatorStateInfo tempAnimatorClipName = player.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
            bool isChange = false;
          
            if (type == "Enter" && (tempAnimatorClipName.IsName("attackNormal1") || tempAnimatorClipName.IsName("attackUp1")))
            {
                //不知道为啥 attackNormal1 会导致 isChange = true 2次，导致敌人受伤2次
                isChange = true;
                animatorClipName = new AnimatorStateInfo();
            } else
            {
                isChange = tempAnimatorClipName.fullPathHash == animatorClipName.fullPathHash ? false : true;
            }
            
            animatorClipName = tempAnimatorClipName;
            var tempDirection = new Vector2(player.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1);
            if (tempAnimatorClipName.IsName("attackUp1"))
            {
                Debug.Log("attackUp1****");
                upHit(tempDirection, false, isChange);
            }
            else if (tempAnimatorClipName.IsName("attackUp2"))
            {
                Debug.Log("attackUp2****");
                upHit(tempDirection, false, isChange);
            }
            else if (tempAnimatorClipName.IsName("attackUp3"))
            {
                Debug.Log("attackUp3****");
                upHit(tempDirection, true, isChange);
            }
            else if (tempAnimatorClipName.IsName("attackNormal4"))
            {
                Debug.Log("attackNormal4****");
                hit(tempDirection, isChange, 2);
            }
            else
            {
                //Debug.Log("attackNormal****");
                hit(tempDirection, isChange);
            }
        } else if(other.gameObject.name == "jumpAttack")
        {
            if(!isJumpHit)
            {
                isJumpHit = true;
                var tempDirection = new Vector2(player.GetComponent<SpriteRenderer>().flipX ? -1 : 1, 1);
                byJumpHit(tempDirection);
                Debug.Log("******byJumpHit!!!" + " isJumpHit=" + isJumpHit);
                Invoke("cancelStatus", 3.5f);
            }
        }
    }

    #endregion

    //防止特殊情况未执行 animatorHitStandUpEvent， 延时强制改变状态
    private void cancelStatus()
    {
        if (isJumpHit || isHitOnGround)
        {
            Debug.Log("******cancelJumpHit isJumpHit=" + isJumpHit);
            animatorHitStandUpEvent();
        }
    }

    private void die()
    {
        isDie = true;
        rigi.isKinematic = true;
        CapCollider.enabled = false;
        stopHit();
        stopUpHit();
        //隐藏血条对象
        enemyHealthObj.SetActive(false);
        //敌人静止
        rigi.velocity = new Vector3(0, 0, 0);
        //敌人摆正
        transform.rotation = Quaternion.Euler(0, 0, 0);
        animator.SetTrigger("triggerDie");
    }

    //死亡动画事件
    private void animatorDieEvent()
    {
        animator.enabled = false;
        Destroy(gameObject, 0.5f);
    }

    private void walk()
    {
        Vector3 targetPosition = player.transform.position;

        // 获取物体当前位置
        Vector3 currentPosition = transform.position;


        Vector3 distance = targetPosition - currentPosition;

        if (Mathf.Abs(distance.x) >= 5.4f || Mathf.Abs(distance.y) > 3f || Mathf.Abs(distance.z) > 2f)
        {
            if(!isWalk)
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
        } else  //离player近，可以攻击
        {
            //停止移动
            stopWalk();
            //开始攻击
            attack();
        }
    }

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
        }
    }

    #region 受击
    //受击
    private void byHit()
    {
        hitDuringTime += Time.deltaTime;
        if (isHit)
        {
            rigi.velocity = new Vector2(direction.x, 0);
            if(hitDuringTime >= hitMaxDuringTime) //受击hitMaxDuringTime秒后，退出受击动画
            {
                hitDuringTime = 0;
                rigi.velocity = Vector2.zero;
                stopHit();
                animatorClipName = new AnimatorStateInfo();
            }
        }
    }

    //受击(上挑)
    private void byUpHit()
    {
        upHitDuringTime += Time.deltaTime;
        upHitAngleDuringTime += Time.deltaTime;
        if (upHitAngleDuringTime >= upHitAngleMaxDuringTime) //超时未攻击 敌人恢复
        {
            isAttackUp3 = false;
            upHitDuringTime = 0;
            upHitAngleDuringTime = 0;
            rigi.velocity = Vector2.zero;
            stopUpHit();
            isHitOnGround = false;
            Log("伤害退出 超时 byUpHit", true);
        }
        else if (isAttackUp3 && isOnGround)
        {
            //围绕player转动
            //计算旋转量
            int directionX = direction.x > 0 ? 1 : -1;
            float angle = 30 * directionX;

            tempAngle = 20 * directionX;

            transform.position += new Vector3(upPosSpeed * 2 * directionX, upPosSpeed, 0);

            //围绕pivotPoint点，物体旋转angle度
            transform.RotateAround(player.transform.position, Vector3.forward, angle);

            //transform.position = new Vector3(transform.position.x + (direction.x > 0 ? 5 : -5), transform.position.y, transform.position.z);
            rigi.AddForce(new Vector3(direction.x > 0 ? -173 : 173, 260, 0), ForceMode.Impulse);

            isOnGround = false;
            angleCount = 0;

            Log("isAttackUp3 = true  ==byUpHit==", true);
        }
        else if (angleCount <= 5 && !isOnGround && upHitDuringTime >= upHitMaxDuringTime)
        {
            ++angleCount;
            upHitDuringTime = 0;
            transform.RotateAround(transform.position, Vector3.forward, tempAngle);
        }
    }

    //被跳起来踢飞
    private void byJumpHit(Vector2 direction)
    {
        stopHit();
        this.direction = direction;
        animator.SetTrigger("triggerJumpHit");
        rigi.AddForce(new Vector3(direction.x > 0 ? JumpHitForce : -JumpHitForce, 0, 0), ForceMode.Impulse);
        hit2Style();
    }

    //受到普通伤害
    public void hit(Vector2 direction, bool isChange, int hitType = 1)
    {
        stopUpHit();
        if (hitType == 1)
        {
            hitStyle(isChange);
        }
        else if (isChange && hitType == 2)
        {
            hit2Style();
        }

        //Log("受到伤害 进入hit函数 isHit = " + (isHit?"true":"false"), true);
        if (isHit)
        {
            if (hitDuringTime > 0)
            {
                hitDuringTime -= hitMaxDuringTime / 2;
            }
        }
        else
        {
            isHit = true;
            spriteRenderer.flipX = direction.x > 0 ? true : false;

            animator.SetBool("isHit", true);
            this.direction = direction;
            //Log("受到伤害 hit isHit = " + (isHit?"true":"false"), true);
        }
    }

    //受击特效
    private void hitStyle(bool isChange)
    {
        if (isChange)
        {
            changeHitDamage();
            //Debug.Log("受到伤害" + hitDamage);
            ObjectPool.Instance.Get(hitObj, hitPoint.transform.position, Vector3.zero, true).GetComponent<Hit>()
            .setFollowTransform(transform)
            .setDie();
        }
    }

    //受击特效2 （踢飞特效）
    private void hit2Style()
    {
        changeHitDamage();
        //Debug.Log("受到伤害" + hitDamage);
        ObjectPool.Instance.Get(hit2Obj, hitPoint.transform.position, Vector3.zero, true).GetComponent<Hit2>()
            .setFollowTransform(transform)
            .setDie();
    }

    //受到上挑伤害
    public void upHit(Vector2 direction, bool isAttackUp3 = false, bool isChange = false)
    {
        stopHit();
        hitStyle(isChange);
        if (isAttackUp3)
        {
            this.isAttackUp3 = isAttackUp3;
        }
        if (isUpHit && upHitAngleDuringTime > 0)
        {
            upHitAngleDuringTime -= upHitAngleMaxDuringTime / 2;
        }
        else
        {
            isUpHit = true;
            
            //吸附
            if(direction.x > 0)
            {
                spriteRenderer.flipX = true;
                transform.position = player.transform.position + new Vector3(5f, 3f, 0);
            } else
            {
                spriteRenderer.flipX = false;
                transform.position = player.transform.position + new Vector3(-5f, 3f, 0);
            }
                
            animator.SetBool("isUpHit", true);
            this.direction = direction;
        }
    }

    //扣减血量
    private void changeHitDamage(float blood = 1)
    {
        //显示血条对象
        enemyHealthObj.SetActive(true);
        //扣血
        hitDamage += blood;
        //显示当前敌人血条
        bloodBar.fillAmount = (maxBlood - hitDamage) / maxBlood;
        //修改头像显示
        enemyAvatar.sprite = Resources.Load<Sprite>("Images/Enemy/Kun/enemyAvatar") as Sprite;
    }

    private void stopHit()
    {
        if(isHit)
        {
            isHit = false;
            animator.SetBool("isHit", false);
        }
    }

    /// <summary>
    /// 结束上挑
    /// </summary>
    /// <param name="isHitOnGround">是否被上挑后，砸在地上</param>
    private void stopUpHit(bool isHitToGround = false)
    {
        if(isHitToGround)
        {
            changeHitDamage(2);
            Debug.Log("砸地上，受到伤害" + hitDamage);
            playAudio("Audios/Tool/hitToGround");
        }
        if(isUpHit)
        {
            isUpHit = false;
            animator.SetBool("isUpHit", false);
        }
    }
    #endregion

    //hitStandUp动画事件 从地面爬起后
    public void animatorHitStandUpFirstEvent()
    {
        ////敌人碰撞体与地面平行
        //CapCollider.direction = 0;
        //CapCollider.center = new Vector3(0, -2, 0);
        //敌人静止
        rigi.velocity = new Vector3(0, 0, 0);
        //敌人摆正
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    //hitStandUp动画事件 从地面爬起后
    public void animatorHitStandUpEvent()
    {
        ////敌人碰撞体与地面垂直
        //CapCollider.direction = 1;
        //CapCollider.center = new Vector3(0, 0, 0);
        //transform.rotation = Quaternion.Euler(0, 0, 0);
        isJumpHit = false;
        isHitOnGround = false;
        Debug.Log("***爬起动画结束 isJumpHit=" + isJumpHit);
    }

    private void stopAttack()
    {
        isAttack = false;
        animator.SetBool("isAttack", false);
    }

    private void stopWalk()
    {
        if(isWalk)
        {
            isWalk = false;
            animator.SetBool("isWalk", false);
        }
    }

    public void playAudio(string fileName)
    {
        string namePath = "";
        if (fileName.StartsWith("Audios/"))
        {
            namePath = fileName;
        } else
        {
            namePath = "Audios/Enemy/" + fileName;
        }

        AudioClip clip = Resources.Load<AudioClip>(namePath);

        audioSource.clip = clip;
        audioSource.Play();
    }

    private void Log(string str, bool isMustLog = false)
    {
        if (isMustLog)
        {
            Debug.Log(str);
        }
        else
        {
            //Debug.Log(str);
        }
    }
}
