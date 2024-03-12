﻿using UnityEngine;

[CreateAssetMenu(menuName = "Movement Data Config")]
public class MovementDataConfig : ScriptableObject
{
	[Header("Run")]
	public float movementSpeed = 10f;
	//Acel/Decel should be between 0 and movementSpeed
	public float acceleration = 1f;
	public float deceleration = 1f;

	public float accelerationInAir = 1f;
	public float decelerationInAir = 1f;
    
	public float velPower = 0.9f;
	
	[Header("Jump")]
	public float jumpForce = 10f;
	
	public float jumpInputBufferTime = 0.5f;
	public float coyoteTimeBuffer = 0.5f;

	public float jumpHangTimeThreshold = 0.5f;
	public float jumpHangAccelerationMult = 1f;
	public float jumpHangMaxSpeedMult = 1f;

	[Header("Gravity")]
	public float gravityScaleWhenJumping = 1f;
	public float gravityScaleWhenFalling = 2f;
	// public float gravityScaleWhenDiving = 3f;

	[Header("Diving")]
	public float diveStartSpeedIncrease = 10f;
	public float diveSpeedGainedPerSecond = 15f;
	public float maxDiveSpeed = 30f;

	[Header("Squashing")]
	public float velocityRequiredForSquashing = 30f;
	public float velocityRequiredForDashSquashing = 30f;
	
	[Header("Shockwaves")]
	public float shockwaveRadius = 2f;
	public float shockwaveForce = 100f;



	// public float fastFallGravityMult = 2f;
	// public float maxFastFallSpeed = 10;
	//
	// public float fallGravityMult = 2f;
	// public float maxFallSpeed = 10f;


