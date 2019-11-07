using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
	public Player playerToSpawn;
    void Start()
    {
		Spawn();
    }
	public void Spawn()
	{
		Instantiate(playerToSpawn, transform.position, Quaternion.identity);
	}
}
