using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Go : MonoBehaviour
{
    private bool isOn = false;

    private void Awake()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }

    public bool getIsOn()
    {
        return isOn;
    }

    public void begin()
    {
        isOn = true;
        Invoke("go", 3);
    }

    public void go()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<AudioSource>().Play();

        Invoke("goStop", 3);
    }

    public void goStop()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<AudioSource>().Stop();

        Invoke("go", 3);
    }
}
