using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerHealth : MonoBehaviour
{
    [Header("Stats")]
    public int maxHP = 100;

    [Header("Flash")]
    public float flashDuration = 0.15f;

    [Header("UI")]
    public Slider healthBarSlider;

    public static event Action OnDeath;

    public int CurrentHP { get; private set; }

    private SpriteRenderer sp;
    private float regenPerSecond = 0f;
    private bool isDead = false;

    void Awake()
    {
        sp = GetComponent<SpriteRenderer>();
        CurrentHP = maxHP;
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;
        CurrentHP = Mathf.Max(CurrentHP - amount, 0);

        StopCoroutine("FlashRed");
        StartCoroutine(FlashRed());

        UpdateHealthBar();

        if (CurrentHP == 0) Die();
    }

    public void IncreaseMaxHP(int amount)
    {
        maxHP += amount;
        CurrentHP = Mathf.Min(CurrentHP + amount, maxHP);
        UpdateHealthBar();
    }

    public void AddRegen(float hpPerSecond)
    {
        bool wasZero = regenPerSecond == 0f;
        regenPerSecond += hpPerSecond;
        if (wasZero) StartCoroutine(RegenLoop());
    }

    private IEnumerator RegenLoop()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(1f);
            if (isDead) yield break;
            CurrentHP = Mathf.Min(CurrentHP + Mathf.RoundToInt(regenPerSecond), maxHP);
            UpdateHealthBar();
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBarSlider != null)
            healthBarSlider.value = (float)CurrentHP / maxHP;
    }

    private IEnumerator FlashRed()
    {
        sp.color = Color.red;
        yield return new WaitForSeconds(flashDuration);
        sp.color = Color.white;
    }

    private void Die()
    {
        isDead = true;
        OnDeath?.Invoke();
        PlayerMovement pm = GetComponent<PlayerMovement>();
        pm.enabled = false;
        pm.TriggerDeath();
    }
}
