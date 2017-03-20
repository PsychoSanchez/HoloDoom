using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Comparers;

public class CubeCommands : MonoBehaviour
{
    private float speed = 10;
    private bool manipulationEvent;

    void StartManipulation()
    {
        manipulationEvent = true;
    }

    void StopManipulation()
    {
        manipulationEvent = false;
    }

    void Update()
    {
        if (manipulationEvent)
        {
            this.transform.position += GestureManager.deltaVector;
        }
    }
}
