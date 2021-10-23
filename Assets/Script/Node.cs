using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Vector3 ID;
    public PieceInfo piece;
    private Transform lastref;
    void Start()
    {
        piece = transform.parent.GetComponent<PieceInfo>();
        ID = piece.positionInCubeID;
        lastref = transform.parent;
        transform.parent = transform.parent.parent;
        BlockPositions.OnSetBlockPosition?.Invoke(this);
    }

    private void OnTriggerStay(Collider other)
    {
        lastref = other.transform;
    }
    public void extractPiece()
    {
        piece = lastref.GetComponent<PieceInfo>();
        piece.positionInCubeID = ID;
    }
}
