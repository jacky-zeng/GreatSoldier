using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerBase : MonoBehaviour
{
    #region 变量定义
    private bool isPlayerGirl = false;

    private int horizontal = 0;
    private int vertical = 0;

    public float moveSpeed = 6f;
    public float cameraMoveSpeed = 15f;
    private bool isOnGround = false;
    public float timeRate = 0.3f;

    protected GameObject canvasContinue;
    protected GameObject canvasTimer;

    private AudioSource audioSource;

    //相机移动方向 -1向左 0静止 1向右
    //private int cameraDirectionMove = 0;

    //无敌时间
    private float unmatchedSecond = 1;
    protected bool isUnmatched = false;
    protected bool isDie = false;
    private bool isEnd = false;

    private SpriteRenderer spriteRenderer;
    protected Animator animator;

    protected Rigidbody rigiBody;

    //攻击
    public GameObject attackObj;
    public GameObject jumpAttackObj;

    /***攻击相关***/
    //普通攻击
    private bool isAttack = false;             //是否正在攻击
    private int attackStep = 0;                //在哪一个攻击连招
    private float attackMaxDuringTime = 0.5f;  //攻击持续最大时间
    private float attackDuringTime = 0;        //攻击已持续时间

    //放血攻击
    protected bool isAttackCircle = false;

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

    //起飞（可以连招）攻击
    private int attackFlyStep = 0;
    private bool isAttackFly = false;
    private float flyForce = 18f;
    private float attackFlyDuringTime = 0;         //已持续时间
    private float attackFlyMaxDuringTime = 1.8f;   //在该时间段内，按了 sd+攻击  才能触发起飞攻击

    //收到伤害
    //受到了几次伤害
    protected float hitDamage = 0;
    //总血量
    protected float maxBlood = 10;
    //是否受到攻击
    protected bool isHit = false;           //受到普通攻击
    protected bool isHitHeavy = false;      //受到重攻击
    protected bool isHitJump = false;       //受到跳跃攻击
    protected bool isHitElectric = false;   //受到电击攻击
    //血条
    private Image bloodBar;

    //相机
    private Transform transformCamera;
    private float transformCameraY;
    private float transformCameraZ;

    private string sceneName;

    private bool isCameraActionEnd = false; //是否运镜结束

    private GameObject mapStage;

    #endregion

    #region 初始化
    protected void BaseAwake()
    {
        if(transform.Find("AttackKnife") != null)
        {
            isPlayerGirl = true;
        }

        PlayerPrefs.SetString("triggerGun", "");
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        rigiBody = GetComponent<Rigidbody>();

        sceneName = SceneManager.GetActiveScene().name;

        transformCamera = Camera.main.transform;
        transformCameraY = transformCamera.position.y;
        transformCameraZ = transformCamera.position.z;

        // 获取重力加速度
        //Vector3 gravity = Physics.gravity;
        // 设置重力加速度
        Physics.gravity = new Vector3(0, -40, 0);
    }

    // Start is called before the first frame update
    protected void BaseStart()
    {
        init();

        switch (sceneName)
        {
            case "SceneSection1_1":
                MusicManager.instance.playAudio("Audios/Background/section1Begin");
                //血条改变
                bloodBar.fillAmount = (maxBlood - hitDamage) / maxBlood;
                transformCamera.position = new Vector3(3, transformCamera.position.y, transformCamera.position.z);
                break;
            case "SceneSection1_2":
                //为了保持与上一个场景中的位置不变
                transform.position = new Vector3(transform.position.x, transform.position.y, GameManager.instance.getPlayerPosZ() - 0.7f);
                hitDamage = GameManager.instance.getPlayerHitDamage();
                //血条改变
                bloodBar.fillAmount = (maxBlood - hitDamage) / maxBlood;

                transformCamera.position = new Vector3(-22.3f, transformCamera.position.y, transformCamera.position.z);
                break;
            case "SceneSection1_3":
                //为了保持与上一个场景中的位置不变
                transform.position = new Vector3(transform.position.x, transform.position.y, GameManager.instance.getPlayerPosZ() + 27.83f);
                hitDamage = GameManager.instance.getPlayerHitDamage();
                //血条改变
                bloodBar.fillAmount = (maxBlood - hitDamage) / maxBlood;

                transformCamera.position = new Vector3(20, transformCamera.position.y, transformCamera.position.z);
                break;
            case "SceneSection2_1":
                MusicManager.instance.playAudio("Audios/Background/section2Bg", true);
                hitDamage = GameManager.instance.getPlayerHitDamage();
                //血条改变
                bloodBar.fillAmount = (maxBlood - hitDamage) / maxBlood;
                transformCamera.position = new Vector3(-5.5f, transformCamera.position.y, transformCamera.position.z);
                break;
        }
    }

    private void init()
    {
        canvasContinue = CanvasContinue.instance.gameObject;
        canvasTimer = CanvasTimer.instance.gameObject;

        bloodBar = GameObject.Find("Canvas").transform.Find("PlayerHeath")
            .transform.Find("PlayerHealthBg").transform.Find("bloodBar").GetComponent<Image>();
        mapStage = GameObject.Find("mapStage");
        
        canvasContinue.SetActive(false);
        canvasTimer.SetActive(false);

        if(isPlayerGirl)
        {
            GameObject.Find("Canvas").transform.Find("PlayerHeath")
           .GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/PlayerGirl/blood_2p") as Sprite;
        } else
        {
            GameObject.Find("Canvas").transform.Find("PlayerHeath")
           .GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Player/blood_1p") as Sprite;
        }
       

        Debug.Log((canvasContinue == null ? "canvasContinue is null" : " canvasContinue") + (bloodBar == null ? "bloodBar is null" : " bloodBar") + (mapStage == null ? "mapStage is null" : " mapStage"));
    }
    #endregion

    #region 帧
    protected void BaseLateUpdate()
    {
        if (isCameraActionEnd)
        {
            /**相机x轴跟随角色**/
            Vector3 currentPosition = transformCamera.position;
            float posX = transform.position.x;
            float posY = transformCameraY;

            //控制相机在一定范围内
            if (sceneName == "SceneSection1_1")
            {
                if (transform.position.x <= 3)
                {
                    posX = 3;
                }
                else if (transform.position.x >= 50.3)
                {
                    posX = 50.3f;
                }
            }
            else if (sceneName == "SceneSection1_2")
            {
                if (transform.position.x <= 2.8)
                {
                    posX = 2.8f;
                }
                else if (transform.position.x >= 16.3f)
                {
                    posX = 16.3f;
                }
                posY = transform.position.z + 33;
                if (posY <= 17.6f)
                {
                    posY = 17.6f;
                }
                else if (posY >= 25.1f)
                {
                    posY = 25.1f;
                }
            }
            else if (sceneName == "SceneSection1_3")
            {
                if (transform.position.x <= 43.8)
                {
                    posX = 43.8f;
                }
                else if (transform.position.x >= 83.2)
                {
                    posX = 83.2f;
                }
            }
            else if (sceneName == "SceneSection2_1")
            {
                if (transform.position.x <= -5.5f)
                {
                    posX = -5.5f;
                }
                else if (transform.position.x >= 72.5f)
                {
                    posX = 72.5f;
                }
            }

            Vector3 targetPosition = new Vector3(posX, posY, transformCameraZ);

            // 计算移动的方向和距离
            Vector3 directionToTarget = (targetPosition - currentPosition).normalized;
            Vector3 moveDirection = directionToTarget * cameraMoveSpeed * Time.deltaTime;
            transformCamera.position = Vector3.MoveTowards(transformCamera.position, targetPosition, moveDirection.magnitude);
        }

        /**相机x轴跟随角色（增加偏移）**/
        //float tempX = transformCamera.position.x - transform.position.x;
        //if (Mathf.Abs(tempX) >= 25)//相机和角色位置相差一定距离的时候
        //{
        //    cameraDirectionMove = tempX > 0 ? -1 : 1;
        //}
        //if (cameraDirectionMove != 0)
        //{
        //    Vector3 currentPosition = transformCamera.position;
        //    Vector3 targetPosition = new Vector3(transform.position.x, transformCameraY, transformCameraZ);
        //    targetPosition += new Vector3(cameraDirectionMove > 0 ? 50 : -50, 0, 0);
        //    // 计算移动的方向和距离
        //    Vector3 directionToTarget = (targetPosition - currentPosition).normalized;
        //    Vector3 moveDirection = directionToTarget * cameraMoveSpeed * Time.deltaTime;

        //    // 移动相机
        //    transformCamera.position = Vector3.MoveTowards(currentPosition, targetPosition, moveDirection.magnitude);

        //    //Debug.Log(currentPosition.x - targetPosition.x);
        //    // 移动到位后，停止移动
        //    if (Mathf.Abs(Mathf.Abs(currentPosition.x - targetPosition.x) - 25) <= 3)
        //    {
        //        cameraDirectionMove = 0;
        //    }
        //}
    }

    // Update is called once per frame
    protected void BaseUpdate()
    {
        if (!isCameraActionEnd)
        {
            Vector3 currentPosition = transformCamera.position;
            float posX = 2.9f;
            switch (sceneName)
            {
                case "SceneSection1_1":
                    /**相机移动**/
                    if (currentPosition.y <= 14.9)
                    {
                        //结束过场动画
                        isCameraActionEnd = true;
                        transformCameraY = 14.8f;
                        MusicManager.instance.playAudio("Audios/Background/bg", true);
                        mapStage.GetComponent<mapStage>().begin();
                    }
                    else
                    {
                        Vector3 targetPosition = new Vector3(3, 14.8f, transformCameraZ);

                        //// 计算移动的方向和距离
                        Vector3 directionToTarget = (targetPosition - currentPosition).normalized;
                        Vector3 moveDirection = directionToTarget * cameraMoveSpeed * Time.deltaTime;
                        transformCamera.position = Vector3.MoveTowards(transformCamera.position, targetPosition, moveDirection.magnitude);
                    }
                    break;
                case "SceneSection1_2":
                    /**相机移动**/
                    if (currentPosition.x >= 2.8)
                    {
                        isCameraActionEnd = true;
                    }
                    else
                    {
                        Vector3 targetPosition = new Vector3(posX, transformCameraY, transformCameraZ);
                        //// 更新相机位置
                        ///private Vector3 camVelocity = Vector3.zero;
                        //transformCamera.position = Vector3.SmoothDamp(currentPosition, targetPosition, ref camVelocity, 5f);

                        //// 计算移动的方向和距离
                        Vector3 directionToTarget = (targetPosition - currentPosition).normalized;
                        Vector3 moveDirection = directionToTarget * cameraMoveSpeed * Time.deltaTime;
                        transformCamera.position = Vector3.MoveTowards(transformCamera.position, targetPosition, moveDirection.magnitude);

                    }
                    break;
                case "SceneSection1_3":
                    /**相机移动**/
                    posX = 43.9f;

                    if (currentPosition.x >= 43.8)
                    {
                        isCameraActionEnd = true;
                    }
                    else
                    {
                        Vector3 targetPosition = new Vector3(posX, transformCameraY, transformCameraZ);
                        //// 更新相机位置
                        ///private Vector3 camVelocity = Vector3.zero;
                        //transformCamera.position = Vector3.SmoothDamp(currentPosition, targetPosition, ref camVelocity, 5f);

                        //// 计算移动的方向和距离
                        Vector3 directionToTarget = (targetPosition - currentPosition).normalized;
                        Vector3 moveDirection = directionToTarget * cameraMoveSpeed * Time.deltaTime;
                        transformCamera.position = Vector3.MoveTowards(transformCamera.position, targetPosition, moveDirection.magnitude);
                    }
                    break;
                case "SceneSection2_1":
                    isCameraActionEnd = true;
                    mapStage.GetComponent<mapStage>().begin();
                    break;
            }
        }
        else
        {
            switch (sceneName)
            {
                case "SceneSection1_1":
                    bool isSectionOneEnemyAllDied1 = GameManager.instance.isSectionOneEnemyAllDied(1);
                    if(isSectionOneEnemyAllDied1 && !GameObject.Find("Go").GetComponent<Go>().getIsOn())
                    {
                        GameObject.Find("Go").GetComponent<Go>().begin();
                    }

                    float playerPosX = transform.position.x;
                    if (playerPosX >= 66 && isSectionOneEnemyAllDied1)
                    {
                        //保存一下当前player的Z轴位置（为了进入下一个场景时，player位置不变）
                        GameManager.instance.setPlayerPosZ(transform.position.z);
                        //保存一下当前player的hitDamage（为了进入下一个场景时，player的hitDamage不变）
                        GameManager.instance.setPlayerHitDamage(hitDamage);
                        //清空对象池
                        ObjectPool.Instance.init();
                        //加载场景2
                        SceneManager.LoadScene("SceneSection1_2", LoadSceneMode.Single);
                    }
                    else if(playerPosX >= 5.5f && playerPosX < 20) //敌人开始
                    {
                        GameManager.instance.sectionOneEnemyBegin(1, 3);
                    }
                    else if (playerPosX >= 39) //敌人开始
                    {
                        GameManager.instance.sectionOneEnemyBegin(1, 6);
                    }
                    break;
                case "SceneSection1_2":
                    bool isSectionOneEnemyAllDied2 = GameManager.instance.isSectionOneEnemyAllDied(2) && GameObject.Find("EnemyLeiShen") == null;
                    if (isSectionOneEnemyAllDied2 && !GameObject.Find("Go").GetComponent<Go>().getIsOn())
                    {
                        GameObject.Find("Go").GetComponent<Go>().begin();
                    }

                    if (transform.position.x >= 31 && isSectionOneEnemyAllDied2)
                    {
                        //保存一下当前player的Z轴位置（为了进入下一个场景时，player位置不变）
                        GameManager.instance.setPlayerPosZ(transform.position.z);
                        //保存一下当前player的hitDamage（为了进入下一个场景时，player的hitDamage不变）
                        GameManager.instance.setPlayerHitDamage(hitDamage);
                        //清空对象池
                        ObjectPool.Instance.init();

                        //加载场景3
                        SceneManager.LoadScene("SceneSection1_3", LoadSceneMode.Single);
                    } else
                    {
                        GameManager.instance.sectionOneEnemyBegin(2, 3);
                    }
                    break;
                case "SceneSection1_3":
                    if (transform.position.x >= 78)
                    {
                        //boss开始生效
                        GameManager.instance.sectionOneBossBegin();
                    }
                    break;
                case "SceneSection2_1":
                    if (transform.position.x >= 100)
                    {
                        //保存一下当前player的hitDamage（为了进入下一个场景时，player的hitDamage不变）
                        GameManager.instance.setPlayerHitDamage(hitDamage);
                        //清空对象池
                        ObjectPool.Instance.init();
                        //加载场景2
                        SceneManager.LoadScene("SceneSection2_2", LoadSceneMode.Single);
                    }
                    else
                    {
                        GameManager.instance.sectionTwoEnemyBegin(1, 8);
                    }
                    break;
            }

            //血条改变
            bloodBar.fillAmount = (maxBlood - hitDamage) / maxBlood;
            //bloodBar.fillAmount = Mathf.Lerp(bloodBar.fillAmount, (maxBlood - hitDamage) / maxBlood, hitDamage * Time.deltaTime);
            //Debug.Log("hitDamage=" + hitDamage + "maxBlood=" + maxBlood + " rs="+ (maxBlood - hitDamage) / maxBlood);
            if (isDie)
            {
                //已死亡
                if ((Input.GetKey(KeyCode.J) || Input.GetKey(KeyCode.K)) && (canvasTimer.GetComponent<CanvasTimer>()).timeLeft < 18)
                {
                    //隐藏倒计时界面
                    canvasContinue.SetActive(false);
                    (canvasTimer.GetComponent<CanvasTimer>()).end();
                    canvasTimer.SetActive(false);
                    //恢复player
                    transform.position = new Vector3(transform.position.x, transform.position.y + 10, transform.position.z);
                    hitDamage = 0;
                    GameManager.instance.setPlayerHitDamage(0);
                    bloodBar.fillAmount = 1;
                    isDie = false;

                    //重生后 设置3秒无敌
                    setUnmatched(3);
                }
            }
            else
            {
                if (!isEnd)
                {
                    if (isUnmatched)
                    {
                        unmatchedSecond -= Time.deltaTime;
                        if (unmatchedSecond <= 0)
                        {
                            //Log("无敌已过" + Time.deltaTime);
                            isUnmatched = false;

                            //恢复不透明
                            Color currentColor = GetComponent<SpriteRenderer>().material.color;
                            GetComponent<SpriteRenderer>().material.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1);
                        }
                    }

                    action();
                }
            }
        }
    }
    #endregion

    #region 动作
    private void action()
    {
        if(isHitElectric) //被电击时，无法操作
        {
            return;
        }
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

        bool isGetMouseButtonDown0 = Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.J);
        bool isButtonAttackCircle = isGetMouseButtonDown0 && Input.GetKey(KeyCode.K);
        if((isButtonAttackCircle || isAttackCircle) && (hitDamage + 2) < maxBlood)
        {
            attackCircle();
        }
        else if (isOnGround == true && isHitHeavy == false && isHitJump == false) //在地面上
        {
            bool tempIsAttackFly = false;
            attackFlyDuringTime += Time.deltaTime;
            if (attackFlyDuringTime >= attackFlyMaxDuringTime)
            {
                attackFlyStep = 0;
                attackFlyDuringTime = 0;
            }

            if (attackFlyStep == 0)
            {
                if (vertical == -1)
                {
                    attackFlyStep = 1;
                }
                else
                {
                    attackFlyStep = 0;
                }
            }
            else if (attackFlyStep == 1)
            {
                if (horizontal != 0)
                {
                    //改变player朝向
                    spriteRenderer.flipX = (horizontal > 0 ? false : true);
                    attackFlyStep = 2;
                }
                else
                {
                    attackFlyStep = 0;
                }
            }
            else if (attackFlyStep == 2)
            {
                if (isGetMouseButtonDown0)
                {
                    //满足起飞攻击条件
                    tempIsAttackFly = attackFly();
                }
            }

            if (!tempIsAttackFly)
            {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.K))   //跳跃
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
        }
        else //在空中
        {
            if (isGetMouseButtonDown0) //空中攻击
            {
                jumpAttack();
            }
            else
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
        if (isAttackFly)
        {
            return;
        }
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
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("attackNormal2"))
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
                attackDuringTime -= 0.3f;
            }
            else if (attackDuringTime < 0)
            {
                attackDuringTime = 0;
            }
        }

        attackDuringTime += Time.deltaTime;
        if (attackDuringTime > attackMaxDuringTime)     //攻击持续时间不足
        {
            //Log(" 攻击结束：" + attackDuringTime + " 已持续攻击的时间：" + attackDuringTime + " attackStep=" + attackStep, true);
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

    private void attackCircle()
    {
        if(isDie)
        {
            return;
        }
        if (!isAttackCircle)
        {
            isAttackCircle = true;

            //放血攻击会扣血
            hitDamage += 2;

            stopWalkAndStand();
            stopAttackFly();
            stopJumpAttack();
            stopAttackUp();

            //使攻击的武器朝向和player保持一致
            attackObj.transform.localScale = new Vector3(spriteRenderer.flipX ? -1 : 1, 1, 1);

            if(isPlayerGirl)
            {
                //给角色一个瞬间当前朝向的力
                rigiBody.AddForce(new Vector3(spriteRenderer.flipX ? -32 : 32, 0, 0), ForceMode.Impulse);
                playAudio("Audios/Tool/wumanAttack1");
            }
           
            animator.SetBool("isAttackCircle", isAttackCircle);

            //1秒后，结束放血攻击
            Invoke("stopAttackCircle", 1f);
        }
        attackDirectionDuringTime += Time.deltaTime;
        if (attackDirectionDuringTime >= attackDirectionMaxDuringTime / 3)  //间隔时间attackDirectionMaxDuringTime下，可改变player攻击的方向
        {
            attackDirectionDuringTime = 0;
            //改变player攻击的方向,使用户和地面只有水平方向的速度,并且使速度/3
            if (horizontal != 0)
            {
                spriteRenderer.flipX = (horizontal > 0 ? false : true);
            }
            rigiBody.velocity = new Vector3(horizontal * moveSpeed / 3, 0, 0);
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
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("attackUp2"))
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
            //Log(" Up攻击结束：" + attackUpDuringTime + " 已持续攻击的时间：" + attackUpDuringTime + " attackUpStep=" + attackUpStep, true);
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

    private bool attackFly()
    {
        if (!isAttackFly && !isAttack && !isJumpAttack && !isAttackUp)
        {
            isAttackFly = true;
            animator.SetBool("isAttackFly", true);

            //使攻击的武器朝向和player保持一致
            attackObj.transform.localScale = new Vector3(spriteRenderer.flipX ? -1 : 1, 1, 1);
            //玩家向上移动一点，便于起跳 （z轴加一点，抵消按S键导致的少量位移）
            rigiBody.velocity = new Vector3(rigiBody.velocity.x, rigiBody.velocity.y, 0);
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z + 1f);
            //给角色一个瞬间斜向上的力
            rigiBody.AddForce(new Vector3(spriteRenderer.flipX ? -flyForce : flyForce, flyForce, 0), ForceMode.Impulse);
        }
        //重置
        attackFlyStep = 0;
        attackFlyDuringTime = 0;
        return isAttackFly;
    }

    private void stopAttackFly()
    {
        if (isAttackFly)
        {
            isAttackFly = false;
            animator.SetBool("isStand", true);
            animator.SetBool("isAttackFly", false);
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

    private void stopAttackCircle()
    {
        if (isAttackCircle)
        {
            isAttackCircle = false;
            animator.SetBool("isAttackCircle", isAttackCircle);
        }
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
        if (isJumpAttack)
        {
            isJumpAttack = false;
            animator.SetBool("isStand", true);
            animator.SetBool("isJumpAttack", false);
        }
    }
    #endregion

    #region 碰撞
    //接触到
    protected void BaseOnCollisionEnter(Collision collision)
    {
        if (isOnGround == false && collision.transform.tag == "Ground")
        {
            isOnGround = true;

            stopJumpAttack();
            stopAttackFly();
            //Log("进入地面Ground OnCollisionEnter", true);
        }
    }

    //待在里面
    protected void BaseOnCollisionStay(Collision collision)
    {
        if (isOnGround == false && collision.transform.tag == "Ground")
        {
            isOnGround = true;

            stopJumpAttack();
            stopAttackFly();
            //Log("待在地面Ground OnCollisionStay", true);
        }
    }

    //离开地面
    protected void BaseOnCollisionExit(Collision collision)
    {
        if (isOnGround == true && collision.transform.tag == "Ground")
        {
            isOnGround = false;
            //Log("离开地面 Ground OnCollisionExit");
        }
    }
    #endregion

    //设置无敌
    public void setUnmatched(float sec, bool isDotSetTransparent = false)
    {
        isUnmatched = true;
        unmatchedSecond = sec;

        if(!isDotSetTransparent)  //过关时只是无敌，不会变透明
        {
            //变透明一点
            Color currentColor = GetComponent<SpriteRenderer>().material.color;
            GetComponent<SpriteRenderer>().material.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0.8f);
        }
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
    #endregion

}