using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RendererController : MonoBehaviour
{
    private Renderer _renderer;

    private void Start() {
        _renderer = GetComponent<Renderer>();
        var bounds = _renderer.bounds;
        bounds.extents *= 300;
        _renderer.bounds = bounds;
    }

    // Draws a wireframe box around the selected object,
    // indicating world space bounding volume.
    public void OnDrawGizmosSelected() {
        
        
    }
    
}
