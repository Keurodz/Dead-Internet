using UnityEngine;
using UnityEngine.UI;

public class AlienAbilityUI : MonoBehaviour
{
    private Text ammoCountText;
    private Slider projectileCooldownSlider;

    public AlienAbilitySystem alienAbilitySystem;

    private void OnEnable()
    {
        alienAbilitySystem.OnAmmoCountChanged += UpdateAmmoCount;
        alienAbilitySystem.OnAbilityCooldownChanged += UpdateProjectileCooldown;
        if (alienAbilitySystem.alienAbilityAvailable)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        alienAbilitySystem.OnAmmoCountChanged -= UpdateAmmoCount;
        alienAbilitySystem.OnAbilityCooldownChanged -= UpdateProjectileCooldown;
    }

    private void Start() {
        projectileCooldownSlider = GetComponentInChildren<Slider>();
        ammoCountText = GetComponentInChildren<Text>();
    }

    public void UpdateAmmoCount(int ammoCount)
    {
        ammoCountText.text = "Ammo Count: " + ammoCount.ToString();
    }

    public void UpdateProjectileCooldown(float cooldownTimeLeft, float totalCooldownTime)
    {
        projectileCooldownSlider.value = cooldownTimeLeft / totalCooldownTime;
    }
}
