using UnityEngine;
using System.Collections;

public class CharacterMotor : MonoBehaviour {
	
	private CharacterCollider character;
	
	private CharacterMotorMoving motorMoving = new CharacterMotorMoving();
	private CharacterMotorJumping motorJumping = new CharacterMotorJumping();
	private CharacterMotorFlying motorFlying = new CharacterMotorFlying();
	
	[System.NonSerialized]
	public Vector3 inputMoveDirection = Vector3.zero;
	
	[System.NonSerialized]
	public bool inputJump = false;
	
	[System.NonSerialized]
	public bool holdingInputJump = false;

	[System.NonSerialized]
	public bool inputFly = false;

	[System.NonSerialized]
	public bool holdingInputDown = false;
	
	[System.NonSerialized]
	public bool inputDown = false;

	[System.NonSerialized]
	public bool flying = false;

	[System.NonSerialized]
	public bool running = false;
	
	void Awake() {
		character = GetComponent<CharacterCollider>();
	}
	
	
	void FixedUpdate() {
		Vector3 velocity = character.GetDeltaPosition() / Time.deltaTime;
	
		motorMoving.ApplyMoving(this, ref velocity);
		motorMoving.ApplyGravity(this, ref velocity);
		if(flying)
			motorFlying.ApplyFlying (this, ref velocity);
		else
			motorJumping.ApplyJumping(this, ref velocity);

		character.Move( velocity*Time.deltaTime );
	}
	
	public bool IsGrounded() {
		return character.IsGrounded();
	}
	
}

class CharacterMotorMoving {
	private float moveSpeed = 6f;
	
	private const float maxGroundAcceleration = 20;
	private const float maxAirAcceleration = 10;

	public const float gravity = 40;
	private const float maxFallSpeed = 20;
	
	public void ApplyMoving(CharacterMotor motor, ref Vector3 velocity) {
		moveSpeed = (motor.running) ? 10f : 6f;
		Vector3 desiredVelocity = motor.inputMoveDirection * moveSpeed;
		Vector3 delta = desiredVelocity - new Vector3(velocity.x, 0, velocity.z);
		float maxDelta = GetMaxAcceleration(motor.IsGrounded()) * Time.deltaTime;
		velocity += Vector3.ClampMagnitude(delta, maxDelta);
	}
	
	public void ApplyGravity(CharacterMotor motor, ref Vector3 velocity) {
		if (!motor.flying) {
			velocity.y -= gravity * Time.deltaTime;
			velocity.y = Mathf.Max (velocity.y, -maxFallSpeed);
		}
		if(motor.IsGrounded()) velocity.y = Mathf.Min(velocity.y, 0);
	}
	
	private static float GetMaxAcceleration(bool grounded) {
		if(grounded) return maxGroundAcceleration;
		return maxAirAcceleration;
	}
	
}

class CharacterMotorJumping {
	
	private const float baseHeight = 1.0f;
	private const float extraHeight = 1.4f;
	
	private bool jumping = false;
	private float jumpStartTime;
	
	public void ApplyJumping(CharacterMotor motor, ref Vector3 velocity) {
		if (motor.IsGrounded() && !jumping && motor.inputJump) {
			jumping = true;
			jumpStartTime = Time.time;
			
			// Apply the jumping force to the velocity. Cancel any vertical velocity first.
			velocity.y = 0;
			velocity += Vector3.up * CalculateJumpVerticalSpeed(baseHeight);
			return;
		}
		if (jumping && motor.holdingInputJump) {
			// увеличиваем высоту прыжка
			if (Time.time < jumpStartTime + extraHeight / CalculateJumpVerticalSpeed(baseHeight)) {
				velocity += Vector3.up * CharacterMotorMoving.gravity * Time.deltaTime;
			}
		}
		if(motor.IsGrounded() || velocity.y <= 0) {
			jumping = false;
		}
	}
	
	
	private static float CalculateJumpVerticalSpeed(float targetJumpHeight) {
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt(2 * targetJumpHeight * CharacterMotorMoving.gravity);
	}
	
}

class CharacterMotorFlying {
	
	private const float baseHeight = 1.0f;
	private const float extraHeight = 1.4f;

	private float flyStartTime;

	public void ApplyFlying(CharacterMotor motor, ref Vector3 velocity) {
		if (motor.flying && motor.inputJump && !motor.IsGrounded()) {

			//Just a little impulsion
			velocity.y = 0;
			velocity += Vector3.up * 0.1f;
			return;
		}
		if (motor.flying && motor.holdingInputJump && !motor.IsGrounded()) {
			//Going up
			velocity += Vector3.up * 0.1f;
			return;
		}

		if (motor.flying && motor.inputDown && !motor.IsGrounded()) {
			
			//Just a little impulsion to down
			velocity.y = 0;
			velocity -= Vector3.up * 0.1f;
			return;
		}
		if (motor.flying && motor.holdingInputDown && !motor.IsGrounded()) {
			//Going Down
			velocity -= Vector3.up * 0.1f;
			return;
		}

		if(motor.flying){
			velocity.y = 0;
		}

		if (motor.IsGrounded())motor.flying = false;
	}
	
	
	private static float CalculateFlyVerticalSpeed(float targetJumpHeight) {
		// From the fly height we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt(2 * targetJumpHeight * CharacterMotorMoving.gravity);
	}
	
}
