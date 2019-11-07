using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementController))]
public class Enemy : MonoBehaviour
{
	public float speed;
	public bool isFacingRight;
	public float stopTimeOnFlip;
	Coroutine flipCoroutine;
	Animator animator;

	Vector2 velocity = new Vector2();
	MovementController movementController;
	SpriteRenderer spriteRenderer;

    void Start()
    {
		animator = GetComponent<Animator>();
		movementController = GetComponent<MovementController>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		velocity.x = speed;
		StartFacing();
	}
    void Update()
    {
		UpdateMove();
		UpdateFlip();
    }
	void UpdateMove()
	{
		movementController.Move(velocity * Time.deltaTime);
	}
	void UpdateFlip()
	{
		if ((velocity.x > 0 && movementController.collisions.right) 
			|| (velocity.x < 0 && movementController.collisions.left))
		{
			Flip();
		}
		else if (movementController.collisions.frontPit)
		{
			Flip();
		}
	}
	void StartFacing()
	{
		if (isFacingRight)
		{
			velocity.x = speed;

		}
		else
		{
			velocity.x = -speed;
			spriteRenderer.flipX = !spriteRenderer.flipX;
		}
	}
	/// <summary>
	/// Return sprite and velocity
	/// </summary>
	void Flip()
	{
		if(flipCoroutine == null)
		{
			flipCoroutine = StartCoroutine(FlipCoroutine());
		}
	}

	IEnumerator FlipCoroutine()
	{
		float actualVelocity = velocity.x;
		velocity.x = 0;

		animator.Play("Idle");

		yield return new WaitForSeconds(stopTimeOnFlip);
		movementController.collisions.frontPit = false;
		animator.Play("Run");
		spriteRenderer.flipX = !spriteRenderer.flipX;
		velocity.x = actualVelocity * -1f;
		flipCoroutine = null;
	}
}
