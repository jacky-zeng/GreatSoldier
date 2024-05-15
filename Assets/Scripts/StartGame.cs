using UnityEngine;
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

    // Use this for initialization
    void Start()
    {
        MusicManager.instance.playAudio("Audios/Background/start");
        initBg();
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

            if (!isBegin && Input.GetKeyDown(KeyCode.Return))
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
                p4Obj.SetActive(false); break;
            case 3:
                p1Obj.SetActive(false);
                p2Obj.SetActive(false);
                p3Obj.SetActive(true);
                p4Obj.SetActive(false); break;
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
        if (pIndex == 1)
        {
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
        SceneManager.LoadScene("SceneSection1_1", LoadSceneMode.Single);
    }

}
