using System;
using AR.ARCore;
using AR.ARCore.Marker;
using AR.ARKit;
using AR.ARKit.FloorScanning;
using AR.ARKit.Marker;
using Assets.Scripts.Common;
using GoogleARCore;
using GoogleARCore.Examples.ObjectManipulation;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace AR
{
    public class ArManager : Singleton<ArManager>
    {
        [Header("AR Settings")]
        private bool m_IsFloorScanning;
        public bool IsFloorScanning
        {
            get => m_IsFloorScanning;
            set
            {
                m_IsFloorScanning = value;

#if UNITY_ANDROID
                arCoreMarkerSection.SetActive(!m_IsFloorScanning);
                arCoreFloorScanningSection.SetActive(m_IsFloorScanning);
                arCoreSession.DeviceCameraDirection = DeviceCameraDirection.FrontFacing;
                arCoreSession.SessionConfig = m_IsFloorScanning ? floorScanningConfig : markerConfig;
                // Resets image indexes
                if(m_IsFloorScanning)
                    arCoreMarkerController.DeletePlacedObjects();
                arCoreSession.DeviceCameraDirection = DeviceCameraDirection.BackFacing;
#elif UNITY_IOS
                arKitMarkerController.enabled = !m_IsFloorScanning;
                arKitMarkerManager.enabled = !m_IsFloorScanning;
                arKitPointCloudController.TogglePointCloudDetection(m_IsFloorScanning);
                arKitPlaneController.TogglePlaneDetection(m_IsFloorScanning);
                arKitPlacementManipulator.enabled = m_IsFloorScanning;
#endif
            }
        }
    [Header("Used on Awake Only!")]
        public bool isFloorScanning;

        [Header("AR Components")]
        public GameObject arKitSection;
        public GameObject arCoreSection;

        [Header("Object placement components")]
        public ArCoreObjectPlacementManipulator arCorePlacementManipulator;
        public ArKitObjectPlacementManipulator arKitPlacementManipulator;

        [Header("Object manipulation systems")] 
        public ArCoreManipulationSystem arCoreManipulationSystem;
        public ArKitManipulationSystem arKitManipulationSystem;

        [Header("Object marker components")] 
        public ArCoreMarkerController arCoreMarkerController;
        public ArKitMarkerController arKitMarkerController;
        public ARTrackedImageManager arKitMarkerManager;

        [Header("ARKit components Section")] 
        public ArKitPointCloudController arKitPointCloudController;
        public ArKitPlaneDetectionController arKitPlaneController;

        [Header("ARCore components Section")] 
        public GameObject arCoreMarkerSection;
        public GameObject arCoreFloorScanningSection;

        [Header("ARCore Session Configs")] 
        public ARCoreSession arCoreSession;
        public ARCoreSessionConfig floorScanningConfig;
        public ARCoreSessionConfig markerConfig;

        protected override void Awake()
        {
            base.Awake();

            IsFloorScanning = isFloorScanning;
#if UNITY_ANDROID
            arKitSection.SetActive(false);
            arCoreSection.SetActive(true);
#elif UNITY_IOS
            arKitSection.SetActive(true);
            arCoreSection.SetActive(false);
#endif
        }

        public void SetMatchFrameRate(bool state)
        {
            // Keep turned on when scanning for floors or markers
            arCoreSession.SessionConfig.MatchCameraFramerate = state;
        }

        public void SetObjectToInstantiate(GameObject prefab)
        {
#if UNITY_ANDROID
            arCorePlacementManipulator.placedPrefab = prefab;
            arCoreMarkerController.prefab = prefab;
#elif UNITY_IOS
            arKitPlacementManipulator.placedPrefab = prefab;
            arKitMarkerController.prefab = prefab;
#endif

        }

        public void SetPlacedObjectVisibility(bool state)
        {
#if UNITY_ANDROID
            arCorePlacementManipulator.SetVisibility(state);
            arCoreMarkerController.SetVisibility(state);
#elif UNITY_IOS
            arKitPlacementManipulator.SetVisibility(state);
            arKitMarkerController.SetVisibility(state);
#endif
        }

        public void DeletePlacedObjects()
        {
#if UNITY_ANDROID
            arCorePlacementManipulator.DeletePlacedObjects();
            // Marker deletion not really working // Maybe not even necessary
            arCoreMarkerController.DeletePlacedObjects();
#elif UNITY_IOS
            arKitPlacementManipulator.DeletePlacedObjects();
#endif
        }

        public void DeleteSelectedObject()
        {
#if UNITY_ANDROID
            arCoreManipulationSystem.Delete();
#elif UNITY_IOS
            arKitManipulationSystem.Delete();
#endif
        }
    }
}
