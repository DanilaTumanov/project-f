using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraBehaviour : MonoBehaviour
{

    [SerializeField] private Camera _mainCamera;
    [SerializeField] private ButtonControlView _switchCameraView;
    
    [SerializeField] private Player _player;
    [SerializeField] private Vector3 _position;
    
    private Quaternion _rotation;

    private void Awake()
    {
        UpdatePosition();
        UpdateRotation();

        if (_switchCameraView != null)
        {
            _switchCameraView.OnButtonDown += CameraSwitchHandler;
        }
    }

    private void LateUpdate()
    {
        UpdatePosition();
        
        #if UNITY_EDITOR
        UpdateRotation();
        #endif
    }


    private void UpdatePosition()
    {
        transform.position = _player.transform.position + _position;
    }


    private void UpdateRotation()
    {
        transform.LookAt(_player.transform);
    }

    private void CameraSwitchHandler()
    {
        if (_mainCamera != null)
        {
            _mainCamera.orthographic = !_mainCamera.orthographic;
        }
    }
    
}
