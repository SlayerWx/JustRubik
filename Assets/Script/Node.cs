using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Vector3 ID;
    public Transform piece;
    private int layerMask = 1 << 9;
    void Start()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 10.0f, layerMask))
        {
           ID = hit.collider.transform.parent.GetComponent<PieceInfo>().positionInCubeID;
        }
        BlockPositions.OnSetBlockPosition?.Invoke(this);
    }
    private void OnEnable()
    {
        ReadCube.OnReadCubeWithRaycast += ReadingCube;
    }
    private void OnDisable()
    {
        ReadCube.OnReadCubeWithRaycast -= ReadingCube;
    }
    void ReadingCube(GameObject me, Transform newPiece)
    {
        if (me == gameObject)
        {
            piece = newPiece.parent;
        }
    }
}
