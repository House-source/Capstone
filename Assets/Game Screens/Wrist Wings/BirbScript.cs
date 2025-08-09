using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirbScript : MonoBehaviour
{
    // Reference to the bird's Rigidbody2D component, used for physics (e.g., flapping).
    public Rigidbody2D myRigidbody;

    // Strength of the upward force applied when the bird flaps.
    public float FlapStrength;

    // Reference to the LogicScript, which handles game events like Game Over.
    public LogicScript logic;

    // Tracks whether the bird is still alive (to disable input after death).
    public bool birdIsAlive = true;

    // Stores the resting acceleration for initialization.
    private Vector3 neutralAcceleration;

    // Stores the last frame's acceleration for motion comparison.
    private Vector3 lastAcceleration;

    // Sensitivity threshold for detecting a motion-based "flap" input.
    public float shakeThreshold = 1.0f;

    // Start is called before the first frame update.
    void Start()
    {
        // Find the game object tagged "Logic" and get its LogicScript component.
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();

        // Record the "resting" acceleration when the game begins
        neutralAcceleration = Input.acceleration;
        lastAcceleration = neutralAcceleration; // Use it as the first value
    }

    // Update is called once per frame.
    void Update()
    {
        // If the bird is no longer alive, skip input processing.
        if (!birdIsAlive)
            return;

        // Get the current acceleration from the device's sensors.
        Vector3 currentAcceleration = Input.acceleration;

        // Calculate the change in z-axis acceleration (shake phone upwards).
        float accelerationChange = currentAcceleration.z - lastAcceleration.z;

        // If the change is larger than the threshold, trigger a flap.
        if (accelerationChange > shakeThreshold)
        {
            // Apply an upward velocity to the bird (simulate a flap).
            myRigidbody.velocity = Vector2.up * FlapStrength;
        }

        // Save the current acceleration for comparison in the next frame.
        lastAcceleration = currentAcceleration;
    }

    // Called when the bird collides with another object (like ground or obstacles).
    private void OnCollisionEnter2D(Collision2D collision)
    {
        logic.gameOver();
        birdIsAlive = false;
    }
}
