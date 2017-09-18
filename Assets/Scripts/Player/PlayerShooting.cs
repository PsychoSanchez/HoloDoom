using System.Collections;
using System.Collections.Generic;
using UnityEngine.VR.WSA.Input;
using UnityEngine;
using UnityEngine.UI;

public class PlayerShooting : OverridableMonoBehaviour
{

    GestureRecognizer recognizer;
    public Weapon CurrentWeapon;
    public Dictionary<WeaponType, Weapon> Weapons;
    public Text AmmoUI;

    // Use this for initialization
    void Start()
    {
        recognizer = new GestureRecognizer();
        recognizer.SetRecognizableGestures(GestureSettings.Tap);
        recognizer.TappedEvent += Recognizer_TappedEvent;
        recognizer.StartCapturingGestures();
        UpdateUI();
    }

    private void Recognizer_TappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
    {
        if (CurrentWeapon == null)
        {
            return;
        }

        CurrentWeapon.Shoot(headRay);
        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CurrentWeapon.Shoot(new Ray(Camera.main.transform.position, Camera.main.transform.forward));
            UpdateUI();
        }
    }
    public void AddAmmo(int amt)
    {
        this.CurrentWeapon.AddAmmo(amt);
        UpdateUI();
    }

    private void UpdateUI()
    {
        AmmoUI.text = CurrentWeapon.Ammo.ToString();
    }
}
