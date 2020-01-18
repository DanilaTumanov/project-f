using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class PlayerHandling : MonoBehaviour
{

    [SerializeField]
    private float _movementSpeed = 10;


    private Rigidbody _rb;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }


    private void Update()
    {
        UpdatePosition();
    }


    private Vector3 GetNormalMovementVector()
    {
        return new Vector3(
            Input.GetAxis("Horizontal"),
            0,
            Input.GetAxis("Vertical")
        ).normalized;
    }
    
    
    private void UpdatePosition()
    {
        _rb.transform.Translate(_movementSpeed * Time.deltaTime * GetNormalMovementVector());
    }
}
