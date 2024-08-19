using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//美露昕的水子弹
public class WaterBall : MonoBehaviour
{
    Rigidbody rigi;
    float speed = 10;

    public GameObject hit;

    private void Awake()
    {
        rigi = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        transform.Rotate(new Vector3(0, 0, 2));
    }

    //会调用此方法，子弹角度
    public void setRotation(float angle)
    {
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        // 将角度转换为弧度
        float radianAngle = angle * Mathf.Deg2Rad;

        // 计算力的矢量
        Vector2 forceVector = new Vector2(Mathf.Cos(radianAngle), Mathf.Sin(radianAngle)) * 1;

        // 应用速度到子弹
        rigi.velocity = forceVector * speed;
        StartCoroutine(DestroyObject(8f));
    }

    IEnumerator DestroyObject(float time)
    {
        yield return new WaitForSeconds(time);
        ObjectPool.Instance.Push(gameObject); //使用对象池，将子弹(敌人的激光射线子弹)回收
    }

    public void setDie()
    {
        ObjectPool.Instance.Push(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 计算力的矢量
        Vector2 forceVector = new Vector2(-1, 0.3f);
        // 应用速度到子弹
        rigi.velocity = forceVector * speed;

        if(other.gameObject.tag != "Player")
        {
            //攻击效果
            ObjectPool.Instance.Get(hit, transform.position, Vector3.zero, true).GetComponent<Hit2>()
             .setDie();

            setDie();
        }
    }

}
