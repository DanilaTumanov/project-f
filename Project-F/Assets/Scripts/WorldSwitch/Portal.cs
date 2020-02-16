using System.Collections;
using System.Collections.Generic;
using Core.Camera;
using UnityEngine;

public class Portal : MonoBehaviour
{

    private int _index;
    private float _radius;

    private int _worldSwitchOrigin;
    private int _worldSwitchRadius;
    

    public void Awake()
    {
        _worldSwitchOrigin = PortalSystem.PortalsOrigin;
        _worldSwitchRadius = PortalSystem.PortalsRadius;
    }

    public void SetRadius(float radius)
    {
        _radius = radius;

        Vector4[] portalsOrigin = Shader.GetGlobalVectorArray(_worldSwitchOrigin);
        float[] portalsRadius = Shader.GetGlobalFloatArray(_worldSwitchRadius);

        portalsOrigin[_index] = transform.position;
        portalsRadius[_index] = _radius;
        
        Shader.SetGlobalVectorArray(_worldSwitchOrigin, portalsOrigin);
        Shader.SetGlobalFloatArray(_worldSwitchRadius, portalsRadius);
    }


    public void SetIndex(int index)
    {
        _index = index;
    }
    
}
