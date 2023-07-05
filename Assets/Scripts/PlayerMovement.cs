using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

//��������ƶ�
public class PlayerMovement : MonoBehaviour
{
    public idle ID;
    private CharacterController characterController;//����public���ܻ��ڴ�����ж�ʧ�����
    public float walkSpeed = 10f;//�����ٶ�
    public float runSpeed = 15f;//�����ٶ�
    public float speed;//�ƶ��ٶ�
    public bool isWalk;
    public bool isRun;//�ж��Ƿ��ڱ���
    public Vector3 moveDirction;//�ƶ�����


    public float jumpForce=3f;//��Ծ����
    public Vector3 velocity;//�������Y���1�������仯������
    public bool isJump;   //�Ƿ���Ծ
    private Transform groundCheck;//������
    private float groundDistance=0.1f;//�����ľ���
    public float gravity = -30f;//����
    public LayerMask groundMash;
    private bool isGround;

   [SerializeField] private float slopeForce=6.0f;//��б�µ�����
   [SerializeField] private float slopeForceRayLength = 2.0f;//б�����ߵĽǶ�

    [Header("��������")]
    [SerializeField]private AudioSource audioSource;//��Դ
    public AudioClip walkingSound;
    public AudioClip runingSound;


    [Header("��λ����")]//�б����
    [SerializeField][Tooltip("���ܰ���")]private KeyCode runInputName;//[ǿ����ʾ][������ʾ] 
    [SerializeField][Tooltip("��Ծ����")] public string jumpInputName = "Jump";

    private void Start()
    {
        characterController = GetComponent<CharacterController>();  //��ȡplayer�ϵ�CharacterController���
        audioSource= GetComponent<AudioSource>();
        runInputName = KeyCode.LeftShift;

        //����ʵ����ק
        groundCheck = GameObject.Find("Player/CheckGround").GetComponent<Transform>();
    }

    private void Update()
    {
        CheckGrounnd();
        Move();
        

    }

    //�ƶ�
    public void Move()
    {
        float h=Input.GetAxis("Horizontal");//��ȡ��ֱ����
        float v = Input.GetAxis("Vertical");//ˮƽ

        isRun = Input.GetKey(runInputName);
        isWalk = (Mathf.Abs(h)>0 || Mathf.Abs(v) > 0)?true:false;
        speed = isRun ? runSpeed : walkSpeed;


        //�����ƶ�����
        moveDirction = (h * transform.right + v * transform.forward).normalized; //x�� z�� normalized������׼��
        characterController.Move(moveDirction*speed*Time.deltaTime);//Time.deltaTime����һ֡����ǰ֡�ļ��������Ϊ��λ����ֻ����
        if (!isGround)//���ڵ��� �ۼ����µ�����
        {
            velocity.y += gravity * Time.deltaTime;
        }
        characterController.Move(velocity* Time.deltaTime);
        Jump();

        if (OnSlpe())
            characterController.Move(Vector3.down * characterController.height / 2 * slopeForce * Time.deltaTime);//��һ�����µ���

        PlayFootStepSound();
    }

    //�����ж���Դ
    public void PlayFootStepSound()
    {
        if (isGround && moveDirction.sqrMagnitude>0.9)
        {
            audioSource.clip = isRun ? runingSound : walkingSound;//���ű��ܻ�����
            if(!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            if(audioSource.isPlaying)
                audioSource.Pause();
        }
            
    }


    //��Ծ
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
        isGround=Physics.CheckSphere(groundCheck.position, groundDistance, groundMash);//��groundCheckλ�����������⣬�ж��Ƿ��ڵ���
        //�ڵ����ϸ�һ�����µ���
        if (isGround && velocity.y<=0)
        {
            velocity.y = -2f;
        }
    }

    //�ж��Ƿ���б����
    public bool OnSlpe()
    {
        if (isJump)
            return false;
        RaycastHit hit;
        //���´������ ����Ƿ���б����
        if(Physics.Raycast(transform.position,Vector3.down,out hit,characterController.height/2* slopeForceRayLength))
        {
            //����Ӵ��ĵ㲻�ڣ�0,1,0���ķ����� ��ô������б����
            if (hit.normal != Vector3.up)
                return true;
        }
        return false;
    }

}
