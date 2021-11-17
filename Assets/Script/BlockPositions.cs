using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class BlockPositions : MonoBehaviour
{
    public static Action<Node> OnSetBlockPosition;
    public static Node[,,] nodes = new Node[3,3,3];
    public static Transform rubikPivot;
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
    public static void SetInPivot(Transform firstPiece, Transform pivot, RotationType type,Vector3 facetype)
    {
        if (!researched)
        {
            Vector3 ID = Vector3.zero;
            foreach (Node n in nodes)
            {
                if(n)
                {
                    if(n.piece == firstPiece)
                    {
                        ID = n.ID;
                    }
                }
            }
            foreach (Node n in nodes)
            {
                if (n)
                {
                    if (facetype == Vector3.forward)
                    {
                        if (type == RotationType.leftright)
                        {
                            if (n.ID.y == ID.y)
                            {
                                n.piece.parent = pivot;
                            }
                        }
                        if (type == RotationType.updown)
                        {
                            if (n.ID.x == ID.x)
                            {
                                n.piece.parent = pivot;
                            }
                        }
                        if (type == RotationType.rotleftright)
                        {
                            if (n.ID.z == ID.z)
                            {
                                n.piece.parent = pivot;
                            }
                        }
                    }
                    if (facetype == Vector3.right)
                    {
                        if (type == RotationType.leftright)
                        {
                            if (n.ID.y == ID.y)
                            {
                                n.piece.parent = pivot;
                            }
                        }
                        if (type == RotationType.updown)
                        {

                            if (n.ID.z == ID.z)
                            {
                                n.piece.parent = pivot;
                            }
                        }
                        if (type == RotationType.rotleftright)
                        {
                            if (n.ID.x == ID.x)
                            {
                                n.piece.parent = pivot;
                            }
                        }
                    }
                    if (facetype == Vector3.up)
                    {
                        if (type == RotationType.leftright)
                        {
                            if (n.ID.z == ID.z)
                            {
                                n.piece.parent = pivot;
                            }
                        }
                        if (type == RotationType.updown)
                        {
                            if (n.ID.x == ID.x)
                            {
                                n.piece.parent = pivot;
                            }
                        }
                        if (type == RotationType.rotleftright)
                        {
                            if (n.ID.z == ID.z)
                            {
                                n.piece.parent = pivot;
                            }
                        }
                    }
                }
            }
        }
        researched = true;
    }
    public static void ReturnToRubik()
    {
        foreach (Node n in nodes)
        {
            if (n)
            {
                if (n.piece.parent != rubikPivot)
                {
                    n.piece.parent = rubikPivot;
                    n.piece.position  = PiecePositionCorrection(n.piece.transform.position);
                }
            }
        }
        researched = false;
    }
    static Vector3 PiecePositionCorrection(Vector3 pos)
    {
        return new Vector3(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z));
    }
}
