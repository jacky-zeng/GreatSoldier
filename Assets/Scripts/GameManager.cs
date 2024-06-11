using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    // 定义一个静态的GameManager实例
    public static GameManager instance;

    private string sceneName; //场景名称
    public GameObject prefabGhost;

    private bool isOkSection1 = false;
    private bool isOkSection2 = false;
    private bool isOkSection3 = false;

    private Dictionary<string, GameObject> section1Enemys = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> section2Enemys = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> section3Enemys = new Dictionary<string, GameObject>();

    void Awake()
    {
        // 如果实例不存在，则设置为当前实例
        if (instance == null)
        {
            instance = this;
            // 使对象不被销毁
            DontDestroyOnLoad(gameObject);
        }
        // 如果已经存在实例，则销毁当前重复的实例
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        switch (sceneName)
        {
            case "SceneSection1_1":
                if (!isOkSection1)
                {
                    section1Load(1);
                    Debug.Log("GameManager " + sceneName);
                }
                break;
            case "SceneSection1_2":
                if (!isOkSection2)
                {
                    Debug.Log("GameManager " + sceneName);
                }
                break;
            case "SceneSection1_3":
                if (!isOkSection3)
                {
                    Debug.Log("GameManager " + sceneName);
                }
                break;
        }
    }

    //加载第一关第index部分的敌人
    private void section1Load(int index)
    {
        if (index == 1)
        {
            isOkSection1 = true;

            GameObject gameObjectInit1 = Instantiate(prefabGhost);
            gameObjectInit1.transform.localPosition = new Vector3(28.73f, 0.85f, 3);  //必须使用localPosition
            gameObjectInit1.GetComponent<Ghost>().changeAnimatorStatus(1);
            string gameObject1Name = "EnemyGhost1_" + index.ToString();
            gameObjectInit1.gameObject.name = gameObject1Name;

            section1Enemys.Add(gameObject1Name, gameObjectInit1);

            GameObject gameObjectInit2 = Instantiate(prefabGhost);
            gameObjectInit2.transform.localPosition = new Vector3(18.73f, 0.85f, 3);  //必须使用localPosition
            gameObjectInit2.GetComponent<Ghost>().changeAnimatorStatus(2);
            string gameObject2Name = "EnemyGhost2_" + index.ToString();
            gameObjectInit2.gameObject.name = gameObject2Name;

            section1Enemys.Add(gameObject2Name, gameObjectInit2);
        }
    }

    //第一关第index部分的敌人死了
    public void section1EnemyDied(int index, string gameObjectName)
    {
        if (index == 1)
        {
            section1Enemys.Remove(gameObjectName);
        }
    }

    //第一关第index部分的敌人全部死了
    public bool isSection1EnemyAllDied(int index)
    {
        if (index == 1)
        {
            if (section1Enemys.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

}