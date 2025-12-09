using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    [Header("跟随设置")]
    public Transform target;
    public Vector3 offset = new Vector3(0, 5, -10);
    public float smoothSpeed = 0.125f;
    
    [Header("旋转跟随设置")]
    public bool followRotation = true;
    public float rotationSmoothSpeed = 0.1f;
    
    void LateUpdate()
    {
        if (target == null) return;
        
        // 计算相对于角色旋转的偏移位置
        Vector3 rotatedOffset = followRotation 
            ? target.rotation * offset 
            : offset;
        
        Vector3 desiredPosition = target.position + rotatedOffset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
        
        // 相机始终看向玩家
        if (followRotation)
        {
            // 平滑旋转到看向目标的方向
            Quaternion desiredRotation = Quaternion.LookRotation(target.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSmoothSpeed);
        }
        else
        {
            transform.LookAt(target);
        }
    }
}
