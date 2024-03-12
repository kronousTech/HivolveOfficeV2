//-----------------------------------------------------------------------
// <copyright file="SelectionManipulator.cs" company="Google">
//
// Copyright 2018 Google LLC. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

using UnityEngine.Serialization;

namespace GoogleARCore.Examples.ObjectManipulation
{
    using UnityEngine;

    /// <summary>
    /// Controls the selection of an object through Tap gesture.
    /// Updates objects animator
    /// </summary>
    public class SelectionManipulator : Manipulator
    {
        public GameObject selectionVisualization;
        private Animator Animator => placedObject.GetComponent<Animator>();

        private float m_ScaledElevation;
        private static readonly int Selected = Animator.StringToHash("Selected");

        /// <summary>
        /// Should be called when the object elevation changes, to make sure that the Selection
        /// Visualization remains always at the plane level. This is the elevation that the object
        /// has, independently of the scale.
        /// </summary>
        /// <param name="elevation">The current object's elevation.</param>
        public void OnElevationChanged(float elevation)
        {
            m_ScaledElevation = elevation * transform.localScale.y;
            selectionVisualization.transform.localPosition = new Vector3(0, -elevation, 0);
        }
        /// <summary>
        /// Should be called when the object elevation changes, to make sure that the Selection
        /// Visualization remains always at the plane level. This is the elevation that the object
        /// has multiplied by the local scale in the y coordinate.
        /// </summary>
        /// <param name="scaledElevation">The current object's elevation scaled with the local y
        /// scale.</param>
        public void OnElevationChangedScaled(float scaledElevation)
        {
            m_ScaledElevation = scaledElevation;
            selectionVisualization.transform.localPosition =
                new Vector3(0, -scaledElevation / transform.localScale.y, 0);
        }

        protected override void Update()
        {
            base.Update();
            if (transform.hasChanged)
            {
                float height = -m_ScaledElevation / transform.localScale.y;
                selectionVisualization.transform.localPosition = new Vector3(0, height, 0);
            }
        }

        protected override bool CanStartManipulationForGesture(TapGesture gesture)
        {
            return true;
        }
        protected override void OnEndManipulation(TapGesture gesture)
        {
            if (gesture.WasCancelled)
            {
                return;
            }

            if (ArCoreManipulationSystem.Instance == null)
            {
                return;
            }

            GameObject target = gesture.TargetObject;
            if (target == gameObject)
            {
                Select();
            }

            // Raycast against the location the player touched to search for planes.
            TrackableHit hit;
            TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon;

            if (!Frame.Raycast(
                gesture.StartPosition.x, gesture.StartPosition.y, raycastFilter, out hit))
            {
                Deselect();
            }
        }

        protected override void OnSelected()
        {
            selectionVisualization.SetActive(true);
            Animator.SetBool(Selected, true);
        }
        protected override void OnDeselected()
        {
            selectionVisualization.SetActive(false);
            Animator.SetBool(Selected, false);
        }
    }
}
