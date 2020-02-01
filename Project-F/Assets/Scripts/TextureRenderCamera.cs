using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureRenderCamera : MonoBehaviour
{

    private Camera _camera;


    public RenderTexture Texture => _camera.activeTexture;
    
    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _camera.targetTexture = new RenderTexture(_camera.pixelWidth, _camera.pixelHeight, 0, RenderTextureFormat.ARGB32);
    }
    
}
