using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace AR.ARKit.Marker
{
    public class ArKitMarkerController : MonoBehaviour
    {
        public Camera mainCamera;
        public ARTrackedImageManager trackedImageManager;
        public ARRaycastManager rayCastManager;
        public ArKitManipulationSystem manipulationSystem;

        [Header("Overlay when detecting markers")]
        public GameObject fitScanToOverlay;

        [Header("Prefabs to Instantiate")]
        public GameObject prefab;
        public GameObject selectionPrefab;

        [Header("Instantiated object animator")]
        public RuntimeAnimatorController runtimeAnimatorController;

        [Header("Instantiated objects")]
        public List<GameObject> placedObjects = new List<GameObject>();

        private void OnEnable()
        {
            trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
        }
        private void OnDisable()
        {
            trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
            fitScanToOverlay.SetActive(false);
        }

        private void UpdateInfo(ARTrackedImage trackedImage)
        {
            if (trackedImage.trackingState != TrackingState.None)
            {
                //trackedImage.gameObject.SetActive(true);
                //planeGo.SetActive(true);

                //// The image extents is only valid when the image is being tracked
                //trackedImage.transform.localScale = new Vector3(trackedImage.size.x, 1f, trackedImage.size.y);
                //
                //// Set the texture
                //var material = planeGo.GetComponentInChildren<MeshRenderer>().material;
                //material.mainTexture = (trackedImage.referenceImage.texture == null) ? defaultTexture : trackedImage.referenceImage.texture;
            }
            else
            {
                //trackedImage.gameObject.SetActive(false);
                //planeGo.SetActive(false);
            }
        }

        private void SetImage(Component trackedImage)
        {
            var placedPrefab = Instantiate(prefab, trackedImage.transform.position, trackedImage.transform.rotation, trackedImage.transform);
            placedPrefab.AddComponent<ArKitObject>();

            // Init object manipulator ( Tracked Image )
            trackedImage.GetComponent<ArKitManipulatorsManager>().ArKitObject = placedPrefab.GetComponent<ArKitObject>();
            trackedImage.GetComponent<ArKitManipulatorsManager>().rayCastManager = rayCastManager;
            trackedImage.GetComponent<ArKitManipulatorsManager>().mainCamera = mainCamera;

            // Instantiate object selected visual queue ( circle under object )
            var selectionVisualization = Instantiate(selectionPrefab, placedPrefab.transform, true);
            selectionVisualization.transform.localPosition = Vector3.zero;
            selectionVisualization.transform.localScale = placedPrefab.transform.localScale;
            selectionVisualization.transform.localRotation = Quaternion.Euler(0, 0, 0);

            // Init prefab components
            placedPrefab.GetComponent<ArKitObject>().Init(trackedImage.GetComponent<ArKitManipulatorsManager>(), selectionVisualization, runtimeAnimatorController);

            // Set object selected
            manipulationSystem.Select(placedPrefab.GetComponent<ArKitObject>());

            // Add to list
            placedObjects.Add(trackedImage.gameObject);
        }

        public void SetVisibility(bool state)
        {
            foreach (var o in placedObjects)
            {
                if (o == null)
                    continue;

                o.gameObject.SetActive(state);
            }
        }

        private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
        {
            foreach (var trackedImage in eventArgs.added)
                SetImage(trackedImage);

            foreach (var trackedImage in eventArgs.updated)
                UpdateInfo(trackedImage);

            foreach (var trackedImage in eventArgs.updated)
                if (trackedImage.trackingState == TrackingState.Tracking)
                {
                    fitScanToOverlay.SetActive(false);
                    return;
                }

            fitScanToOverlay.SetActive(true);
        }
    }
}