using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class MovementController : MonoBehaviour
{
	public int _horizontalRayCount;
	public int _verticalRayCount;
	public LayerMask _layerObstacle;
	public Collisions _collisions;

	BoxCollider2D _boxCollider;
	Vector2 _bottomLeft, _bottomRight, _topLeft, _topRight;

	float _verticalRaySpacing, _horizontalRaySpacing;

	float _skinWidth = 1 / 16f; //Attention à bien mettre le f pour float sinon la division revoie un entier et ça part en couille

	public struct Collisions
	{
		public bool top, bottom, left, right;

		public void Reset()
		{
			top = bottom = left = right = false;
		}
	}
	// Start is called before the first frame update
	void Start()
    {
		_boxCollider = GetComponent<BoxCollider2D>();

		CalculateSpacing();
	}

	// Update is called once per frame
	void Update()
    {

	}

	public void Move(Vector2 velocity)
	{
		_collisions.Reset();

		CalculateBounds();
		if(velocity.x != 0)
			HorizontalMove(ref velocity);
		if (velocity.y != 0)
			VerticalMove(ref velocity);
		transform.Translate(velocity);
	}

	void HorizontalMove(ref Vector2 velocity)
	{
		// XXX brique sort du mur, reassign valeur de distance dans le boucle

		float direction = Mathf.Sign(velocity.x);
		float distance = Mathf.Abs(velocity.x) + _skinWidth;
		Vector2 baseOrigin = direction == 1 ? _bottomRight : _bottomLeft;

		for (int i = 0; i < _verticalRayCount; i++)
		{
			Vector2 origin = baseOrigin + new Vector2(0, _verticalRaySpacing * i);

			Debug.DrawLine(origin, origin + new Vector2(direction * distance, 0));
			RaycastHit2D hit = Physics2D.Raycast(
				origin,
				new Vector2(direction, 0),
				distance,
				_layerObstacle
				);

			if (hit)
			{
				velocity.x = (hit.distance - _skinWidth)* direction;
				if (direction < 0)
					_collisions.left = true;
				else if (direction > 0)
					_collisions.right = true;
			}
		}
	}
	void VerticalMove(ref Vector2 velocity)
	{
		float direction = Mathf.Sign(velocity.y);
		float distance = Mathf.Abs(velocity.y) + _skinWidth;
		Vector2 baseOrigin = direction == 1 ? _topLeft : _bottomLeft;

		for (int i = 0; i < _horizontalRayCount; i++)
		{
			Vector2 origin = baseOrigin + new Vector2(_horizontalRaySpacing * i, 0);

			Debug.DrawLine(origin, origin + new Vector2(0, direction * distance));
			RaycastHit2D hit = Physics2D.Raycast(
				origin,
				new Vector2(0, direction),
				distance,
				_layerObstacle
				);

			if (hit)
			{
				velocity.y = (hit.distance - _skinWidth) * direction;
				if (direction < 0)
					_collisions.bottom = true;
				else
					_collisions.top = true;
			}
		}
	}
	void CalculateSpacing()
	{
		Bounds bounds = _boxCollider.bounds; //à recalculer à chaque fois car on utilise le space.World
		bounds.Expand(_skinWidth * -2f); //On rétressi de 1 pour chaque bord

		_boxCollider = GetComponent<BoxCollider2D>();
		_verticalRaySpacing = bounds.size.y / (_verticalRayCount - 1);
		_horizontalRaySpacing = bounds.size.x / (_horizontalRayCount - 1);
	}
	void CalculateBounds()
	{
		Bounds bounds = _boxCollider.bounds;
		bounds.Expand(_skinWidth * -2f); //On rétressi de 1 pour chaque bord

		_bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
		_bottomRight = new Vector2(bounds.max.x, bounds.min.y);
		_topLeft = new Vector2(bounds.min.x, bounds.max.y);
		_topRight = new Vector2(bounds.max.x, bounds.max.y);
	}
}
