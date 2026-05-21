using System.Collections;
using UnityEngine;

public class BuildingAccidentHandler : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;

    [Header("Fire")]
    [SerializeField] private bool canBurn = true;
    [SerializeField] private GameObject fireVisual;

    public bool IsOnFire { get; private set; }

    private Coroutine fireRoutine;

    private void Awake()
    {
        currentHealth = maxHealth;

        if (fireVisual != null)
            fireVisual.SetActive(false);
    }

    public void TryIgnite(float chance, int damagePerTick, float tickInterval)
    {
        if (!canBurn || IsOnFire)
            return;

        if (Random.value > chance)
            return;

        StartFire(damagePerTick, tickInterval);
    }

    public void StartFire(int damagePerTick, float tickInterval)
    {
        if (IsOnFire)
            return;

        IsOnFire = true;

        if (fireVisual != null)
            fireVisual.SetActive(true);

        fireRoutine = StartCoroutine(FireDamageRoutine(damagePerTick, tickInterval));
    }

    private IEnumerator FireDamageRoutine(int damagePerTick, float tickInterval)
    {
        while (IsOnFire)
        {
            TakeDamage(damagePerTick);
            yield return new WaitForSeconds(tickInterval);
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
            DestroyBuilding();
    }

    private void DestroyBuilding()
    {
        IsOnFire = false;

        if (fireRoutine != null)
            StopCoroutine(fireRoutine);

        if (fireVisual != null)
            fireVisual.SetActive(false);

        Destroy(gameObject);
    }
}