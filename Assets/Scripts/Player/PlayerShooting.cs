using System.Collections;
using System.Collections.Generic;
using UnityEngine.VR.WSA.Input;
using UnityEngine;

public class PlayerShooting : MonoBehaviour {

    GestureRecognizer recognizer;
    public Weapon CurrentWeapon;
    public Weapon[] Weapons;

    // Use this for initialization
    void Start () {
        recognizer = new GestureRecognizer();
        recognizer.SetRecognizableGestures(GestureSettings.Tap);
        recognizer.TappedEvent += Recognizer_TappedEvent;
        recognizer.StartCapturingGestures();
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
        // if (Input.GetMouseButtonDown(0))
        // {
        //    CurrentWeapon.Shoot(new Ray());
        // }
    }
}
