using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���������ת
//���player������ת����ʵ��
//�����������ת����ʵ��
public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 1000f;//����������
    public Transform playerBody;//���λ��
    public float xRotation = 0f;
    //public float yRotation = 0f;


    
    void Start()
    {
        //�����������Ϸ���ڵ����Ĳ����ع��
        Cursor.lockState = CursorLockMode.Locked;
    }

    
    void Update()
    {

        float mouseX = Input.GetAxis("Mouse X")* mouseSensitivity * Time.deltaTime;//����ƶ�����
        float mouseY = Input.GetAxis("Mouse Y")* mouseSensitivity * Time.deltaTime;//����ƶ�����
        xRotation-=mouseY;//��������ת��Ч���ۼ�
        xRotation=Mathf.Clamp(xRotation,-70f,70f);//���ƾ�ͷ��ת����

        //yRotation += mouseX;
        //transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);//������ת�ķ���
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up*mouseX);//��Һ�����ת
        
    }
}
