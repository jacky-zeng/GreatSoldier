using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hit : MonoBehaviour
{
    //private bool isPlayAudio = false;
    //private bool isDie = false;
    private AudioSource audioSource;

    private Transform posFollow = null;

    private void Awake()
    {
        //GetComponent<Animator>().enabled = false;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {

        if (posFollow != null)
        {
            transform.position = posFollow.position;
        }
    }

    public Hit setFollowTransform(Transform obj)
    {
        posFollow = obj;
        return this;
    }

    //直接改为主动触发，而不是写在动画事件中
    public void setDie()
    {
        playAudio("hitBlood");
        Invoke("die", 0.2f);
    }

    public void animatorHitAudioEvent()
    {
        //if(!isPlayAudio)
        //{
        //    isPlayAudio = true;
        //    Debug.Log("animatorHitAudioEvent");
        //    playAudio("hitBlood");
        //}
        
    }

    public void animatorHitDieEvent()
    {
        //if(!isDie)
        //{
        //    isDie = true;
        //    Debug.Log("animatorHitDieEvent");
        //    //GetComponent<Animator>().enabled = false;
        //    Invoke("die", 0.6f);
        //}
       
    }

    private void die()
    {
        ObjectPool.Instance.Push(gameObject);
        //GetComponent<Animator>().enabled = false;
        //gameObject.SetActive(false);
        //GetComponent<Animator>().enabled = true;
    }

    public void playAudio(string fileName)
    {
        string namePath = "Audios/Tool/" + fileName;
        AudioClip clip = Resources.Load<AudioClip>(namePath);

        audioSource.clip = clip;
        audioSource.Play();
    }
}
