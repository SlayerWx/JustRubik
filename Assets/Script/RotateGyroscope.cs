using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
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
    private void OnEnable()
    {
        InputPlayer.OnGyroscopeRotatationEuler += GyroRotateEuler;
    }
    private void OnDisable()
    {
        InputPlayer.OnGyroscopeRotatationEuler -= GyroRotateEuler;
    }
    void Update()
    {
#if !UNITY_EDITOR
        GyroModifyCamera();
#endif
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
    Vector3 GyroRotateEuler()
    {
        return transform.rotation.eulerAngles;
    }
    protected void OnGUI()
    {
        //GUI.skin.label.fontSize = Screen.width / 40;

        //GUILayout.Label("Orientation: " + Screen.orientation);
        //GUILayout.Label("euler: " + transform.rotation.eulerAngles);
    }
}
