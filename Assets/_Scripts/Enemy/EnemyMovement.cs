using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    public Animator animator;

    int hashSpeed;

    void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        hashSpeed = Animator.StringToHash("Speed");
    }

    public void Move(float direction)
    {
        if (direction == 0)
        {
            animator.SetFloat(hashSpeed, 0f);
            return;
        }

        Vector3 move = transform.forward * direction;
        transform.position += move * moveSpeed * Time.deltaTime;

        animator.SetFloat(hashSpeed, (direction));
    }

    public void Stop()
    {
        Move(0f);
    }
}