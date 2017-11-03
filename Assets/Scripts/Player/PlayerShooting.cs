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
        OnClick(headRay);
    }

    private void OnClick(Ray headRay)
    {
        switch (AppStateManager.Instance.GetAppState())
        {
            case AppState.Playing:
                if (CurrentWeapon == null)
                {
                    return;
                }
                CurrentWeapon.Shoot(headRay);
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
