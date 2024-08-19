using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeiLuXing : MonoBehaviour
{
    private Animator animator;
    private AudioSource audioSource;
    private bool isBegin = false;
    private bool isGameEnd = false;
    private bool isDoorOpen = false;
    private float beginAfterTime = 3f;

    public GameObject medicineBall;
    public GameObject waterBall;

    public GameObject openDoor;

    private int moveSpeed = 10;
    private bool isMoveUp = false;

    private Vector3 beginPos = new Vector3(76f, 1.666f, -2f);
    private Vector3 endPos = new Vector3(76f, 1.666f, -13f);
    private Vector3 gameEndPos = new Vector3(60.3f, 1.666f, -1.39f);
    private Vector3 gameEndDoorOpenPos = new Vector3(96.3f, 1.666f, -1.39f);

    private bool isAttackHeavy = false; //是否在释放大招
    private bool isAttackM = false;     //是否在投掷药物子弹
    private bool isAttackBall = false;  //是否在投掷水弹

    private float attackMaxDuringTime = 3f;  //攻击持续最大时间
    private float attackDuringTime = 0;      //攻击已持续时间

    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        GetComponent<SpriteRenderer>().flipX = true;
    }

    #region 初始化
    void Start()
    {
        Invoke("begin", beginAfterTime); //延时开始
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        if (isBegin)
        {
            if (isGameEnd)
            {
                moveToGameEndPos();
            }
            else
            {
                if (!isAttackHeavy && !isAttackM && !isAttackM)
                {
                    walk();
                }
            }
        }
        if(isDoorOpen)
        {
            Vector3 moveDirection = new Vector3(1, 0, 0) * moveSpeed * Time.deltaTime;

            transform.position = Vector3.MoveTowards(transform.position, gameEndDoorOpenPos, moveDirection.magnitude);
        }
    }

    public void begin()
    {
        isBegin = true;
    }

    //由玩家调用
    public void gameEnd()
    {
        isGameEnd = true;
        GameObject.Find("wallRight").SetActive(false);
    }

    private void moveToGameEndPos()
    {
        Vector3 moveDirection = new Vector3(0, 0, -1) * moveSpeed * Time.deltaTime;

        transform.position = Vector3.MoveTowards(transform.position, gameEndPos, moveDirection.magnitude);

        if (Mathf.Abs(transform.position.x - gameEndPos.x) <= 0.5f) //就位了
        {
            attackHeavy();
        }
    }

    //移动
    private void walk()
    {
        if (isMoveUp) //向上移动
        {
            Vector3 moveDirection = new Vector3(0, 0, 1) * moveSpeed * Time.deltaTime;

            transform.position = Vector3.MoveTowards(transform.position, beginPos, moveDirection.magnitude);

            float distance = Mathf.Abs(transform.position.z - beginPos.z);

            attackDuringTime += Time.deltaTime;

            if (distance <= 0.5f)
            {
                isMoveUp = false;
            }
            else if (!isAttackM && distance > 1.5f && distance < 2)
            {
                if (attackDuringTime >= attackMaxDuringTime)
                {
                    attackDuringTime = 0;
                    attackM();
                }

            }
        }
        else      //向下移动
        {
            Vector3 moveDirection = new Vector3(0, 0, -1) * moveSpeed * Time.deltaTime;

            transform.position = Vector3.MoveTowards(transform.position, endPos, moveDirection.magnitude);

            float distance = Mathf.Abs(transform.position.z - endPos.z);

            attackDuringTime += Time.deltaTime;

            if (distance <= 0.5f)
            {
                isMoveUp = true;
            }
            else if (!isAttackBall && distance > 1.5f && distance < 2)
            {
                if (attackDuringTime >= attackMaxDuringTime)
                {
                    attackDuringTime = 0;
                    attackBall();
                }
            }
        }
    }

    //发射药物子弹
    private void attackM()
    {
        if (!isAttackM)
        {
            isAttackM = true;
            animator.SetBool("isAttackM", isAttackM);
            Invoke("stopAttackM", 3.5f);
        }
    }

    //发射水子弹
    private void attackBall()
    {
        if (!isAttackBall)
        {
            isAttackBall = true;
            animator.SetBool("isAttackBall", isAttackBall);
            Invoke("stopAttackBall", 3.5f);
        }
    }

    private void stopAttackHeavy()
    {
        isBegin = false;

        if (isAttackHeavy)
        {
            isAttackHeavy = false;
            animator.SetBool("isAttackHeavy", isAttackHeavy);
        }
        //animator.enabled = false;

        //打开门
        openDoor.SetActive(true);

        isDoorOpen = true;
    }

    private void stopAttackM()
    {
        if (isAttackM)
        {
            isAttackM = false;
            animator.SetBool("isAttackM", isAttackM);
        }
    }

    private void stopAttackBall()
    {
        if (isAttackBall)
        {
            isAttackBall = false;
            animator.SetBool("isAttackBall", isAttackBall);
        }
    }

    //动画调用药物子弹发射
    public void animatorAttackMEvent()
    {
        if (isAttackM)
        {
            playAudio("Audios/Enemy/MeiLuXing/yao");

            //药物子弹位置
            Vector3 pos = transform.position;
            pos.x -= 2.5f;
            //pos.y += 2.5f;
            GameObject bulletMedicineBall = ObjectPool.Instance.Get(medicineBall, pos, Vector3.one, true);

            //改变旋转角度
            bulletMedicineBall.GetComponent<Medicine>().setRotation(180);

            stopAttackM();
        }
    }

    //动画调用水子弹发射
    public void animatorAttackBallEvent()
    {
        if (isAttackBall)
        {
            playAudio("Audios/Enemy/MeiLuXing/paopao");
            //子弹位置
            Vector3 pos = transform.position;
            pos.x -= 2.5f;
            //pos.y += 2.5f;
            GameObject bulletWaterBall = ObjectPool.Instance.Get(waterBall, pos, Vector3.one, true);

            //改变旋转角度
            bulletWaterBall.GetComponent<WaterBall>().setRotation(180);

            stopAttackBall();
        }
    }

    //把门喷开
    public void attackHeavy()
    {
        if (!isAttackHeavy)
        {
            playAudio("Audios/Enemy/MeiLuXing/water");
            isAttackHeavy = true;
            GetComponent<SpriteRenderer>().flipX = false;

            stopAttackBall();
            stopAttackM();

            isAttackHeavy = true;
            animator.SetBool("isAttackHeavy", isAttackHeavy);

            Invoke("stopAttackHeavy", 2.5f);
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
            namePath = "Audios/Enemy/" + fileName;
        }

        AudioClip clip = Resources.Load<AudioClip>(namePath);

        audioSource.clip = clip;
        audioSource.Play();
    }

}
