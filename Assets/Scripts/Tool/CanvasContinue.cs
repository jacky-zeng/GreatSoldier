using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasContinue : MonoBehaviour
{
    // 定义一个静态的Continue实例
    public static CanvasContinue instance;

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
}
