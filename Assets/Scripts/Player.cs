using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementController))]
public class Player : MonoBehaviour
{
	[Tooltip("Number of meter by second")]
	public float _speed;
	public float _gravity;
	public float _jumpForce;

	Vector2 velocity = new Vector2();
	MovementController movementController;

    // Start is called before the first frame update
    void Start()
    {
		movementController = GetComponent<MovementController>();
	}

    // Update is called once per frame
    void Update()
    {
		int horizontal = 0;

		if (movementController._collisions.bottom || movementController._collisions.top)
			velocity.y = 0;

		if (Input.GetKey(KeyCode.D))
		{
			horizontal += 1;
		}
		if(Input.GetKey(KeyCode.Q))
		{
			horizontal -= 1;
		}
		if(Input.GetKeyDown(KeyCode.Space) && movementController._collisions.bottom)
		{
			Jump();
		}

		velocity.x = horizontal * _speed;

		velocity.y += _gravity * Time.deltaTime * -1f;


		movementController.Move(velocity * Time.deltaTime);
	}
	void Jump()
	{
		velocity.y = _jumpForce;
	}
}
