using AR.ARKit.Manipulators;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace AR.ARKit
{
    public class ArKitManipulatorsManager : MonoBehaviour
    {
        [Header("Elements set on ArKitObjectPlacementManipulator")]
        public ARRaycastManager rayCastManager;
        public Camera mainCamera;
        public ArKitObject ArKitObject
        {
            set
            {
                selectionManipulator.arKitObject = value;
                translationManipulator.arKitObject = value;
                rotationManipulator.arKitObject = value;
                scaleManipulator.arKitObject = value;
            }
        }
       
        [Header("Manipulators")]
        public ArKitSelectionManipulator selectionManipulator;
        public ArKitTranslationManipulator translationManipulator;
        public ArKitRotationManipulator rotationManipulator;
        public ArKitScalingManipulator scaleManipulator;

        private void Awake()
        {
            selectionManipulator.manager = this;
            rotationManipulator.manager = this;
            translationManipulator.manager = this;
            scaleManipulator.manager = this;
        }
    }
}