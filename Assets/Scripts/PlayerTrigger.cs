using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    private PlayerMovement _playerMovement;
    private CapsuleCollider _collider;
    private float _defaultHeightCollider;

    void Start()
    {
        _collider = GetComponent<CapsuleCollider>();
        _playerMovement = GetComponentInParent<PlayerMovement>();
        _defaultHeightCollider = _collider.height;
    }

    void Update()
    {
        
    }

    public void SetColliderToRoll(bool isRolling) {
        if (isRolling) {
            _collider.height = _defaultHeightCollider / 2;
            _collider.center = new Vector3(0f, -_collider.height / 2f, 0f);
        }
        else {
            _collider.height = _defaultHeightCollider;
            _collider.center = Vector3.zero;
        }
    }

    

}
