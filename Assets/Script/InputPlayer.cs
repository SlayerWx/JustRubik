using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
public class InputPlayer : MonoBehaviour
{
    public GameObject test;
    public Transform pivot;
    Vector2[] touches = new Vector2[5];
    int layerMask = 1 << 9; 
    public Vector3 direction;
    public float speed;
    bool selectedDirection = false;
    bool rayGetBlock = false;
    Vector3 touchDirection;
    Vector3 rotation;
    Transform refFirstBlock;
    public BlockPositions.RotationType axisSelected;
    //const float decideAxisBuffer = 4.4f;
    const float decideAxisBufferY = 0.04f;
    const float decideAxisBufferX = 0.07f;
    const float deltaCompensation = 5.5f; 
    const float sensitivity = 7f;
    public delegate Vector3 CallGetGyroscopeRotation();
    public static event CallGetGyroscopeRotation OnGyroscopeRotatationEuler;
    bool canInput = true;
    bool fixedRotation = false;
    bool fixing = false;
    Vector3 hitAux;
    float deltaSensitivity = 5;
    public static Action OnCubeEndAutorotate;
    bool HitCorrectionCheck = false;
    Vector3 deltaPositionFirst;
    Vector3 deltaPositionSecond;
    private void Start()
    {
        rayGetBlock = false;
        selectedDirection = false;
        canInput = true;
        fixedRotation = false;
        fixing = false;
    }
    void Update()
    {
        if (Input.touchCount > 0 && canInput && !fixing)
        {
            rotation = Vector3.zero;
            RaycastHit hit;
            Touch touch = Input.GetTouch(0);
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            //Vector3 auxi;
            //auxi.x = touch.position.x;
            //auxi.y = touch.position.y;
            //auxi.z = Camera.main.nearClipPlane + deltaSensitivity;
            //Instantiate(test, Camera.main.ScreenToWorldPoint(auxi), Quaternion.identity, null);
            //Debug.Log(Camera.main.ScreenToWorldPoint(auxi));
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (Physics.Raycast(ray, out hit, 100.0f, layerMask) && hit.collider.gameObject != null)
                    {
                        rayGetBlock = true;
                        refFirstBlock = hit.collider.transform.parent;

                        //hit = HitCorrection(hit);

                        switch (refFirstBlock.transform.position)
                        {
                            case Vector3 _ when (Mathf.Abs(refFirstBlock.position.x) +
                            Mathf.Abs(hit.collider.transform.parent.lossyScale.x / 2))
                            <= Mathf.Abs(hit.point.x):
                                direction = Vector3.right;
                                break;
                            case Vector3 _ when Mathf.Abs(refFirstBlock.position.y) +
                            Mathf.Abs((hit.collider.transform.parent.lossyScale.y / 2))
                            <= Mathf.Abs(hit.point.y):
                                direction = Vector3.up;
                                break;
                            case Vector3 _ when Mathf.Abs(refFirstBlock.position.z) +
                            Mathf.Abs((hit.collider.transform.parent.lossyScale.z / 2))
                            <= Mathf.Abs(hit.point.z):
                                direction = Vector3.forward;
                                break;


                        }
                        hitAux= hit.point;
                        selectedDirection = false;
                        deltaPositionFirst = touch.position;
                        deltaPositionFirst.z = Camera.main.nearClipPlane + deltaSensitivity;
                        deltaPositionSecond = Vector3.zero;
                    }
                    break;
                case TouchPhase.Moved:
                    if (rayGetBlock)
                    {
                        if (deltaPositionSecond != Vector3.zero)
                        {
                            deltaPositionFirst = deltaPositionSecond;
                        }
                        deltaPositionSecond = touch.position;
                        deltaPositionSecond.z = Camera.main.nearClipPlane + deltaSensitivity;
                        touchDirection = (Camera.main.ScreenToWorldPoint(deltaPositionFirst) * deltaCompensation -
                        Camera.main.ScreenToWorldPoint(deltaPositionSecond) * deltaCompensation) * -1;
                        if (Mathf.Abs(touchDirection.x) > decideAxisBufferX && !selectedDirection)
                        {
                            axisSelected = BlockPositions.RotationType.leftright;
                            selectedDirection = true;
                        }
                        if (Mathf.Abs(touchDirection.y) > decideAxisBufferY && !selectedDirection)
                        {
                            axisSelected = BlockPositions.RotationType.updown;
                            selectedDirection = true;
                        }
                        if (selectedDirection)
                        {
                            #region Axisfoward
                            if (direction == Vector3.forward && axisSelected == BlockPositions.RotationType.leftright)
                            {
                                if (hitAux.z <= 0.0f)
                                {
                                    rotation.y -= ((touchDirection.x + touchDirection.z) / 2) * sensitivity;
                                }
                                else
                                {
                                    rotation.y += ((touchDirection.x + touchDirection.z) / 2) * sensitivity;
                                }
                            }
                            if (direction == Vector3.forward && axisSelected == BlockPositions.RotationType.updown)
                            {
                                if (hitAux.z <= 0.0f)
                                {
                                    rotation.x += ((touchDirection.y + touchDirection.z)/2) * sensitivity;
                                }else
                                {
                                    rotation.x -= ((touchDirection.y + touchDirection.z) / 2) * sensitivity;
                                }
                              
                            }
                            if (direction == Vector3.forward && axisSelected == BlockPositions.RotationType.rotleftright)
                            {
                                rotation.z -= touchDirection.x * sensitivity;
                            }
                            #endregion

                            #region AxisRight
                            if (direction == Vector3.right && axisSelected == BlockPositions.RotationType.leftright)
                            {
                                if (hitAux.x < 0.0f)
                                {
                                    rotation.y += ((touchDirection.x + touchDirection.z) / 2) * sensitivity;
                                }
                                else
                                {
                                    rotation.y -= ((touchDirection.x + touchDirection.z) / 2) * sensitivity;
                                }
                            }
                            if (direction == Vector3.right && axisSelected == BlockPositions.RotationType.updown)
                            {
                                if (hitAux.x < 0.0f)
                                {
                                    rotation.z -= touchDirection.y * sensitivity;
                                }
                                else
                                {
                                    rotation.z += touchDirection.y * sensitivity;
                                }
                            }
                            if (direction == Vector3.right && axisSelected == BlockPositions.RotationType.rotleftright)
                            {
                                rotation.x -= touchDirection.x * sensitivity;
                            }
                            #endregion

                            #region AxisUp
                            if (direction == Vector3.up && axisSelected == BlockPositions.RotationType.leftright)
                            {
                                if (hitAux.y < 0.0f)
                                {
                                    rotation.z += ((touchDirection.x + touchDirection.z) / 2) * sensitivity;
                                }
                                else
                                {
                                    rotation.z -= ((touchDirection.x + touchDirection.z) / 2) * sensitivity;
                                }

                            }
                            if (direction == Vector3.up && axisSelected == BlockPositions.RotationType.updown)
                            {
                                rotation.x += ((touchDirection.y + touchDirection.z) / 2) * sensitivity;
                            }
                            if (direction == Vector3.up && axisSelected == BlockPositions.RotationType.rotleftright)
                            {
                                rotation.y -= touchDirection.x * sensitivity;

                            }
                            #endregion

                            BlockPositions.SetInPivot(refFirstBlock, pivot, axisSelected, direction);
                            refFirstBlock.transform.parent = pivot;
                            pivot.Rotate(rotation, Space.Self);
                        }

                    }
                    
                    break;
                case TouchPhase.Ended:
                    rayGetBlock = false;
                    selectedDirection = false;
                    canInput = false;
                    fixedRotation = true;
                    hitAux = Vector3.zero;
                    break;
            }
        }
        else if(fixedRotation && !fixing)
        {
            StartCoroutine(AutoRotate());
        }
    }
    IEnumerator AutoRotate()
    {
        fixing = true;
        Quaternion targetQuaternion = Quaternion.identity;
        Vector3 vec = pivot.eulerAngles;
        vec.x = Mathf.Round(vec.x / 90) * 90;
        vec.y = Mathf.Round(vec.y / 90) * 90;
        vec.z = Mathf.Round(vec.z / 90) * 90;
        targetQuaternion.eulerAngles = vec;
        while (fixedRotation)
        {
            float step = speed * Time.deltaTime;
            pivot.rotation = Quaternion.RotateTowards(pivot.rotation, targetQuaternion, step);
            if (Quaternion.Angle(pivot.rotation, targetQuaternion) <= 1)
            {
                pivot.rotation = targetQuaternion;
                fixedRotation = false;
            }
            yield return null;
        }
        BlockPositions.ReturnToRubik();
        pivot.rotation = Quaternion.identity;
        canInput = true;
        fixing = false;
        OnCubeEndAutorotate?.Invoke();
    }

    /*RaycastHit HitCorrection(RaycastHit hit)
    {
        // Just in case, also make sure the collider also has a renderer
        // material and texture
        MeshCollider meshCollider = hit.collider as MeshCollider;
        if (meshCollider == null || meshCollider.sharedMesh == null)
        {
            return hit;
        }
        //Debug.Log("A");
        Mesh mesh = meshCollider.sharedMesh;
        Vector3[] normals = mesh.normals;
        int[] triangles = mesh.triangles;
        // Extract local space normals of the triangle we hit
        Vector3 n0 = normals[triangles[hit.triangleIndex * 3 + 0]];
        Vector3 n1 = normals[triangles[hit.triangleIndex * 3 + 1]];
        Vector3 n2 = normals[triangles[hit.triangleIndex * 3 + 2]];

        // interpolate using the barycentric coordinate of the hitpoint
        Vector3 baryCenter = hit.barycentricCoordinate;

        // Use barycentric coordinate to interpolate normal
        Vector3 interpolatedNormal = n0 * baryCenter.x + n1 * baryCenter.y + n2 * baryCenter.z;
        // normalize the interpolated normal
        interpolatedNormal = interpolatedNormal.normalized;

        // Transform local space normals to world space
        Transform hitTransform = hit.collider.transform;
        interpolatedNormal = hitTransform.TransformDirection(interpolatedNormal);

        // Display with Debug.DrawLine
        Debug.DrawRay(hit.point, interpolatedNormal);
        Vector3 ray =  hit.point;
        RaycastHit hit2;
        if (Physics.Raycast(ray, interpolatedNormal, out hit2, 10.0f, layerMask))
        {
            refFirstBlock = hit2.collider.transform;
            Vector3 ray2 = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            Debug.Log("hit2");
            RaycastHit hit3;
            if (Physics.Raycast(ray2, ((hit2.point - ((hit2.point - refFirstBlock.position).normalized * 0.15f)) - ray2).normalized, out hit3,50.0f,layerMask))
            {
                Debug.DrawRay(ray2, ((hit2.point - ((hit2.point - refFirstBlock.position).normalized * 0.15f)) - ray2).normalized * 50.0f, Color.cyan);
                Debug.Log("hit3");
                HitCorrectionCheck = true;
                return hit3;
            }

        }
        return hit; 
    }*/
}
