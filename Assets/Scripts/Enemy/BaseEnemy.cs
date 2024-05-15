using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseEnemy : MonoBehaviour
{
    #region 变量定义
    public GameObject player;
    public GameObject hitPoint;
    public GameObject hitObj;
    public GameObject hit2Obj;
    public GameObject emenyAttackObj;
    //血条对象
    public GameObject enemyHealthObj;
    //血条头像
    public Image enemyAvatar;
    //血条
    public Image bloodBar;
    public float moveSpeed = 3;

    public int JumpHitForce = 260;
    public int flyHitForce = 20;
    public float upPosSpeed = 3;          //被上挑时的离心距离

    //上挑时的吸附
    public Vector3 xiFuLeft = new Vector3(2.5f, 1.5f, 0);
    public Vector3 xiFuRight = new Vector3(-2.5f, 1.5f, 0);
    //总血量
    public float maxBlood = 10;

    [HideInInspector]
    public Sprite enemyAvatarSprite;

    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public Rigidbody rigi;
    [HideInInspector]
    public SpriteRenderer spriteRenderer;
    [HideInInspector]
    public AudioSource audioSource;
    [HideInInspector]
    public CapsuleCollider CapCollider;

    private AnimatorStateInfo animatorClipName = new AnimatorStateInfo();
   
    private float hitDuringTime = 0;
    private float hitMaxDuringTime = 0.6f;

    private float upHitDuringTime = 0;
    private float upHitMaxDuringTime = 0.05f;

    private float upHitAngleDuringTime = 0;
    private float upHitAngleMaxDuringTime = 1.2f;

    Vector2 direction = Vector2.one;

    //受到了几次伤害
    [HideInInspector]
    public float hitDamage = 0;
    [HideInInspector]
    public bool isDie = false;
    [HideInInspector]
    public bool isWalk = false;
    [HideInInspector]
    public bool isAttack = false;
    [HideInInspector]
    public bool isAttackHeavy = false;
    [HideInInspector]
    public bool isHit = false;           //是否收到普通伤害
    [HideInInspector]
    public bool isUpHit = false;         //是否被上挑
    [HideInInspector]
    public bool isJumpHit = false;       //是否被跳起来踢飞
    [HideInInspector]
    public bool isAttackUp3 = false;     //是否被上挑挑飞
    [HideInInspector]
    public bool isHitOnGround = false;   //是否被打趴在地上
    [HideInInspector]
    public float tempAngle = 0;          //中途旋转
    [HideInInspector]
    public int angleCount = 0;           //已经旋转的次数
    [HideInInspector]
    public bool isOnGround = true;       //是否在地面上
    #endregion

    public void BaseStart()
    {
        animator = GetComponent<Animator>();
        rigi = GetComponent<Rigidbody>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        CapCollider = GetComponent<CapsuleCollider>();
    }

    #region 碰撞

    public void OnCollisionEnterGround()
    {
        if (!isOnGround)
        {
            isOnGround = true;
            isAttackUp3 = false;
            upHitDuringTime = 0;
            rigi.velocity = Vector2.zero;
            stopUpHit(true);
            isHitOnGround = true;
            Log("进入地面Ground OnCollisionEnter", true);
        }
        //敌人碰撞体与地面垂直
        CapCollider.direction = 1;
    }

    public void OnTriggerEnterOrStay(Collider other, string type)
    {
        if(isDie)
        {
            return;
        }
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
            else if (tempAnimatorClipName.IsName("attackFly"))
            {
                if (!isJumpHit)
                {
                    isJumpHit = true;
                    Debug.Log("attackFly****");
                    byFlyHit(tempDirection);
                    Invoke("cancelStatus", 0.3f);
                }
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
    public void cancelStatus()
    {
        if (isJumpHit || isHitOnGround)
        {
            Debug.Log("******cancelJumpHit isJumpHit=" + isJumpHit);
            animatorHitStandUpEvent();
        }
    }

    //死亡动画事件
    public void animatorDieEvent()
    {
        animator.enabled = false;
        Destroy(gameObject, 0.5f);
    }

    #region 受击
    //受击
    public void byHit()
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
    public void byUpHit()
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
    public void byJumpHit(Vector2 direction)
    {
        stopHit();
        this.direction = direction;
        animator.SetTrigger("triggerJumpHit");
        rigi.AddForce(new Vector3(direction.x > 0 ? JumpHitForce : -JumpHitForce, 0, 0), ForceMode.Impulse);
        hit2Style();
    }

    //被起飞向上击飞
    public void byFlyHit(Vector2 direction)
    {
        stopHit();
        this.direction = direction;
        animator.SetTrigger("triggerJumpHit");
        rigi.velocity = new Vector3(direction.x > 0 ? flyHitForce/3 : -flyHitForce/3, flyHitForce, 0);
        CapCollider.direction = 0;
        isOnGround = false;
        //rigi.AddForce(new Vector3(direction.x > 0 ? JumpHitForce : -JumpHitForce, JumpHitForce, 0), ForceMode.Impulse);
        hitStyle(true);
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
    public void hitStyle(bool isChange)
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
    public void hit2Style()
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
                transform.position = player.transform.position + xiFuLeft;
            } else
            {
                spriteRenderer.flipX = false;
                transform.position = player.transform.position + xiFuRight;
            }
                
            animator.SetBool("isUpHit", true);
            this.direction = direction;
        }
    }

    //扣减血量
    public void changeHitDamage(float blood = 1)
    {
        //显示血条对象
        enemyHealthObj.SetActive(true);
        //扣血
        hitDamage += blood;
        //显示当前敌人血条
        bloodBar.fillAmount = (maxBlood - hitDamage) / maxBlood;
        //修改头像显示
        enemyAvatar.sprite = enemyAvatarSprite;  
    }

    public void stopHit()
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
    public void stopUpHit(bool isHitToGround = false)
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
        if(isOnGround)
        {
            //禁用敌人碰撞体
            CapCollider.enabled = false;
        }
        //敌人静止
        rigi.velocity = new Vector3(0, 0, 0);
        //敌人摆正
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    //hitStandUp动画事件 从地面爬起后
    public void animatorHitStandUpEvent()
    {
        //启用敌人碰撞体
        CapCollider.enabled = true;
        
        isJumpHit = false;
        isHitOnGround = false;
        Debug.Log("***爬起动画结束 isJumpHit=" + isJumpHit);
    }

    public void stopAttack()
    {
        isAttack = false;
        animator.SetBool("isAttack", false);
    }

    public void stopAttackHeavy()
    {
        isAttackHeavy = false;
        animator.SetBool("isAttackHeavy", false);
    }

    public void stopWalk()
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

    public void Log(string str, bool isMustLog = false)
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
