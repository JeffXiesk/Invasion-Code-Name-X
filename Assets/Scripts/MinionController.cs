using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Fusion;

public class MinionController : NetworkBehaviour {
    
    [Networked]
    public int Hp { get; set; }

    [SerializeField]
    private int maxHp = 100;

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
            Hp = maxHp;
    }

    // Update is called once per frame
    public override void FixedUpdateNetwork()
    {
        
    }

    public void TakeDamage(int damage)
    {
        if (Object.HasStateAuthority) {
            Hp -= damage;
        }
    }
}