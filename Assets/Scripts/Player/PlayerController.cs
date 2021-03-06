﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.VR.WSA.Input;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerController : OverridableMonoBehaviour
{
    GestureRecognizer recognizer;
    public Weapon[] Weapons;
    public int CurrentWeapon;

    // Use this for initialization
    void Start()
    {
        recognizer = new GestureRecognizer();
        recognizer.SetRecognizableGestures(GestureSettings.Tap);
        recognizer.SetRecognizableGestures(GestureSettings.Hold);
        recognizer.TappedEvent += Recognizer_TappedEvent;
        recognizer.HoldCompletedEvent += HoldCompletedEvent;
        recognizer.StartCapturingGestures();
        HideWeapons();
        Weapons[CurrentWeapon].gameObject.SetActive(true);
        UpdateUI();
    }

    private void HoldCompletedEvent(InteractionSourceKind source, Ray headRay)
    {
        Debug.Log("Hold");
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
                if (CurrentWeapon > Weapons.Length || Weapons[CurrentWeapon] == null)
                {
                    return;
                }
                Weapons[CurrentWeapon].Shoot(headRay);
                UpdateUI();
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
            NextWeapon();
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

    private void NextWeapon()
    {
        HideWeapons();
        CurrentWeapon++;
        if (CurrentWeapon >= Weapons.Length)
        {
            CurrentWeapon = 0;
        }
        Weapons[CurrentWeapon].gameObject.SetActive(true);
        UpdateUI();
    }

    private void HideWeapons()
    {
        foreach (var weapon in Weapons)
        {
            if (weapon == null)
            {
                continue;
            }
            weapon.gameObject.SetActive(false);
        }
    }

    public void AddAmmo(int amt)
    {
        Weapons[CurrentWeapon].AddAmmo(amt);
        UpdateUI();
    }
    private void UpdateUI()
    {
        UIManager.Instance.UpdateAmmoCounter(Weapons[CurrentWeapon].Ammo);
    }
}
