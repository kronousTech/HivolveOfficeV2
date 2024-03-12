using UnityEngine;

namespace AR.ARKit
{
    public class ArKitObject : MonoBehaviour
    {
        public ArKitManipulatorsManager manager;
        public GameObject selectionVisualization;
        public Animator animator;

        public bool IsScaling => manager.scaleManipulator.isScaling;
        public bool IsRotating => manager.rotationManipulator.isRotating;

        public void Init(ArKitManipulatorsManager manager, GameObject selectionVisualization,
            RuntimeAnimatorController runtimeAnimatorController)
        {
            this.manager = manager;
            this.selectionVisualization = selectionVisualization;
            this.animator = gameObject.AddComponent<Animator>();
            this.animator.runtimeAnimatorController = runtimeAnimatorController;
        }

        public bool IsSelected 
        {
            get => manager.selectionManipulator.IsSelected;
            set => manager.selectionManipulator.IsSelected = value;
        }
    }
}