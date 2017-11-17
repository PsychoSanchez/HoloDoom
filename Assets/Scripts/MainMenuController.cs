using System.Collections;
using System.Collections.Generic;
using HUX.Interaction;
using HUX.Receivers;
using UnityEngine;

public class MainMenuController : InteractionReceiver
{

    protected override void OnTapped(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
    {
        switch (obj.name)
        {
            case "StartSinglePlayerButton":
                GameManager.Instance.StartSinglePlayerGame();
                break;
            default:
                break;
        }
    }
}
