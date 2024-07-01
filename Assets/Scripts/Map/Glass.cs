using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glass : MonoBehaviour
{
    private bool isBroken = false;

    public GameObject glassWall;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //private void OnCollisionEnter(Collision collision)
    //{
        
    //}

    //private void OnTriggerStay(Collider other)
    //{
    //    Debug.Log(other.gameObject.name + "Glass stay");
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (!isBroken)
        {
            string otherGameObjectName = other.gameObject.name;
            if (otherGameObjectName == "jumpAttack" || otherGameObjectName == "AttackKnife" || otherGameObjectName == "Attack")
            {
                isBroken = true;
                GetComponent<Animator>().enabled = true;

                AudioSource audioSource = GetComponent<AudioSource>();
                AudioClip clip = Resources.Load<AudioClip>("Audios/Tool/glassBreak");
                audioSource.clip = clip;
                audioSource.Play();
            }
        }
        //Debug.Log(other.gameObject.name);
    }

    //动画播放完毕后 1.禁用动画 2.禁用碰撞体 3.禁用空气墙
    public void animatorEndEvent()
    {
        GetComponent<Animator>().enabled = false;
        GetComponent<BoxCollider>().enabled = false;
        glassWall.SetActive(false);
    }

}
