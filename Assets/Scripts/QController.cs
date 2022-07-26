using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class QController : NetworkBehaviour
{
    [Networked]
    private TickTimer life { get; set; }

    [SerializeField]
    private float bulletSpeed = 20f;

    [SerializeField]
    public Vector3 direction;

    public override void Spawned()
    {
        bulletSpeed = 10f;
        FindObjectOfType<AudioManager>().Play("HeroBullet");
        transform.rotation = Quaternion.LookRotation(new Vector3(0, 0, 0));
        life = TickTimer.CreateFromSeconds(Runner, 1.3f);
    }

    public override void FixedUpdateNetwork()
    {
        if (life.Expired(Runner))
        {
            Runner.Despawn(Object);
        }
        else
        {
            direction = Vector3.Normalize(new Vector3(direction.x, direction.y, 0));
            transform.position += bulletSpeed * direction * Runner.DeltaTime;
            transform.Rotate(new Vector3(0,0,450) * Runner.DeltaTime);
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.tag);
        if (other.CompareTag("BulletWall"))
            Runner.Despawn(Object);
        if ((other.CompareTag("PlayerRed") && gameObject.CompareTag("BulletBlue")) || (other.CompareTag("PlayerBlue") && gameObject.CompareTag("BulletRed")))
        {
            var player = other.GetComponent<PlayerController>();
            player.TakeDamage(10);
            Runner.Despawn(Object);
        }
        if ((other.CompareTag("TurretRed") && gameObject.CompareTag("BulletBlue")) || (other.CompareTag("TurretBlue") && gameObject.CompareTag("BulletRed")))
        {
            TurretController turret = other.GetComponent<TurretController>();
            CrystalController crystal = other.GetComponent<CrystalController>();
            PurchasedTurretController purchaseturret = other.GetComponent<PurchasedTurretController>();
            if (turret != null)
            {
                turret.TakeDamage(50);
            }
            else if (crystal != null)
            {
                crystal.TakeDamage(50);
            }
            else
            {
                purchaseturret.TakeDamage(50);
            }
            Runner.Despawn(Object);
        }
    }
}
