using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotRotate : MonoBehaviour
{
    [SerializeField] float sensitivity = 0.4f;
    [SerializeField] float speed = 300.0f;
    Vector3 rotation;
    List<GameObject> activeSide;
    Vector3 localFoward;
    Vector3 touchRef;
    bool dragging = false;
    bool autoRotating = false;
    Quaternion targetQuaternion;
    [SerializeField] CubeState cubeState;
    [SerializeField] ReadCube readCube;
    public static bool inProgress = false;
    
    void Start()
    {
    }

    void Update()
    {
        if(dragging && !inProgress)
        {
            SpinSide(activeSide);
            if(Input.GetMouseButtonUp(0))
            {
                dragging = false;
                RotateToRightAngle();
            }
        }
        if (autoRotating)
        {
            AutoRotate();
        }
    }
    void SpinSide(List<GameObject> side)
    {
        rotation = Vector3.zero;
        Vector3 touchOffset = (Input.mousePosition - touchRef);
        if(side == cubeState.front)
        {
            rotation.z = (touchOffset.x + touchOffset.y) * sensitivity * -1;
        }
        if (side == cubeState.up)
        {
            rotation.y = (touchOffset.x + touchOffset.y) * sensitivity * 1;
        }
        if (side == cubeState.back)
        {
            rotation.z = (touchOffset.x + touchOffset.y) * sensitivity * 1;
        }
        if (side == cubeState.down)
        {
            rotation.y = (touchOffset.x + touchOffset.y) * sensitivity * -1;
        }
        if (side == cubeState.left)
        {
            rotation.x = (touchOffset.x + touchOffset.y) * sensitivity * 1;
        }
        if (side == cubeState.right)
        {
            rotation.x = (touchOffset.x + touchOffset.y) * sensitivity * 1;
        }
        transform.Rotate(rotation,Space.Self);
        touchRef = Input.mousePosition;
    }
    public void Rotate(List<GameObject> side)
    {
        activeSide = side;
        touchRef = Input.mousePosition;
        dragging = true;
        localFoward = Vector3.zero - side[4].transform.parent.transform.localPosition;
    }
    public void RotateToRightAngle()
    {
        Vector3 vec = transform.localEulerAngles;
        vec.x = Mathf.Round(vec.x / 90) * 90; 
        vec.y = Mathf.Round(vec.y / 90) * 90;
        vec.z = Mathf.Round(vec.z / 90) * 90;
        targetQuaternion.eulerAngles = vec;
        autoRotating = true;
    }
    void AutoRotate()
    {
        float step = speed * Time.deltaTime;
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation,targetQuaternion,step);
        if(Quaternion.Angle(transform.localRotation,targetQuaternion)<=1)
        {
            transform.localRotation = targetQuaternion;
            cubeState.PutDown(activeSide,transform.parent);
            readCube.ReadState();
            autoRotating = false;
            dragging = false;
            inProgress = false;
        }
    }
}
