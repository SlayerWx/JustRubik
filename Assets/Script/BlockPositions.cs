using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class BlockPositions : MonoBehaviour
{
    public static Action<Node> OnSetBlockPosition;
    public static Node[,,] nodes = new Node[3,3,3];
    public static Transform rubikPivot;
    public GameObject node;
    public static bool researched;
    public enum RotationType
    {
        leftright,updown,rotleftright
    }
    private void OnEnable()
    {
        OnSetBlockPosition += CollectPieceInfo;
        rubikPivot = transform;
        researched = false;
    }
    private void OnDisable()
    {
        OnSetBlockPosition -= CollectPieceInfo;
    }
    void CollectPieceInfo(Node info)
    {
        nodes[(int)info.ID.x,(int)info.ID.y,(int)info.ID.z] = info;
    }
    public static void SetInPivot(PieceInfo firstPiece, Transform pivot, RotationType type,Vector3 facetype)
    {
        if (!researched)
        {
            foreach (Node n in nodes)
            {
                if (type == RotationType.leftright)
                {
                    if (facetype != Vector3.up)
                    {
                        if (n)
                        {
                            if (n.ID.y == firstPiece.positionInCubeID.y)
                            {
                                n.piece.transform.parent = pivot;
                            }
                        }
                    }
                    if (facetype == Vector3.up)
                    {
                        if (n)
                        {
                            if (n.ID.z == firstPiece.positionInCubeID.z)
                            {
                                n.piece.transform.parent = pivot;
                            }
                        }
                    }
                }
                if (type == RotationType.updown)
                {
                    if (facetype != Vector3.right)
                    {
                        if (n)
                        {
                            if (n.ID.x == firstPiece.positionInCubeID.x)
                            {
                                n.piece.transform.parent = pivot;
                            }
                        }
                    }
                    if (facetype == Vector3.right)
                    {
                        if (n)
                        {
                            if (n.ID.z == firstPiece.positionInCubeID.z)
                            {
                                n.piece.transform.parent = pivot;
                            }
                        }
                    }
                }
            }


            /*foreach (PieceInfo piece in nodes)
            {
                if (piece)
                {
                    switch (type)
                    {
                        case RotationType.leftright:
                            switch (facetype)
                            {
                                case Vector3 _ when Vector3.forward == facetype:
                                    if (piece.transform.localPosition.y == firstPiece.transform.localPosition.y)
                                    {
                                        piece.transform.parent = pivot;
                                    }
                                    break;
                                case Vector3 _ when Vector3.right == facetype:
                                    if (piece.transform.localPosition.y == firstPiece.transform.localPosition.y)
                                    {
                                        piece.transform.parent = pivot;
                                    }
                                    break;
                                case Vector3 _ when Vector3.up == facetype:
                                    if (piece.transform.localPosition.z == firstPiece.transform.localPosition.z)
                                    {
                                        piece.transform.parent = pivot;
                                    }
                                    break;
                            }
                            break;
                        case RotationType.updown:
                            switch (facetype)
                            {
                                case Vector3 _ when Vector3.forward == facetype:
                                    if (piece.transform.localPosition.x == firstPiece.transform.localPosition.x)
                                    {
                                        piece.transform.parent = pivot;
                                    }
                                    break;
                                case Vector3 _ when Vector3.right == facetype:
                                    if (piece.transform.localPosition.z == firstPiece.transform.localPosition.z)
                                    {
                                        piece.transform.parent = pivot;
                                    }
                                    break;
                                case Vector3 _ when Vector3.up == facetype:
                                    if (piece.transform.localPosition.x == firstPiece.transform.localPosition.x)
                                    {
                                        piece.transform.parent = pivot;
                                    }
                                    break;
                            }
                            break;
                        case RotationType.rotleftright:
                            if (piece.transform.localPosition.z == firstPiece.transform.localPosition.z)
                            {
                                piece.transform.parent = pivot;
                            }
                            break;
                    }
                }
            }*/
        }
        researched = true;
    }
    public static void ReturnToRubik()
    {
        foreach (Node n in nodes)
        {
            if (n)
            {
                if (n.piece.transform.parent != rubikPivot)
                {
                    n.piece.transform.parent = rubikPivot;
                    n.extractPiece();
                }
                n.piece.transform.position = PiecePositionCorrection(n.piece.transform.position);
            }
        }
        researched = false;
    }
    static Vector3 PiecePositionCorrection(Vector3 pos)
    {
        return new Vector3(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z));
    }
}
