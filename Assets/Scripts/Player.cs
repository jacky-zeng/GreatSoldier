using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region 变量定义
    private int horizontal = 0;
    private int vertical = 0;

    public float moveSpeed = 6f;
    public float cameraMoveSpeed = 15f;
    private bool isOnGround = false;
    public float timeRate = 0.3f;

    private AudioSource audioSource;

    //相机移动方向 -1向左 0静止 1向右
    private int cameraDirectionMove = 0;

    //无敌时间
    private float unmatchedSecond = 0;
    private bool isUnmatched = false;
    private bool isDie = false;
    private bool isEnd = false;

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private Rigidbody rigiBody;

    //攻击
    public GameObject attackObj;
    public GameObject jumpAttackObj;

    /***攻击相关***/
    //普通攻击
    private bool isAttack = false;             //是否正在攻击
    private int attackStep = 0;                //在哪一个攻击连招
    private float attackMaxDuringTime = 0.5f;  //攻击持续最大时间
    private float attackDuringTime = 0;        //攻击已持续时间

    //上挑攻击
    private bool isAttackUp = false;             //是否正在攻击
    private int attackUpStep = 0;                //在哪一个攻击连招
    private float attackUpMaxDuringTime = 0.8f;  //攻击持续最大时间
    private float attackUpDuringTime = 0;        //攻击已持续时间

    private float attackDirectionMaxDuringTime = 0.3f;  //攻击持续最大时间 （方向）
    private float attackDirectionDuringTime = 0;        //攻击已持续时间（方向）

    //跳跃攻击
    private bool isJumpAttack = false;
    public float jumpForce = 18f;

    //收到伤害
    //受到了几次伤害
    private int hitDamage = 0;
    //总血量
    public int maxBlood = 10;
    private bool isHit = false;

    //相机
    private Transform transformCamera;
    private float transformCameraY;
    private float transformCameraZ;

    #endregion

    #region 初始化
    private void Awake()
    {
        PlayerPrefs.SetString("triggerGun", "");
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        rigiBody = GetComponent<Rigidbody>();

        transformCamera = Camera.main.transform;
        transformCameraY = transformCamera.position.y;
        transformCameraZ = transformCamera.position.z;

        // 获取重力加速度
        //Vector3 gravity = Physics.gravity;
        // 设置重力加速度
        Physics.gravity = new Vector3(0, -40, 0);
    }

    // Start is called before the first frame update
    void Start()
    {

    }
    #endregion

    #region 帧
    private void LateUpdate()
    {
        /**相机x轴跟随角色**/
        float tempX = transformCamera.position.x - transform.position.x;
        if (Mathf.Abs(tempX) >= 25)//相机和角色位置相差一定距离的时候
        {
            cameraDirectionMove = tempX > 0 ? -1 : 1;
        }

        if (cameraDirectionMove != 0)
        {
            Vector3 currentPosition = transformCamera.position;
            Vector3 targetPosition = new Vector3(transform.position.x, transformCameraY, transformCameraZ);
            targetPosition += new Vector3(cameraDirectionMove > 0 ? 50 : -50, 0, 0);
            // 计算移动的方向和距离
            Vector3 directionToTarget = (targetPosition - currentPosition).normalized;
            Vector3 moveDirection = directionToTarget * cameraMoveSpeed * Time.deltaTime;

            // 移动相机
            transformCamera.position = Vector3.MoveTowards(currentPosition, targetPosition, moveDirection.magnitude);

            //Debug.Log(currentPosition.x - targetPosition.x);
            // 移动到位后，停止移动
            if (Mathf.Abs(Mathf.Abs(currentPosition.x - targetPosition.x) - 25) <= 3)
            {
                cameraDirectionMove = 0;
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (!isEnd)
        {
            if (isUnmatched)
            {
                unmatchedSecond -= Time.deltaTime;
                if (unmatchedSecond <= 0)
                {
                    Log("无敌已过" + Time.deltaTime);
                    isUnmatched = false;
                }
            }

            if (!isDie)
            {
                action();
            }
        }
    }
    #endregion

    #region 动作
    private void action()
    {
        //计算出horizontal和vertical
        if (Input.GetKey(KeyCode.W))
        {
            vertical = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            vertical = -1;
        }
        else
        {
            vertical = 0;
        }

        if (Input.GetKey(KeyCode.D))
        {
            horizontal = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            horizontal = -1;
        }
        else
        {
            horizontal = 0;
        }

        bool isGetMouseButtonDown0 = Input.GetMouseButtonDown(0);

        if (isOnGround == true) //在地面上
        {
            if (Input.GetKeyDown(KeyCode.Space))   //跳跃
            {
                jump();
            }
            else if (vertical == 1 && horizontal == 0 && (isAttackUp || isGetMouseButtonDown0)) //上挑攻击：正在攻击过程中 或者 按了攻击键
            {
                attackUp(isGetMouseButtonDown0);
            }
            else if (isAttack || isGetMouseButtonDown0)  //普通攻击：正在攻击过程中 或者 按了攻击键
            {
                attackNormal(isGetMouseButtonDown0);
            }
            else  //移动
            {
                move();
            }
        }
        else //在空中
        {
            if(isGetMouseButtonDown0) //空中攻击
            {
                jumpAttack();
            } else
            {
                jumpMove();
            }
            
        }
    }

    private void jumpAttack()
    {
        isJumpAttack = true;
        //使攻击的武器朝向和player保持一致
        jumpAttackObj.transform.localScale = new Vector3(spriteRenderer.flipX ? -1 : 1, 1, 1);
        animator.SetBool("isJumpAttack", true);
        //给角色一个瞬间当前朝向的力
        rigiBody.AddForce(new Vector3(spriteRenderer.flipX ? -6 : 6, 0, 0), ForceMode.Impulse);
    }

    //在空中时，也能间断施加力
    private void jumpMove()
    {
        if (timeRate >= 1.2f)
        {
            int extTimes = 1;
            if (!spriteRenderer.flipX && horizontal > 0)
            {
                extTimes = 1;
            }
            else if (spriteRenderer.flipX && horizontal < 0)
            {
                extTimes = 1;
            }
            else
            {
                extTimes = 3;
            }
            rigiBody.AddForce(new Vector3(horizontal * extTimes * 100f, 0, 0));
            timeRate = 0;
        }
        else
        {
            timeRate += Time.deltaTime;
        }
    }

    private void move()
    {
        stopAttack();
        stopAttackUp();
        stopJumpAttack();

        //根据horizontal和vertical，使用动画
        //animator.SetFloat("horizontal", horizontal);
        //animator.SetFloat("vertical", vertical);

        if (horizontal != 0 || vertical != 0) //在 水平/垂直 移动
        {
            animator.SetBool("isStand", false);
            //角色朝向
            if (horizontal != 0)
            {
                spriteRenderer.flipX = (horizontal > 0 ? false : true);
            }
            //获取和设置刚体的线性速度
            rigiBody.velocity = new Vector3(horizontal * moveSpeed, 0, vertical * moveSpeed);

            animator.SetBool("isWalk", true);
        }
        else
        {
            rigiBody.velocity = new Vector3(0, rigiBody.velocity.y, 0);

            animator.SetBool("isWalk", false);
            animator.SetBool("isStand", true);
        }
    }

    private void attackNormal(bool isGetMouseButtonDown0)
    {
        stopWalkAndStand();
        stopAttackUp();

        //使攻击的武器朝向和player保持一致
        attackObj.transform.localScale = new Vector3(spriteRenderer.flipX ? -1 : 1, 1, 1);

        if (isGetMouseButtonDown0)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("attackNormal1"))
            {
                attackStep = 2;
                animator.SetInteger("attackStep", attackStep);
            }
            else if(animator.GetCurrentAnimatorStateInfo(0).IsName("attackNormal2"))
            {
                attackStep = 3;
                animator.SetInteger("attackStep", attackStep);
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("attackNormal3"))
            {
                attackStep = 4;
                animator.SetInteger("attackStep", attackStep);
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("attackNormal4"))
            {
                attackStep = 5;
                animator.SetInteger("attackStep", attackStep);
            }
            else
            {
                attackStep = 1;
                animator.SetInteger("attackStep", attackStep);
            }

            if (attackDuringTime > 0) //只要不停按攻击，就延长攻击持续时间
            {
                attackDuringTime -= 0.25f;
            }
            else if (attackDuringTime < 0)
            {
                attackDuringTime = 0;
            }
        }

        attackDuringTime += Time.deltaTime;
        if (attackDuringTime > attackMaxDuringTime)     //攻击持续时间不足
        {
            //Log(" 攻击结束：" + attackDuringTime + " 已持续攻击的时间：" + attackDuringTime + " attackStep=" + attackStep);
            isAttack = false;
            attackStep = 0;
            animator.SetInteger("attackStep", attackStep);
            animator.SetBool("isAttack", isAttack);
            attackDuringTime = 0;  //重置攻击持续时间
        }
        else
        {
            if (!isAttack)
            {
                isAttack = true;
                animator.SetBool("isAttack", isAttack);
                attackStep = 1;
                animator.SetInteger("attackStep", 1);
            }
            attackDirectionDuringTime += Time.deltaTime;
            if (attackDirectionDuringTime >= attackDirectionMaxDuringTime)  //间隔时间attackDirectionMaxDuringTime下，可改变player攻击的方向
            {
                attackDirectionDuringTime = 0;
                //改变player攻击的方向,使用户和地面只有水平方向的速度,并且使速度/4
                if (horizontal != 0)
                {
                    spriteRenderer.flipX = (horizontal > 0 ? false : true);
                }
                rigiBody.velocity = new Vector3(horizontal * moveSpeed / 4, 0, 0);
            }
        }
    }

    private void attackUp(bool isGetMouseButtonDown0)
    {
        stopWalkAndStand();
        stopAttack();
        //使攻击的武器朝向和player保持一致
        attackObj.transform.localScale = new Vector3(spriteRenderer.flipX ? -1 : 1, 1, 1);

        if (isGetMouseButtonDown0)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("attackUp1"))
            {
                attackUpStep = 2;
                animator.SetInteger("attackUpStep", attackUpStep);
            }
            else if(animator.GetCurrentAnimatorStateInfo(0).IsName("attackUp2"))
            {
                attackUpStep = 3;
                animator.SetInteger("attackUpStep", attackUpStep);
            }
            else
            {
                attackUpStep = 1;
                animator.SetInteger("attackUpStep", attackUpStep);
            }

            if (attackUpDuringTime > 0) //只要不停按攻击，就延长攻击持续时间
            {
                attackUpDuringTime -= 0.25f;
            }
            else if (attackUpDuringTime < 0)
            {
                attackUpDuringTime = 0;
            }
        }

        attackUpDuringTime += Time.deltaTime;
        if (attackUpDuringTime > attackUpMaxDuringTime)     //攻击持续时间不足
        {
            Log(" Up攻击结束：" + attackUpDuringTime + " 已持续攻击的时间：" + attackUpDuringTime + " attackUpStep=" + attackUpStep, true);
            isAttackUp = false;
            attackUpStep = 0;
            animator.SetInteger("attackUpStep", attackUpStep);
            animator.SetBool("isAttackUp", isAttackUp);
            attackUpDuringTime = 0;  //重置攻击持续时间
        }
        else
        {
            if (!isAttackUp)
            {
                isAttackUp = true;
                animator.SetBool("isAttackUp", isAttackUp);
                attackUpStep = 1;
                animator.SetInteger("attackUpStep", attackUpStep);
            }
            attackDirectionDuringTime += Time.deltaTime;
            if (attackDirectionDuringTime >= attackDirectionMaxDuringTime)  //间隔时间attackDirectionMaxDuringTime下，可改变player攻击的方向
            {
                attackDirectionDuringTime = 0;
                //改变player攻击的方向,使用户和地面只有水平方向的速度,并且使速度/4
                if (horizontal != 0)
                {
                    spriteRenderer.flipX = (horizontal > 0 ? false : true);
                }
                rigiBody.velocity = new Vector3(horizontal * moveSpeed / 4, 0, 0);
            }
        }
    }

    private void jump()
    {
        stopAttack();
        //玩家向上移动一点，便于起跳
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);
        //给角色一个瞬间向上的力
        rigiBody.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
        //跳跃动画 (暂无)
    }

    private void stopWalkAndStand()
    {
        animator.SetBool("isWalk", false);
        animator.SetBool("isStand", false);
    }

    private void stopAttackUp()
    {
        if (isAttackUp)
        {
            isAttackUp = false;
            animator.SetBool("isAttackUp", isAttackUp);
            attackUpStep = 0;
            animator.SetInteger("attackUpStep", attackUpStep);
        }
    }

    private void stopAttack()
    {
        if (isAttack)
        {
            isAttack = false;
            animator.SetBool("isAttack", isAttack);
            attackStep = 0;
            animator.SetInteger("attackStep", attackStep);
        }
    }

    private void stopJumpAttack()
    {
        if(isJumpAttack)
        {
            isJumpAttack = false;
            animator.SetBool("isStand", true);
            animator.SetBool("isJumpAttack", false);
        }
    }
    #endregion

    #region 碰撞
    //接触到
    private void OnCollisionEnter(Collision collision)
    {
        if (isOnGround == false && collision.transform.tag == "Ground")
        {
            isOnGround = true;

            stopJumpAttack();
            Log("进入地面Ground OnCollisionEnter", true);
        }
    }

    //待在里面
    private void OnCollisionStay(Collision collision)
    {
        if (isOnGround == false && collision.transform.tag == "Ground")
        {
            isOnGround = true;

            stopJumpAttack();
            Log("待在地面Ground OnCollisionStay", true);
        }
    }

    //离开地面
    private void OnCollisionExit(Collision collision)
    {
        if (isOnGround == true && collision.transform.tag == "Ground")
        {
            isOnGround = false;
            Log("离开地面 Ground OnCollisionExit");
        }
    }

    //trigger碰撞
    private void OnTriggerEnter(Collider collision)
    {
        OnTriggerEnterOrStay(collision, "Enter");
    }

    private void OnTriggerStay(Collider collision)
    {
        OnTriggerEnterOrStay(collision, "Stay");
    }

    private void OnTriggerEnterOrStay(Collider collision, string type)
    {
        if (collision.gameObject.name == "EnemyAttack") //收到敌人攻击
        {
            Log("hitDamage="+ hitDamage + "受到伤害EnemyAttack" + collision.transform.parent.position.x + "|" + transform.position.x, true);
            hit(collision.transform.parent.position.x > transform.position.x ? true : false);
        }
    }

    #endregion

    //受到普通伤害
    public void hit(bool directionHit)
    {
        //hit2Style();

        if (!isHit)
        {
            isHit = true;
            ++hitDamage;
            playAudio("Audios/Tool/manHit1");
            rigiBody.AddForce(new Vector3(directionHit ? -10 : 10, 0, 0), ForceMode.Impulse);
            animator.SetTrigger("triggerHit");
        }
    }

    //hit动画事件 受击结束后
    private void animatorHitEndEvent()
    {
        isHit = false;
  
        ////静止
        //rigi.velocity = new Vector3(0, 0, 0);
        ////摆正
        //transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    public void playAudio(string fileName)
    {
        string namePath = "";
        if (fileName.StartsWith("Audios/"))
        {
            namePath = fileName;
        }
        else
        {
            namePath = "Audios/Player/" + fileName;
        }

        AudioClip clip = Resources.Load<AudioClip>(namePath);

        audioSource.clip = clip;
        audioSource.Play();
    }

    #region 日志
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
    #endregion
}