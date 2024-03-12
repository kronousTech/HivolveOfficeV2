using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace AR.ARKit.Manipulators
{
    public class ArKitTranslationManipulator : ArKitManipulator
    {
        private static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

        private bool m_CanMove;

        public override void UpdateManipulator()
        {
            if (Input.touchCount != 1)
                return;

            var touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began when IsPointerOverUiElement(touch.position):
                    m_CanMove = false;
                    return;
                case TouchPhase.Began:
                {
                    var ray = manager.mainCamera.ScreenPointToRay(touch.position);
                    if (Physics.Raycast(ray, out var hit))
                        m_CanMove = arKitObject == hit.transform.GetComponent<ArKitObject>();
                    else
                        m_CanMove = false;
                    break;
                }
                case TouchPhase.Moved when m_CanMove:
                {
                    if (manager.rayCastManager.Raycast(touch.position, s_Hits, TrackableType.PlaneWithinPolygon))
                    {
                        var hitPose = s_Hits[0].pose;

                        arKitObject.transform.position = hitPose.position;
                    }

                    break;
                }
                case TouchPhase.Ended:
                    m_CanMove = false;
                    break;
            }
        }
      
        private static bool IsPointerOverUiElement(Vector2 position)
        {
            var eventData = new PointerEventData(EventSystem.current);
            eventData.position = position;
            var results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);
            return results.Count > 0;
        }
    }
}
