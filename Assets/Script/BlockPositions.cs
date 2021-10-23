using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class BlockPositions : MonoBehaviour
{
    public static Action<PieceInfo> OnSetBlockPosition;
    public static PieceInfo[,,] pieces = new PieceInfo[3,3,3];
    public static Transform rubikPivot;
    private static bool researched;
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
    void CollectPieceInfo(PieceInfo info)
    {
        pieces[(int)info.positionInCubeID.x,(int)info.positionInCubeID.y,(int)info.positionInCubeID.z] = info;
    }
    public static void SetInPivot(PieceInfo firstPiece, Transform pivot, RotationType type,Vector3 facetype)
    {
        if (!researched)
        {
            foreach (PieceInfo piece in pieces)
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
            }
        }
        researched = true;
    }
    public static void ReturnToRubik()
    {
        foreach (PieceInfo piece in pieces)
        {
            if (piece)
            {
                piece.transform.parent = rubikPivot;
                piece.transform.position = PiecePositionCorrection(piece.transform.position);
            }
        }
        researched = false;
    }
    static Vector3 PiecePositionCorrection(Vector3 pos)
    {
        return new Vector3(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z));
    }
}
