using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//钢刀 （从上面砸下）
public class GangKnife : MonoBehaviour
{
    private bool isBegin = false;
    private bool isBeginReal = false;
    private float beginRealAfterTime = 0.5f;

    private float attackMaxDuringTime = 5f;  //攻击持续最大时间
    private float attackDuringTime = 0;      //攻击已持续时间

    private GameObject player;

    private float beginY = 15;
    private float endY = -9f;
    private int speed = 8;

    private bool isAttackEnd = false;
    private bool isCanAttack = true;

    private void Awake()
    {

    }

    #region 初始化
    void Start()
    {
        player = GameObject.Find("Player");
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        if (isBeginReal)
        {
            if (attackDuringTime == 0)
            {
                GetComponent<AudioSource>().Play();
            }

            if (attackDuringTime >= attackMaxDuringTime)
            {
                attackDuringTime = 0;
            }
            else
            {
                attackDuringTime += Time.deltaTime;
            }

            if (isAttackEnd && attackDuringTime >= 2)
            {
                moveBack();
            }
            else if (isCanAttack)
            {
                //下砸攻击
                attack();
            }
        }
    }

    public void begin()
    {
        if (!isBegin)
        {
            isBegin = true;
            Invoke("beginReal", beginRealAfterTime); //延时，为了player能够看到开始时的敌人动画
        }
    }

    private void beginReal()
    {
        if (player == null)
        {
            player = GameObject.Find("Player");
        }
        isBeginReal = true;
    }

    //下砸
    private void attack()
    {
        Vector3 targetPosition = new Vector3(transform.position.x, endY, 0);

        Vector3 moveDirection = (new Vector3(0, -1, 0)) * speed * 1.5f * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveDirection.magnitude);

        if (Mathf.Abs(transform.position.y - endY) <= 1.5f)  //说明下砸到位了
        {
            isCanAttack = false;
            isAttackEnd = true;
        }
    }

    //上拉回原位
    private void moveBack()
    {
        Vector3 targetPosition = new Vector3(transform.position.x, beginY, 0);

        Vector3 moveDirection = (new Vector3(0, 1, 0)) * speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveDirection.magnitude);

        if (Mathf.Abs(transform.position.y - beginY) <= 1.5f)  //说明上拉到位了
        {
            isCanAttack = true;
            isAttackEnd = false;
            transform.position = new Vector3(player.transform.position.x, transform.position.y, transform.position.z);
        }
    }
}
