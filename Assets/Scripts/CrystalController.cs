using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Fusion;
using System.Threading;

public class CrystalController : NetworkBehaviour {

    [Networked(OnChanged = nameof(OnHpChanged))]
    public int Hp { get; set; }

    [SerializeField]
    private int maxHp = 1000;

    [SerializeField]
    private Image hpBar = null;

    [SerializeField]
    private TurretBullet TurretBulletBlue;

    [SerializeField]
    private TurretBullet TurretBulletRed;

    private float kBulletInterval = 0.3f;

    private float mSpawnBulletAt = 0f;

    
    public override void Spawned()
    {
        maxHp = 1000;
        kBulletInterval = 0.3f;
        mSpawnBulletAt = Time.realtimeSinceStartup - kBulletInterval; // assume one was shot
        if (Object.HasStateAuthority)
            Hp = maxHp;
    }

    // Update is called once per frame
    public override void FixedUpdateNetwork()
    {  
        if (CanSpawn()){
            
            if (gameObject.CompareTag("TurretBlue"))
            {
                GameObject g = CheckIfEnemy("PlayerRed");
                if (g != null){
                TurretBullet bullet = Runner.Spawn(TurretBulletBlue,gameObject.transform.position,Quaternion.LookRotation(g.transform.position - gameObject.transform.position),Object.InputAuthority);
                mSpawnBulletAt = Time.realtimeSinceStartup;
                Debug.Assert(bullet!=null);
                bullet.direction = Vector3.Normalize(g.transform.position - gameObject.transform.position);
                }
            }
            else
            {
                GameObject g = CheckIfEnemy("PlayerBlue");
                if (g != null){
                TurretBullet bullet = Runner.Spawn(TurretBulletRed,gameObject.transform.position,Quaternion.LookRotation(g.transform.position - gameObject.transform.position),Object.InputAuthority);
                mSpawnBulletAt = Time.realtimeSinceStartup;
                Debug.Assert(bullet!=null);
                bullet.direction = Vector3.Normalize(g.transform.position - gameObject.transform.position);
                }
            }
        }
        if (Hp <= 0) {
            FindObjectOfType<AudioManager>().Play("TurretDestroy");
            Runner.Despawn(Object);
        }
        

    }

    public void TakeDamage(int damage)
    {
        if (Object.HasStateAuthority) {
            if (gameObject.CompareTag("TurretBlue") && GameObject.Find("TurretBlue(Clone)")== null ||
            gameObject.CompareTag("TurretRed") && GameObject.Find("TurretRed(Clone)")== null )
            {
                FindObjectOfType<AudioManager>().Play("TurretHit");
                Hp -= damage;
            }
        }
    }

    private static void OnHpChanged(Changed<CrystalController> changed)
    {
        changed.Behaviour.hpBar.fillAmount = (float)changed.Behaviour.Hp / changed.Behaviour.maxHp;
        // if (changed.Behaviour.Hp <= 0)
            // changed.Behaviour.winOrLose_RPC();
    }

    private GameObject CheckIfEnemy(string tag)
    {
        Collider2D[] hitsBuffer = new Collider2D[10];
        // Debug.Log("Egg OnTriggerEnter");
        // Collision with hero (especially when first spawned) does not count
        int numHits = Physics2D.OverlapCircleNonAlloc(transform.position, 10, hitsBuffer);
        foreach (Collider2D c in hitsBuffer){
            if (c == null)
                break;
            if (c.gameObject.tag == tag){
                return c.gameObject;
            }
        }
        return null;
    }

    public bool CanSpawn(){
        return TimeTillNext() <= 0f;
    }

    public float TimeTillNext()
    {
        float sinceLastEgg = Time.realtimeSinceStartup - mSpawnBulletAt;
        return kBulletInterval - sinceLastEgg;
    }

    // [Rpc(RpcSources.InputAuthority,RpcTargets.All)]
    private void winOrLose_RPC()
    {
        // if (Object.InputAuthority == Runner.LocalPlayer) {
            // if (Hp <= 0)
            // {
                GameObject.Find("Canvas").transform.GetChild(1).gameObject.SetActive(false);
                GameObject.Find("Canvas").transform.GetChild(0).gameObject.GetComponent<GameOverScreen>().Setup("Thanks !!!");
            // }
            // else
            // {
                // GameObject.Find("Canvas").transform.GetChild(0).gameObject.GetComponent<GameOverScreen>().Setup("You Win");
            // }
        // }
        // Application.Quit();
    }
}