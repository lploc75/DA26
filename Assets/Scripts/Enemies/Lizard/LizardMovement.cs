using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Animator anim;
    public float moveSpeed;
    public float attackRange = 2f; // Khoảng cách tấn công
    public Transform target; // Vị trí của người chơi
    public float attackSpeed = 0.5f; // Tốc độ tấn công (thời gian giữa các đòn tấn công)

    private Rigidbody2D rb;
    private Vector2 direction;
    private bool isAttacking;
    private bool isPlayerInRange = false; // Kiểm tra nếu người chơi trong phạm vi tấn công

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Kiểm tra xem có phải đang tấn công không, nếu không thì di chuyển tự động hoặc theo mục tiêu
        if (!isAttacking)
        {
            if (isPlayerInRange)
            {
                // Nếu người chơi trong phạm vi tấn công, tiến lại gần và tấn công
                MoveAndAttack();
            }
            else
            {
                // Di chuyển tự động khi không có đối tượng trong phạm vi
                MoveAutomatically();
            }
        }

        // Cập nhật hướng di chuyển sau mỗi thay đổi direction
        Animate();
    }

    private void FixedUpdate()
    {
        // Di chuyển nhân vật nếu không tấn công và không có đối tượng trong phạm vi tấn công
        if (!isAttacking && !isPlayerInRange)
        {
            rb.linearVelocity = direction * moveSpeed; // Di chuyển tự động khi không có đối tượng trong phạm vi
        }
    }

    // Di chuyển tự động đến vị trí mục tiêu (hoặc theo hướng bất kỳ)
    private void MoveAutomatically()
    {
        // Di chuyển tự động theo một hướng ngẫu nhiên (hoặc theo logic nào đó)
        direction = new Vector2(-1, 0); // Di chuyển sang phải, điều chỉnh nếu cần
        Debug.Log("MoveAutomatically: direction " + direction);
    }

    private void Animate()
    {
        // Cập nhật các giá trị X, Y trong Animator dựa trên hướng di chuyển
        anim.SetFloat("X", direction.x);  // Cập nhật giá trị X
        anim.SetFloat("Y", direction.y);  // Cập nhật giá trị Y
        anim.SetBool("IsMoving", direction.magnitude > 0);  // Kiểm tra có đang di chuyển không
        Debug.Log("X: " + direction.x + " Y: " + direction.y);
    }

    private void MoveAndAttack()
    {
        // Tính khoảng cách đến người chơi
        float distanceToPlayer = Vector2.Distance(transform.position, target.position);

        if (distanceToPlayer > attackRange)
        {
            // Nếu người chơi còn xa, tiến lại gần
            direction = (target.position - transform.position).normalized;
            rb.linearVelocity = direction * moveSpeed; // Di chuyển đến mục tiêu
            anim.SetBool("IsMoving", true);
        }
        else
        {
            // Nếu trong phạm vi tấn công, tấn công
            rb.linearVelocity = Vector2.zero; // Dừng di chuyển
            anim.SetBool("IsMoving", false);
            Attack();
        }
    }

    private void Attack()
    {
        if (!isAttacking)
        {
            Debug.Log("Attack started");
            isAttacking = true;
            anim.SetBool("IsAttack", true); // Kích hoạt animation tấn công
            StartCoroutine(WaitForAttackAnimation());
        }
    }

    private IEnumerator WaitForAttackAnimation()
    {
        // Đợi cho đến khi animation tấn công kết thúc, sử dụng attackSpeed để điều chỉnh tốc độ tấn công
        yield return new WaitForSeconds(attackSpeed); // Điều chỉnh thời gian tấn công (tốc độ tấn công)
        isAttacking = false; // Đặt lại trạng thái tấn công
        anim.SetBool("IsAttack", false);
        Debug.Log("Attack completed, ready to attack again.");
    }

    // Phát hiện khi người chơi vào khu vực tấn công
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Đảm bảo rằng chỉ có đối tượng người chơi mới được phát hiện
        {
            isPlayerInRange = true; // Người chơi đã vào trong phạm vi
        }
    }

    // Phát hiện khi người chơi rời khỏi khu vực tấn công
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false; // Người chơi đã ra khỏi phạm vi
        }
    }

    // Liên tục kiểm tra khi người chơi ở trong vùng phạm vi
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true; // Người chơi vẫn ở trong phạm vi
        }
    }
}
