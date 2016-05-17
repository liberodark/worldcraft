using UnityEngine;
using System.Collections;

public class CharacterInputController : MonoBehaviour {
	
	private CharacterCollider character;
	private CharacterMotor motor;
	private CharacterMotorSwimming waterMotor;
	
	private Map map;
	private float jumpPressedTime = -100;
	private float flyPressedTime = -100;
	private float downPressedTime = -100;
	private float lastTimeAxis = 0;
	private float xAxis;
	private float yAxis;


	// Use this for initialization
	void Awake() {
		character = GetComponent<CharacterCollider>();
		motor = GetComponent<CharacterMotor>();
		waterMotor = GetComponent<CharacterMotorSwimming>();
		map = (Map) GameObject.FindObjectOfType( typeof(Map) );
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 direction;

		xAxis = InputManager.inputManager().getHorizontalAxis();
		yAxis = InputManager.inputManager().getVerticalAxis();

		direction = new Vector3(xAxis, 0, yAxis);
		direction = Vector3.ClampMagnitude(direction, 1);

		//Smile, you're filmed
		if (InputManager.inputManager ().screenshotInput) {
			System.IO.Directory.CreateDirectory(Application.dataPath+"/screenshots");
			Application.CaptureScreenshot (Application.dataPath+"/screenshots/" + Time.time + ".jpg");
			GameStateManager.manager.SendMessage("OnScreenShot", SendMessageOptions.DontRequireReceiver);
		}

		
		if(IsInWater()) {
			waterMotor.enabled = true;
			motor.enabled = false;
			
			waterMotor.inputEmersion = InputManager.inputManager().isJumpInputHold;
			waterMotor.inputMoveDirection = transform.TransformDirection(direction);
			
			
		} else {
			waterMotor.enabled = false;
			motor.enabled = true;
			
			motor.inputMoveDirection = transform.TransformDirection(direction);

			if(InputManager.inputManager().flyInput) {
				flyPressedTime = Time.time;
				motor.flying = ((Time.time - flyPressedTime <= 0.2f) && !motor.flying);
			}

			if(InputManager.inputManager().goDownInput) {
				downPressedTime = Time.time;
			}
			if( !InputManager.inputManager().isGoDownInputHold ) {
				downPressedTime = -100;
			}
			motor.inputDown = Time.time - jumpPressedTime <= 0.2f;
			motor.holdingInputDown = InputManager.inputManager().isGoDownInputHold;

			if(InputManager.inputManager().jumpInput) {
				jumpPressedTime = Time.time;
			}
			if( !InputManager.inputManager().isJumpInputHold ) {
				jumpPressedTime = -100;
			}
			motor.inputJump = Time.time - jumpPressedTime <= 0.2f;
			motor.holdingInputJump = InputManager.inputManager().isJumpInputHold;

			if(InputManager.inputManager().isHorizontalInput || InputManager.inputManager().isVerticalInput){
				motor.running = (Time.time - lastTimeAxis) <= 0.2f;
				lastTimeAxis = Time.time;
			}

			if(!InputManager.inputManager().isHorizontalInputHold && !InputManager.inputManager().isVerticalInputHold && motor.running){
				motor.running = false;
			}

		}
	}
	
	private bool IsInWater() {
		Vector3 bottom = transform.position;
		Vector3 top = bottom + Vector3.up*character.height;
		Vector3 pos = Vector3.Lerp(bottom, top, 0.2f);
		return map.GetBlock( Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z) ).IsFluid();
	}
	
}
