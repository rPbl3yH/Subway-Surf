using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternGround : MonoBehaviour
{
    public float OffsetToNextPatternSpawn;
    public float WidthPattern = 8f;

    private void OnDrawGizmos() {
        var centerPoint = new Vector3(0f, 0.1f, transform.position.z + OffsetToNextPatternSpawn / 2f);
        var size = new Vector3(WidthPattern, 0f, OffsetToNextPatternSpawn);
        Gizmos.DrawWireCube(centerPoint, size);
    }
}
