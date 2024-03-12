using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace AR.ARKit.FloorScanning
{
    public class ArKitPointCloudController : MonoBehaviour
    {
        public ARPointCloudManager arPointCloudManager;

        public void TogglePointCloudDetection(bool state)
        {
            arPointCloudManager.enabled = state;
            SetAllPlanesActive(state);
        }
        private void SetAllPlanesActive(bool value)
        {
            foreach (var point in arPointCloudManager.trackables)
                point.gameObject.SetActive(value);
        }
    }
}
