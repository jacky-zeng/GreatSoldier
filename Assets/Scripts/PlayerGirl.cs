
using UnityEngine;
using UnityEngine.Video;

public class PlayerGirl : PlayerBase
{
    private bool isCheckHitMan = true;
    private bool isDisablePlayer = false;
    private bool isTriggerManPlayerOk = false;

    #region 初始化
    private void Awake()
    {
        base.BaseAwake();
    }

    // Start is called before the first frame update
    void Start()
    {
        base.BaseStart();
    }

    #endregion

    #region 帧
    private void LateUpdate()
    {
        base.BaseLateUpdate();
    }

    // Update is called once per frame
    void Update()
    {
        if (base.hitDamage >= base.maxBlood) //死亡
        {
            die();
        }
        else if (isCheckHitMan && base.sceneName == "SceneSection2_1")
        {
            //敌人全部消灭后，人物往电视机那里走
            if (isDisablePlayer || GameManager.instance.isSectionTwoEnemyAllDied(1))
            {
                GameObject.Find("manPlayer").GetComponent<Animator>().SetBool("isLaugh", true);
                GameObject.Find("knife").GetComponent<SpriteRenderer>().enabled = true;
                isDisablePlayer = true;
                walk(transform.position, new Vector3(0.7085f, 0.9297f, 0.0812f));
            }

            //远离时 视频静音
            if(transform.position.x > 18.776f)
            {
                GameObject.Find("Video").GetComponent<VideoPlayer>().Pause();
                //GameObject.Find("Video").GetComponent<VideoPlayer>().enabled = false;
            } else if(isTriggerManPlayerOk)
            {
                GameObject.Find("Video").GetComponent<VideoPlayer>().Play();
                //GameObject.Find("Video").GetComponent<VideoPlayer>().enabled = true;
            }
        }

        if(!isDisablePlayer)
        {
            base.BaseUpdate();
        }
    }
    #endregion

    private void walk(Vector3 currentPosition, Vector3 targetPosition)
    {
        //朝向
        GetComponent<SpriteRenderer>().flipX = true;

        base.stopAttack();
        base.stopAttackUp();
        base.stopJumpAttack();

        animator.SetBool("isWalk", true);

        // 计算移动的方向和距离
        Vector3 directionToTarget = (targetPosition - currentPosition).normalized;
        Vector3 moveDirection = directionToTarget * (moveSpeed / 1.5f) * Time.deltaTime;

        // 移动
        transform.position = Vector3.MoveTowards(currentPosition, targetPosition, moveDirection.magnitude);

        //走到电视机前，人物“嘿嘿2”
        //踢飞人物，电视机器关掉
        if (Mathf.Abs(Vector3.Distance(currentPosition, targetPosition)) <= 1.5f)  //currentPosition == targetPosition 
        {
            //不在检查是否攻击了manPlayer
            isCheckHitMan = false;

            //trigger后，播放人物“嘿嘿”动画
            GameObject obj = GameObject.Find("turnOnTv");

            //人物“嘿嘿”笑
            AudioClip clip = Resources.Load<AudioClip>("Audios/Show/manLaugh");
            obj.GetComponent<AudioSource>().clip = clip;
            obj.GetComponent<AudioSource>().Play();

            Invoke("hitFlyScreen", 1.5f);
        }
    }

    #region 碰撞
    //接触到
    private void OnCollisionEnter(Collision collision)
    {
        base.BaseOnCollisionEnter(collision);
    }

    //待在里面
    private void OnCollisionStay(Collision collision)
    {
        base.BaseOnCollisionStay(collision);
    }

