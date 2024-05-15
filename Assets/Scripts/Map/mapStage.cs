using UnityEngine;

public class mapStage : MonoBehaviour
{
    public GameObject playerHeath;
    private bool isBegin = false;
    #region 帧
    // Update is called once per frame
    void Update()
    {
        if (transform.position.x <= -49)
        {
            playerHeath.SetActive(true);
            Destroy(gameObject);
        }
        else if (isBegin)
        {
            Vector3 targetPosition = new Vector3(-50, transform.position.y, transform.position.z);

            //// 计算移动的方向和距离
            Vector3 directionToTarget = (targetPosition - transform.position).normalized;
            Vector3 moveDirection = directionToTarget * 15 * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveDirection.magnitude);
        }

    }

    public void begin()
    {
        isBegin = true;
    }
    #endregion

}