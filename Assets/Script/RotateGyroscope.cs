using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class RotateGyroscope : MonoBehaviour
{

    private Gyroscope phoneGyro;
    private Quaternion correctionQuaternion;
    public float smoothnessFollow = 0.125f;
    Quaternion targetRot;
    void Start()
    {
        phoneGyro = Input.gyro;
        phoneGyro.enabled = true;
        correctionQuaternion = Quaternion.Euler(90f, 0f, 0f);
    }
    private void OnEnable()
    {
        InputPlayer.OnGyroscopeRotatationEuler += GyroRotateEuler;
    }
    private void OnDisable()
    {
        InputPlayer.OnGyroscopeRotatationEuler -= GyroRotateEuler;
    }
    private void Update()
    {
        targetRot = Input.gyro.attitude;
    }
    void FixedUpdate()
    {
#if !UNITY_EDITOR
        GyroModifyCamera();
#endif
    }
    void GyroModifyCamera()
    {
       // transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + Vector3.up);

        Quaternion gyroQuaternion = GyroToUnity(targetRot);
        //Quaternion gyroQuaternion = GyroToUnity(Quaternion.Euler(targetRot));
        // rotate coordinate system 90 degrees. Correction Quaternion has to come first
        Quaternion calculatedRotation = correctionQuaternion * gyroQuaternion;
        transform.rotation = Quaternion.Lerp(transform.rotation,calculatedRotation,smoothnessFollow);
        
    }

    private static Quaternion GyroToUnity(Quaternion q)
    {
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }
    Vector3 GyroRotateEuler()
    {
        return transform.rotation.eulerAngles;
    }
}
