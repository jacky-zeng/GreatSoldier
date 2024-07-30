using UnityEngine;

public class Player : PlayerBase
{
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
        base.BaseUpdate();
    }
    #endregion

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
                else
                {
                    //Log("Normal hitDamage=" + base.hitDamage + "受到伤害EnemyAttack" + collision.transform.parent.position.x + "|" + transform.position.x, true);

                    hit(directionHit);
                }
            }
            else if (collision.gameObject.name.StartsWith("Basketball")) // Basketball(Clone) 受到篮球攻击
            {
                hit(true);
                collision.gameObject.GetComponent<BasketBall>().setDie();
            }
        }
    }
    #endregion

    //受到普通伤害
    public void hit(bool directionHit)
    {
        //hit2Style();

        if (!base.isHit && !base.isAttackCircle)
        {
            base.isHit = true;
            ++base.hitDamage;
            playAudio("Audios/Tool/manHit1");
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
            playAudio("Audios/Tool/manHit1");
            base.rigiBody.AddForce(new Vector3(directionHit ? -15 : 15, 15, 0), ForceMode.Impulse);
            base.animator.SetTrigger("triggerHitHeavy");
            Invoke("animatorHitHeavyEndEvent", 2.5f);
        }
    }

    //受到跳起来的下砸伤害
    public void hitJump(bool directionHit)
    {
        if (!base.isHitJump && !base.isAttackCircle)
        {
            base.isHitJump = true;
            base.hitDamage += 3;
            playAudio("Audios/Tool/manHit1");
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

    private void die()
    {
        if(!base.isDie)
        {
            base.isDie = true;

            //慢放
            Time.timeScale = 0.3f;

            base.animator.SetBool("isDie", true);
            (base.canvasTimer.GetComponent<CanvasTimer>()).timeLeft = 19;
            playAudio("Audios/Tool/manHit");
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