using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 4f;
    public Animator animator;

    int hashSpeed;

    // Reference to player health to check guarding
    public PlayerHealth health;

    void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        hashSpeed = Animator.StringToHash("Speed");

        if (health == null)
            health = GetComponent<PlayerHealth>();
    }

    //Movimiento
    public void TickMovement()
    {
        // Stop movement if guarding or dead
        if ((health != null && health.isGuarding) || (health != null && health.IsDead))
        {
            animator.SetFloat(hashSpeed, 0f);
            return;
        }

        float input = Input.GetAxisRaw("Horizontal");

        Vector3 moveDir = transform.forward * input;
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        animator.SetFloat(hashSpeed, input);
    }
}