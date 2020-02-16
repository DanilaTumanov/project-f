using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PostprocessingWorldCapture : MonoBehaviour
{

    private Camera _camera;

    public RenderTexture Texture => _camera.activeTexture;

    public RenderTexture ColorBuffer { get; private set; }
    public RenderTexture DepthBuffer { get; private set; }
    
    private void Awake()
    {
        _camera = GetComponent<Camera>();
        
        _camera.depthTextureMode = _camera.depthTextureMode | DepthTextureMode.Depth;
        //_camera.targetTexture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
        
        ColorBuffer = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.Default);
        DepthBuffer = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.Depth);
 
        _camera.SetTargetBuffers(ColorBuffer.colorBuffer, DepthBuffer.depthBuffer);
    }
    
}
