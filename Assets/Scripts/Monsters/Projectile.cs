using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : OverridableMonoBehaviour
{
    public long Id { get; private set; }
    public float projectileSpeed = 3.0f;
    public int damage = 15;
    UpdateTimer projUpdate;
    // Use this for initialization
    void Start()
    {
        projUpdate = new UpdateTimer(.025f);
        projUpdate.onTimeout += (a, e) =>
        {
            this.transform.position += (-1) * this.transform.forward * projectileSpeed * Time.deltaTime;
        };
    }

    public void SetId(long id)
    {
        Id = id;
    }

    // Update is called once per frame
    void Update()
    {
        projUpdate.Tick(Time.deltaTime);
        //this.transform.Translate(this.transform.forward * projectileSpeed * Time.deltaTime);
    }
}
