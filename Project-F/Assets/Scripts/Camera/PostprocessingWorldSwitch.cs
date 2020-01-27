using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PostprocessingWorldSwitch : MonoBehaviour
{

    [SerializeField]
    private PostprocessingWorldCapture _worldCapture;

    [SerializeField]
    private Material _worldSwitchMat;

    [SerializeField]
    private Transform _switchCenter;

    
    private Camera _camera;
    
    private static readonly int SecondWorld = Shader.PropertyToID("_SecondWorld");
    private static readonly int SecondWorldDepth = Shader.PropertyToID("_SecondWorldDepth");
    private static readonly int InverseRotationMatrix = Shader.PropertyToID("_InverseRotationMatrix");
    private static readonly int OriginViewPos = Shader.PropertyToID("_OriginViewPos");


    private void Awake()
    {
        _camera = GetComponent<Camera>();
        
        _camera.depthTextureMode = _camera.depthTextureMode | DepthTextureMode.Depth;
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        var inverseRotationMatrix = Matrix4x4.TRS(
            Vector3.zero,
            transform.rotation,
            Vector3.one
        );

        var originViewPos = transform.InverseTransformVector(_switchCenter.position - transform.position);//Vector3.Dot(_switchCenter.position - transform.position, transform.forward);
        
        _worldSwitchMat.SetTexture(SecondWorld, _worldCapture.ColorBuffer);
        _worldSwitchMat.SetTexture(SecondWorldDepth, _worldCapture.DepthBuffer);
        _worldSwitchMat.SetMatrix(InverseRotationMatrix, inverseRotationMatrix);
        _worldSwitchMat.SetVector(OriginViewPos, originViewPos);
        
        Graphics.Blit(src, dest, _worldSwitchMat);
    }
    
}
