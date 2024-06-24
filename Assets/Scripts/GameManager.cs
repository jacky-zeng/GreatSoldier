using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    // 定义一个静态的GameManager实例
    public static GameManager instance;

    private string sceneName; //场景名称
    public GameObject prefabGhost;
    public GameObject prefabKnife;

    private bool isOkSectionOne1 = false;
    private bool isOkSectionOne2 = false;
    private bool isOkSectionOne3 = false;

    private Dictionary<string, GameObject> sectionOne1Enemys = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> sectionOne2Enemys = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> sectionOne3Enemys = new Dictionary<string, GameObject>();

    //player相关信息
    private float playerPosZ;           //切换场景时，记录在上一个场景中的z值
    private float playerHitDamage = 0;  //切换场景时，记录在上一个场景中的hitDamage值

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
                if (!isOkSectionOne1)
                {
                    sectionOneLoad(1);
                    //Debug.Log("GameManager " + sceneName);
                }
                break;
            case "SceneSection1_2":
                if (!isOkSectionOne2)
                {
                    sectionOneLoad(2);
                    //Debug.Log("GameManager " + sceneName);
                }
                break;
            case "SceneSection1_3":
                if (!isOkSectionOne3)
                {
                    //Debug.Log("GameManager " + sceneName);
                }
                break;
        }
    }

    public void init()
    {
        isOkSectionOne1 = false;
        isOkSectionOne2 = false;
        isOkSectionOne3 = false;

        sectionOne1Enemys = new Dictionary<string, GameObject>();
        sectionOne2Enemys = new Dictionary<string, GameObject>();
        sectionOne3Enemys = new Dictionary<string, GameObject>();

        //清空对象池
        ObjectPool.Instance.init();
    }

    public float getPlayerPosZ()
    {
        return playerPosZ;
    }

    public void setPlayerPosZ(float posZ)
    {
        playerPosZ = posZ;
    }

    public float getPlayerHitDamage()
    {
        return playerHitDamage;
    }

    public void setPlayerHitDamage(float hitDamage)
    {
        playerHitDamage = hitDamage;
    }

    //加载第一关第index部分的敌人
    private void sectionOneLoad(int index)
    {
        //敌人gameobject命名 #关卡  ｜第几个敌人  _关卡中的场景index
        if (index == 1)
        {
            isOkSectionOne1 = true;

            enemyAdd(index, "EnemyGhost#1|1", new Vector3(28.73f, 0.85f, 3), 1);
            enemyAdd(index, "EnemyGhost#1|2", new Vector3(23.9f, 0.85f, -1), 2);
            enemyAdd(index, "EnemyGhost#1|3", new Vector3(28.88f, 0.846f, -8.95f), 3);

            enemyAdd(index, "EnemyGhost#1|4", new Vector3(58.5f, 0.85f, -4.35f), 2);

            enemyAdd(index, "EnemyKnife#1|5", new Vector3(58.5f, 0.85f, -4.35f), 2);
            enemyAdd(index, "EnemyKnife#1|6", new Vector3(52.5f, 0.85f, -2.35f), 2);
        }
        else if (index == 2)
        {
            isOkSectionOne2 = true;
            enemyAdd(index, "EnemyGhost#1|1", new Vector3(-1.7f, 0.846f, -10.52f), 3);
            enemyAdd(index, "EnemyKnife#1|2", new Vector3(8f, 0.846f, -8.52f), 3);
        }
    }

    private void enemyAdd(int index, string name, Vector3 pos, int animatorType)
    {
        GameObject gameObjectInit = null;
        if (name.StartsWith("EnemyGhost"))
        {
            gameObjectInit = Instantiate(prefabGhost);
            gameObjectInit.transform.localPosition = pos;  //必须使用localPosition
            gameObjectInit.GetComponent<Ghost>().changeAnimatorStatus(animatorType);
        } else if(name.StartsWith("EnemyKnife"))
        {
            gameObjectInit = Instantiate(prefabKnife);
            gameObjectInit.transform.localPosition = pos;  //必须使用localPosition
        }
        
        string gameObjectName = name + "_" + index.ToString();
        gameObjectInit.gameObject.name = gameObjectName;
        if (index == 1)
        {
            sectionOne1Enemys.Add(gameObjectName, gameObjectInit);
        }
        else if (index == 2)
        {
            sectionOne2Enemys.Add(gameObjectName, gameObjectInit);
        }
    }

    //Player.cs调用该方法（player走到一定位置，触发敌人开始）
    public void sectionOneEnemyBegin(int index, int max)
    {
        if (index == 1)
        {
            foreach (KeyValuePair<string, GameObject> sectionOne1Enemy in sectionOne1Enemys)
            {
                int enemy = int.Parse(sectionOne1Enemy.Key.Split("_")[0].Split("|")[1]);
                if (enemy <= max)
                {
                    if (sectionOne1Enemy.Key.StartsWith("EnemyGhost"))
                    {
                        sectionOne1Enemy.Value.GetComponent<Ghost>().begin();
                    }
                    else if (sectionOne1Enemy.Key.StartsWith("EnemyKnife"))
                    {
                        sectionOne1Enemy.Value.GetComponent<Knife>().begin();
                    }
                }
            }
        }
        else if (index == 2)
        {
            foreach (KeyValuePair<string, GameObject> sectionOne2Enemy in sectionOne2Enemys)
            {
                int enemy = int.Parse(sectionOne2Enemy.Key.Split("_")[0].Split("|")[1]);
                if (enemy <= max)
                {
                    if (sectionOne2Enemy.Key.StartsWith("EnemyGhost"))
                    {
                        sectionOne2Enemy.Value.GetComponent<Ghost>().begin();
                    }
                    else if (sectionOne2Enemy.Key.StartsWith("EnemyKnife"))
                    {
                        sectionOne2Enemy.Value.GetComponent<Knife>().begin();
                    }
                }
            }
        }
    }

    //第一关的boss开始
    public void sectionOneBossBegin()
    {
        GameObject boss = GameObject.Find("EnemyKun");
        if (boss != null)
        {
            //boss.gameObject.GetComponent<Kun>().playAudio("Audios/Background/section1Boss");
            boss.gameObject.GetComponent<Kun>().begin();
        }
    }

    //第一关第index部分的敌人死了
    public void sectionEnemyDied(string gameObjectName)
    {
        int section = int.Parse(gameObjectName.Split('|')[0].Split("#")[1]);  // EnemyGhost#1|1_1
        int index = int.Parse(gameObjectName.Split('_')[1]);
        if (section == 1)
        {
            if (index == 1)
            {
                sectionOne1Enemys.Remove(gameObjectName);
            }
            if (index == 2)
            {
                sectionOne2Enemys.Remove(gameObjectName);
            }
        }
    }

    //第一关第index部分的敌人全部死了
    public bool isSectionOneEnemyAllDied(int index)
    {
        if (index == 1)
        {
            if (sectionOne1Enemys.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else if (index == 2)
        {
            if (sectionOne2Enemys.Count == 0)
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