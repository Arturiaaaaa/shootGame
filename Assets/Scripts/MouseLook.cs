using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//摄像机的旋转
//玩家player左右旋转控制实现
//摄像机上下旋转控制实现
public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 1000f;//视线灵敏度
    public Transform playerBody;//玩家位置
    public float xRotation = 0f;
    //public float yRotation = 0f;


    
    void Start()
    {
        //光标锁定在游戏窗口的中心并隐藏光标
        Cursor.lockState = CursorLockMode.Locked;
    }

    
    void Update()
    {

        float mouseX = Input.GetAxis("Mouse X")* mouseSensitivity * Time.deltaTime;//鼠标移动左右
        float mouseY = Input.GetAxis("Mouse Y")* mouseSensitivity * Time.deltaTime;//鼠标移动上下
        xRotation-=mouseY;//将上下旋转的效果累加
        xRotation=Mathf.Clamp(xRotation,-70f,70f);//限制镜头旋转度数

        //yRotation += mouseX;
        //transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);//两种旋转的方法
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up*mouseX);//玩家横向旋转
        
    }
}
