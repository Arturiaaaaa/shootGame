using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class idle : MonoBehaviour
{
    public WeaponController WC;
    public PlayerMovement PM;
    public Animator anim;

    public bool isRoad ;//×°µ¯
    public bool isAiming;//Ãé×¼
    public bool isShooting;//Éä»÷


    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        //¶¯»­
        anim.SetBool("Run", PM.isRun);
        anim.SetBool("Walk", PM.isWalk);
        anim.SetBool("Reload", isRoad);
        anim.SetBool("Shoot", isShooting);


        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        if (info.IsName("Reload"))
            isRoad = true;
        else
            isRoad = false;

    }

    //»»µ¯
    public  void DoReloadAnimation()
    {
        if (WC.currentBullets > 0)
        {
            anim.Play("Reload", 0, 0);
            WC.audioSource.clip = WC.ReloadClip;
            WC.audioSource.Play();
        }
    }


}
