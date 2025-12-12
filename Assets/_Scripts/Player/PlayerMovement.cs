using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 4f;
    public Animator animator;

    int hashSpeed;

    void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        hashSpeed = Animator.StringToHash("Speed");
    }

    public void TickMovement()
    {
        float input = Input.GetAxisRaw("Horizontal"); 

        Vector3 moveDir = transform.forward * input;
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        animator.SetFloat(hashSpeed, input);
    }
}