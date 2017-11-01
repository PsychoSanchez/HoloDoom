﻿using System.Collections;
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
    Death
}
public class UIManager : Singleton<UIManager>
{
    public GameObject GameHUD;
    public GameObject StatusMessages;
    public GameObject Menu;
    public GameObject PlayerId;
    public GameObject DeathScreen;

    int messageNumber = 0;
    void Start()
    {
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
        switch (mode)
        {
            case UIMode.Game:
                if (Menu != null)
                {
                    Menu.SetActive(false);
                }
                DeathScreen.SetActive(false);
                GameHUD.SetActive(true);
                break;
            case UIMode.Death:
                DeathScreen.SetActive(true);
                GameHUD.SetActive(false);
                break;
            case UIMode.Menu:
                if (Menu != null)
                {
                    Menu.SetActive(true);
                }
                GameHUD.SetActive(false);
                break;
            case UIMode.None:
                if (Menu != null)
                {
                    Menu.SetActive(false);
                }
                GameHUD.SetActive(false);
                break;
        }
    }

}
