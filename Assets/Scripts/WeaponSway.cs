using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    //摇摆的参数
    public float amout;//摇摆幅度
    public float smoothAmout;//摇摆平滑值
    public float maxAmout;//最大摇摆幅度

    [SerializeField] private Vector3 originPosition;//初始位置

    void Start()
    {
        originPosition = transform.localPosition;    //相对于父级的位置
    }


    void Update()
    {
        //获取鼠标轴值
        float movementX=-Input.GetAxis("Mouse X")*amout;
        float movementY = -Input.GetAxis("Mouse Y")*amout;
        //限制
        movementX = Mathf.Clamp(movementX,-maxAmout,maxAmout);
        movementY = Mathf.Clamp(movementY, -maxAmout, maxAmout);

        //手臂位置变化
        Vector3 finnallyPosition= new Vector3(movementX, movementY, 0);
        transform.localPosition = Vector3.Lerp(transform.localPosition, finnallyPosition+originPosition,Time.deltaTime*smoothAmout);
    }
}
