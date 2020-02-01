using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Collider))]
public class WorldSwitchRenderer : MonoBehaviour
{

    [SerializeField]
    private GameObject _renderedObject;

    [SerializeField] 
    private int _switchToLayer;
    
    private int _defautLayer;


    private Collider _collider;

    
    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _collider.isTrigger = true;

        _defautLayer = gameObject.layer;
        
        SetLayerRecursively(gameObject, _defautLayer);
    }


    private void OnTriggerEnter(Collider other)
    {
        SetLayerRecursively(gameObject, _switchToLayer);
    }

    
    private void OnTriggerExit(Collider other)
    {
        SetLayerRecursively(gameObject, _defautLayer);
    }
    

    private void SetLayerRecursively(GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform child in go.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}
