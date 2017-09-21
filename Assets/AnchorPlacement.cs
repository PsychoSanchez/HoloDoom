using System.Collections;
using System.Collections.Generic;
using HoloToolkit.Sharing;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class AnchorPlacement : Singleton<AnchorPlacement>
{
    public GameObject cursorObj;

    public bool GotTransform { get; private set; }
    ObjectCursor cursor;
    GameObject anchorObj;
    // Use this for initialization
    void Start()
    {
        GotTransform = false;
        cursor = this.cursorObj.GetComponent<ObjectCursor>();

        // We care about getting updates for the anchor transform.
        CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.StageTransform] = this.OnStageTransfrom;

        // And when a new user join we will send the anchor transform we have.
        SharingSessionTracker.Instance.SessionJoined += Instance_SessionJoined;
    }

    private void Instance_SessionJoined(object sender, SharingSessionTracker.SessionJoinedEventArgs e)
    {
        if (GotTransform)
        {
            CustomMessages.Instance.SendStageTransform(transform.localPosition, transform.localRotation);
        }
    }

    void OnStageTransfrom(NetworkInMessage msg)
    {
        // We read the user ID but we don't use it here.
        msg.ReadInt64();

        transform.localPosition = CustomMessages.Instance.ReadVector3(msg);
        transform.localRotation = CustomMessages.Instance.ReadQuaternion(msg);

        if (GotTransform == false)
        {
            AppStateManager.Instance.SetCurrentAppState(AppState.Ready);
        }

        GotTransform = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (AppStateManager.Instance.GetCurrentAppState() != AppState.WaitingForAnchor)
        {
            return;
        }
        if (GotTransform)
        {
            if (ImportExportAnchorManager.Instance.AnchorEstablished)
            {

            }
        }
        else if (cursor != null)
        {
            transform.position = cursor.Position;
            transform.rotation = cursor.Rotation;
        }
    }

    public void OnSelect()
    {
        // Note that we have a transform.
        GotTransform = true;

        // And send it to our friends.
        CustomMessages.Instance.SendStageTransform(transform.localPosition, transform.localRotation);
        AppStateManager.Instance.SetCurrentAppState(AppState.Ready);
    }
}
