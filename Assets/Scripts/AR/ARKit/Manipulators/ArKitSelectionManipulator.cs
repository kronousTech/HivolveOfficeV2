using UnityEngine;

namespace AR.ARKit.Manipulators
{
    public class ArKitSelectionManipulator : ArKitManipulator
    {
        private bool m_IsSelected;
        private static readonly int Selected = Animator.StringToHash("Selected");

        public bool IsSelected
        {
            get => m_IsSelected;
            set
            {
                m_IsSelected = value;

                if (m_IsSelected)
                {
                    Select();
                }
                else
                {
                    Deselect();
                }
            }
        }
        private GameObject SelectionVisualization => arKitObject.selectionVisualization;
        private Animator Animator => arKitObject.animator;

        // TODO: CONTROL SELECTION VISUALIZATION SIZE BASED ON SCALE MANIPULATOR

        private void Select()
        {
            SelectionVisualization.SetActive(true);
            Animator.SetBool(Selected, true);
        }
        private void Deselect()
        {
            SelectionVisualization.SetActive(false);
            Animator.SetBool(Selected, false);
        }

        public override void UpdateManipulator()
        {
            // No updates on selection manipulator.
            // Is selected is controller in the ArKitManipulatorController.
        }
    }
}