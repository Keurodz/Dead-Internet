using UnityEngine;
using System.Collections;

// This script is responsible for handling the alien's ability.
// It should be attached to the player object to enable the alien ability.
public class AlienAbilitySystem : MonoBehaviour
{
    [SerializeField]
    private bool alienAbilityAvailable = true;

    public GameObject projectilePrefab;
    private Transform firePoint;

    void Start()
    {
        firePoint = transform.Find("FirePoint");
    }

    // Update is called once per frame
    void Update()
    {
        HandleProjectileAbility();
    }

    // handles the alien ability if it is available and the player presses the 'E' key
    private void HandleProjectileAbility() {
        if (alienAbilityAvailable) {
            if (Input.GetKeyDown(KeyCode.E)) {
                ShootProjectile();
            }
        }
    }

    // shoots the alien laser projectile
    private void ShootProjectile() {
        if (projectilePrefab != null && firePoint != null)
        {
            Vector3 targetDirection = DetermineShotDirection();

            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            StartCoroutine(MoveProjectile(projectile, targetDirection));
        }
    }

    // checks if there is a closer ground or floating block to aim at
    // and returns the direction to shoot towards
    private Vector3 DetermineShotDirection()
    {
        float maxDistance = 100f;
        RaycastHit groundHit, floatingHit;
        bool foundGround = Physics.Raycast(firePoint.position, firePoint.forward, out groundHit, maxDistance);
        bool foundFloating = Physics.Raycast(firePoint.position + Vector3.up * 3f, firePoint.forward, out floatingHit, maxDistance);

        Vector3 targetDirection = firePoint.forward;
 
        if (!foundGround && !foundFloating)
        {
            return targetDirection;
        }
        if (foundGround && !foundFloating)
        {
            targetDirection = (groundHit.point - firePoint.position).normalized;
        }
        if (!foundGround && foundFloating)
        {
            targetDirection = (floatingHit.point - firePoint.position).normalized; 
        }

        if (foundGround && foundFloating)
        {
            Vector3 targetPoint = Vector3.Distance(groundHit.point, firePoint.position) < Vector3.Distance(floatingHit.point, firePoint.position) ? groundHit.point : floatingHit.point;
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
                    Destroy(projectile); 
                    yield break;
                }
            }
            projectile.transform.position += moveStep;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Destroy(projectile); 
    }
}
