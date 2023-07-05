using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class WeaponController : MonoBehaviour
{
    public idle ID;
    public PlayerMovement PM;
    public Enemy enemy;
    public Transform shooterPoint;//射击位置
    public int bulletsMag = 50;//一个弹匣子弹数量
    public int range = 100;//武器的射程
    public int bulletLeft = 200;//备弹
    public int currentBullets;//当前子弹数量
    private bool GunShootInput;//左键射击

    public float fireRate = 0.1f;//射速 越小越快
    public float fireTimer=0;//计时器

    public ParticleSystem muzzleFlash;//枪口火焰特效
    public Light muzzleFlashLight;//枪口火焰灯光
    public GameObject hitParticle;//子弹击中粒子特效
    public GameObject bulletHloe;//弹孔
    private Camera gunCamera;

    //音频特效
    public AudioSource audioSource;
    public AudioClip ASoundClip;//枪射击音效
    public AudioClip ReloadClip;//换弹音效

    [Header("键位设置")]
    [SerializeField][Tooltip("填装子弹按键")]private KeyCode reloadInputName;

   

    [Header("UI设置")]
    public Image CrossHairUI;
    public Text AmmoTextUI;


    // Start is called before the first frame update
    void Start()
    {
        gunCamera=Camera.main;
        audioSource = GetComponent<AudioSource>();
        reloadInputName = KeyCode.R;//R健换弹
        currentBullets=bulletsMag;
        UpdateAmmoUI();

    }

    // Update is called once per frame
    void Update()
    {
        GunShootInput=Input.GetMouseButton(0);
        if (GunShootInput && currentBullets > 0)
        {
            GunFire();
            ID.isShooting = true;
            ID.anim.SetBool("Shoot", true);
        }
        else
        {
            ID.isShooting = false;
            ID.anim.SetBool("Shoot", false);
            muzzleFlashLight.enabled = false;
        }

        
        if (Input.GetKeyDown(reloadInputName) && currentBullets < bulletsMag && bulletLeft > 0)
            Reload();


        if (fireTimer<fireRate)
            fireTimer += Time.deltaTime;

        DoingAim();
    }

    //射击
    public void GunFire()
    {
        //控制射速 每帧减少射线执行的次数
        if (fireTimer < fireRate || currentBullets  <= 0 || PM.isRun || ID.isRoad) return;
        Vector3 shootDirection = shooterPoint.forward;//射击的方向 前

        // 使用NavMesh.Raycast方法进行射线检测

        NavMeshHit hitNav;
        if (NavMesh.SamplePosition(shooterPoint.position, out hitNav, 12f, NavMesh.AllAreas))
        {
            Debug.Log("enemy.target射线" + enemy.target.position); 
            Debug.Log("shooterPoint射线" + shooterPoint.position);
            if (NavMesh.Raycast(shooterPoint.position, enemy.target.position - shooterPoint.position, out NavMeshHit navHit, NavMesh.AllAreas))
            //if (NavMesh.Raycast(shooterPoint.position, enemy.target.position - hitNav.position, out NavMeshHit navHit, NavMesh.AllAreas))
            {
                // 如果射线与NavMesh相交
                enemy.enemyBlood -= 10;
                Debug.Log("敌人血量" + enemy.enemyBlood);

            }
        }

        RaycastHit hit;
        if(Physics.Raycast(shooterPoint.position,shootDirection,out hit,range))//判断射击
        {
            Debug.Log(hit.transform.name);

            GameObject hitParticleEffect = Instantiate(hitParticle,hit.point,Quaternion.FromToRotation(Vector3.up,hit.normal));//实例化子弹射出火光特效
            GameObject bulletHloeEffect = Instantiate(bulletHloe, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));//实例化弹孔特效
            //回收特效
            Destroy(hitParticleEffect,1f);
            Destroy(bulletHloeEffect,3f);

        }
        

        if (!ID.isAiming)
            ID.anim.CrossFadeInFixedTime("Shoot",0.1f);//创建淡入淡出效果(普通开火)
        else
            ID.anim.CrossFadeInFixedTime("Shoot", 0.1f);//创建淡入淡出效果(普通开火)
        
        
        PlayerSootSound();//播放射击音效
        muzzleFlash.Play();
        muzzleFlashLight.enabled = true;
        currentBullets--;
        UpdateAmmoUI();

        fireTimer = 0f;//重置计时器
    }

    //换子弹匣
    public void Reload()
    {
        if(bulletLeft<=0)
           return ;
        ID.DoReloadAnimation();
        //计算需要填装的子弹数
        int bulletToLoad  = bulletsMag-currentBullets;
        int bulletToReduce = (bulletLeft>=bulletToLoad)?bulletToLoad:bulletLeft;
        bulletLeft -= bulletToReduce;//减少备弹
        currentBullets += bulletToReduce;//当前子弹数增加
        UpdateAmmoUI();
    }

    public void PlayerSootSound()
    {
        audioSource.clip = ASoundClip;
        audioSource.Play();
    }

    public void UpdateAmmoUI()
    {
        AmmoTextUI.text = currentBullets +"/"+bulletLeft;
    }

   //瞄准
   public void DoingAim()
    {
        if(Input.GetMouseButton(1) && !ID.isRoad && !PM.isRun)
        {
            //瞄准 准星消失 视野向前
            ID.isAiming = true;
            ID.anim.SetBool("Aim", true);
            CrossHairUI.gameObject.SetActive(false);
            gunCamera.fieldOfView = 25;

        }
        else//非瞄准
        {
            ID.isAiming = false;
            ID.anim.SetBool("Aim", false);
            CrossHairUI.gameObject.SetActive(true);
            gunCamera.fieldOfView = 60;
        }


    }
    
}
