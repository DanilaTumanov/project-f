using System;
using UnityEngine;

namespace Core.Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class WorldSwitchCamera : MonoBehaviour
    {


        [SerializeField]
        private Transform _switchCenter;

        [SerializeField]
        [Range(0, 50)]
        private float _radius;

        [SerializeField]
        private WorldMaterialsData[] _worldsMaterials;


        private UnityEngine.Camera _camera;
    
        private static readonly int WorldSwitchOrigin = Shader.PropertyToID("_WorldSwitchOrigin");
        private static readonly int WorldSwitchRadius = Shader.PropertyToID("_WorldSwitchRadius");
        private static readonly int IsMainWorld = Shader.PropertyToID("_IsMainWorld");


        private void Awake()
        {
            _camera = GetComponent<UnityEngine.Camera>();
            
            Array.ForEach(_worldsMaterials[0].materials, mat => mat.SetFloat(IsMainWorld, 1));
            Array.ForEach(_worldsMaterials[1].materials, mat => mat.SetFloat(IsMainWorld, 0));
            //_camera.depthTextureMode = _camera.depthTextureMode | DepthTextureMode.Depth;
        }


        private void Update()
        {
            Shader.SetGlobalVector(WorldSwitchOrigin, _switchCenter.position);
            Shader.SetGlobalFloat(WorldSwitchRadius, _radius);
        }
    }


    [Serializable]
    public struct WorldMaterialsData
    {
        public string name;
        public Material[] materials;
    }
    
}
