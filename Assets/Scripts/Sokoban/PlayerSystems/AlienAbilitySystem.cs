using System;
using UnityEngine;
using System.Collections;

// This script is responsible for handling the alien's ability.
// It should be attached to the player object to enable the alien ability.
public class AlienAbilitySystem : MonoBehaviour
{
    // determines if the alien ability is available
    public bool alienAbilityAvailable = true;

    // the maximum amount of ammo for the alien ability
    public int alienAbilityMaxAmmo = 3;

    // the projectile prefab to shoot
    [SerializeField]
    private GameObject projectilePrefab;

    // the cooldown time for the alien ability
    [SerializeField]
    private float alienAbilityTotalCooldown = 5f;

    // the current amount of ammo for the alien ability
    private int alienAbilityCurrentAmmo;

    // the cooldown timer for the alien ability
    private float alienAbilityCooldownTimer;

    // the fire point to shoot the projectile from
    private Transform firePoint;

    // action listeners for chnages to update the UI
    public event Action<int> OnAmmoCountChanged;
    public event Action<float, float> OnAbilityCooldownChanged;
    public event Action<bool> OnAbilityAvailabilityChanged;
    public event Action<int> OnAlienAbilityMaxAmmoCountChanged;

    // sets the ammo count and invokes the action listener
    public void SetAmmoCount(int count)
    {
        alienAbilityCurrentAmmo = count;
        OnAmmoCountChanged?.Invoke(alienAbilityCurrentAmmo);
    }

    // sets the ability cooldown and invokes the action listener

    public void SetAbilityCooldown(float time)
    {
        alienAbilityCooldownTimer = time;
        OnAbilityCooldownChanged?.Invoke(alienAbilityCooldownTimer, alienAbilityTotalCooldown);
    }

    // sets the ability availability and invokes the action listener
    public void SetAbilityAvailability(bool available)
    {
        alienAbilityAvailable = available;
        OnAbilityAvailabilityChanged?.Invoke(alienAbilityAvailable);
    }

    // sets the max ammo and invokes the action listener
    public void SetAlienAbilityMaxAmmo(int maxAmmo)
    {
        alienAbilityMaxAmmo = maxAmmo;
        OnAlienAbilityMaxAmmoCountChanged?.Invoke(alienAbilityMaxAmmo);
    }

    void Start()
    {
        firePoint = transform.Find("FirePoint");
        ReloadPlayerAbility();
    }

    // Update is called once per frame
    void Update()
    {
        HandleAlienAbilityCooldown();
        HandleProjectileAbility();
    }

    // resets the player's ammo count and cooldown timer
    public void ReloadPlayerAbility() {
        SetAmmoCount(alienAbilityMaxAmmo);
        SetAbilityCooldown(alienAbilityTotalCooldown);
    }

    // updates the alien ability cooldown timer
    private void HandleAlienAbilityCooldown()
    {
        SetAbilityCooldown(alienAbilityCooldownTimer + Time.deltaTime);
    }

    // handles the alien ability if it is available and the player presses the 'E' key
    private void HandleProjectileAbility() {
        if (alienAbilityAvailable) {
            if (Input.GetKeyDown(KeyCode.E)) {
                ShootProjectile();
            }
        }
    }

    // is the projectile able to be shot?
    private bool CanShootProjectile() {
        return projectilePrefab != null &&
            firePoint != null &&
            alienAbilityCurrentAmmo > 0 &&
            alienAbilityCooldownTimer > alienAbilityTotalCooldown;
    }

    // shoots the alien laser projectile
    private void ShootProjectile() {
        if (CanShootProjectile())
        {
            Vector3 targetDirection = DetermineShotDirection();

            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            StartCoroutine(MoveProjectile(projectile, targetDirection));
            SetAbilityCooldown(0f);
            SetAmmoCount(this.alienAbilityCurrentAmmo - 1);
        }
    }

    // checks if there is a closer ground or floating block to aim at
    // and returns the direction to shoot towards
    private Vector3 DetermineShotDirection()
    {
        float maxDistance = 100f;
        RaycastHit groundHit, floatingHit;
        Vector3 groundFirePoint = firePoint.position;
        Vector3 floatingFirePoint = firePoint.position + Vector3.up * 3f;
        bool foundGround = Physics.Raycast(groundFirePoint, firePoint.forward, out groundHit, maxDistance);
        bool foundFloating = Physics.Raycast(floatingFirePoint, firePoint.forward, out floatingHit, maxDistance);

        Vector3 targetDirection = firePoint.forward;
 
        if (!foundGround && !foundFloating)
        {
            return targetDirection;
        }
        if (foundGround && !foundFloating)
        {
            targetDirection = (groundHit.point - groundFirePoint).normalized;
        }
        if (!foundGround && foundFloating)
        {
            targetDirection = (floatingHit.point - groundFirePoint).normalized; 
        }

        if (foundGround && foundFloating)
        {
            Vector3 targetPoint = Vector3.Distance(groundHit.point, groundFirePoint) < Vector3.Distance(floatingHit.point, floatingFirePoint) ? groundHit.point : floatingHit.point;
            targetDirection = (targetPoint - firePoint.position).normalized; 
        }

        return targetDirection;
    }

    // coroutine to move the projectile and handle collision with game object
    private IEnumerator MoveProjectile(GameObject projectile, Vector3 direction)
    {
        float speed = 5f;
        float lifetime = 3f;
        float elapsedTime = 0f;

        while (elapsedTime < lifetime)
        {
            Vector3 moveStep = direction * speed * Time.deltaTime;
            
            if (Physics.Raycast(projectile.transform.position, direction, out RaycastHit hit, moveStep.magnitude))
            {
                if (hit.collider.CompareTag("Sokoban"))
                {
                    var gridBlock = hit.collider.GetComponent<ISokobanInteractable>();

                    if (gridBlock != null)
                    {
                        gridBlock.TryFloat();
                    }
                }
                Destroy(projectile); 
                yield break;
            }
            projectile.transform.position += moveStep;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Destroy(projectile); 
    }
}
