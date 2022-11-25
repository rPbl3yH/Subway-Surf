using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioSource _audioPickUp;
    [SerializeField] private AudioSource _audioBackground;
    
    void Start()
    {
        _audioBackground.Play();
        GameManager.Instance.EventManager.OnCoinPickedUp += OnCoinPickedUp;
    }

    private void OnCoinPickedUp() {
        _audioPickUp.Play();
    }
}
