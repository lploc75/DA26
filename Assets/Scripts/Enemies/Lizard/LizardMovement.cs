using UnityEngine;

public class LizardMovement : MonoBehaviour
{
    public float moveSpeed = 2f;  // Tốc độ di chuyển
    public float changeDirectionTime = 2f;  // Thời gian thay đổi hướng di chuyển
    private Animator animator;
    private Rigidbody2D rb;

    private float timeToChangeDirection;
    private Vector2 moveDirection;

    private void Start()
    {
        animator = GetComponent<Animator>();  // Lấy Animator component
        rb = GetComponent<Rigidbody2D>();    // Lấy Rigidbody2D component
        rb.gravityScale = 0;  // Vô hiệu hóa gravity (trọng lực)
        timeToChangeDirection = changeDirectionTime;
        SetRandomDirection();  // Chọn hướng di chuyển ngẫu nhiên ban đầu
    }

    private void Update()
    {
        // Kiểm tra thời gian thay đổi hướng di chuyển
        timeToChangeDirection -= Time.deltaTime;
        if (timeToChangeDirection <= 0)
        {
            SetRandomDirection();  // Thay đổi hướng di chuyển
            timeToChangeDirection = changeDirectionTime;
        }

        // Di chuyển nhân vật theo hướng
        rb.linearVelocity = moveDirection * moveSpeed;  // Sử dụng velocity để di chuyển nhân vật

        // Cập nhật trạng thái chuyển động
        bool isMoving = moveDirection.magnitude > 0;
        animator.SetBool("isMoving", isMoving);  // Cập nhật trạng thái isMoving

        // Cập nhật hướng di chuyển cho Animator
        if (Mathf.Abs(moveDirection.x) > Mathf.Abs(moveDirection.y))
        {
            // Di chuyển ngang (trái/phải)
            if (moveDirection.x < 0)
            {
                animator.SetFloat("moveDirection", -1);  // Left
            }
            else if (moveDirection.x > 0)
            {
                animator.SetFloat("moveDirection", 1);  // Right
            }
        }
        else
        {
            // Di chuyển dọc (lên/xuống)
            if (moveDirection.y > 0)
            {
                animator.SetFloat("moveDirection", 0);  // Front
            }
            else if (moveDirection.y < 0)
            {
                animator.SetFloat("moveDirection", 2);  // Back
            }
        }

        // Debug các trạng thái và hướng di chuyển
        Debug.Log("Current move direction: " + moveDirection);
    }


    // Chọn hướng di chuyển ngẫu nhiên
    private void SetRandomDirection()
    {
        // Chọn ngẫu nhiên hướng di chuyển (trái, phải, lên, xuống)
        float randomDirection = Random.Range(0f, 1f);
        if (randomDirection < 0.25f)
        {
            moveDirection = Vector2.left;  // Di chuyển sang trái
        }
        else if (randomDirection < 0.5f)
        {
            moveDirection = Vector2.right;  // Di chuyển sang phải
        }
        else if (randomDirection < 0.75f)
        {
            moveDirection = Vector2.up;  // Di chuyển lên trên
        }
        else
        {
            moveDirection = Vector2.down;  // Di chuyển xuống dưới
        }

        // Debug hướng di chuyển
        Debug.Log("New move direction set: " + moveDirection);
    }
}
