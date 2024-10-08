﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    private bool isCameraActionEnd = false;
    private int startIndex = 1;
    private int pIndex = 1;

    public Image startBg;

    public GameObject p1Obj;
    public GameObject p2Obj;
    public GameObject p3Obj;
    public GameObject p4Obj;

    private bool isBegin = false;

    private void Awake()
    {
        //Screen.SetResolution(1920, 1080, false);
        Screen.SetResolution(1600, 900, false);
    }

    // Use this for initialization
    void Start()
    {
        PlayerPrefs.SetInt("isTest", 0);
        CanvasContinue.instance.gameObject.SetActive(false);
        CanvasTimer.instance.gameObject.SetActive(false);

        MusicManager.instance.playAudio("Audios/Background/start");
        initBg();
        GameManager.instance.init();
    }

    // Update is called once per frame
    void Update()
    {
        if (isCameraActionEnd)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                --pIndex;
                changeP();
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                ++pIndex;
                changeP();
            }

            if (!isBegin && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.J)))
            {
                begin();
            }
        }
    }

    private void initBg()
    {
        ++startIndex;
        if (startIndex >= 6)
        {
            isCameraActionEnd = true;
            p1Obj.SetActive(true);
        }
        else
        {
            startBg.sprite = Resources.Load<Sprite>("Images/Map/Start/" + startIndex) as Sprite;
            Invoke("initBg", 0.8f);
        }
    }

    //选角色
    private void changeP()
    {
        if (pIndex <= 0)
        {
            pIndex = 4;
        }
        else if (pIndex >= 5)
        {
            pIndex = 1;
        }

        MusicManager.instance.playAudio("Audios/Background/ding4");
        switch (pIndex)
        {
            case 1:
                p1Obj.SetActive(true);
                p2Obj.SetActive(false);
                p3Obj.SetActive(false);
                p4Obj.SetActive(false);
                break;
            case 2:
                p1Obj.SetActive(false);
                p2Obj.SetActive(true);
                p3Obj.SetActive(false);
                p4Obj.SetActive(false);
                break;
            case 3:
                p1Obj.SetActive(false);
                p2Obj.SetActive(false);
                p3Obj.SetActive(true);
                p4Obj.SetActive(false);
                break;
            case 4:
                p1Obj.SetActive(false);
                p2Obj.SetActive(false);
                p3Obj.SetActive(false);
                p4Obj.SetActive(true);
                break;
        }
    }

    private void begin()
    {
        if (pIndex == 1 || pIndex == 3) //  目前只支持2个角色选择
        {
            GameManager.instance.setPIndex(pIndex);
            isBegin = true;
            MusicManager.instance.playAudio("Audios/Background/begin");
            Invoke("beginSection", 0.3f);
        } else
        {
            MusicManager.instance.playAudio("Audios/Background/stop");
        }
    }

    private void beginSection()
    {
        //SceneManager.LoadScene("SceneSection2_1", LoadSceneMode.Single);
        SceneManager.LoadScene("SceneSection1_1", LoadSceneMode.Single);
    }

}
