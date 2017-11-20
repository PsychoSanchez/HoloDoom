using System;
using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Sharing;
using HUX.Interaction;
using HUX.Receivers;
using UnityEngine;

public class MainMenuController : InteractionReceiver
{
    public int ReleaseMenuButtonAfter = 3;
    DateTime startTime;
    protected override void OnTapped(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
    {
        if (obj == null)
        {
            return;
        }
        switch (obj.name)
        {
            case "StartSinglePlayerButton":
                GameManager.Instance.StartSinglePlayerGame();
                startTime = DateTime.Now;
                break;
            case "ConnectServerButton":
                SharingStage.Instance.ConnectToServer(SharingStage.Instance.ServerAddress, SharingStage.Instance.ServerPort);
                break;
            case "Anchor":
                if ((DateTime.Now - startTime).Seconds > ReleaseMenuButtonAfter)
                {
                    GameManager.Instance.StopGame();
                }
                break;
            default:
                break;
        }
    }
}