    //离开地面
    private void OnCollisionExit(Collision collision)
    {
        base.BaseOnCollisionExit(collision);
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

    //trigger碰撞
    protected void OnTriggerEnterOrStay(Collider collision, string type)
    {
        if (!base.isDie && !base.isUnmatched)
        {
            if (collision.gameObject.name == "EnemyAttack") //受到敌人攻击
            {
                bool directionHit = collision.transform.parent.position.x > transform.position.x ? true : false;
                if (collision.transform.parent.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("attackHeavy"))
                {
                    //Log("Heavy hitDamage=" + base.hitDamage + "受到伤害EnemyAttack" + collision.transform.parent.position.x + "|" + transform.position.x, true);

                    hitHeavy(directionHit);
                }
                else if (collision.transform.parent.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("jumpAttack"))
                {
                    hitJump(directionHit);
                }
                if (collision.transform.parent.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("attackElectric"))
                {
                    hitElectric();
                }
                else
                {
                    //Log("Normal hitDamage=" + base.hitDamage + "受到伤害EnemyAttack" + collision.transform.parent.position.x + "|" + transform.position.x, true);

                    hit(directionHit);
                }
            }
            else if (collision.gameObject.name.StartsWith("ToolGangKnife")) //  受到闸刀攻击
            {
                bool directionHit = collision.transform.position.x > transform.position.x ? true : false;
                hitHeavy(directionHit);
            }
            else if (collision.gameObject.name.StartsWith("Basketball")) // Basketball(Clone) 受到篮球攻击
            {
                hit(true);
                collision.gameObject.GetComponent<BasketBall>().setDie();
            }
            else if (collision.gameObject.name.StartsWith("trigger")) // Basketball(Clone) 受到篮球攻击
            {
                if(collision.gameObject.name == "triggerManPlayer")
                {
                    isTriggerManPlayerOk = true;
                    //trigger后，播放开机动画
                    Destroy(collision.gameObject);
                    GameObject obj = GameObject.Find("turnOnTv");
                    obj.GetComponent<Animator>().enabled = true;

                    //开机声
                    AudioClip clip = Resources.Load<AudioClip>("Audios/Show/turnOnTv");
                    obj.GetComponent<AudioSource>().clip = clip;
                    obj.GetComponent<AudioSource>().Play();

                    Invoke("videoBegin", 1f);
                }
            }
        }
    }
    #endregion

    //播放视频
    private void videoBegin()
    {
        //开机后，禁用开机动画,人物“嘿嘿”笑
        GameObject obj = GameObject.Find("turnOnTv");
        obj.GetComponent<Animator>().enabled = false;
        obj.GetComponent<SpriteRenderer>().enabled = false;
        AudioClip clip = Resources.Load<AudioClip>("Audios/Show/manLaugh");
        obj.GetComponent<AudioSource>().clip = clip;
        obj.GetComponent<AudioSource>().Play();

        //播放视频
        GameObject.Find("Video").GetComponent<VideoPlayer>().enabled = true;
    }

    //踢飞到屏幕
    private void hitFlyScreen()
    {
        playAudio("Audios/Tool/wumanAttack1");

        //关闭视频
        GameObject.Find("Video").GetComponent<VideoPlayer>().enabled = false;

        GameObject obj = GameObject.Find("turnOnTv");
        obj.GetComponent<Animator>().enabled = false;

        //关闭视频的图片
        obj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/Map/Show/turnOnTvOff") as Sprite;
        obj.GetComponent<SpriteRenderer>().enabled = true;

        //踢飞动画
        GameObject.Find("manPlayer").GetComponent<Animator>().SetBool("isHit", true);
       
        //撞击屏幕后的声音
        AudioClip clip = Resources.Load<AudioClip>("Audios/Show/manHitScreen");
        obj.GetComponent<AudioSource>().clip = clip;
        obj.GetComponent<AudioSource>().Play();

        //玩家可以自由移动了
        isDisablePlayer = false;
    }

    //受到普通伤害
    public void hit(bool directionHit)
    {
        //hit2Style();

        if (!base.isHit && !base.isAttackCircle)
        {
            base.isHit = true;
            ++base.hitDamage;
            playAudio("Audios/Tool/womanHit");
            base.rigiBody.AddForce(new Vector3(directionHit ? -10 : 10, 0, 0), ForceMode.Impulse);
            base.animator.SetTrigger("triggerHit");
            Invoke("animatorHitEndEvent", 2.5f);
        }
    }

    //受到重击伤害
    public void hitHeavy(bool directionHit)
    {
        if (!base.isHitHeavy && !base.isAttackCircle)
        {
            base.isHitHeavy = true;
            base.hitDamage += 2;
            playAudio("Audios/Tool/womanHit");
            base.rigiBody.AddForce(new Vector3(directionHit ? -15 : 15, 15, 0), ForceMode.Impulse);
            base.animator.SetTrigger("triggerHitHeavy");
            Invoke("animatorHitHeavyEndEvent", 2.5f);
        }
    }

    //受到电击伤害
    public void hitElectric()
    {
        if(!base.isHitElectric)
        {
            base.isHitElectric = true;
            ++base.hitDamage;
            playAudio("Audios/Tool/womanHit");
            base.setUnmatched(1.5f, true); //被电时，无敌1.5秒
            base.animator.SetTrigger("triggerHitE");
            Invoke("animatorHitElectricEndEvent", 1f);
        }
    }

    //受到跳起来的下砸伤害
    public void hitJump(bool directionHit)
    {
        if (!base.isHitJump && !base.isAttackCircle)
        {
            base.isHitJump = true;
            base.hitDamage += 3;
            playAudio("Audios/Tool/womanHit");
            base.rigiBody.AddForce(new Vector3(directionHit ? -15 : 15, 20, 0), ForceMode.Impulse);
            base.animator.SetTrigger("triggerHitHeavy");
            Invoke("animatorHitHeavyEndEvent", 2.5f);
        }
    }

    //hit动画事件 受击结束后
    private void animatorHitEndEvent()
    {
        base.isHit = false;

        ////静止
        //rigi.velocity = new Vector3(0, 0, 0);
        ////摆正
        //transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    //hitHeavy动画事件 受击结束后
    private void animatorHitHeavyEndEvent()
    {
        base.isHitHeavy = false;
        base.isHitJump = false;

        ////静止
        //rigi.velocity = new Vector3(0, 0, 0);
        ////摆正
        //transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    private void animatorHitElectricEndEvent()
    {
        base.isHitElectric = false;
    }

    private void die()
    {
        if (!base.isDie)
        {
            base.isDie = true;

            //慢放
            Time.timeScale = 0.3f;

            base.animator.SetBool("isDie", true);
            (base.canvasTimer.GetComponent<CanvasTimer>()).timeLeft = 19;
            playAudio("Audios/Tool/womanDie");
        }
    }

    //播放完死亡动画后，调用该方法
    public void animatorDieEvent()
    {
        Time.timeScale = 1;
        //显示倒计时界面
        base.canvasContinue.SetActive(true);
        base.canvasTimer.SetActive(true);
        (base.canvasTimer.GetComponent<CanvasTimer>()).begin(19);

        base.animator.SetBool("isDie", false);
    }
}