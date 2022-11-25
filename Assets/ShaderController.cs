using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderController : MonoBehaviour
{
    [Range(-5, +5)][SerializeField] private float X_Axis;
    [Range(-5, +5)][SerializeField] private float Y_Axis;
    [SerializeField] private Material[] _enviromentMaterials;

    private void Update() {
        foreach (var material in _enviromentMaterials) {
            material.SetFloat(Shader.PropertyToID(nameof(X_Axis)), X_Axis);
            material.SetFloat(Shader.PropertyToID(nameof(Y_Axis)), Y_Axis);
        }
    }
}
