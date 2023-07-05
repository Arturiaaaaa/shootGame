using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    //ҡ�ڵĲ���
    public float amout;//ҡ�ڷ���
    public float smoothAmout;//ҡ��ƽ��ֵ
    public float maxAmout;//���ҡ�ڷ���

    [SerializeField] private Vector3 originPosition;//��ʼλ��

    void Start()
    {
        originPosition = transform.localPosition;    //����ڸ�����λ��
    }


    void Update()
    {
        //��ȡ�����ֵ
        float movementX=-Input.GetAxis("Mouse X")*amout;
        float movementY = -Input.GetAxis("Mouse Y")*amout;
        //����
        movementX = Mathf.Clamp(movementX,-maxAmout,maxAmout);
        movementY = Mathf.Clamp(movementY, -maxAmout, maxAmout);

        //�ֱ�λ�ñ仯
        Vector3 finnallyPosition= new Vector3(movementX, movementY, 0);
        transform.localPosition = Vector3.Lerp(transform.localPosition, finnallyPosition+originPosition,Time.deltaTime*smoothAmout);
    }
}
