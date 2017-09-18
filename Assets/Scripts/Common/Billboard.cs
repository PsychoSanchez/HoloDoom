using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : OverridableMonoBehaviour
{
    private float _renderUpdate;


    // Use this for initialization
    void Start()
    {
        _renderUpdate = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        _renderUpdate += Time.deltaTime;
        if (_renderUpdate >= 0.16f)
        {
            transform.forward = Camera.main.transform.forward;
            _renderUpdate = 0f;
        }
    }
}
