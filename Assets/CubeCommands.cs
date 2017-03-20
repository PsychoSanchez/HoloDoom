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

    void Update()
    {
        if (manipulationEvent)
        {
            var vector = GestureManager.deltaVector;
            this.transform.position += new Vector3(vector.x*speed,0,0);
        }
    }
}
