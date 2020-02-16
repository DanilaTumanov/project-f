using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Camera
{
    
    public class PortalSystem : MonoBehaviour
    {


        [SerializeField] private Transform _switchCenter;

        [SerializeField]
        [Range(0, 50)]
        private float _radius;

        [SerializeField] private WorldMaterialsData[] _worldsMaterials;

        [SerializeField] private Portal _portalPrefab;

        
        
        private List<Portal> _portals;

    
        public static readonly int PortalsOrigin = Shader.PropertyToID("_PortalsOrigin");
        public static readonly int PortalsRadius = Shader.PropertyToID("_PortalsRadius");
        private static readonly int IsMainWorld = Shader.PropertyToID("_IsMainWorld");


        private void Awake()
        {
            _portals = new List<Portal>();
            
            Shader.SetGlobalVectorArray(PortalsOrigin, new Vector4[5]);
            Shader.SetGlobalFloatArray(PortalsRadius, new float[5]);
            
            Array.ForEach(_worldsMaterials[0].materials, mat => mat.SetFloat(IsMainWorld, 1));
            Array.ForEach(_worldsMaterials[1].materials, mat => mat.SetFloat(IsMainWorld, 0));
            //_camera.depthTextureMode = _camera.depthTextureMode | DepthTextureMode.Depth;
            
        }


        private void Update()
        {
            /*Shader.SetGlobalVector(WorldSwitchOrigin, _switchCenter.position);
            Shader.SetGlobalFloat(WorldSwitchRadius, _radius);*/
            if (Input.GetMouseButtonDown(0))
            {
                var ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo, 1000))
                {
                    SetPortal(hitInfo.transform.position);
                }
            }
        }


        public Portal SetPortal(Vector3 position)
        {
            var portal = Instantiate(_portalPrefab, position, Quaternion.identity);
            
            _portals.Add(portal);

            UpdatePortals();
            
            // DEBUG
            portal.SetRadius(5);

            return portal;
        }


        public void RemovePortal(Portal portal)
        {
            _portals.Remove(portal);
            
            UpdatePortals();
        }
        

        private void UpdatePortals()
        {
            var i = 0;
            
            foreach (var portal in _portals)
            {
                portal.SetIndex(i);
                i++;
            }
            
            for (i = 0; i < _portals.Count; i++)
            {
                Shader.DisableKeyword($"PORTAL{i + 1}");
            }

            if (_portals.Count > 0)
            {
                Shader.EnableKeyword($"PORTAL{_portals.Count}");   
            }
        }
    }


    [Serializable]
    public struct WorldMaterialsData
    {
        public string name;
        public Material[] materials;
    }
    
}
