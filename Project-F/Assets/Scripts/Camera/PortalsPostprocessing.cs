using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PortalsPostprocessing : MonoBehaviour
{

    [SerializeField] private Material _postprocessingMaterial;

    private Camera _camera;


    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }


    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, _postprocessingMaterial);
    }
}
