using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PostprocessingWorldSwitch : MonoBehaviour
{

    [SerializeField]
    private TextureRenderCamera _firstWorldCullingCamera;
    
    [SerializeField]
    private TextureRenderCamera _secondWorldCullingCamera;
    
    [SerializeField]
    private PostprocessingWorldCapture _worldCapture;

    [SerializeField]
    private Material _worldSwitchMat;

    [SerializeField]
    private Transform _switchCenter;

    [SerializeField]
    private WorldSwitchHideTrigger _worldSwitchHideTrigger;

    [SerializeField]
    [Range(0, 15)]
    private float _radius;


    private Camera _camera;
    
    private static readonly int SecondWorld = Shader.PropertyToID("_SecondWorld");
    private static readonly int SecondWorldDepth = Shader.PropertyToID("_SecondWorldDepth");
    private static readonly int InverseRotationMatrix = Shader.PropertyToID("_InverseRotationMatrix");
    private static readonly int OriginViewPos = Shader.PropertyToID("_OriginViewPos");
    private static readonly int WorldSwitchOrigin = Shader.PropertyToID("_WorldSwitchOrigin");
    private static readonly int WorldSwitchRadius = Shader.PropertyToID("_WorldSwitchRadius");
    private static readonly int FirstWorldCulling = Shader.PropertyToID("_FirstWorldCulling");
    private static readonly int SecondWorldCulling = Shader.PropertyToID("_SecondWorldCulling");


    private void Awake()
    {
        _camera = GetComponent<Camera>();
        
        _camera.depthTextureMode = _camera.depthTextureMode | DepthTextureMode.Depth;
    }

    private void Update()
    {
        _worldSwitchHideTrigger.SetRadius(_radius);
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        var inverseRotationMatrix = Matrix4x4.TRS(
            Vector3.zero,
            transform.rotation,
            Vector3.one
        );

        var originViewPos = transform.InverseTransformVector(_switchCenter.position - transform.position);//Vector3.Dot(_switchCenter.position - transform.position, transform.forward);
        
        Shader.SetGlobalVector(WorldSwitchOrigin, _switchCenter.position);
        Shader.SetGlobalFloat(WorldSwitchRadius, _radius);
        
        _worldSwitchMat.SetTexture(SecondWorld, _worldCapture.ColorBuffer);
        _worldSwitchMat.SetTexture(SecondWorldDepth, _worldCapture.DepthBuffer);
        _worldSwitchMat.SetTexture(FirstWorldCulling, _firstWorldCullingCamera.Texture);
        _worldSwitchMat.SetTexture(SecondWorldCulling, _secondWorldCullingCamera.Texture);
        _worldSwitchMat.SetMatrix(InverseRotationMatrix, inverseRotationMatrix);
        _worldSwitchMat.SetVector(OriginViewPos, originViewPos);
        
        Graphics.Blit(src, dest, _worldSwitchMat);
        
    }
    
}
