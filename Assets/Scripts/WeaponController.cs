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
    public Transform shooterPoint;//���λ��
    public int bulletsMag = 50;//һ����ϻ�ӵ�����
    public int range = 100;//���������
    public int bulletLeft = 200;//����
    public int currentBullets;//��ǰ�ӵ�����
    private bool GunShootInput;//������

    public float fireRate = 0.1f;//���� ԽСԽ��
    public float fireTimer=0;//��ʱ��

    public ParticleSystem muzzleFlash;//ǹ�ڻ�����Ч
    public Light muzzleFlashLight;//ǹ�ڻ���ƹ�
    public GameObject hitParticle;//�ӵ�����������Ч
    public GameObject bulletHloe;//����
    private Camera gunCamera;

    //��Ƶ��Ч
    public AudioSource audioSource;
    public AudioClip ASoundClip;//ǹ�����Ч
    public AudioClip ReloadClip;//������Ч

    [Header("��λ����")]
    [SerializeField][Tooltip("��װ�ӵ�����")]private KeyCode reloadInputName;

   

    [Header("UI����")]
    public Image CrossHairUI;
    public Text AmmoTextUI;


    // Start is called before the first frame update
    void Start()
    {
        gunCamera=Camera.main;
        audioSource = GetComponent<AudioSource>();
        reloadInputName = KeyCode.R;//R������
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

    //���
    public void GunFire()
    {
        //�������� ÿ֡��������ִ�еĴ���
        if (fireTimer < fireRate || currentBullets  <= 0 || PM.isRun || ID.isRoad) return;
        Vector3 shootDirection = shooterPoint.forward;//����ķ��� ǰ

        // ʹ��NavMesh.Raycast�����������߼��

        NavMeshHit hitNav;
        if (NavMesh.SamplePosition(shooterPoint.position, out hitNav, 12f, NavMesh.AllAreas))
        {
            Debug.Log("enemy.target����" + enemy.target.position); 
            Debug.Log("shooterPoint����" + shooterPoint.position);
            if (NavMesh.Raycast(shooterPoint.position, enemy.target.position - shooterPoint.position, out NavMeshHit navHit, NavMesh.AllAreas))
            //if (NavMesh.Raycast(shooterPoint.position, enemy.target.position - hitNav.position, out NavMeshHit navHit, NavMesh.AllAreas))
            {
                // ���������NavMesh�ཻ
                enemy.enemyBlood -= 10;
                Debug.Log("����Ѫ��" + enemy.enemyBlood);

            }
        }

        RaycastHit hit;
        if(Physics.Raycast(shooterPoint.position,shootDirection,out hit,range))//�ж����
        {
            Debug.Log(hit.transform.name);

            GameObject hitParticleEffect = Instantiate(hitParticle,hit.point,Quaternion.FromToRotation(Vector3.up,hit.normal));//ʵ�����ӵ���������Ч
            GameObject bulletHloeEffect = Instantiate(bulletHloe, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));//ʵ����������Ч
            //������Ч
            Destroy(hitParticleEffect,1f);
            Destroy(bulletHloeEffect,3f);

        }
        

        if (!ID.isAiming)
            ID.anim.CrossFadeInFixedTime("Shoot",0.1f);//�������뵭��Ч��(��ͨ����)
        else
            ID.anim.CrossFadeInFixedTime("Shoot", 0.1f);//�������뵭��Ч��(��ͨ����)
        
        
        PlayerSootSound();//���������Ч
        muzzleFlash.Play();
        muzzleFlashLight.enabled = true;
        currentBullets--;
        UpdateAmmoUI();

        fireTimer = 0f;//���ü�ʱ��
    }

    //���ӵ�ϻ
    public void Reload()
    {
        if(bulletLeft<=0)
           return ;
        ID.DoReloadAnimation();
        //������Ҫ��װ���ӵ���
        int bulletToLoad  = bulletsMag-currentBullets;
        int bulletToReduce = (bulletLeft>=bulletToLoad)?bulletToLoad:bulletLeft;
        bulletLeft -= bulletToReduce;//���ٱ���
        currentBullets += bulletToReduce;//��ǰ�ӵ�������
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

   //��׼
   public void DoingAim()
    {
        if(Input.GetMouseButton(1) && !ID.isRoad && !PM.isRun)
        {
            //��׼ ׼����ʧ ��Ұ��ǰ
            ID.isAiming = true;
            ID.anim.SetBool("Aim", true);
            CrossHairUI.gameObject.SetActive(false);
            gunCamera.fieldOfView = 25;

        }
        else//����׼
        {
            ID.isAiming = false;
            ID.anim.SetBool("Aim", false);
            CrossHairUI.gameObject.SetActive(true);
            gunCamera.fieldOfView = 60;
        }


    }
    
}
