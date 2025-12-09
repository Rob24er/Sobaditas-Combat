using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Animator animator;

    int hashSpeed;

    void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        hashSpeed = Animator.StringToHash("Speed");
    }

    public void TickMovement()
    {
        if (animator != null)
        {
            animator.SetFloat(hashSpeed, 0f);
        }
    }
}