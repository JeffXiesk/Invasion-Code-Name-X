using System.Collections;
using System.Threading;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;
public class PlayerController : NetworkBehaviour
{
    [SerializeField]
    float speed = 0;
    [SerializeField]
    const float initial_speed = 5;
    [SerializeField]
    public Bullet bulletPrefab = null;
    [SerializeField]
    public Bullet bulletPrefabRed = null;
    [SerializeField]
    private Image hpBar = null;
    [SerializeField]
    private NetworkPrefabRef turretPurchasedPrefab;
    [SerializeField]
    private NetworkPrefabRef turretPurchasedPrefabRed;
    [SerializeField]
    private QController Qblue;
    [SerializeField]
    private QController Qred;
    [SerializeField]
    private bool isCooling = false;

    [Networked]
    private TickTimer Ending { get; set; }
    [Networked]
    private TickTimer timer { get; set; }
    [Networked]
    private TickTimer cooldown { get; set; }
    [SerializeField]
    private bool isRenai = false;

    public Tilemap obstacles;
    public Tilemap obstacles2;

    [Networked]
    public float money {get; set; }

    [Networked(OnChanged = nameof(OnHpChanged))]
    public int Hp { get; set; }

    [SerializeField]
    private int maxHp = 100;

    [Networked]
    public NetworkButtons ButtonsPrevious { get; set; }

    private StatusBar sb;

    public override void Spawned()
    {
        sb = GameObject.Find("Canvas").transform.GetChild(1).gameObject.GetComponent<StatusBar>();
        if (Object.InputAuthority == Runner.LocalPlayer)
            Camera.main.transform.position = gameObject.transform.position + new Vector3(0, 0, -10);
        obstacles = GameObject.Find("Wall").GetComponent<Tilemap>();
        obstacles2 = GameObject.Find("BulletWall").GetComponent<Tilemap>();
        if (Object.HasStateAuthority)
        {
            Hp = maxHp;
            money = 500f;
            if (Object.InputAuthority == Runner.LocalPlayer)
            {
                sb.SetMoney(money);
            }
        } else {
            if (Object.InputAuthority == Runner.LocalPlayer)
            {
                sb.SetMoney(money);
            }
        }
            
    }

