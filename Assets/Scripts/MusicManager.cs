using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    // 定义一个静态的MusicManager实例
    public static MusicManager instance;
    // 拖入您的音乐源
    private AudioSource musicSource;
    void Awake()
    {
        // 如果实例不存在，则设置为当前实例
        if (instance == null)
        {
            instance = this;
            musicSource = GetComponent<AudioSource>();
            // 使对象不被销毁
            DontDestroyOnLoad(gameObject);
        }
        // 如果已经存在实例，则销毁当前重复的实例
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void playAudio(string fileName, bool isLoop = false)
    {
        AudioClip clip = Resources.Load<AudioClip>(fileName);

        musicSource.loop = isLoop;
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
}
