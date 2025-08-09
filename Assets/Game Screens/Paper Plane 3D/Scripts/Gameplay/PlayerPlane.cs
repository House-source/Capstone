using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Plane.Gameplay
{
    public class PlayerPlane : MonoBehaviour
    {
        // Current rotation angles for tilt (x = roll, y = pitch)
        public Vector2 m_Angle = Vector2.zero;

        // Reference to the visual part of the plane to rotate
        public Transform m_Base;

        // Speed of turning (currently unused in this script)
        Vector2 m_TurnSpeed = Vector2.zero;

        // Explosion particle prefab to instantiate on collision
        public GameObject m_ExplodeParticle;

        // Static reference to the current PlayerPlane instance (singleton-style)
        public static PlayerPlane m_Main;

        // Offset used to "zero" the accelerometer input during calibration
        private Vector3 m_CalibrationOffset = Vector3.zero;

        // Called when the script instance is being loaded
        private void Awake()
        {
            // Store this instance in the static variable for global access
            m_Main = this;
        }

        // Called before the first frame update
        void Start()
        {
            // Automatically calibrate the accelerometer at the start
            Calibrate();
        }

       
        // Calibrates the current phone orientation as the neutral (zero) input.
        // Useful to adjust for how the player is holding the phone.
        public void Calibrate()
        {
            m_CalibrationOffset = Input.acceleration;
        }

        // Called once per frame
        void Update()
        {
            // Get the raw accelerometer input from the device
            Vector3 rawAccel = Input.acceleration;

            // Adjust it based on the initial calibration
            Vector3 adjustedAccel = rawAccel - m_CalibrationOffset;

            // Map accelerometer input to movement on the X (sideways) and Y (up/down) axes
            // Multipliers control sensitivity
            // Clamp the result to ensure it stays within range to prevent excessive input
            float InputX = Mathf.Clamp(adjustedAccel.x * 2.0f, -1.0f, 1.0f); // X-axis input (side-to-side tilt)
            float InputY = Mathf.Clamp(adjustedAccel.z * 4.0f, -1.0f, 1.0f); //  Z-axis input (forward-backward tilt)

            // Create movement vector
            Vector3 movement = new Vector3(InputX, InputY, 0);

            // Smoothly interpolate (lerp) the angle based on input for more fluid rotation
            m_Angle.x = Mathf.Lerp(m_Angle.x, 60.0f * InputX, 5 * Time.deltaTime); // roll
            m_Angle.y = Mathf.Lerp(m_Angle.y, 20.0f * InputY, 5 * Time.deltaTime); // pitch

            // Apply rotation to the base of the plane using Euler angles
            m_Base.localRotation = Quaternion.Euler(-1f * m_Angle.y, 0, -m_Angle.x);

            // Apply movement to the plane’s position
            transform.position += movement;

            // Clamp the plane's position to keep it within bounds
            Vector3 pos = transform.position;
            pos.y = Mathf.Clamp(pos.y, 8, 30);     // Keep within vertical range
            pos.x = Mathf.Clamp(pos.x, -18, 18);   // Keep within horizontal range
            pos.z = 0;                             // Keep Z fixed
            transform.position = pos;

            // Check for collisions using a sphere around the plane
            Collider[] hits = Physics.OverlapSphere(transform.position, 2.5f);
            foreach (Collider hit in hits)
            {
                // Ignore collision with self
                if (hit.gameObject == gameObject)
                    continue;

                // Spawn explosion effect if assigned
                if (m_ExplodeParticle != null)
                {
                    GameObject obj = Instantiate(m_ExplodeParticle);
                    obj.transform.position = transform.position;
                }

                // Signal game over to the GameControl
                GameControl.m_Current.HandleGameOver();

                // Disable the plane (hide it and stop logic)
                gameObject.SetActive(false);
                break;
            }
        }
    }
}
