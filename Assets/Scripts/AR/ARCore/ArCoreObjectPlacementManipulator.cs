//-----------------------------------------------------------------------
// <copyright file="PawnManipulator.cs" company="Google">
//
// Copyright 2019 Google LLC. All Rights Reserved.
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

using System.Collections.Generic;
using GoogleARCore;
using GoogleARCore.Examples.ObjectManipulation;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AR.ARCore
{
    public class ArCoreObjectPlacementManipulator : Manipulator
    {
        public Camera firstPersonCamera;

        [Header("Instantiated object animator")]
        public RuntimeAnimatorController runtimeAnimatorController;

        [Header("Prefabs to Instantiate")]
        public GameObject placedPrefab;
        public GameObject manipulatorPrefab;

        public List<GameObject> placedObjects;

        private void Awake()
        {
            firstPersonCamera = Camera.main;
        }

        protected override bool CanStartManipulationForGesture(TapGesture gesture)
        {
            if (gesture.TargetObject == null)
            {
                return true;
            }

            return false;
        }

        protected override void OnEndManipulation(TapGesture gesture)
        {
            if (gesture.WasCancelled)
            {
                return;
            }

            // If gesture is targeting an existing object we are done.
            if (gesture.TargetObject != null)
            {
                return;
            }

            // BUGGED - NO FUCKING IDEIA WHY
            //if (IsPointerOverUiObject(gesture))
            //    return;

            // Raycast against the location the player touched to search for planes.
            TrackableHit hit;
            TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon;

            if (Frame.Raycast(
                gesture.StartPosition.x, gesture.StartPosition.y, raycastFilter, out hit))
            {
                // Use hit pose and camera pose to check if hittest is from the
                // back of the plane, if it is, no need to create the anchor.
                if ((hit.Trackable is DetectedPlane) &&
                    Vector3.Dot(firstPersonCamera.transform.position - hit.Pose.position,
                        hit.Pose.rotation * Vector3.up) < 0)
                {
                    Debug.Log("Hit at back of the current DetectedPlane");
                }
                else
                {
                    // Instantiate game object at the hit pose.
                    var prefab = Instantiate(placedPrefab, hit.Pose.position, hit.Pose.rotation);

                    // Add object animator
                    prefab.AddComponent<Animator>();
                    prefab.GetComponent<Animator>().runtimeAnimatorController = runtimeAnimatorController;

                    // Instantiate manipulator.
                    var manipulator = Instantiate(manipulatorPrefab, hit.Pose.position, hit.Pose.rotation);

                    // Make game object a child of the manipulator.
                    prefab.transform.parent = manipulator.transform;

                    // Create an anchor to allow ARCore to track the hitpoint as understanding of
                    // the physical world evolves.
                    var anchor = hit.Trackable.CreateAnchor(hit.Pose);

                    // Set whole object child of this
                    anchor.transform.parent = transform;

                    // Make manipulator a child of the anchor.
                    manipulator.transform.parent = anchor.transform;

                    // Select the placed object.
                    manipulator.GetComponent<Manipulator>().Select();

                    // Set manipulator attached object
                    manipulator.GetComponent<Manipulator>().placedObject = prefab;

                    placedObjects.Add(manipulator);
                }
            }
        }

        public void SetVisibility(bool state)
        {
            foreach (var o in placedObjects)
            {
                if (o == null) 
                    continue;

                o.SetActive(state);
                if(!state)
                    o.GetComponent<Manipulator>().Deselect();
            }
        }
        public void DeletePlacedObjects()
        {
            foreach (var o in placedObjects)
            {
                if (o == null)
                    continue;

                o.GetComponent<Manipulator>().Deselect();

                // Deletes Anchor
                Destroy(o.transform.parent.gameObject);
            }

            // Resets List
            placedObjects = new List<GameObject>();
        }

        private static bool IsPointerOverUiObject(TapGesture gesture)
        {
            // Referencing this code for GraphicRaycaster https://gist.github.com/stramit/ead7ca1f432f3c0f181f
            // the ray cast appears to require only eventData.position.
            var eventDataCurrentPosition = new PointerEventData(EventSystem.current)
            {
                position = new Vector2(gesture.StartPosition.x, gesture.StartPosition.y)
            };

            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }
    }
}
