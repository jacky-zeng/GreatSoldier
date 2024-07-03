using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerKnife : MonoBehaviour
{
    #region 碰撞

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("PlayerKnife OnCollisionEnter "  + collision.gameObject.name);
    }

    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("PlayerKnife OnCollisionStay "  + collision.gameObject.name);
    }

    //trigger碰撞
    private void OnTriggerEnter(Collider collision)
    {
        OnTriggerEnterOrStay(collision, "Enter");
    }

    private void OnTriggerStay(Collider collision)
    {
        OnTriggerEnterOrStay(collision, "Stay");
    }

    private void OnTriggerEnterOrStay(Collider collision, string type)
    {
        Debug.Log("PlayerKnife OnTriggerEnterOrStay" + type + collision.gameObject.name);
    }

    #endregion
}