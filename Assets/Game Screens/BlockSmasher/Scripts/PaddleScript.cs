using UnityEngine;

public class PaddleScript : MonoBehaviour
{
    public new Rigidbody2D rigidbody { get; private set; }
    public float maxBounceAngle = 75f;

    public float moveSpeed = 200; // Speed multiplier for paddle movement
    public float dirX;            // Horizontal input direction from accelerometer


    // Called when the script is first loaded
    public void Awake()
    {
        // Cache the Rigidbody2D component attached to the paddle
        this.rigidbody = GetComponent<Rigidbody2D>();
    }

    // Called once per frame
    private void Update()
    {
        // Get horizontal tilt from device accelerometer and multiply by move speed
        dirX = Input.acceleration.x * moveSpeed;

    }

    // Called at fixed intervals, synced with physics engine
    private void FixedUpdate()
    {
        // Apply horizontal movement based on input
        rigidbody.velocity = new Vector2(dirX, 0f);
    }

    // Resets paddle to center of screen and stops its movement
    public void ResetPaddle()
    {
        this.transform.position = new Vector2(0f, this.transform.position.y);
        this.rigidbody.velocity = Vector2.zero;
    }

    // Called when paddle collides with another 2D collider
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Try to get the BallScript from the colliding object
        BallScript ball = collision.gameObject.GetComponent<BallScript>();

        if (ball != null)
        {
            // Paddle's center position
            Vector3 paddlePosition = this.transform.position;

            // Where the ball hit the paddle
            Vector2 contactPoint = collision.GetContact(0).point;

            // Distance from paddle center to hit point
            float offset = paddlePosition.x - contactPoint.x;

            // Half-width of the paddle
            float width = collision.otherCollider.bounds.size.x / 2;

            // Current angle of the ball's velocity relative to vertical (up)
            float currentAngle = Vector2.SignedAngle(Vector2.up, ball.rigidbody.velocity);

            // Calculate bounce angle based on offset and max allowed angle
            float bounceAngle = (offset / width) * this.maxBounceAngle;

            // Clamp the new angle to within max limits
            float newAngle = Mathf.Clamp(currentAngle + bounceAngle, -this.maxBounceAngle, this.maxBounceAngle);

            // Rotate the ball's velocity to reflect the new angle
            Quaternion rotation = Quaternion.AngleAxis(newAngle, Vector3.forward);
            ball.rigidbody.velocity = rotation * Vector2.up * ball.rigidbody.velocity.magnitude;
        }
    }
}
