using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // 定义一个静态的GameManager实例
    public static GameManager instance;

    private string sectionName;

    private string sceneName; //场景名称
    public GameObject prefabGhost;
    public GameObject prefabKnife;
    public GameObject prefabKunBasketBall;
    public GameObject prefabPlayer;
    public GameObject prefabPlayerGirl;

    private GameObject gameObjectPlayer;

    private bool isOkSectionOne1 = false;
    private bool isOkSectionOne2 = false;
    private bool isOkSectionOne3 = false;

    private bool isOkSectionTwo1 = false;

    private Dictionary<string, GameObject> sectionOne1Enemys = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> sectionOne2Enemys = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> sectionOne3Enemys = new Dictionary<string, GameObject>();

    private Dictionary<string, GameObject> sectionTwo1Enemys = new Dictionary<string, GameObject>();

    //player相关信息
    private float playerPosZ;           //切换场景时，记录在上一个场景中的z值
    private float playerHitDamage = 0;  //切换场景时，记录在上一个场景中的hitDamage值
    private int pIndex;                 //选的哪个角色

    private bool isKunBasketBallOk = false;

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

        // 注册场景加载完成的事件
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // 注销事件，避免内存泄露
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 当场景加载完成时，这个函数会被调用
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded");
    }

    private void Update()
    {
        sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        switch (sceneName)
        {
            case "SceneSection1_1":
                if (!isOkSectionOne1)
                {
                    isOkSectionOne1 = true;
                    sectionOneLoad(1);
                    //Debug.Log("GameManager " + sceneName);
                }
                break;
            case "SceneSection1_2":
                if (!isOkSectionOne2)
                {
                    isOkSectionOne2 = true;
                    sectionOneLoad(2);
                    //Debug.Log("GameManager " + sceneName);
                }
                break;
            case "SceneSection1_3":
                if (!isOkSectionOne3)
                {
                    isOkSectionOne3 = true;
                    sectionOneLoad(3);
                    //Debug.Log("GameManager " + sceneName);
                }
                if (!isKunBasketBallOk && gameObjectPlayer.transform.position.x >= 78)
                {
                    isKunBasketBallOk = true;
                    float leftEdgeX = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane)).x;

                    sectionOneKunBasketBallBegin(leftEdgeX);
                }

                break;
            case "SceneSection2_1":
                if (!isOkSectionTwo1)
                {
                    isOkSectionTwo1 = true;
                    sectionTwoLoad(1);
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

    public void nextSection(string name)
    {
        if(gameObjectPlayer.GetComponent<Player>() != null)
        {
            gameObjectPlayer.GetComponent<Player>().setUnmatched(10, true);
        } else
        {
            gameObjectPlayer.GetComponent<PlayerGirl>().setUnmatched(10, true);
        }

        sectionName = name;
        Invoke("nextSectionInvoke", 5);
    }

    private void nextSectionInvoke()
    {
        //清空对象池
        ObjectPool.Instance.init();

        SceneManager.LoadScene(sectionName, LoadSceneMode.Single);
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

    public int getPIndex()
    {
        return pIndex;
    }

    public void setPIndex(int pIndexIn)
    {
        pIndex = pIndexIn;
    }

    //加载第一关第index部分的敌人
    private void sectionOneLoad(int index)
    {
        //角色加载
        initPlayer(new Vector3(-2.5f, 0.53f, -4.83f));
       
        int section = 1;
        //敌人gameobject命名 #关卡  ｜第几个敌人  _关卡中的场景index
        if (index == 1)
        {
            //Vector3(-2.5,3.29999995,-4.82999992)
            enemyAdd(section, index, "EnemyGhost#1|1", new Vector3(28.73f, 0.85f, 3), 1);
            enemyAdd(section, index, "EnemyGhost#1|2", new Vector3(23.9f, 0.85f, -1), 2);
            enemyAdd(section, index, "EnemyGhost#1|3", new Vector3(28.88f, 0.846f, -8.95f), 3);

            enemyAdd(section, index, "EnemyGhost#1|4", new Vector3(58.5f, 0.85f, -4.35f), 2);

            enemyAdd(section, index, "EnemyKnife#1|5", new Vector3(58.5f, 0.85f, -4.35f), 2);
            enemyAdd(section, index, "EnemyKnife#1|6", new Vector3(52.5f, 0.85f, -2.35f), 2);
        }
        else if (index == 2)
        {
            gameObjectPlayer.transform.localPosition = new Vector3(-2.5f, 3.3f, -4.83f);  //必须使用localPosition
            enemyAdd(section, index, "EnemyGhost#1|1", new Vector3(-1.7f, 0.846f, -10.52f), 3);
            enemyAdd(section, index, "EnemyKnife#1|2", new Vector3(8f, 0.846f, -8.52f), 3);
        } else
        {
            gameObjectPlayer.transform.localPosition = new Vector3(31, -15.08f, 2.79f);  //必须使用localPosition
        }
    }

    //加载第二关第index部分的敌人
    private void sectionTwoLoad(int index)
    {
        //角色加载
        initPlayer(new Vector3(-18.6f, 0.53f, -4.83f));

        int section = 2;
        //敌人gameobject命名 #关卡  ｜第几个敌人  _关卡中的场景index
        if (index == 1)
        {
            enemyAdd(section, index, "EnemyKnife#2|1", new Vector3(49.5f, 0.85f, -5f), 2);
            enemyAdd(section, index, "EnemyKnife#2|2", new Vector3(50.5f, 0.85f, -5f), 2);
            enemyAdd(section, index, "EnemyKnife#2|3", new Vector3(30.5f, 0.85f, -5f), 2);

            enemyAdd(section, index, "EnemyGhost#2|4", new Vector3(30.5f, 0.85f, -5f), 1);
        }
        else if (index == 2)
        {

        }
    }

    private void initPlayer(Vector3 pos)
    {
        gameObjectPlayer = null;
        int tempPIndex = getPIndex();
        if(tempPIndex == 1)
        {
            gameObjectPlayer = Instantiate(prefabPlayer);
            gameObjectPlayer.transform.localPosition = pos;  //必须使用localPosition
            gameObjectPlayer.name = "Player";
        } else if(tempPIndex == 3)
        {
            gameObjectPlayer = Instantiate(prefabPlayerGirl);
            gameObjectPlayer.transform.localPosition = pos;  //必须使用localPosition
            gameObjectPlayer.name = "Player";
        }
    }

    /// <summary>
    /// 自动加入敌人
    /// </summary>
    /// <param name="section">关卡</param>
    /// <param name="index">关卡中的第几部分</param>
    /// <param name="name">物体名称</param>
    /// <param name="pos">加载位置</param>
    /// <param name="animatorType">默认播放哪个初始动画</param>
    private void enemyAdd(int section, int index, string name, Vector3 pos, int animatorType = 0)
    {
        GameObject gameObjectInit = null;
        if (section == 1)
        {
            if (name.StartsWith("EnemyGhost"))
            {
                gameObjectInit = Instantiate(prefabGhost);
                gameObjectInit.transform.localPosition = pos;  //必须使用localPosition
                gameObjectInit.GetComponent<Ghost>().changeAnimatorStatus(animatorType);
            }
            else if (name.StartsWith("EnemyKnife"))
            {
                gameObjectInit = Instantiate(prefabKnife);
                gameObjectInit.transform.localPosition = pos;  //必须使用localPosition
            }
            else if (name.StartsWith("EnemyKunBasketBall"))
            {
                gameObjectInit = Instantiate(prefabKunBasketBall);
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
        else if (section == 2)
        {
            if (name.StartsWith("EnemyGhost"))
            {
                gameObjectInit = Instantiate(prefabGhost);
                gameObjectInit.transform.localPosition = pos;  //必须使用localPosition
                gameObjectInit.GetComponent<Ghost>().changeAnimatorStatus(animatorType);
            }
            else if (name.StartsWith("EnemyKnife"))
            {
                gameObjectInit = Instantiate(prefabKnife);
                gameObjectInit.transform.localPosition = pos;  //必须使用localPosition
            }

            string gameObjectName = name + "_" + index.ToString();
            gameObjectInit.gameObject.name = gameObjectName;
            if (index == 1)
            {
                sectionTwo1Enemys.Add(gameObjectName, gameObjectInit);
            }
            else if (index == 2)
            {

            }
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
                    else if (sectionOne1Enemy.Key.StartsWith("EnemyKunBasketBall"))
                    {
                        sectionOne1Enemy.Value.GetComponent<KunBasketBall>().begin();
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
                    else if (sectionOne2Enemy.Key.StartsWith("EnemyKunBasketBall"))
                    {
                        sectionOne2Enemy.Value.GetComponent<KunBasketBall>().begin();
                    }
                }
            }
        }
    }

    //Player.cs调用该方法（player走到一定位置，触发敌人开始）
    public void sectionTwoEnemyBegin(int index, int max)
    {
        if (index == 1)
        {
            foreach (KeyValuePair<string, GameObject> sectionTwo1Enemy in sectionTwo1Enemys)
            {
                int enemy = int.Parse(sectionTwo1Enemy.Key.Split("_")[0].Split("|")[1]);
                if (enemy <= max)
                {
                    if (sectionTwo1Enemy.Key.StartsWith("EnemyGhost"))
                    {
                        sectionTwo1Enemy.Value.GetComponent<Ghost>().begin();
                    }
                    else if (sectionTwo1Enemy.Key.StartsWith("EnemyKnife"))
                    {
                        sectionTwo1Enemy.Value.GetComponent<Knife>().begin();
                    }
                    else if (sectionTwo1Enemy.Key.StartsWith("EnemyKunBasketBall"))
                    {
                        sectionTwo1Enemy.Value.GetComponent<KunBasketBall>().begin();
                    }
                    
                }
            }
        }
        else if (index == 2)
        {
           
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

    //第一关添加扔篮球的
    public void sectionOneKunBasketBallBegin(float x)
    {
        enemyAdd(1, 3, "EnemyKunBasketBall#1|1", new Vector3(x + 5, -13.26f, 0.27f));
        enemyAdd(1, 3, "EnemyKunBasketBall#1|2", new Vector3(x + 5, -13.26f, 1.2f));

        sectionOneEnemyBegin(1, 3);
    }

    //第section关第index部分的敌人死了
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
            else if (index == 2)
            {
                sectionOne2Enemys.Remove(gameObjectName);
            }
        }
        else if (section == 2)
        {
            if (index == 1)
            {
                sectionTwo1Enemys.Remove(gameObjectName);
            }
            else if (index == 2)
            {

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

    //第二关第index部分的敌人全部死了
    public bool isSectionTwoEnemyAllDied(int index)
    {
        if (index == 1)
        {
            if (sectionTwo1Enemys.Count == 0)
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
            
        }
        return false;
    }
}