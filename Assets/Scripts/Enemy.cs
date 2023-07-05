using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public WeaponController controller;
    
    public NavMeshAgent enemyMesh;
    public float speed=5f;//走路
    public float diatance;//移动的范围

    public Transform target;
    public float attackDistance = 10f;
    public float moveToDistance = 20f;
    private Coroutine moveCoroutine;
    private Coroutine attackCoroutine;


    public float time = 0f;
    public static float playerBlood = 20000;//玩家血量
    public  float enemyBlood = 100;//敌人血量


    public Text BloodUI;
    public GameObject DieUI;//死亡UI

    public Animator animEnemy;//敌人的动画
    public bool isShoot;
    public bool isWalk;
    public bool isDie;

    void Start()
    {
        moveCoroutine = StartCoroutine(Move());
        attackCoroutine = StartCoroutine(Attack());

        //初始化
        enemyMesh.speed = 0;
        UpdateDieUI();
        animEnemy = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        animEnemy.SetBool("Ewalk", isWalk);
        animEnemy.SetBool("Eshoot", isShoot);
        animEnemy.SetBool("EDie", isDie);

        isWalk = true;

        if (playerBlood <= 0)
        {
            //玩家失败
            DieUI.SetActive(true);

        }

        if(enemyBlood <= 0)
        {
            isDie = true;
            StopCoroutine(moveCoroutine);
            StopCoroutine(attackCoroutine);
        }


    }
    IEnumerator Move()
    {
        while (true)
        {
            yield return null;

            enemyMesh.speed = speed;
            //随机位置
            Vector3 movev3 = new Vector3(Random.Range(this.transform.position.x - diatance, this.transform.position.x + diatance),this.transform.position.y, Random.Range(this.transform.position.z - diatance, this.transform.position.z + diatance));
            enemyMesh.SetDestination(movev3);

            //等待一段时间再走
            yield return new WaitForSeconds(Random.Range(0, 3));
            enemyMesh.speed = 0;
        }
    }

    
    IEnumerator Attack()
    {
        while (true)
        {
            yield return null;
            
            if (Vector3.Distance(target.transform.position, this.transform.position) <= moveToDistance)
            {
                StopCoroutine(moveCoroutine);//停止移动协程
                isWalk=false;
                enemyMesh.SetDestination(target.transform.position);//移向玩家

                if (Vector3.Distance(target.transform.position, this.transform.position) <= attackDistance)
                {
                    //攻击
                    enemyMesh.speed = 0;
                    isShoot = true;
                    yield return new WaitForSeconds(1f);
                    playerBlood -= 10;
                    UpdateDieUI();
                }
                enemyMesh.speed = speed;
                isWalk=true;
                isShoot=false;
            }

        }
    }

    public void UpdateDieUI()
    {
        BloodUI.text = "HP:" + playerBlood;
    }


}
