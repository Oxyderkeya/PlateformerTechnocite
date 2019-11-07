using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementController))]
public class Player : MonoBehaviour
{
	delegate void MyDelegate();
	MyDelegate UpdateAnimation;

	[Header("Run speed")]
	[Tooltip("Number of meter by second")]
	public float maxSpeed;
	public float timeToMaxSpeed;

	[Header("Jump")]
	public uint maxAirJump;
	[Tooltip("Unity value of max jump height")]
	public float jumpHeight;
	[Tooltip("Time in seconds to reach the jump height")]
	public float timeToMaxJump;
	[Tooltip("Can i change direction in air?")]
	[Range(0, 1)]
	public float airControl;

	[Header("Other")]
	public bool animationByParameters;

	int doubleJumpCount;

	float acceleration;
	float minSpeedThreshold;

	float gravity;
	float jumpForce;
	float maxFallingSpeed;
	int horizontal = 0;
	bool doubleJumping;

	Animator anim;
	AnimationTimes animationTimes;
	SpriteRenderer spriteRenderer;

	Vector2 velocity = new Vector2();
	MovementController movementController;

	bool freeze;

	// Start is called before the first frame update
	void Start()
	{
		// Math calculation acceleration
		// s = distance
		// a = acceleration
		// t = time
		// s = 1 / 2 at²
		// a = 2s / t²
		acceleration = (2f * maxSpeed) / Mathf.Pow(timeToMaxSpeed, 2);

		minSpeedThreshold = acceleration / Application.targetFrameRate * 2f;
		movementController = GetComponent<MovementController>();
		anim = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		animationTimes = GetComponent<AnimationTimes>();

		// Math calculation for gravity and jumpForce
		gravity = -(2 * jumpHeight) / Mathf.Pow(timeToMaxJump, 2);
		jumpForce = Mathf.Abs(gravity) * timeToMaxJump;
		maxFallingSpeed = -jumpForce;

		if (animationByParameters)
			UpdateAnimation = UpdateAnimationByParameters;
		else
			UpdateAnimation = UpdateAnimationByCode;
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.H))
		{
			anim.SetTrigger("hit");
		}

		if (movementController.collisions.bottom || movementController.collisions.top)
			velocity.y = 0;

		horizontal = 0;

		if (Input.GetKey(KeyCode.D) && !freeze)
		{
			horizontal += 1;
		}
		if (Input.GetKey(KeyCode.Q) && !freeze)
		{
			horizontal -= 1;
		}

		float controlModifier = 1f;
		if (!movementController.collisions.bottom)
		{
			controlModifier = airControl;
		}

		velocity.x += horizontal * acceleration * controlModifier * Time.deltaTime;

		if (Mathf.Abs(velocity.x) > maxSpeed)
			velocity.x = maxSpeed * horizontal;


		if (horizontal == 0)
		{
			if (velocity.x > minSpeedThreshold)
				velocity.x -= acceleration * Time.deltaTime;
			else if (velocity.x < -minSpeedThreshold)
				velocity.x += acceleration * Time.deltaTime;
			else
				velocity.x = 0;
		}

		if ((movementController.collisions.left || movementController.collisions.right) && velocity.y < 0)
		{
			velocity.y += gravity * Time.deltaTime / 3f;
		}
		else
		{
			velocity.y += gravity * Time.deltaTime;
		}

		if (velocity.y < maxFallingSpeed)
		{
			velocity.y = maxFallingSpeed;
		}

		UpdateJump();
		UpdateFlip();

		movementController.Move(velocity * Time.deltaTime);

		UpdateAnimation();

	}

	void UpdateAnimationByCode()
	{
		// Au sol
		if (movementController.collisions.bottom)
		{
			if (velocity.x == 0)
				anim.Play("FrogIdle");
			else if (velocity.x != 0)
				anim.Play("FrogRun");
		}
		// En l'air
		else
		{
			if (!doubleJumping)
			{
				if (movementController.collisions.left ||
					movementController.collisions.right)
					anim.Play("FrogWallSticking");
				else if (velocity.y > 0)
					anim.Play("FrogJumping");
				else if (velocity.y < 0)
					anim.Play("FrogFalling");
			}
		}
	}

	void UpdateAnimationByParameters()
	{
		int vertical = (int)Mathf.Sign(velocity.y);
		if (movementController.collisions.bottom)
			vertical = 0;

		anim.SetInteger("horizontal", horizontal);
		anim.SetInteger("vertical", vertical);
		anim.SetBool("grounded", movementController.collisions.bottom);
		anim.SetBool("doubleJumping", doubleJumping);
	}

	void UpdateFlip()
	{
		if (velocity.x > 0)
		{
			// regarde vers la droite
			spriteRenderer.flipX = false;
		}
		else if (velocity.x < 0)
		{
			// regarde vers la gauche
			spriteRenderer.flipX = true;
		}
	}

	void UpdateJump()
	{
		if (movementController.collisions.bottom)
		{
			doubleJumpCount = 0;
			doubleJumping = false;
		}

		if (Input.GetKeyDown(KeyCode.Space) && !freeze)
		{
			// Normal jump
			if (movementController.collisions.bottom)
			{
				Jump();
			}
			// Wall jump
			else if (
				!movementController.collisions.bottom &&
				(movementController.collisions.left ||
				 movementController.collisions.right))
			{
				WallJump();
			}
			// Normal or airJump
			else if (doubleJumpCount < maxAirJump && !movementController.collisions.bottom)
			{
				DoubleJump();
			}
		}
	}

	void Jump()
	{
		velocity.y = jumpForce;
	}

	void WallJump()
	{
		int direction = movementController.collisions.left ? 1 : -1;
		velocity.x = maxSpeed * direction;
		Jump();
	}

	void DoubleJump()
	{
		StartCoroutine(DoubleJumpCoroutine());
	}

	IEnumerator DoubleJumpCoroutine()
	{
		Jump();
		doubleJumpCount++;
		doubleJumping = true;
		anim.Play("FrogDoubleJumping");

		while (!anim.GetCurrentAnimatorStateInfo(0).IsName("FrogDoubleJumping"))
		{
			yield return null;
		}

		while (true)
		{
			if (!anim.GetCurrentAnimatorStateInfo(0).IsName("FrogDoubleJumping") ||
				movementController.collisions.bottom)
				break;
			yield return null;
		}
		doubleJumping = false;
	}
	private void OnTriggerEnter2D(Collider2D collision)
	{
		Enemy enemy = collision.gameObject.GetComponent<Enemy>();

		if(enemy != null)
		{
			HitEnemy();
		}
	}
	Coroutine hitEnemy;
	void HitEnemy()
	{
		if(hitEnemy == null)
		{
			hitEnemy = StartCoroutine(HitEnemyCoroutine());
		}
	}
	IEnumerator HitEnemyCoroutine()
	{
		anim.SetTrigger("hit");
		freeze = true;

		yield return new WaitForSeconds(animationTimes.GetTime("FrogHit"));

		while(!anim.GetCurrentAnimatorStateInfo(0).IsName("FrogHit"))
		{
			yield return null;
		}
		while (anim.GetCurrentAnimatorStateInfo(0).IsName("FrogHit"))
		{
			yield return null;
		}
		SpawnPlayer spawnPlayer = FindObjectOfType<SpawnPlayer>();
		spawnPlayer.Spawn();
		Destroy(gameObject);
	}
}