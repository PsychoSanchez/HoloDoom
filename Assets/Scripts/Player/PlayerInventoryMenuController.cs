using System.Collections;
using System.Collections.Generic;
using HUX.Interaction;
using HUX.Receivers;
using UnityEngine;

public class PlayerInventoryMenuController : InteractionReceiver
{
    protected override void OnTapped(GameObject obj, InteractionManager.InteractionEventArgs eventArgs)
    {
        if (obj == null)
        {
            return;
        }
    }
}
