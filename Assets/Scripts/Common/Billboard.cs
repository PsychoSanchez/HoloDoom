using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : OverridableMonoBehaviour
{
    public float UpdateRate = 60;
    private float _renderUpdate;
    private float _delayBetweenUpdates;

    // Use this for initialization
    void Start()
    {
        _renderUpdate = 0f;
        _delayBetweenUpdates = 1 / UpdateRate;
    }

    // Update is called once per frame
    void Update()
    {
        _renderUpdate += Time.deltaTime;
        if (_renderUpdate >= _delayBetweenUpdates)
        {
            // transform.forward = Camera.main.transform.forward;
            var target = Camera.main.transform;
            var vec1 = target.position - gameObject.transform.position;
            // var vec2 = gameObject.transform.forward;
            // var angle = Vector3.Angle(vec1, vec2);
            transform.forward = vec1;
            _renderUpdate = 0f;
        }
    }
}
