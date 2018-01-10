using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRelatedPosition : MonoBehaviour
{
    public Transform ParentTransform;
    public Vector3 MarginFromObject;

    // Use this for initialization
    void Start()
    {
        CheckTransform();
    }

    private void CheckTransform()
    {
        if (ParentTransform == null)
        {
            var camera = GameObject.FindGameObjectsWithTag("MainCamera");
            if (camera == null || camera.Length < 1)
            {
                return;
            }

            ParentTransform = camera[0].transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ParentTransform == null)
        {
            return;
        }

        var rotation = Quaternion.LookRotation(ParentTransform.forward, Vector3.up);
        rotation.x = 0;
        rotation.z = 0;
        transform.rotation = rotation;
        transform.position = ParentTransform.position + transform.forward * MarginFromObject.z - new Vector3(MarginFromObject.x, MarginFromObject.y, 0);
    }
}
