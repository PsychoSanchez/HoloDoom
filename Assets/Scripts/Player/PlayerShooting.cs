using System.Collections;
using System.Collections.Generic;
using UnityEngine.VR.WSA.Input;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShooting : MonoBehaviour {

    GestureRecognizer recognizer;
    public Weapon CurrentWeapon;
    public Dictionary<WeaponType, Weapon> Weapons;
    public Text AmmoUI;
    
    // Use this for initialization
    void Start () {
        recognizer = new GestureRecognizer();
        recognizer.SetRecognizableGestures(GestureSettings.Tap);
        recognizer.TappedEvent += Recognizer_TappedEvent;
        recognizer.StartCapturingGestures();
        AmmoUI.text = CurrentWeapon.Ammo.ToString();
    }

    private void Recognizer_TappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
    {
        if (CurrentWeapon == null)
        {
            return;
        }

        CurrentWeapon.Shoot(headRay);
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            CurrentWeapon.Shoot(new Ray(Camera.main.transform.position, Camera.main.transform.forward));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ammo")
        {
            var ammo = other.GetComponent<Ammo>();

            if (ammo == null)
            {
                return;
            }

            this.CurrentWeapon.AddAmmo(ammo.GetAmmo());
            AmmoUI.text = CurrentWeapon.Ammo.ToString();

            if (other.gameObject == null)
            {
                return;
            }

            Destroy(other.gameObject, .5f);
        }
    }
}
