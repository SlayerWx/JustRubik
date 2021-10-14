using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotateGyroscope : MonoBehaviour
{

    private Gyroscope phoneGyro;
    private Quaternion correctionQuaternion;

    void Start()
    {
        phoneGyro = Input.gyro;
        phoneGyro.enabled = true;
        correctionQuaternion = Quaternion.Euler(90f, 0f, 0f);
    }

    void Update()
    {
        GyroModifyCamera();
    }

    void GyroModifyCamera()
    {
        Quaternion gyroQuaternion = GyroToUnity(Input.gyro.attitude);
        // rotate coordinate system 90 degrees. Correction Quaternion has to come first
        Quaternion calculatedRotation = correctionQuaternion * gyroQuaternion;
        transform.rotation = calculatedRotation;
    }

    private static Quaternion GyroToUnity(Quaternion q)
    {
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }
}
