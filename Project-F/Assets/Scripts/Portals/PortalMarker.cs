using System;
using UnityEngine;

namespace Portals
{
    public class PortalMarker : MonoBehaviour
    {
        [SerializeField] private float _portalThrowRadius = 5f;

        public float ThrowRadius => _portalThrowRadius;
        
        private bool _isAvailable;

        private void Awake()
        {
            SetAvailable(false);
        }

        public void SetAvailable(bool isAvailable)
        {
            _isAvailable = isAvailable;
            gameObject.SetActive(_isAvailable);
        }

        public void SetPosition(Vector3 playerPosition, Vector3 markerPosition)
        {
            if (Vector3.Distance(playerPosition, markerPosition) <= _portalThrowRadius)
            {
                transform.position = markerPosition;
            }
        }
    }
}