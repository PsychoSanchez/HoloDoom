using System.Collections;
using System.Collections.Generic;
using UnityEngine.VR.WSA.Input;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerController : OverridableMonoBehaviour
{

    private GestureRecognizer recognizer;
    private PlayerInventory inventory;

    // Use this for initialization
    void Start()
    {
        InitRecognizer();
        inventory = GetComponent<PlayerInventory>();
    }

    private void InitRecognizer()
    {
        recognizer = new GestureRecognizer();
        recognizer.SetRecognizableGestures(GestureSettings.Tap);
        recognizer.TappedEvent += Recognizer_TappedEvent;
        recognizer.StartCapturingGestures();
    }

    private void Recognizer_TappedEvent(InteractionSourceKind source, int tapCount, Ray headRay)
    {
        OnClick(headRay);
    }

    private void OnClick(Ray headRay)
    {
        switch (AppStateManager.Instance.GetAppState())
        {
            case AppState.Playing:
                // Get weapon
                var weapon = inventory.CurrentWeapon;
                if (weapon == null)
                {
                    return;
                }
                if (!weapon.IsReloading)
                {
                    // Get ammo                    
                    var ammo = inventory.GetAmmo(weapon.AmmoType, weapon.DefaultAmmoPerShot);
                    // Shoot
                    weapon.Shoot(headRay, ammo);
                }
                break;
            case AppState.WaitingForAnchor:
                AnchorPlacement.Instance.SendMessage("OnSelect");
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnClick(new Ray(Camera.main.transform.position, Camera.main.transform.forward));
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            // NextWeapon();
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            AppStateManager.Instance.SetAppState(AppState.Playing);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            AppStateManager.Instance.SetAppState(AppState.Ready);
        }
    }
}
