using UnityEngine;

public class CharacterControllerExample : MonoBehaviour
{
    public float moveSpeed = 3f; // 移动速度
    public float jumpForce = 5f; // 跳跃力量
    public bool Updown;

    private Rigidbody m_Rigidbody;
    private bool isGrounded;

    private void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // 获取输入
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // 计算移动方向
        Vector3 moveDirection = new Vector3(moveX, 0f, moveZ);
        moveDirection.Normalize();

        // 应用移动力量
        Vector3 movement = moveDirection * moveSpeed;
        movement.y = m_Rigidbody.velocity.y;
        m_Rigidbody.velocity = movement;
    }
}