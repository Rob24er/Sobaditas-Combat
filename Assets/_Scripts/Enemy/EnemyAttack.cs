using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{
    public Animator animator;
    public PlayerHealth health;

    [Header("Bloqueo automatico")]
    public bool enableAutoBlock = true;
    public float minIdleTime = 1.5f; 
    public float maxIdleTime = 3.5f;   
    public float blockDuration = 2f;   

    bool isGuarding;
    Coroutine blockRoutine;

    int hashIsGuarding;

    void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (health == null) health = GetComponent<PlayerHealth>();

        hashIsGuarding = Animator.StringToHash("IsGuarding");
    }

    void OnEnable()
    {
        if (enableAutoBlock && blockRoutine == null)
        {
            blockRoutine = StartCoroutine(BlockLoop());
        }
    }

    void OnDisable()
    {
        if (blockRoutine != null)
        {
            StopCoroutine(blockRoutine);
            blockRoutine = null;
        }
        SetGuard(false);
    }

    public void TickAI()
    {
    }

    IEnumerator BlockLoop()
    {
        while (true)
        {
            if (health != null && health.IsDead)
            {
                SetGuard(false);
                yield break;
            }

            float waitTime = Random.Range(minIdleTime, maxIdleTime);
            yield return new WaitForSeconds(waitTime);

            SetGuard(true);
            yield return new WaitForSeconds(blockDuration);
            SetGuard(false);
        }
    }

    void SetGuard(bool value)
    {
        if (isGuarding == value) return;

        isGuarding = value;

        if (health != null)
        {
            health.isGuarding = value;
            health.canBlockHighAndMid = true;
        }

        if (animator != null)
        {
            animator.SetBool(hashIsGuarding, value);
        }
    }
}
