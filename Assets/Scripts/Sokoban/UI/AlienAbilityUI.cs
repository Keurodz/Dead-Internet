using UnityEngine;
using UnityEngine.UI;

public class AlienAbilityUI : MonoBehaviour
{
    public Text ammoCountText;
    public Slider projectileCooldownSlider;

    public AlienAbilitySystem alienAbilitySystem;

    public void UpdateAmmoCount(int ammoCount)
    {
        ammoCountText.text = "Ammo Count: " + ammoCount.ToString();
    }

    public void UpdateProjectileCooldown(float cooldownTime, float cooldownTimer)
    {
        projectileCooldownSlider.value = cooldownTimer / cooldownTime;
    }

    public void UpdateUI(int ammoCount, float cooldownTime, float cooldownTimer)
    {
        UpdateAmmoCount(ammoCount);
        UpdateProjectileCooldown(cooldownTime, cooldownTimer);
    }

    public void Update() {

    }


}
