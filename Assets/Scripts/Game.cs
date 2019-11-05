using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to control the frame rate
/// </summary>
public class Game : MonoBehaviour
{
    void Start()
    {
		QualitySettings.vSyncCount = 0; //vSync doit être désactiver pour autoriser le targerFrameRate
		Application.targetFrameRate = 60;
    }
}
