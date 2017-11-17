using System;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Sharing;
using HoloToolkit.Unity;
using UnityEngine;
using UnityEngine.UI;

public enum UIMode
{
    Game = 0,
    PlacingAnchor,
    Menu,
    None,
    Death,
    WaitingForPlayers,
    Syncing,
    Scanning
}
public class UIManager : Singleton<UIManager>
{
    public GameObject GameHUD;
    public GameObject StatusMessages;
    public GameObject Menu;
    public GameObject PlayerId;
    public GameObject DeathScreen;
    public GameObject StatusSign;

    private Text ammoText;
    private Text armorText;
    private Text healthText;
    private PlayerHeadAnimationComponent playerHead;
    int messageNumber = 0;
    void Start()
    {
        ammoText = GetHUDTextElement("AmmoText");
        armorText = GetHUDTextElement("ArmorText");
        healthText = GetHUDTextElement("HealthText");
        playerHead = GameHUD.transform.Find("HeadAnimation").GetComponent<PlayerHeadAnimationComponent>();

        StatusSign.SetActive(false);
        GameHUD.SetActive(false);
        DeathScreen.SetActive(false);
        StatusMessages.SetActive(true);
        SetPlayerId(SharingStage.Instance.Manager.GetLocalUser().GetID());
    }

    private Text GetHUDTextElement(string text)
    {
        var ammoGO = GameHUD.transform.Find(text);
        if (ammoGO == null)
        {
            Debug.LogWarning(text + " not found");
            return null;
        }

        return ammoGO.gameObject.GetComponent<Text>();
    }

    public void SetPlayerId(long userId)
    {
        PlayerId.GetComponent<Text>().text = "Soldier " + userId.ToString();
    }

    public void LogMessage(string message)
    {
        StatusMessages.GetComponent<Text>().text += "\n#" + messageNumber + " " + message;
        messageNumber++;
    }

    public void SetStatusMessageVisibility(bool visibility)
    {
        StatusMessages.SetActive(visibility);
    }

    public void UpdateAmmoCounter(int ammo)
    {
        if (ammoText == null)
        {
            return;
        }
        ammoText.GetComponent<Text>().text = ammo.ToString();
    }

    public void UpdateHealthAndAmmoCounters(int hp, int ap)
    {
        if (healthText != null)
        {
            healthText.text = hp.ToString();
            playerHead.UpdateAnimation(hp);
        }
        if (armorText != null)
        {
            armorText.text = ap.ToString();
        }
    }

    public void SetMode(UIMode mode)
    {
        HideAllUI();
        switch (mode)
        {
            case UIMode.Game:
                GameHUD.SetActive(true);
                break;
            case UIMode.Death:
                DeathScreen.SetActive(true);
                break;
            case UIMode.Scanning:
                ShowSign("Scanning...");
                break;
            case UIMode.Syncing:
                ShowSign("Syncing...");
                break;
            case UIMode.WaitingForPlayers:
                ShowSign("Waiting for players...");
                break;
            case UIMode.Menu:
                if (Menu != null)
                {
                    Menu.SetActive(true);
                }
                break;
            case UIMode.PlacingAnchor:
                StatusMessages.SetActive(true);
                break;
            case UIMode.None:
            default:
                break;
        }
    }

    private void ShowSign(string msg)
    {
        StatusSign.GetComponent<Text>().text = msg;
        StatusSign.SetActive(true);
    }

    private void HideAllUI()
    {
        StatusSign.SetActive(false);
        GameHUD.SetActive(false);
        DeathScreen.SetActive(false);
        if (Menu != null)
        {
            Menu.SetActive(false);
        }
    }
}
