using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class TurretBullet : NetworkBehaviour
{
    [Networked]
    private TickTimer life { get; set; }

    [SerializeField]
    private float bulletSpeed = 10f;

    [SerializeField]
    public Vector3 direction;

    public override void Spawned()
    {
        bulletSpeed = 10f;
        FindObjectOfType<AudioManager>().Play("TurretBullet");
        transform.rotation = Quaternion.LookRotation(new Vector3(0,0,0));
        life = TickTimer.CreateFromSeconds(Runner, 5f);
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
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.CompareTag("PlayerRed") && gameObject.CompareTag("BulletBlue"))|| (other.CompareTag("PlayerBlue") && gameObject.CompareTag("BulletRed")))
        {
            var player = other.GetComponent<PlayerController>();
            player.TakeDamage(10);
            Runner.Despawn(Object);
        }
    }
}
