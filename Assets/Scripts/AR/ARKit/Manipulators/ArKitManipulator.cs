using UnityEngine;

namespace AR.ARKit.Manipulators
{
    public abstract class ArKitManipulator : MonoBehaviour
    {
        [HideInInspector] public ArKitObject arKitObject;
        [HideInInspector] public ArKitManipulatorsManager manager;

        private void Update()
        {
            if(arKitObject.IsSelected)
                UpdateManipulator();
        }
        public abstract void UpdateManipulator();
    }
}