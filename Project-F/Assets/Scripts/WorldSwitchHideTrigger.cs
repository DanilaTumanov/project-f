using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class WorldSwitchHideTrigger : MonoBehaviour
{

    private CapsuleCollider _collider;

    
    private void Awake()
    {
        _collider = GetComponent<CapsuleCollider>();
        _collider.radius = 0;
    }


    public void SetRadius(float radius)
    {
        _collider.radius = radius;
    }

}
