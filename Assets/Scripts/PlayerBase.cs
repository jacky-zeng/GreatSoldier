using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerBase : MonoBehaviour
{
    #region 变量定义
    protected bool isDisableAction = false; //是否禁用用户操作
    private bool isPlayerGirl = false;
    private bool isSection2End = false;

    private int horizontal = 0;
    private int vertical = 0;

    public float moveSpeed = 6f;
    protected float cameraMoveSpeed = 8.5f;
    protected float cameraMoveSpeedAuto = 15f;
    private bool isOnGround = false;
    public float timeRate = 0.3f;

    protected GameObject canvasContinue;
    protected GameObject canvasTimer;

    private AudioSource audioSource;

    [Header("Dash参数")]
    public float dashTime;         //冲锋时长
    private float dashTimeLeft;    //冲锋剩余时间
    private float lastDash = -10f; //上一次dash时间点
    public float dashCoolDown;
    public float dashSpeed;
    public bool isDashing;

    private bool clickDash = false; //玩家按了闪避
    private float dashFillTime = 0;

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

    protected bool isWalk = false;             //是否正在移动
    protected bool isStand = false;            //是否是站着

    //攻击
    public GameObject attackObj;
    public GameObject jumpAttackObj;

    /***攻击相关***/
    //普通攻击
    private bool isAttack = false;             //是否正在攻击
    private int attackStep = 0;                //在哪一个攻击连招
    private float attackMaxDuringTime = 0.8f;  //攻击持续最大时间
    private float attackDuringTime = 0;        //攻击已持续时间

    //放血攻击
    protected bool isAttackCircle = false;

    //上挑攻击
    private bool isAttackUp = false;             //是否正在攻击
    private int attackUpStep = 0;                //在哪一个攻击连招
    private float attackUpMaxDuringTime = 0.8f;  //攻击持续最大时间
    private float attackUpDuringTime = 0;        //攻击已持续时间

    private float attackDirectionMaxDuringTime = 0.2f;  //攻击持续最大时间 （方向）
    private float attackDirectionDuringTime = 0;        //攻击已持续时间（方向）

    //跳跃攻击
    private bool isJumpAttack = false;
    public float jumpForce = 18f;

    //起飞（可以连招）攻击
    //private int attackFlyStep = 0;
    private bool isAttackFly = false;
    private float flyForce = 18f;
    //private float attackFlyDuringTime = 0;         //已持续时间
    //private float attackFlyMaxDuringTime = 1.8f;   //在该时间段内，按了 sd+攻击  才能触发起飞攻击

    //受到伤害
    //受到了几次伤害
    protected float hitDamage = 0;
    //总血量
    protected float maxBlood = 10;
    //是否受到攻击
    protected bool isHit = false;           //受到普通攻击
    protected bool isHitHeavy = false;      //受到重攻击
    protected bool isHitJump = false;       //受到跳跃攻击
    //血条
    private Image bloodBar;
    //闪避CD
    private Image sharkBar;
    //结束字幕
    private RectTransform txtEnd;

    //相机
    private Transform transformCamera;
    private float transformCameraY;
    private float transformCameraZ;

    protected string sceneName;

    private bool isCameraActionEnd = false; //是否运镜结束

    private bool isCameraMove = true;

    private GameObject mapStage;

    public float animationDuration = 30f; // 动画持续时间

    //private GameObject manPlayer;

    #endregion

    #region 初始化
    protected void BaseAwake()
    {
        if (transform.Find("AttackKnife") != null)
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
            case "SceneSection2_2":
                //GameManager 会 initPlayer
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

        sharkBar = GameObject.Find("Canvas").transform.Find("PlayerHeath").transform.Find("sharkBar").GetComponent<Image>();

        mapStage = GameObject.Find("mapStage");

        canvasContinue.SetActive(false);
        canvasTimer.SetActive(false);

        if (isPlayerGirl)
        {
            GameObject.Find("Canvas").transform.Find("PlayerHeath")
           .GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/PlayerGirl/blood_2p") as Sprite;
        }
        else
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
            else if (sceneName == "SceneSection2_2")
            {
                if (transform.position.x <= -5.5f)
                {
                    posX = -5.5f;
                }
                else if (transform.position.x >= 68.7f)
                {
                    posX = 68.7f;
                }
            }

            /**相机x轴跟随角色（偏移相机中心点一定距离，镜头才跟随）**/
            Vector3 targetPosition = new Vector3(posX, posY, transformCameraZ);
            if(sceneName == "SceneSection1_2")
            {
                isCameraMove = true;
            } else
            {
                if (posX <= currentPosition.x - 7 || posX >= currentPosition.x + 5)
                {
                    isCameraMove = true;
                }
                else if (Mathf.Abs(currentPosition.x - targetPosition.x) <= 0.5f) //说明基本移动到位
                {
                    isCameraMove = false;
                }
            }
            

            if (isCameraMove)
            {
                // 计算移动的方向和距离
                Vector3 directionToTarget = (targetPosition - currentPosition).normalized;
                Vector3 moveDirection = directionToTarget * cameraMoveSpeed * Time.deltaTime;
                transformCamera.position = Vector3.MoveTowards(transformCamera.position, targetPosition, moveDirection.magnitude);
            }
        }
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
                        Vector3 moveDirection = directionToTarget * cameraMoveSpeedAuto * Time.deltaTime;
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
                        Vector3 moveDirection = directionToTarget * cameraMoveSpeedAuto * Time.deltaTime;
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
                        Vector3 moveDirection = directionToTarget * cameraMoveSpeedAuto * Time.deltaTime;
                        transformCamera.position = Vector3.MoveTowards(transformCamera.position, targetPosition, moveDirection.magnitude);
                    }
                    break;
                case "SceneSection2_1":
                    isCameraActionEnd = true;
                    mapStage.GetComponent<mapStage>().begin();
                    break;
                case "SceneSection2_2":
                    isCameraActionEnd = true;
                    break;
            }
        }
        else
        {
            switch (sceneName)
            {
                case "SceneSection1_1":
                    bool isSectionOneEnemyAllDied1 = GameManager.instance.isSectionOneEnemyAllDied(1);
                    if (isSectionOneEnemyAllDied1 && !GameObject.Find("Go").GetComponent<Go>().getIsOn())
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
                    else if (playerPosX >= 5.5f && playerPosX < 20) //敌人开始
                    {
                        GameManager.instance.sectionOneEnemyBegin(1, 3);
                    }
                    else if (playerPosX >= 39) //敌人开始
                    {
                        GameManager.instance.sectionOneEnemyBegin(1, 6);
                    }
                    break;
                case "SceneSection1_2":
                    bool isSectionOneEnemyAllDied2 = GameManager.instance.isSectionOneEnemyAllDied(2) && GameObject.Find("EnemyLeiShen").GetComponent<LeiShen>().getIsDie();
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
                    }
                    else
                    {
                        GameManager.instance.sectionOneEnemyBegin(2, 3);
                    }
                    break;
                case "SceneSection1_3":
                    if (transform.position.x >= 50)
                    {
                        GameObject.Find("ToolGangKnife").GetComponent<GangKnife>().begin();
                    }
                    if (transform.position.x >= 78)
                    {
                        //boss开始生效
                        GameManager.instance.sectionOneBossBegin();
                    }
                    break;
                case "SceneSection2_1":
                    if (!isDisableAction && transform.position.x >= 66 && GameManager.instance.isSectionTwoEnemyAllDied(1))
                    {
                        isDisableAction = true;
                        GameObject.Find("wallEnd").GetComponent<BoxCollider>().enabled = false;
                        GameObject.Find("groundEnd").GetComponent<BoxCollider>().enabled = false;

                        jump();
                        jumpAttack();

                        Invoke("beginSection2_2", 1f);
                    }
                    else if (transform.position.x >= 45)
                    {
                        GameManager.instance.sectionTwoEnemyBegin(1, 10);
                    }
                    else if (transform.position.x >= 0)
                    {
                        GameManager.instance.sectionTwoEnemyBegin(1, 6);
                    }
                    break;
                case "SceneSection2_2":
                    if (!isDisableAction && transform.position.x >= 70 && GameManager.instance.isSectionTwoEnemyAllDied(2))
                    {
                        isDisableAction = true;

                        //保存一下当前player的Z轴位置（为了进入下一个场景时，player位置不变）
                        GameManager.instance.setPlayerPosZ(transform.position.z);
                        //保存一下当前player的hitDamage（为了进入下一个场景时，player的hitDamage不变）
                        GameManager.instance.setPlayerHitDamage(hitDamage);
                        //清空对象池
                        ObjectPool.Instance.init();

                        GameObject.Find("MeiLuXing").GetComponent<MeiLuXing>().gameEnd();

                        stopAttackCircle();
                        stopAttackFly();
                        stopAttack();
                        stopAttackUp();
                        stopJumpAttack();

                        isWalk = false;
                        animator.SetBool("isWalk", isWalk);
                        isStand = true;
                        animator.SetBool("isStand", isStand);

                        Invoke("SceneSection2_2_end", 5f);
                        //todo 下一关
                    }
                    else if (isSection2End)
                    {
                        moveToTargetPos(new Vector3(96.3f, 1.666f, -1.39f));
                    }
                    else if (transform.position.x >= 0)
                    {
                        GameManager.instance.sectionTwoEnemyBegin(2, 6);
                    }
                    break;
            }

            if(clickDash)
            {
                dashFillTime += Time.deltaTime;
                if(dashFillTime <= dashCoolDown)
                {
                    sharkBar.fillAmount = dashFillTime / dashCoolDown;
                }
                else
                {
                    clickDash = false;
                    dashFillTime = 0;
                }
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

                    //使玩家不会被击飞很远
                    if (Mathf.Abs(rigiBody.velocity.x) >= 2 * moveSpeed)
                    {
                        rigiBody.velocity = new Vector3(rigiBody.velocity.x / Mathf.Abs(rigiBody.velocity.x) * moveSpeed * 1.2f, rigiBody.velocity.y, rigiBody.velocity.z);
                    }

                    action();
                }
            }
        }
    }

    protected void BaseFixUpdate()
    {
        Dash();
    }
    #endregion

    //加载第2关第2部分
    private void beginSection2_2()
    {
        //保存一下当前player的hitDamage（为了进入下一个场景时，player的hitDamage不变）
        GameManager.instance.setPlayerHitDamage(hitDamage);
        //清空对象池
        ObjectPool.Instance.init();
        //加载场景2
        SceneManager.LoadScene("SceneSection2_2", LoadSceneMode.Single);
    }

    //第2关结束
    private void SceneSection2_2_end()
    {
        isSection2End = true;
        txtEnd = GameObject.Find("Canvas").transform.Find("txtEnd").GetComponent<RectTransform>();
        MusicManager.instance.playAudio("Audios/Background/section1End");
        Debug.Log("SceneSection2_2_end txtEnd.position.y" + txtEnd.position.y);
        StartCoroutine(FadeInText());

        //manPlayer = GameObject.Find("manPlayer");
        //manPlayer.transform.position = new Vector3(60.3f, 1.666f, -1.39f);
        //manPlayer.GetComponent<Animator>().SetBool("isRun", true);
        //StartCoroutine(manPlayerMoveToTargetPos());
    }

    private void moveToTargetPos(Vector3 targetPosition)
    {
        //朝向
        GetComponent<SpriteRenderer>().flipX = false;

        stopAttackCircle();
        stopAttackFly();
        stopAttack();
        stopAttackUp();
        stopJumpAttack();

        isStand = false;
        animator.SetBool("isStand", isStand);

        isWalk = true;
        animator.SetBool("isWalk", isWalk);

        // 计算移动的方向和距离
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        Vector3 moveDirection = directionToTarget * (moveSpeed / 1.2f) * Time.deltaTime;

        // 移动
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveDirection.magnitude);
    }

    //字幕控制
    IEnumerator FadeInText()
    {
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration && txtEnd.position.y <= 200)
        {
            elapsedTime += Time.deltaTime;
            txtEnd.position = new Vector3(txtEnd.position.x, txtEnd.position.y + 8, txtEnd.position.z);
            yield return null;
        }

        txtEnd.position = new Vector3(txtEnd.position.x, 200, txtEnd.position.z);
    }

    ////移动man
    //IEnumerator manPlayerMoveToTargetPos()
    //{
    //    float elapsedTime = 0f;
    //    manPlayer.transform.position = new Vector3(60.3f, 1.666f, -1.39f);

    //    Debug.Log("manPlayerMoveToTargetPos==begin =" + manPlayer.transform.position.x);

    //    Vector3 targetPosition = new Vector3(96.3f, 1.666f, -1.39f);

    //    while (elapsedTime < animationDuration)
    //    {
    //        elapsedTime += Time.deltaTime;
    //        // 计算移动的方向和距离
    //        Vector3 directionToTarget = (targetPosition - manPlayer.transform.position).normalized;
    //        Vector3 moveDirection = directionToTarget * (moveSpeed / 1.2f) * Time.deltaTime;

    //        // 移动
    //        manPlayer.transform.position = Vector3.MoveTowards(manPlayer.transform.position, targetPosition, moveDirection.magnitude);
    //        yield return null;
    //    }
    //    Debug.Log("manPlayerMoveToTargetPos===" );
    //}

    #region 动作
    private void action()
    {
        if (isDisableAction) //被电击时，无法操作
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

        bool isAttackButtonDown = Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.J);
        bool isButtonAttackCircle = isAttackButtonDown && Input.GetKey(KeyCode.K);
        if ((isButtonAttackCircle || isAttackCircle) && (hitDamage + 2) < maxBlood)
        {
            attackCircle();
        }
        else if (isOnGround == true && isHitHeavy == false && isHitJump == false) //在地面上
        {
            //bool tempIsAttackFly = false;
            //attackFlyDuringTime += Time.deltaTime;
            //if (attackFlyDuringTime >= attackFlyMaxDuringTime)
            //{
            //    attackFlyStep = 0;
            //    attackFlyDuringTime = 0;
            //}

            //if (attackFlyStep == 0)
            //{
            //    if (vertical == -1)
            //    {
            //        attackFlyStep = 1;
            //    }
            //    else
            //    {
            //        attackFlyStep = 0;
            //    }
            //}
            //else if (attackFlyStep == 1)
            //{
            //    if (horizontal != 0)
            //    {
            //        //改变player朝向
            //        spriteRenderer.flipX = (horizontal > 0 ? false : true);
            //        attackFlyStep = 2;
            //    }
            //    else
            //    {
            //        attackFlyStep = 0;
            //    }
            //}
            //else if (attackFlyStep == 2)
            //{
            //    if (isAttackButtonDown)
            //    {
            //        //满足起飞攻击条件
            //        tempIsAttackFly = attackFly();
            //    }
            //}



            //if (!tempIsAttackFly)
            //{
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.K))   //跳跃
            {
                jump();
            }
            else if (vertical == 1 && horizontal == 0 && (isAttackUp || isAttackButtonDown)) //上挑攻击：正在攻击过程中 或者 按了攻击键
            {
                attackUp(isAttackButtonDown);
            }
            else if (isAttack || isAttackButtonDown)  //普通攻击：正在攻击过程中 或者 按了攻击键
            {
                attackNormal(isAttackButtonDown);
            }
            else if (Input.GetKeyDown(KeyCode.I))
            {
                attackFly();
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                if (Time.time >= (lastDash + dashCoolDown))
                {
                    //可以执行dash
                    ReadyToDash();
                }
            }
            else  //移动
            {
                move();
            }
            //}
        }
        else //在空中
        {
            if (isAttackButtonDown) //空中攻击
            {
                jumpAttack();
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                if (Time.time >= (lastDash + dashCoolDown))
                {
                    //可以执行dash
                    ReadyToDash();
                }
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

    private void ReadyToDash()
    {
        isDashing = true;

        dashTimeLeft = dashTime;

        lastDash = Time.time;

        sharkBar.fillAmount = 1;
    }

    private void Dash()
    {
        if (isDashing)
        {
            clickDash = true;
            //冲锋时，无敌帧
            setUnmatched(dashTime);

            int direct = spriteRenderer.flipX ? -1 : 1;
            if (dashTimeLeft > 0)
            {
                //if (rigiBody.velocity.y > 0 && !isOnGround)
                //{
                //    rigiBody.velocity = new Vector2(dashSpeed * direct, jumpForce);//在空中Dash向上
                //}
                rigiBody.velocity = new Vector2(dashSpeed * direct, rigiBody.velocity.y);//地面Dash

                dashTimeLeft -= Time.deltaTime;

                ShadowPool.instance.GetFormPool();
            }
            if (dashTimeLeft <= 0)
            {
                isDashing = false;
                //if (!isOnGround)
                //{
                //    //目的为了在空中结束 Dash 的时候可以接一个小跳跃。根据自己需要随意删减调整
                //    rigiBody.velocity = new Vector2(dashSpeed * direct, jumpForce);
                //}
            }
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
            if(isStand)
            {
                isStand = false;
                animator.SetBool("isStand", isStand);
            }
            //角色朝向
            if (horizontal != 0)
            {
                spriteRenderer.flipX = (horizontal > 0 ? false : true);
            }
            //获取和设置刚体的线性速度
            rigiBody.velocity = new Vector3(horizontal * moveSpeed, 0, vertical * moveSpeed);

            if(!isWalk)
            {
                isWalk = true;
                animator.SetBool("isWalk", isWalk);
            }
        }
        else
        {
            rigiBody.velocity = new Vector3(0, rigiBody.velocity.y, 0);

            if(isWalk)
            {
                isWalk = false;
                animator.SetBool("isWalk", isWalk);
            }
           
            if(!isStand)
            {
                isStand = true;
                animator.SetBool("isStand", isStand);
            }
        }
    }

    private void attackNormal(bool isAttackButtonDown)
    {
        stopWalkAndStand();
        stopAttackUp();

        //使攻击的武器朝向和player保持一致
        attackObj.transform.localScale = new Vector3(spriteRenderer.flipX ? -1 : 1, 1, 1);

        attackDuringTime += Time.deltaTime;

        if (isAttackButtonDown && attackDuringTime > attackDirectionMaxDuringTime)
        {
            attackDuringTime = 0;
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
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("attackNormal5"))
            {
                attackStep = 1;
                animator.SetInteger("attackStep", attackStep);
            }
            else
            {
                attackStep = 1;
                animator.SetInteger("attackStep", attackStep);
            }
        }

        
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
        if (isDie)
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

            if (isPlayerGirl)
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

    private void attackUp(bool isAttackButtonDown)
    {
        stopWalkAndStand();
        stopAttack();
        //使攻击的武器朝向和player保持一致
        attackObj.transform.localScale = new Vector3(spriteRenderer.flipX ? -1 : 1, 1, 1);

        if (isAttackButtonDown)
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
            //玩家向上移动一点，便于起跳
            rigiBody.velocity = new Vector3(rigiBody.velocity.x, rigiBody.velocity.y, 0);
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);
            //给角色一个瞬间斜向上的力
            rigiBody.AddForce(new Vector3(spriteRenderer.flipX ? -flyForce : flyForce, flyForce, 0), ForceMode.Impulse);
        }
        //重置
        //attackFlyStep = 0;
        //attackFlyDuringTime = 0;
        return isAttackFly;
    }

    protected void stopAttackFly()
    {
        if (isAttackFly)
        {
            isAttackFly = false;

            isStand = true;
            animator.SetBool("isStand", isStand);
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

    protected void stopAttackCircle()
    {
        if (isAttackCircle)
        {
            isAttackCircle = false;
            animator.SetBool("isAttackCircle", isAttackCircle);
        }
    }

    private void stopWalkAndStand()
    {
        if(isWalk)
        {
            isWalk = false;
            animator.SetBool("isWalk", isWalk);
        }

        if(isStand)
        {
            isStand = false;
            animator.SetBool("isStand", isStand);
        }
    }

    protected void stopAttackUp()
    {
        if (isAttackUp)
        {
            isAttackUp = false;
            animator.SetBool("isAttackUp", isAttackUp);
            attackUpStep = 0;
            animator.SetInteger("attackUpStep", attackUpStep);
        }
    }

    protected void stopAttack()
    {
        if (isAttack)
        {
            isAttack = false;
            animator.SetBool("isAttack", isAttack);
            attackStep = 0;
            animator.SetInteger("attackStep", attackStep);
        }
    }

    protected void stopJumpAttack()
    {
        if (isJumpAttack)
        {
            isJumpAttack = false;
            isStand = true;
            animator.SetBool("isStand", isStand);
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

        if (!isDotSetTransparent)  //过关时只是无敌，不会变透明
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