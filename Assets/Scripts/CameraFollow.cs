using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	Player player;
	GameObject bg;
	private void Start()
	{
		StartCoroutine(FindPlayerCoroutine());
		StartCoroutine(FollowPlayer());
	}
	IEnumerator FindPlayerCoroutine()
	{
		while(true)
		{
			if(player == null)
			{
				player = FindObjectOfType<Player>(); //à ne pas faire trop de FindObjectOfType à chaque frame, ça prends beaucoup de ressources
			}
			yield return null;
		}
	}
	IEnumerator FollowPlayer()
	{
		while (true)
		{
			if (player != null)
			{
				transform.position = new Vector3(
					player.transform.position.x, 
					player.transform.position.y, 
					transform.position.z);
			}
			yield return null;
		}
	}
}
