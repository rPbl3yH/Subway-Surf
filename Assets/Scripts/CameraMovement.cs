using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private float _speedCam;

    void Update()
    {
        
    }

    private void LateUpdate() {
        float newY = Mathf.Lerp(transform.position.y, _playerTransform.position.y, _speedCam);
        transform.position = new Vector3(transform.position.x, newY, _playerTransform.position.z);
    }
}
