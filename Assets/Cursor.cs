using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    private MeshRenderer meshRenderer;

	void Start ()
	{
	    meshRenderer = this.gameObject.GetComponentInChildren<MeshRenderer>();
	}

	void Update () {
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;

        RaycastHit hitInfo;

        if (Physics.Raycast(headPosition, gazeDirection, out hitInfo))
        {
            meshRenderer.enabled = true;

            this.transform.position = hitInfo.point;

            this.transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
        }
        else
        {
            meshRenderer.enabled = false;
        }
    }
}
