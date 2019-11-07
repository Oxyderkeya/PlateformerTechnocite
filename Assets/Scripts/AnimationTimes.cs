using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTimes : MonoBehaviour
{
	Animator anim;

	Dictionary<string, float> timeDict = new Dictionary<string, float>();

	private void Start()
	{
		anim = GetComponent<Animator>();

		CalculateAnimationTimes();
	}

	private void CalculateAnimationTimes()
	{
		AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;

		foreach (AnimationClip clip in clips)
		{
			timeDict.Add(clip.name, clip.length);
		}

		foreach (KeyValuePair<string, float> clipTime in timeDict)
		{
			//Debug.Log("Key : " + clipTime.Key + " Value : " + clipTime.Value);
		}
	}

	public float GetTime(string clipName)
	{
		return timeDict[clipName];
	}

}
