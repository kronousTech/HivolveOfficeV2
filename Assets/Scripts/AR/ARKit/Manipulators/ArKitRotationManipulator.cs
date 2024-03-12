using UnityEngine;

namespace AR.ARKit.Manipulators
{
    public class ArKitRotationManipulator : ArKitManipulator
    {
        private const float PinchTurnRatio = Mathf.PI / 2;
        private const float MinTurnAngle = 0.75f;

        private static float s_TurnAngleDelta;
        private static float s_TurnAngle;

        public bool isRotating;

        public float triggerAngle;

        private void LateUpdate()
        {
            if (!arKitObject.IsSelected) 
                return;

            var desiredRotation = arKitObject.transform.rotation;

            Calculate();

            if (Mathf.Abs(s_TurnAngleDelta) > 0)
            { // rotate
                var rotationDeg = Vector3.zero;
                rotationDeg.y = -s_TurnAngleDelta;
                desiredRotation *= Quaternion.Euler(rotationDeg);
            }

            arKitObject.transform.rotation = desiredRotation;
        }

        private void Calculate()
        {
            s_TurnAngle = s_TurnAngleDelta = 0;

            // if two fingers are touching the screen at the same time ...
            if (Input.touchCount == 2 && !arKitObject.IsScaling)
            {
                Touch touch1 = Input.touches[0];
                Touch touch2 = Input.touches[1];

                // ... if at least one of them moved ...
                if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
                {
                    // ... or check the delta angle between them ...
                    s_TurnAngle = Angle(touch1.position, touch2.position);
                    float prevTurn = Angle(touch1.position - touch1.deltaPosition,
                        touch2.position - touch2.deltaPosition);
                    s_TurnAngleDelta = Mathf.DeltaAngle(prevTurn, s_TurnAngle);

                    // ... if it's greater than a minimum threshold, it's a turn!
                    if (!isRotating && Mathf.Abs(s_TurnAngleDelta) > triggerAngle)
                    {
                        isRotating = true;
                    }
                    if (isRotating && Mathf.Abs(s_TurnAngleDelta) > MinTurnAngle)
                    {
                        s_TurnAngleDelta *= PinchTurnRatio;
                    }
                    else
                    {
                        s_TurnAngle = s_TurnAngleDelta = 0;
                    }
                }
            }
            else isRotating = false;
        }
        private static float Angle(Vector2 pos1, Vector2 pos2)
        {
            Vector2 from = pos2 - pos1;
            Vector2 to = new Vector2(1, 0);

            float result = Vector2.Angle(from, to);
            Vector3 cross = Vector3.Cross(from, to);

            if (cross.z > 0)
            {
                result = 360f - result;
            }

            return result;
        }

        public override void UpdateManipulator()
        {
            // Uses late update Instead
        }
    }
}