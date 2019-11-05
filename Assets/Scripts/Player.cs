using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementController))]
public class Player : MonoBehaviour
{
	public float _acceleration;
	[Tooltip("Number of meter by second")]
	public float _maxSpeed;
	float _minSpeedThreshold = 0.02f; //seuil

	[Tooltip("Unity value of max jump height")]
	public float _jumpHeight;
	[Tooltip("Time in seconds ro reach the jump height")]
	public float _timeToMaxJump;
	[Tooltip("0: cannot move in air, 1: same as in the ground")]
	public float _airControl;
	float _maxFallingSpeed;

	float _speed;
	float _gravity;
	float _jumpForce;
	int _horizontal = 0;

	Vector2 velocity = new Vector2();
	MovementController movementController;

    // Start is called before the first frame update
    void Start()
    {
		_minSpeedThreshold = _acceleration * Application.targetFrameRate * 2f;
		Debug.Log(_minSpeedThreshold);
		movementController = GetComponent<MovementController>();

		//Math calculation for gravity and jumpForce
		_gravity = -(2 * _jumpHeight) / Mathf.Pow(_timeToMaxJump, 2);
		_jumpForce = Mathf.Abs(_gravity) * _timeToMaxJump;
		_maxFallingSpeed = -_jumpForce;
	}

    // Update is called once per frame
    void Update()
    {
		if (movementController._collisions.bottom || movementController._collisions.top)
			velocity.y = 0;

		_horizontal = 0;

		if (Input.GetKey(KeyCode.D))
		{
			_horizontal += 1;
		}
		if(Input.GetKey(KeyCode.Q))
		{
			_horizontal -= 1;
		}

		if (Input.GetKeyDown(KeyCode.Space) && movementController._collisions.bottom)
		{
			Jump();
		}

		float controlModifier = 1f;
		if (!movementController._collisions.bottom)
			controlModifier = _airControl;

		velocity.x += _horizontal * _acceleration * controlModifier * Time.deltaTime;

		//Pareil que en dessous en plus court
		if(Mathf.Abs(velocity.x) > _maxSpeed)
		{
			velocity.x = _maxSpeed * _horizontal;
		}
		//if (velocity.x > _maxSpeed)
		//	velocity.x = _maxSpeed;
		//if (velocity.x < -_maxSpeed)
		//	velocity.x = -_maxSpeed;

		if (_horizontal == 0)
		{
			if (velocity.x > _minSpeedThreshold)
				velocity.x -= _acceleration * Time.deltaTime;
			else if (velocity.x < -_minSpeedThreshold)
				velocity.x += -_acceleration * Time.deltaTime;
			else
				velocity.x = 0;
		}
			

		velocity.y += _gravity * Time.deltaTime;
		if(velocity.y < _maxFallingSpeed)
		{
			velocity.y = _maxFallingSpeed;
		}
		movementController.Move(velocity * Time.deltaTime);
	}
	void Jump()
	{
		velocity.y = _jumpForce;
	}
}
