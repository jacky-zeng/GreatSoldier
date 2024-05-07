using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hit2 : MonoBehaviour
{
    private AudioSource audioSource;

    private Transform posFollow = null;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if(posFollow != null)
        {
            transform.position = posFollow.position;
        }
    }

    public Hit2 setFollowTransform(Transform obj)
    {
        posFollow = obj;
        return this;
    }

    //直接改为主动触发，而不是写在动画事件中
    public void setDie()
    {
        playAudio("hit");
        Invoke("die", 0.5f);
    }


    private void die()
    {
        ObjectPool.Instance.Push(gameObject);
    }

    public void playAudio(string fileName)
    {
        string namePath = "Audios/Tool/" + fileName;
        AudioClip clip = Resources.Load<AudioClip>(namePath);

        audioSource.clip = clip;
        audioSource.Play();
    }
}
