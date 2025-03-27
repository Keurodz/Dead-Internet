using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AlienAbilityUI : MonoBehaviour
{
    // private Text ammoCountText;
    private Slider projectileCooldownSlider;
    // the alien ability system to track bullet count and cooldown
    public AlienAbilitySystem alienAbilitySystem;

    // the bullet sprite to render
    [SerializeField] private Sprite bulletSprite; 
    // the spacing between bullets
    [SerializeField] private float bulletSpacing = 5f; 
    // the size of the bullet image (should be square)
    [SerializeField] private float imageSize = 64f; 
    // the parent transform for the bullet icons
    [SerializeField] private Transform ammoDisplayParent;
    // the opacity of the bullet image when used
    [SerializeField] private float usedBulletOpacity = 0.3f;

    // references to the bullet images to modify opacity
    private List<Image> bulletImages = new List<Image>();
    // the maximum number of bullets
    private int maxAmmo = 3;


    private void OnEnable()
    {
        alienAbilitySystem.OnAmmoCountChanged += UpdateAmmoCount;
        alienAbilitySystem.OnAbilityCooldownChanged += UpdateProjectileCooldown;
        alienAbilitySystem.OnAbilityAvailabilityChanged += UpdateAbilityAvailability;
        alienAbilitySystem.OnAlienAbilityMaxAmmoCountChanged += UpdateAlienAbilityMaxAmmoCount;
    }

    private void OnDisable()
    {
        alienAbilitySystem.OnAmmoCountChanged -= UpdateAmmoCount;
        alienAbilitySystem.OnAbilityCooldownChanged -= UpdateProjectileCooldown;
        alienAbilitySystem.OnAbilityAvailabilityChanged -= UpdateAbilityAvailability;
        alienAbilitySystem.OnAlienAbilityMaxAmmoCountChanged -= UpdateAlienAbilityMaxAmmoCount;
    }

    private void Awake() {
        projectileCooldownSlider = GetComponentInChildren<Slider>();

        if (ammoDisplayParent == null) {
            ammoDisplayParent = transform;
        }
    }

    private void Start() {
        this.maxAmmo = alienAbilitySystem.alienAbilityMaxAmmo;
        CreateBulletIcons();
    }

    // creates the bullet icons for ammo display
    private void CreateBulletIcons()
    {
        ClearBullets();

        for (int i = 0; i < this.maxAmmo; i++)
        {
            GameObject bulletObj = new GameObject("Bullet_" + i);
            bulletObj.transform.SetParent(ammoDisplayParent, false);
            
            Image bulletImage = bulletObj.AddComponent<Image>();
            bulletImage.sprite = bulletSprite;
            
            RectTransform bulletRect = bulletObj.GetComponent<RectTransform>();
            
            bulletRect.sizeDelta = new Vector2(imageSize, imageSize);
            
            float xPos = i * (imageSize + bulletSpacing);
            bulletRect.anchoredPosition = new Vector2(xPos, 0);
            
            bulletImages.Add(bulletImage);
        }
    }

    // clears the bullet icons
    private void ClearBullets() {
        foreach (Image bullet in bulletImages)
        {
            Destroy(bullet.gameObject);
        }
        bulletImages.Clear();
    }

    // when the ammo count is updated, update the bullet icons
    public void UpdateAmmoCount(int ammoCount)
    {
        int currentAmmo = Mathf.Clamp(ammoCount, 0, maxAmmo);
        
        for (int i = 0; i < bulletImages.Count; i++)
        {
            Color bulletColor = bulletImages[i].color;
            bulletColor.a = (i < currentAmmo) ? 1.0f : usedBulletOpacity;
            bulletImages[i].color = bulletColor;
        }

        if (alienAbilitySystem.alienAbilityAvailable)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    // when the projectile cooldown is updated, update the slider
    public void UpdateProjectileCooldown(float cooldownTimeLeft, float totalCooldownTime)
    {
        projectileCooldownSlider.value = cooldownTimeLeft / totalCooldownTime;
    }

    // when the ability availability is updated, update the UI
    public void UpdateAbilityAvailability(bool available)
    {
        if (available)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    // when the max ammo is updated, update the bullet icons
    public void UpdateAlienAbilityMaxAmmoCount(int maxAmmo)
    {
        this.maxAmmo = maxAmmo;
        CreateBulletIcons();
    }
}
