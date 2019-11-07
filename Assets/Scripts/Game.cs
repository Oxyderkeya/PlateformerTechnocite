using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
	void Awake()
	{
		QualitySettings.vSyncCount = 0;  // VSync must be disabled to allow targetFrameRate
		Application.targetFrameRate = 60;
	}
}
