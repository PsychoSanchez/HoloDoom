using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Sharing;
using HoloToolkit.Unity;
using UnityEngine;
using UnityEngine.UI;

public enum UIMode
{
    Game = 0,
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

    int messageNumber = 0;
    void Start()
    {
        StatusSign.SetActive(false);
        GameHUD.SetActive(false);
        DeathScreen.SetActive(false);
        StatusMessages.SetActive(true);
        if (Menu != null)
        {
            Menu.SetActive(false);
        }
        SetPlayerId(SharingStage.Instance.Manager.GetLocalUser().GetID());
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
