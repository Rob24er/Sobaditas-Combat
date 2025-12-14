using UnityEngine;
using TMPro;

public class InGameTextVfx : MonoBehaviour
{
    [Header("Lifetime")]
    public float lifeTime = 1.2f;

    [Header("Movement")]
    public float floatSpeed = 1f;

    [Header("References")]
    public TextMeshPro text;

    void Awake()
    {
        if (text == null)
            text = GetComponentInChildren<TextMeshPro>();
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // simple upward float
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;
    }

   

    
    
}
