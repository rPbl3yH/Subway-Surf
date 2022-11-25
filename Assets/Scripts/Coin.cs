using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private float _rotateSpeed;
    
    private void Update() {
        transform.Rotate(Vector3.up * _rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other) {
        print("other = " + other);
        var player = other.GetComponentInParent<PlayerController>();
        if(player) { 
            GameManager.Instance.EventManager.CoinPickedUp();
            Destroy(gameObject);
        }
    }
}
