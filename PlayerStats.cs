using NTC.Global.Cache;
using System.Collections;
using UniRx;
using UnityEngine;

public class PlayerStats : MonoCache
{
    public ReactiveProperty<float> health = new ReactiveProperty<float>(100);
    public ReactiveProperty<float> stamina = new ReactiveProperty<float>(100);

    [SerializeField]
    private float staminaDrain = 1f;

    [SerializeField]
    private float staminaRegen = 1f;

    [SerializeField]
    private float staminaDelay;

    private float durationDelay;

    public void ApplyDamage(float damage)
    {
        health.Value -= damage;
    }

    public void RegenHealth (float heal)
    {
        health.Value += heal;
    }

    public void StaminaChanger (bool isSprint)
    {
        if (isSprint)
        {
            stamina.Value -= Time.deltaTime * staminaDrain;
            durationDelay = -staminaDelay;
        }
        else if(stamina.Value < 100f)
        {
            durationDelay += Time.deltaTime;

            stamina.Value += Time.deltaTime * staminaRegen * Mathf.Clamp(durationDelay, 0, 1f);
            stamina.Value = Mathf.Clamp(stamina.Value, 0, 100f);
        } 

    }

    #region Context menu test
    [ContextMenu("Heal 10")]
    public void HealTestMenu ()
    {
        RegenHealth(10);
    }

    [ContextMenu("Damage 10")]
    public void DamageTestMenu ()
    {
        ApplyDamage(10);
    }

    #endregion
}
