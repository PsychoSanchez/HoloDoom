using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : OverridableMonoBehaviour
{
    public long Id { get; private set; }
    public float projectileSpeed = 1.0f;
    public int damage = 15;
    // Use this for initialization
    void Start()
    {
    }

    public void SetId(long id)
    {
        Id = id;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position += (-1) * this.transform.forward * projectileSpeed * Time.deltaTime;
        //this.transform.Translate(this.transform.forward * projectileSpeed * Time.deltaTime);
    }
}
