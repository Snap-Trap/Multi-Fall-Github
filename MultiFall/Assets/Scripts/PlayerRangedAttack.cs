using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerRangedAttack : NetworkBehaviour
{
    public InputAction rangedAttackAction;

    public float bulletSpeed;

    public float rangedCooldown = 1f;
    private bool canAttack = true;

    public GameObject bulletPrefab;
    private Transform firePoint;

    public void Awake()
    {
        firePoint = transform.Find("FirePoint");
    }

    public void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (rangedAttackAction.triggered)
        {
            Debug.Log("Ranged attack triggered");
            RangedAttackRpc();
        }
    }

    [Rpc(SendTo.Server)]
    public void RangedAttackRpc()
    {
        if (canAttack)
        {
            GameObject TempBullet = Instantiate(bulletPrefab, firePoint.position, transform.rotation);

            Rigidbody rigidBullet = TempBullet.GetComponent<Rigidbody>();

            TempBullet.GetComponent<Rigidbody>().AddForce(transform.forward * bulletSpeed);

            StartCoroutine(AttackCooldown());
        }
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(rangedCooldown);
        Debug.Log(gameObject.name + " RANGED attack cooldown has ended");
        canAttack = true;
    }

    public void OnEnable()
    {
        rangedAttackAction.Enable();
    }

    public void OnDisable()
    {
        rangedAttackAction.Disable();
    }
}
