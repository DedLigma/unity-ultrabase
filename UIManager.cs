using NTC.Global.Cache;
using TMPro;
using UniRx;
using UniRx.Extensions;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoCache
{
    private CompositeDisposable disposable = new CompositeDisposable();

    public PlayerStats playerStats;

    public PlayerInteract playerInteract;

    public Image healthBar, staminaBar;

    public TextMeshProUGUI interactText;

    protected override void OnEnabled ()
    {
        playerStats.health.Subscribe(healthValue =>
        {
            healthBar.fillAmount = healthValue / 100;
        }).AddTo(disposable);

        playerStats.stamina.Subscribe(staminaValue =>
        {
            staminaBar.fillAmount = staminaValue / 100;
        }).AddTo(disposable);

        playerInteract.interactText.Subscribe(textValue =>
        {
            interactText.text = textValue;
        }).AddTo(disposable);
    }

    protected override void OnDisabled ()
    {
        disposable.Clear();
    }
}
