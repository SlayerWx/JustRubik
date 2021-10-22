using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceInfo : MonoBehaviour
{
    public Vector3 positionInCubeID; //left to right,Top to Bot,front to back

    private void Start()
    {
        BlockPositions.OnSetBlockPosition?.Invoke(this);
    }
}