	// [Header("Gravity")]
	// [HideInInspector] public float gravityStrength; //Downwards force (gravity) needed for the desired jumpHeight and jumpTimeToApex.
	// [HideInInspector] public float gravityScale; //Strength of the player's gravity as a multiplier of gravity (set in ProjectSettings/Physics2D).
	// 									  //Also the value the player's rigidbody2D.gravityScale is set to.
	// [Space(5)]
	// public float fallGravityMult; //Multiplier to the player's gravityScale when falling.
	// public float maxFallSpeed; //Maximum fall speed (terminal velocity) of the player when falling.
	// [Space(5)]
	// public float fastFallGravityMult; //Larger multiplier to the player's gravityScale when they are falling and a downwards input is pressed.
	// 								  //Seen in games such as Celeste, lets the player fall extra fast if they wish.
	// public float maxFastFallSpeed; //Maximum fall speed(terminal velocity) of the player when performing a faster fall.
	//
	// [Space(20)]
	//
	// [Header("Run")]
	// public float runMaxSpeed; //Target speed we want the player to reach.
	// public float runAcceleration; //The speed at which our player accelerates to max speed, can be set to runMaxSpeed for instant acceleration down to 0 for none at all
	// [HideInInspector] public float runAccelAmount; //The actual force (multiplied with speedDiff) applied to the player.
	// public float runDecceleration; //The speed at which our player decelerates from their current speed, can be set to runMaxSpeed for instant deceleration down to 0 for none at all
	// [HideInInspector] public float runDeccelAmount; //Actual force (multiplied with speedDiff) applied to the player .
	// [Space(5)]
	// [Range(0f, 1)] public float accelInAir; //Multipliers applied to acceleration rate when airborne.
	// [Range(0f, 1)] public float deccelInAir;
	// [Space(5)]
	// public bool doConserveMomentum = true;
	//
	// [Space(20)]
	//
	// [Header("Jump")]
	// public float jumpHeight; //Height of the player's jump
	// public float jumpTimeToApex; //Time between applying the jump force and reaching the desired jump height. These values also control the player's gravity and jump force.
	// [HideInInspector] public float jumpForce; //The actual force applied (upwards) to the player when they jump.
	//
	// [Header("Both Jumps")]
	// public float jumpCutGravityMult; //Multiplier to increase gravity if the player releases thje jump button while still jumping
	// [Range(0f, 1)] public float jumpHangGravityMult; //Reduces gravity while close to the apex (desired max height) of the jump
	// public float jumpHangTimeThreshold; //Speeds (close to 0) where the player will experience extra "jump hang". The player's velocity.y is closest to 0 at the jump's apex (think of the gradient of a parabola or quadratic function)
	// [Space(0.5f)]
	// public float jumpHangAccelerationMult; 
	// public float jumpHangMaxSpeedMult; 				
	//
	// [Header("Wall Jump")]
	// public Vector2 wallJumpForce; //The actual force (this time set by us) applied to the player when wall jumping.
	// [Space(5)]
	// [Range(0f, 1f)] public float wallJumpRunLerp; //Reduces the effect of player's movement while wall jumping.
	// [Range(0f, 1.5f)] public float wallJumpTime; //Time after wall jumping the player's movement is slowed for.
	// public bool doTurnOnWallJump; //Player will rotate to face wall jumping direction
	//
	// [Space(20)]
	//
	// [Header("Slide")]
	// public float slideSpeed;
	// public float slideAccel;
	//
	//    [Header("Assists")]
	// [Range(0.01f, 0.5f)] public float coyoteTime; //Grace period after falling off a platform, where you can still jump
	// [Range(0.01f, 0.5f)] public float jumpInputBufferTime; //Grace period after pressing jump where a jump will be automatically performed once the requirements (eg. being grounded) are met.
	//
	//
	// //Unity Callback, called when the inspector updates
	//    private void OnValidate()
	//    {
	// 	//Calculate gravity strength using the formula (gravity = 2 * jumpHeight / timeToJumpApex^2) 
	// 	gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);
	// 	
	// 	//Calculate the rigidbody's gravity scale (ie: gravity strength relative to unity's gravity value, see project settings/Physics2D)
	// 	gravityScale = gravityStrength / Physics2D.gravity.y;
	//
	// 	//Calculate are run acceleration & deceleration forces using formula: amount = ((1 / Time.fixedDeltaTime) * acceleration) / runMaxSpeed
	// 	runAccelAmount = (50 * runAcceleration) / runMaxSpeed;
	// 	runDeccelAmount = (50 * runDecceleration) / runMaxSpeed;
	//
	// 	//Calculate jumpForce using the formula (initialJumpVelocity = gravity * timeToJumpApex)
	// 	jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;
	//
	// 	#region Variable Ranges
	// 	runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, runMaxSpeed);
	// 	runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, runMaxSpeed);
	// 	#endregion
	// }
}

 public class MovementData
 {
	 public float movementSpeed;
	 //Acel/Decel should be between 0 and movementSpeed
	 public float acceleration;
	 public float deceleration;

	 public float accelerationInAir;
	 public float decelerationInAir;
    
	 public float velPower;
	
	 [Header("Jump")]
	 public float jumpForce;
	
	 public float jumpInputBufferTime;
	 public float coyoteTimeBuffer;

	 public float jumpHangTimeThreshold;
	 public float jumpHangAccelerationMult;
	 public float jumpHangMaxSpeedMult;

	 [Header("Gravity")]
	 public float gravityScaleWhenJumping;
	 public float gravityScaleWhenFalling;
	 // public float gravityScaleWhenDiving = 3f;

	 [Header("Diving")]
	 public float diveStartSpeedIncrease;
	 public float diveSpeedGainedPerSecond;
	 public float maxDiveSpeed;

	 public float velocityRequiredForSquashing;
	 public float velocityRequiredForDashSquashing;

	 [Header("Shockwaves")]
	 public float shockwaveRadius;
	 public float shockwaveForce;
	 
	 public MovementData(MovementDataConfig config)
	 {
		 movementSpeed = config.movementSpeed;
		 acceleration = config.acceleration;
		 deceleration = config.deceleration;
		 accelerationInAir = config.accelerationInAir;
		 decelerationInAir = config.decelerationInAir;
		 velPower = config.velPower;
		 jumpForce = config.jumpForce;
		 jumpInputBufferTime = config.jumpInputBufferTime;
		 coyoteTimeBuffer = config.coyoteTimeBuffer;
		 jumpHangTimeThreshold = config.jumpHangTimeThreshold;
		 jumpHangAccelerationMult = config.jumpHangAccelerationMult;
		 jumpHangMaxSpeedMult = config.jumpHangMaxSpeedMult;
		 gravityScaleWhenJumping = config.gravityScaleWhenJumping;
		 gravityScaleWhenFalling = config.gravityScaleWhenFalling;
		 diveStartSpeedIncrease = config.diveStartSpeedIncrease;
		 diveSpeedGainedPerSecond = config.diveSpeedGainedPerSecond;
		 maxDiveSpeed = config.maxDiveSpeed;
		 
		 velocityRequiredForSquashing = config.velocityRequiredForSquashing;
		 velocityRequiredForDashSquashing = config.velocityRequiredForDashSquashing;
		 
		 shockwaveRadius = config.shockwaveRadius;
		 shockwaveForce = config.shockwaveForce;
	 }
 }