using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Camera))]
public class WorldsSwitchMask : MonoBehaviour
{

    [SerializeField]
    private Shader _worldsSwitchMaskShader;

    [SerializeField]
    private string _keyword;
    
    private Camera _camera;

    private RenderTexture _worldMask;


    public RenderTexture WorldMask => _worldMask;


    private void Awake()
    {
        _camera = GetComponent<Camera>();
        
        _worldMask = new RenderTexture(_camera.pixelWidth, _camera.pixelHeight, 0, RenderTextureFormat.RG16);
        _camera.targetTexture = _worldMask;
        _camera.SetReplacementShader(_worldsSwitchMaskShader, string.Empty);
    }


    private void OnPreRender()
    {
        Shader.EnableKeyword(_keyword);
    }

    private void OnPostRender()
    {
        Shader.DisableKeyword(_keyword);
    }
    
}
