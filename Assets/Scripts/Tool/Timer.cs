using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    //开始时，剩余的时间
    [HideInInspector]
    public float timeLeft;

    private int oldIntTimeLeft = 0;

    //是否开始
    [HideInInspector]
    public bool isBegin = false;

    //显示秒的组件
    public GameObject secTen;   //十位
    public GameObject secOne;   //个位

    void Start()
    {

    }

    void Update()
    {
        if (isBegin)
        {
            oldIntTimeLeft = (int)System.Math.Floor(timeLeft);
            timeLeft -= Time.deltaTime;
            int newIntTimeLeft = (int)System.Math.Floor(timeLeft);

            if (oldIntTimeLeft != newIntTimeLeft /* && newIntTimeLeft <= 5*/) //说明秒发生了变化/*且剩余5秒*/，则播放秒钟音效
            {
                AudioSource audioSource = gameObject.GetComponent<AudioSource>();
                audioSource.Play();
            }

            change(newIntTimeLeft);
        }
    }

    public void begin(float timeLeftIn)
    {
        timeLeft = timeLeftIn;
        isBegin = true;
    }

    public void end()
    {
        isBegin = false;
    }

    //数字显示
    private void change(int num)
    {
        if (num > 99)
        {
            throw new System.Exception("最大支持99");
        }
        else if (num < 0) //倒计时结束
        {
            isBegin = false;
            //返回游戏开始页面
            SceneManager.LoadScene("SceneStart", LoadSceneMode.Single);
        }
        else
        {
            int tenUnit = Mathf.FloorToInt(num / 10);
            int oneUnit = num % 10;
            (secTen.GetComponent<Image>()).sprite = Resources.Load<Sprite>("Images/Map/Timer/" + tenUnit.ToString()) as Sprite;
            (secOne.GetComponent<Image>()).sprite = Resources.Load<Sprite>("Images/Map/Timer/" + oneUnit.ToString()) as Sprite;
        }
    }
}
