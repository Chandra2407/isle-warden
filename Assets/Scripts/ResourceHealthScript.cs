using UnityEngine;

public class ResourceHealthScript : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
        {
            DestroyResource();
        }
    }

    private void DestroyResource()
    {
        // Handle resource death logic here
        Destroy(gameObject);
    }
}
