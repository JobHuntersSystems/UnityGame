using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Stats")]
    public int maxHP = 100;

    [Header("Flash")]
    public float flashDuration = 0.15f;

    public int CurrentHP { get; private set; }

    private SpriteRenderer sp;

    void Awake()
    {
        sp = GetComponent<SpriteRenderer>();
        CurrentHP = maxHP;
    }

    public void TakeDamage(int amount)
    {
        CurrentHP -= amount;
        CurrentHP = Mathf.Max(CurrentHP, 0);

        StopAllCoroutines();
        StartCoroutine(FlashRed());

        if (CurrentHP == 0)
            Die();
    }

    private IEnumerator FlashRed()
    {
        sp.color = Color.red;
        yield return new WaitForSeconds(flashDuration);
        sp.color = Color.white;
    }

    private void Die()
    {
        gameObject.SetActive(false);
    }
}
