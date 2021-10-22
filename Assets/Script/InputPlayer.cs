using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class InputPlayer : MonoBehaviour
{
    public Transform pivot;
    Vector2[] touches = new Vector2[5];
    int layerMask = 1 << 8; 
    public Vector2 startPos;
    public Vector3 direction;
    public float speed;
    bool selectedDirection = false;
    bool rayGetBlock = false;
    Vector2 touchDirection;
    Vector3 rotation;
    PieceInfo refFirstBlock;
    public BlockPositions.RotationType axisSelected;
    const float decideAxisBuffer = 4.4f;
    const float sensitivity = 0.4f;
    public delegate Vector3 CallGetGyroscopeRotation();
    public static event CallGetGyroscopeRotation OnGyroscopeRotatationEuler;
    bool canInput = true;
    bool fixedRotation = false;
    bool fixing = false;
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
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (Physics.Raycast(ray, out hit, 100.0f, layerMask) && hit.collider.gameObject != null)
                    {
                        rayGetBlock = true;
                        refFirstBlock = hit.collider.transform.GetComponent<PieceInfo>();


                        switch (refFirstBlock.transform.position)
                        {
                            case Vector3 _ when (Mathf.Abs(refFirstBlock.transform.position.x) +
                            Mathf.Abs(hit.collider.transform.lossyScale.x / 2))
                            <= Mathf.Abs(hit.point.x):
                                direction = Vector3.right;
                                break;
                            case Vector3 _ when Mathf.Abs(refFirstBlock.transform.position.y) +
                            Mathf.Abs((hit.collider.transform.lossyScale.y / 2))
                            <= Mathf.Abs(hit.point.y):
                                direction = Vector3.up;
                                break;
                            case Vector3 _ when Mathf.Abs(refFirstBlock.transform.position.z) +
                            Mathf.Abs((hit.collider.transform.lossyScale.z / 2))
                            <= Mathf.Abs(hit.point.z):
                                direction = Vector3.forward;
                                break;


                        }

                        selectedDirection = false;
                        //Debug.Log(ray.direction);
                    }
                    break;
                case TouchPhase.Moved:
                    if (rayGetBlock)
                    {
                        //touchDirection = Camera.main.ScreenToWorldPoint(touch.deltaPosition);
                        touchDirection = touch.deltaPosition;
                        /*float zGyroscope = (float)(OnGyroscopeRotatationEuler?.Invoke().z);
                        if ((Mathf.Abs(zGyroscope - 90) < Mathf.Abs(zGyroscope - 0)) &&
                            (Mathf.Abs(zGyroscope - 90) < Mathf.Abs(zGyroscope - 359.9f) ||
                            (Mathf.Abs(zGyroscope - 270) < Mathf.Abs(zGyroscope - 0)) &&
                            (Mathf.Abs(zGyroscope - 270) < Mathf.Abs(zGyroscope - 359.9f))))
                        {
                            zGyroscope = touchDirection.x;
                            touchDirection.x = touchDirection.y;
                            touchDirection.y = zGyroscope;
                        }*/

                        if (Mathf.Abs(touchDirection.x) > decideAxisBuffer && !selectedDirection)
                        {
                            axisSelected = BlockPositions.RotationType.leftright;
                            selectedDirection = true;
                        }
                        if (Mathf.Abs(touchDirection.y) > decideAxisBuffer && !selectedDirection)
                        {
                            axisSelected = BlockPositions.RotationType.updown;
                            selectedDirection = true;
                        }
                        if (selectedDirection)
                        {
                            #region Axisfoward
                            if (direction == Vector3.forward && axisSelected == BlockPositions.RotationType.leftright)
                            {
                                rotation.y -= touchDirection.x * sensitivity;
                            }
                            if (direction == Vector3.forward && axisSelected == BlockPositions.RotationType.updown)
                            {
                                rotation.x += touchDirection.y * sensitivity;
                            }
                            if (direction == Vector3.forward && axisSelected == BlockPositions.RotationType.rotleftright)
                            {
                                rotation.z -= touchDirection.x * sensitivity;
                            }
                            #endregion

                            #region AxisRight
                            if (direction == Vector3.right && axisSelected == BlockPositions.RotationType.leftright)
                            {
                                rotation.y -= touchDirection.x * sensitivity;
                            }
                            if (direction == Vector3.right && axisSelected == BlockPositions.RotationType.updown)
                            {
                                rotation.z += touchDirection.y * sensitivity;
                            }
                            if (direction == Vector3.right && axisSelected == BlockPositions.RotationType.rotleftright)
                            {
                                rotation.x -= touchDirection.x * sensitivity;
                            }
                            #endregion

                            #region AxisUp
                            if (direction == Vector3.up && axisSelected == BlockPositions.RotationType.leftright)
                            {
                                rotation.z -= touchDirection.x * sensitivity;
                            }
                            if (direction == Vector3.up && axisSelected == BlockPositions.RotationType.updown)
                            {
                                rotation.x += touchDirection.y * sensitivity;
                            }
                            if (direction == Vector3.up && axisSelected == BlockPositions.RotationType.rotleftright)
                            {
                                rotation.y -= touchDirection.x * sensitivity;

                            }
                            #endregion

                        }
                        BlockPositions.SetInPivot(refFirstBlock, pivot, axisSelected,direction);
                        refFirstBlock.transform.parent = pivot;
                        pivot.Rotate(rotation, Space.Self);

                    }
                    
                    break;
                case TouchPhase.Ended:
                    rayGetBlock = false;
                    selectedDirection = false;
                    canInput = false;
                    fixedRotation = true;
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
    }
}
