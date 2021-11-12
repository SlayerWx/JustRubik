using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ReadCube : MonoBehaviour
{
    public GameObject emptyGO;
    [Space]
    public Transform tUp;
    public Transform tDown;
    public Transform tLeft;
    public Transform tRight;
    public Transform tFront;
    public Transform tBack;

    private int layerMask = 1 << 9;

    List<GameObject> frontRays = new List<GameObject>();
    List<GameObject> backRays = new List<GameObject>();
    List<GameObject> leftRays = new List<GameObject>();
    List<GameObject> rightRays = new List<GameObject>();
    List<GameObject> uptRays = new List<GameObject>();
    List<GameObject> downRays = new List<GameObject>();


    public static Action<GameObject,Transform> OnReadCubeWithRaycast;
    private void OnEnable()
    {
        InputPlayer.OnCubeEndAutorotate += ReadFaceAll;
    }
    private void OnDisable()
    {
        InputPlayer.OnCubeEndAutorotate -= ReadFaceAll;

    }
    void Start()
    {
        SetRayTransforms();
        ReadFaceAll();

    }

    private void ReadFaceAll()
    {
        ReadFace(uptRays,tUp);
        ReadFace(downRays, tDown);
        ReadFace(leftRays, tLeft);
        ReadFace(rightRays, tRight);
        ReadFace(frontRays, tFront);
        ReadFace(backRays, tBack);
    }

    void SetRayTransforms()
    {
        uptRays = BuildRays(tUp, new Vector3(90, 90, 0));
        downRays = BuildRays(tDown, new Vector3(270, 90, 0));
        leftRays = BuildRays(tLeft, new Vector3(0, 90, 0));
        rightRays = BuildRays(tRight, new Vector3(0, 270, 0));
        frontRays = BuildRays(tFront, new Vector3(0, 0, 0));
        backRays = BuildRays(tBack, new Vector3(0, 180, 0));
    }
    List<GameObject> BuildRays(Transform rayTransform, Vector3 direction)
    {
        int rayCount = 0;
        List<GameObject> rays = new List<GameObject>();

        // 0|1|2
        // 3|4|5
        // 6|7|8

        for (int y = 1; y > -2; y--)
        {
            for (int x = -1; x < 2; x++)
            {
                Vector3 startPos = new Vector3(rayTransform.localPosition.x + x,
                    rayTransform.localPosition.y + y,
                    rayTransform.localPosition.z);
                GameObject rayStart = Instantiate(emptyGO, startPos, Quaternion.identity, rayTransform);
                rayStart.name = rayCount.ToString();
                rays.Add(rayStart);
                rayCount++;

            }
        }
        rayTransform.localRotation = Quaternion.Euler(direction);
        return rays;
    }
    public List<GameObject> ReadFace(List<GameObject> rayStarts, Transform rayTransform)
    {
        /*
        // Evento de raycast
        //consigue el transform del cubo
        //
        OnReadCubeWithRaycast?.Invoke();*/
        List<GameObject> facesHit = new List<GameObject>();
        foreach (GameObject rayStart in rayStarts)
        {
            Vector3 ray = rayStart.transform.position;
            RaycastHit hit;
            if (Physics.Raycast(ray, rayTransform.forward, out hit, 10.0f, layerMask))
            {
                Debug.DrawRay(ray, rayTransform.forward * hit.distance, Color.yellow);
                facesHit.Add(hit.collider.gameObject);
                OnReadCubeWithRaycast?.Invoke(rayStart,hit.transform);
            }
            else
            {
                Debug.DrawRay(ray, rayTransform.forward * 10.0f, Color.red);
            }


            //evento de cubo
            //su propio transform para comparar
            //el nuevo ID

        }
        return facesHit;
    }
}