    // Update is called once per frame
    public override void FixedUpdateNetwork()
    {
        if (GameObject.Find("Player(Clone)") == null || GameObject.Find("Player 1(Clone)") == null)
            return;
     
        if (GameObject.Find("CrystalBlue(Clone)") == null){
            Debug.Log("Crystal Blue not found !!! ");
            if (Object.InputAuthority == Runner.LocalPlayer)
                if (gameObject.tag == "PlayerRed"){
                    GameObject.Find("Canvas").transform.GetChild(1).gameObject.SetActive(false);
                    GameObject.Find("Canvas").transform.GetChild(0).gameObject.GetComponent<GameOverScreen>().Setup("You Win !!!");
                } else {
                    GameObject.Find("Canvas").transform.GetChild(1).gameObject.SetActive(false);
                    GameObject.Find("Canvas").transform.GetChild(0).gameObject.GetComponent<GameOverScreen>().Setup("You Lose !!!");                
                } 
            return;
        } else{
            Debug.Log("Crystal Blue found !!! ");
        }
        
        if (GameObject.Find("CrystalRed(Clone)") == null){
            Debug.Log("Crystal Red not found !!! ");
            if (Object.InputAuthority == Runner.LocalPlayer)
                if (gameObject.tag == "PlayerBlue"){
                    GameObject.Find("Canvas").transform.GetChild(1).gameObject.SetActive(false);
                    GameObject.Find("Canvas").transform.GetChild(0).gameObject.GetComponent<GameOverScreen>().Setup("You Win !!!");
                } else {
                    GameObject.Find("Canvas").transform.GetChild(1).gameObject.SetActive(false);
                    GameObject.Find("Canvas").transform.GetChild(0).gameObject.GetComponent<GameOverScreen>().Setup("You Lose !!!");                
                } 
            return;
        } else{
            Debug.Log("Crystal Red found !!! ");
        }


        money += 5 * Runner.DeltaTime;
        
        if (Object.HasStateAuthority)
        {
            if (Object.InputAuthority == Runner.LocalPlayer)
            {
                sb.SetMoney(money);
            }
        } else {
            if (Object.InputAuthority == Runner.LocalPlayer)
            {
                sb.SetMoney(money);
            }
        }        
        
        if (GetInput(out NetworkInputData data))
        {
            NetworkButtons buttons = data.buttons;
            var pressed = buttons.GetPressed(ButtonsPrevious);
            ButtonsPrevious = buttons;
            bool isKeyDown = false;

            if (pressed.IsSet(InputButtons.Esc))
            {
                Application.Quit();
            }

            if (pressed.IsSet(InputButtons.Mouse0))
            {
                if (gameObject.CompareTag("PlayerBlue"))
                {
                    Bullet bullet = Runner.Spawn(bulletPrefab, transform.position, Quaternion.LookRotation(data.mousePosition - transform.position), Object.InputAuthority);
                    Debug.Assert(bullet != null);
                    bullet.direction = Vector3.Normalize(data.mousePosition - transform.position);
                    FindObjectOfType<AudioManager>().Play("HeroBullet");
                }
                else
                {
                    Bullet bullet = Runner.Spawn(bulletPrefabRed, transform.position, Quaternion.LookRotation(data.mousePosition - transform.position), Object.InputAuthority);
                    if (bullet != null) {                    
                        bullet.direction = Vector3.Normalize(data.mousePosition - transform.position);
                        FindObjectOfType<AudioManager>().Play("HeroBullet");
                    }
                }
            }

            if (buttons.IsSet(InputButtons.D))
            {
                speed = initial_speed;
                gameObject.transform.localScale = new Vector3(1, 1, 1);
                Vector3 moveToPosition = gameObject.transform.position + new Vector3(speed, 0, 0) * Runner.DeltaTime;
                Vector3Int obstacleMapTile = obstacles.WorldToCell(moveToPosition);
                if (obstacles.GetTile(obstacleMapTile) == null && obstacles2.GetTile(obstacleMapTile) == null) {
                    gameObject.transform.position = moveToPosition;
                    if (Object.InputAuthority == Runner.LocalPlayer)
                        Camera.main.transform.position = moveToPosition + new Vector3(0, 0, -10);
                }
                isKeyDown = true;
            }
            if (buttons.IsSet(InputButtons.A))
            {
                speed = initial_speed;
                gameObject.transform.localScale = new Vector3(-1, 1, 1);
                Vector3 moveToPosition = gameObject.transform.position - new Vector3(speed, 0, 0) * Runner.DeltaTime;
                Vector3Int obstacleMapTile = obstacles.WorldToCell(moveToPosition);
                if (obstacles.GetTile(obstacleMapTile) == null && obstacles2.GetTile(obstacleMapTile) == null) {
                    gameObject.transform.position = moveToPosition;
                    if (Object.InputAuthority == Runner.LocalPlayer)
                        Camera.main.transform.position = moveToPosition + new Vector3(0, 0, -10);
                }
                isKeyDown = true;
            }
            if (buttons.IsSet(InputButtons.W))
            {
                speed = initial_speed;
                Vector3 moveToPosition = gameObject.transform.position + new Vector3(0, speed, 0) * Runner.DeltaTime;
                Vector3Int obstacleMapTile = obstacles.WorldToCell(moveToPosition);
                if (obstacles.GetTile(obstacleMapTile) == null && obstacles2.GetTile(obstacleMapTile) == null) {
                    gameObject.transform.position = moveToPosition;
                    if (Object.InputAuthority == Runner.LocalPlayer)
                        Camera.main.transform.position = moveToPosition + new Vector3(0, 0, -10);
                }
                isKeyDown = true;
            }
            if (buttons.IsSet(InputButtons.S))
            {
                speed = initial_speed;
                Vector3 moveToPosition = gameObject.transform.position - new Vector3(0, speed, 0) * Runner.DeltaTime;
                Vector3Int obstacleMapTile = obstacles.WorldToCell(moveToPosition);
                if (obstacles.GetTile(obstacleMapTile) == null && obstacles2.GetTile(obstacleMapTile) == null) {
                    gameObject.transform.position = moveToPosition;
                    if (Object.InputAuthority == Runner.LocalPlayer)
                        Camera.main.transform.position = moveToPosition + new Vector3(0, 0, -10);
                }
                isKeyDown = true;
            }
            if (pressed.IsSet(InputButtons.M))
            {
                Vector3 turretPosition = new Vector3(data.mousePosition.x, data.mousePosition.y, 0);
                if (Vector3.Distance(turretPosition, gameObject.transform.position) <= 5)
                {
                    if (gameObject.CompareTag("PlayerBlue")&&money>=200)
                    {
                        Runner.Spawn(turretPurchasedPrefab, turretPosition, Quaternion.identity, Object.InputAuthority);
                        money -= 200;
                        if (Object.HasStateAuthority)
                        {
                            if (Object.InputAuthority == Runner.LocalPlayer)
                            {
                                sb.SetMoney(money);
                            }
                            isCooling = false;
                        } else {
                            if (Object.InputAuthority == Runner.LocalPlayer)
                            {
                                sb.SetMoney(money);
                            }
                        }    
                    }
                    if (gameObject.CompareTag("PlayerRed")&&money>=200)
                    {
                        Runner.Spawn(turretPurchasedPrefabRed, turretPosition, Quaternion.identity, Object.InputAuthority);
                        money -= 200;
                        
                        if (Object.HasStateAuthority)
                        {
                            if (Object.InputAuthority == Runner.LocalPlayer)
                            {
                                sb.SetMoney(money);
                            }
                            isCooling = false;
                        } else {
                            if (Object.InputAuthority == Runner.LocalPlayer)
                            {
                                sb.SetMoney(money);
                            }
                        }  

                    }
                }
            }
            if (pressed.IsSet(InputButtons.Q))
            {
                if (cooldown.Expired(Runner))
                {
                    isCooling = false;
                }
                if (gameObject.CompareTag("PlayerBlue")&&!isCooling)
                {
                    QController bullet = Runner.Spawn(Qblue, transform.position, Quaternion.LookRotation(data.mousePosition - transform.position), Object.InputAuthority);
                    Debug.Assert(bullet != null);
                    bullet.direction = Vector3.Normalize(data.mousePosition - transform.position);
                    FindObjectOfType<AudioManager>().Play("HeroBullet");
                    cooldown = TickTimer.CreateFromSeconds(Runner, 3f);
                    isCooling = true;
                }
                else if(gameObject.CompareTag("PlayerRed")&&!isCooling)
                {
                    QController bullet = Runner.Spawn(Qred, transform.position, Quaternion.LookRotation(data.mousePosition - transform.position), Object.InputAuthority);
                    Debug.Assert(bullet != null);
                    bullet.direction = Vector3.Normalize(data.mousePosition - transform.position);
                    FindObjectOfType<AudioManager>().Play("HeroBullet");
                    cooldown = TickTimer.CreateFromSeconds(Runner, 3f);
                    isCooling = true;
                }
            }
            if (!isKeyDown)
            {
                speed = 0;
            }
            if (Hp <= 0 && !isRenai)
            {
                timer = TickTimer.CreateFromSeconds(Runner, 5.0f);
                FindObjectOfType<AudioManager>().Play("HeroDie");
                gameObject.transform.position = new Vector3(-1, 39, 0);
                if (Object.InputAuthority == Runner.LocalPlayer)
                    Camera.main.transform.position = new Vector3(-1, 39, -10);
                isRenai = true;
            }
            if (Hp <= 0) {
                if (gameObject.CompareTag("PlayerBlue")&& timer.Expired(Runner))
                {
                    gameObject.transform.position = new Vector3(-17, 0, 0);
                    Hp = maxHp;
                    if (Object.InputAuthority == Runner.LocalPlayer)
                        Camera.main.transform.position = new Vector3(-17, 0, -10);
                    isRenai = false;
                }
                if(gameObject.CompareTag("PlayerRed")&& timer.Expired(Runner))
                {
                    gameObject.transform.position = new Vector3(36, 0, 0);
                    Hp = maxHp;
                    if (Object.InputAuthority == Runner.LocalPlayer)
                        Camera.main.transform.position = new Vector3(36, 0, -10);
                    isRenai = false;
                }
            }
        }


    }


    public void TakeDamage(int damage)
    {
        if (Object.HasStateAuthority) {
            FindObjectOfType<AudioManager>().Play("HeroHurt");
            Hp -= damage;
        }  
    }

    private static void OnHpChanged(Changed<PlayerController> changed)
    {
        changed.Behaviour.hpBar.fillAmount = (float)changed.Behaviour.Hp / changed.Behaviour.maxHp;
    }
}
