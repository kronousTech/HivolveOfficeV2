using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.ARFoundation;

namespace AR.ARKit.FloorScanning
{
    public class ArKitPlaneDetectionController : MonoBehaviour
    {
        public ARPlaneManager arPlaneManager;

        public void TogglePlaneDetection(bool state)
        {
            arPlaneManager.enabled = state;
            SetAllPlanesActive(state);
        }
        private void SetAllPlanesActive(bool value)
        {
            foreach (var plane in arPlaneManager.trackables)
                plane.gameObject.SetActive(value);
        }
    }
}
