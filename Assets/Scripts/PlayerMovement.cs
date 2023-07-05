using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

//控制玩家移动
public class PlayerMovement : MonoBehaviour
{
    public idle ID;
    private CharacterController characterController;//设置public可能会在打包后有丢失的情况
    public float walkSpeed = 10f;//行走速度
    public float runSpeed = 15f;//奔跑速度
    public float speed;//移动速度
    public bool isWalk;
    public bool isRun;//判断是否在奔跑
    public Vector3 moveDirction;//移动方向


    public float jumpForce=3f;//跳跃力度
    public Vector3 velocity;//设置玩家Y轴的1个冲量变化（力）
    public bool isJump;   //是否跳跃
    private Transform groundCheck;//地面检测
    private float groundDistance=0.1f;//与地面的距离
    public float gravity = -30f;//重力
    public LayerMask groundMash;
    private bool isGround;

   [SerializeField] private float slopeForce=6.0f;//走斜坡的力度
   [SerializeField] private float slopeForceRayLength = 2.0f;//斜坡射线的角度

    [Header("声音设置")]
    [SerializeField]private AudioSource audioSource;//音源
    public AudioClip walkingSound;
    public AudioClip runingSound;


    [Header("键位设置")]//列表标题
    [SerializeField][Tooltip("奔跑按键")]private KeyCode runInputName;//[强制显示][悬浮提示] 
    [SerializeField][Tooltip("跳跃按键")] public string jumpInputName = "Jump";

    private void Start()
    {
        characterController = GetComponent<CharacterController>();  //获取player上的CharacterController组件
        audioSource= GetComponent<AudioSource>();
        runInputName = KeyCode.LeftShift;

        //代码实现拖拽
        groundCheck = GameObject.Find("Player/CheckGround").GetComponent<Transform>();
    }

    private void Update()
    {
        CheckGrounnd();
        Move();
        

    }

    //移动
    public void Move()
    {
        float h=Input.GetAxis("Horizontal");//获取垂直轴体
        float v = Input.GetAxis("Vertical");//水平

        isRun = Input.GetKey(runInputName);
        isWalk = (Mathf.Abs(h)>0 || Mathf.Abs(v) > 0)?true:false;
        speed = isRun ? runSpeed : walkSpeed;


        //设置移动方向
        moveDirction = (h * transform.right + v * transform.forward).normalized; //x轴 z轴 normalized向量标准化
        characterController.Move(moveDirction*speed*Time.deltaTime);//Time.deltaTime从上一帧到当前帧的间隔（以秒为单位）（只读）
        if (!isGround)//不在地面 累加向下的重力
        {
            velocity.y += gravity * Time.deltaTime;
        }
        characterController.Move(velocity* Time.deltaTime);
        Jump();

        if (OnSlpe())
            characterController.Move(Vector3.down * characterController.height / 2 * slopeForce * Time.deltaTime);//给一个向下的力

        PlayFootStepSound();
    }

    //播放行动音源
    public void PlayFootStepSound()
    {
        if (isGround && moveDirction.sqrMagnitude>0.9)
        {
            audioSource.clip = isRun ? runingSound : walkingSound;//播放奔跑或行走
            if(!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            if(audioSource.isPlaying)
                audioSource.Pause();
        }
            
    }


    //跳跃
    public void Jump()
    {
        isJump=Input.GetButtonDown(jumpInputName);

        if (isJump && isGround)
        {
            velocity.y=Mathf.Sqrt(jumpForce*-2f* gravity);
        }

    }

    public void CheckGrounnd()
    {
        isGround=Physics.CheckSphere(groundCheck.position, groundDistance, groundMash);//在groundCheck位置上做球体监测，判断是否在地上
        //在地面上给一个向下的力
        if (isGround && velocity.y<=0)
        {
            velocity.y = -2f;
        }
    }

    //判断是否在斜坡上
    public bool OnSlpe()
    {
        if (isJump)
            return false;
        RaycastHit hit;
        //向下打出射线 检测是否在斜坡上
        if(Physics.Raycast(transform.position,Vector3.down,out hit,characterController.height/2* slopeForceRayLength))
        {
            //如果接触的点不在（0,1,0）的方向上 那么人物在斜坡上
            if (hit.normal != Vector3.up)
                return true;
        }
        return false;
    }

}
