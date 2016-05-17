using UnityEngine;
using System.Collections;

public class CharacterMotorSwimming : MonoBehaviour {
	
	private const float moveSpeed = 3f;
	private const float maxAcceleration = 15;
	
	private const float gravity = 10;
	private const float maxFallSpeed = 10;
	
	private CharacterCollider character;
	
	
	[System.NonSerialized]
	public Vector3 inputMoveDirection = Vector3.zero;
	
	[System.NonSerialized]
	public bool inputEmersion = false;
	
	
	void Awake() {
		character = GetComponent<CharacterCollider>();
	}
	
	
	void FixedUpdate() {
		Vector3 velocity = character.GetDeltaPosition() / Time.deltaTime;
		ApplyMoving(ref velocity);
		ApplyGravity(ref velocity);
		ApplyEmersion(ref velocity);

		character.Move( velocity*Time.deltaTime );
	}
	
	
	private void ApplyMoving(ref Vector3 velocity) {
		Vector3 desiredVelocity = inputMoveDirection * moveSpeed;
		Vector3 delta = desiredVelocity - new Vector3(velocity.x, 0, velocity.z);
		velocity += Vector3.ClampMagnitude(delta, maxAcceleration);
	}
	
	private void ApplyGravity(ref Vector3 velocity) {
		if(InputManager.inputManager().isJumpInputHold){
			velocity.y -= gravity / 8 * Time.deltaTime;
			velocity.y = Mathf.Max(velocity.y, -maxFallSpeed);
		}
		else{
			velocity.y -= gravity / 8 * Time.deltaTime;
			velocity.y = Mathf.Max(velocity.y, -maxFallSpeed/2);
		}
	}
	
	private void ApplyEmersion(ref Vector3 velocity) {
		if(InputManager.inputManager().isJumpInputHold){
			velocity.y += 6 * gravity * Time.deltaTime;
			velocity.y = (velocity.y == Mathf.Min(velocity.y, maxFallSpeed)) ? velocity.y + 0.3f : maxFallSpeed;
		}
		else if(inputEmersion) {
			velocity.y += 5 * gravity * Time.deltaTime;
			velocity.y = (velocity.y == Mathf.Min(velocity.y, maxFallSpeed)) ? velocity.y + 0.3f : maxFallSpeed;
		}
	}
	
	
	
}
