using HoloToolkit.Sharing;
using HoloToolkit.Unity;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CustomMessages : Singleton<CustomMessages>
{
    /// <summary>
    /// Message enum containing our information bytes to share.
    /// The first message type has to start with UserMessageIDStart
    /// so as not to conflict with HoloToolkit internal messages.
    /// </summary>
    public enum GameMessageID
    {
        UserHeadTransform = MessageID.UserMessageIDStart,
        UserAvatar,
        MatchPlaying,
        UserHealthUpdated,
        RemoteUserReceiveDamage,
        EnemyHit,
        ShootProjectile,
        SpawnEnemy,
        PlayerFound,
        EnemyTransform,
        EnemyDeath,
        StageTransform,
        UpdateAppState,
        ResetStage,
        ExplodeTarget,
        Max
    }

    public enum UserMessageChannels
    {
        Anchors = MessageChannel.UserMessageChannelStart,
    }

    /// <summary>
    /// Cache the local user's ID to use when sending messages
    /// </summary>
    public long localUserID
    {
        get; set;
    }

    public delegate void MessageCallback(NetworkInMessage msg);
    private Dictionary<GameMessageID, MessageCallback> _MessageHandlers = new Dictionary<GameMessageID, MessageCallback>();
    public Dictionary<GameMessageID, MessageCallback> MessageHandlers
    {
        get
        {
            return _MessageHandlers;
        }
    }

    /// <summary>
    /// Helper object that we use to route incoming message callbacks to the member
    /// functions of this class
    /// </summary>
    NetworkConnectionAdapter connectionAdapter;

    /// <summary>
    /// Cache the connection object for the sharing service
    /// </summary>
    NetworkConnection serverConnection;

    void Start()
    {
        InitializeMessageHandlers();
    }

    void InitializeMessageHandlers()
    {
        SharingStage sharingStage = SharingStage.Instance;
        if (sharingStage != null)
        {
            serverConnection = sharingStage.Manager.GetServerConnection();
            connectionAdapter = new NetworkConnectionAdapter();
        }

        connectionAdapter.MessageReceivedCallback += OnMessageReceived;

        // Cache the local user ID
        this.localUserID = SharingStage.Instance.Manager.GetLocalUser().GetID();

        for (byte index = (byte)GameMessageID.UserHeadTransform; index < (byte)GameMessageID.Max; index++)
        {
            if (MessageHandlers.ContainsKey((GameMessageID)index) == false)
            {
                MessageHandlers.Add((GameMessageID)index, null);
            }

            serverConnection.AddListener(index, connectionAdapter);
        }
    }

    private NetworkOutMessage CreateMessage(byte MessageType)
    {
        NetworkOutMessage msg = serverConnection.CreateMessage(MessageType);
        msg.Write(MessageType);
        // Add the local userID so that the remote clients know whose message they are receiving
        msg.Write(localUserID);
        return msg;
    }

    public void SendHeadTransform(Vector3 position, Quaternion rotation, byte HasAnchor)
    {
        // If we are connected to a session, broadcast our head info
        if (!IsConnected())
        {
            return;
        }

        // Create an outgoing network message to contain all the info we want to send
        NetworkOutMessage msg = CreateMessage((byte)GameMessageID.UserHeadTransform);

        AppendTransform(msg, position, rotation);

        msg.Write(HasAnchor);

        // Send the message as a broadcast, which will cause the server to forward it to all other users in the session.
        this.serverConnection.Broadcast(
            msg,
            MessagePriority.Immediate,
            MessageReliability.UnreliableSequenced,
            MessageChannel.Avatar);
    }

    public void SetMatchPlaying(bool bPlaying)
    {
        if (!IsConnected())
        {
            return;
        }
        // Create an outgoing network message to contain all the info we want to send
        NetworkOutMessage msg = CreateMessage((byte)GameMessageID.MatchPlaying);
        msg.Write((bPlaying) ? 1 : 0);

        this.serverConnection.Broadcast(
          msg,
          MessagePriority.Immediate,
          MessageReliability.Reliable,
          MessageChannel.Avatar);
    }

    public void SendAppState(AppState state)
    {
        if (!IsConnected())
        {
            return;
        }
        // Create an outgoing network message to contain all the info we want to send
        NetworkOutMessage msg = CreateMessage((byte)GameMessageID.UpdateAppState);
        msg.Write((Int16)state);

        this.serverConnection.Broadcast(
          msg,
          MessagePriority.Immediate,
          MessageReliability.Reliable,
          MessageChannel.Avatar);
    }

    public void SendUserHealthUpdate(int health)
    {
        if (!IsConnected())
        {
            return;
        }
        // Create an outgoing network message to contain all the info we want to send
        NetworkOutMessage msg = CreateMessage((byte)GameMessageID.UserHealthUpdated);
        msg.Write(health);
        // Send the message as a broadcast, which will cause the server to forward it to all other users in the session.
        this.serverConnection.Broadcast(
            msg,
            MessagePriority.Immediate,
            MessageReliability.Reliable,
            MessageChannel.Avatar);
    }
    public void SendRemoteUserReceiveDamage(long remoteUserId, int damage)
    {
        if (!IsConnected())
        {
            return;
        }

        // Create an outgoing network message to contain all the info we want to send
        NetworkOutMessage msg = CreateMessage((byte)GameMessageID.RemoteUserReceiveDamage);
        msg.Write(remoteUserId);
        msg.Write(damage);

        // Send the message as a broadcast, which will cause the server to forward it to all other users in the session.
        this.serverConnection.Broadcast(
            msg,
            MessagePriority.Immediate,
            MessageReliability.Reliable,
            MessageChannel.Avatar);
    }
    public void SendShootProjectile(long enemyId, long projId, Vector3 position, Quaternion rotation)
    {
        if (!IsConnected())
        {
            return;
        }
        // Create an outgoing network message to contain all the info we want to send
        NetworkOutMessage msg = CreateMessage((byte)GameMessageID.ShootProjectile);
        msg.Write(enemyId);
        msg.Write(projId);
        AppendVector3(msg, position);
        AppendQuaternion(msg, rotation);

        // Send the message as a broadcast, which will cause the server to forward it to all other users in the session.
        this.serverConnection.Broadcast(
            msg,
            MessagePriority.Immediate,
            MessageReliability.Reliable,
            MessageChannel.Avatar);
    }

    public void SendSpawnEnemy(long enemyID, Vector3 position, Quaternion rotation)
    {
        if (!IsConnected())
        {
            return;
        }
        // Create an outgoing network message to contain all the info we want to send
        NetworkOutMessage msg = CreateMessage((byte)GameMessageID.SpawnEnemy);

        msg.Write(enemyID);
        AppendTransform(msg, position, rotation);

        // Send the message as a broadcast, which will cause the server to forward it to all other users in the session.
        this.serverConnection.Broadcast(
            msg,
            MessagePriority.Immediate,
            MessageReliability.Reliable,
            MessageChannel.Avatar);
    }

    public void MonsterFoundPlayer(long enemyId)
    {
        if (!IsConnected())
        {
            return;
        }

        NetworkOutMessage msg = CreateMessage((byte)GameMessageID.PlayerFound);
        msg.Write(enemyId);

        this.serverConnection.Broadcast(
                 msg,
                 MessagePriority.Immediate,
                 MessageReliability.Reliable,
                 MessageChannel.Avatar);
    }

    public void SendEnemyTransform(long enemyId, Vector3 position)
    {
        if (!IsConnected())
        {
            return;
        }
        // Create an outgoing network message to contain all the info we want to send
        NetworkOutMessage msg = CreateMessage((byte)GameMessageID.EnemyTransform);

        msg.Write(enemyId);
        AppendVector3(msg, position);

        // Send the message as a broadcast, which will cause the server to forward it to all other users in the session.
        this.serverConnection.Broadcast(
            msg,
            MessagePriority.Immediate,
            MessageReliability.Reliable,
            MessageChannel.Avatar);
    }

    public void SendEnemyHit(long enemyId, int amt)
    {
        if (!IsConnected())
        {
            return;
        }
        // Create an outgoing network message to contain all the info we want to send
        NetworkOutMessage msg = CreateMessage((byte)GameMessageID.EnemyHit);

        msg.Write(enemyId);
        msg.Write(amt);

        // Send the message as a broadcast, which will cause the server to forward it to all other users in the session.
        this.serverConnection.Broadcast(
            msg,
            MessagePriority.Medium,
            MessageReliability.ReliableOrdered,
            MessageChannel.Avatar);
    }

    public void SendUserAvatar(int UserAvatarID)
    {
        if (!IsConnected())
        {
            return;
        }
        // Create an outgoing network message to contain all the info we want to send
        NetworkOutMessage msg = CreateMessage((byte)GameMessageID.UserAvatar);

        msg.Write(UserAvatarID);

        // Send the message as a broadcast, which will cause the server to forward it to all other users in the session.
        this.serverConnection.Broadcast(
            msg,
            MessagePriority.Medium,
            MessageReliability.Reliable,
            MessageChannel.Avatar);
    }

    public void SendStageTransform(Vector3 position, Quaternion rotation)
    {
        if (!IsConnected())
        {
            return;
        }
        // Create an outgoing network message to contain all the info we want to send
        NetworkOutMessage msg = CreateMessage((byte)GameMessageID.StageTransform);

        AppendTransform(msg, position, rotation);

        // Send the message as a broadcast, which will cause the server to forward it to all other users in the session.
        this.serverConnection.Broadcast(
            msg,
            MessagePriority.Immediate,
            MessageReliability.ReliableOrdered,
            MessageChannel.Avatar);
    }

    public void SendResetStage()
    {
        if (!IsConnected())
        {
            return;
        }
        // Create an outgoing network message to contain all the info we want to send
        NetworkOutMessage msg = CreateMessage((byte)GameMessageID.ResetStage);

        // Send the message as a broadcast, which will cause the server to forward it to all other users in the session.
        this.serverConnection.Broadcast(
            msg,
            MessagePriority.Immediate,
            MessageReliability.ReliableOrdered,
            MessageChannel.Avatar);
    }

    public void SendExplodeTarget()
    {
        if (!IsConnected())
        {
            return;
        }
        // Create an outgoing network message to contain all the info we want to send.
        NetworkOutMessage msg = CreateMessage((byte)GameMessageID.ExplodeTarget);

        // Send the message as a broadcast, which will cause the server to forward it to all other users in the session.
        this.serverConnection.Broadcast(
            msg,
            MessagePriority.Immediate,
            MessageReliability.ReliableOrdered,
            MessageChannel.Avatar);
    }

    protected override void OnDestroy()
    {
        if (this.serverConnection != null)
        {
            for (byte index = (byte)GameMessageID.UserHeadTransform; index < (byte)GameMessageID.Max; index++)
            {
                this.serverConnection.RemoveListener(index, this.connectionAdapter);
            }
            this.connectionAdapter.MessageReceivedCallback -= OnMessageReceived;
        }

        base.OnDestroy();
    }

    void OnMessageReceived(NetworkConnection connection, NetworkInMessage msg)
    {
        byte messageType = msg.ReadByte();
        MessageCallback messageHandler = MessageHandlers[(GameMessageID)messageType];
        if (messageHandler != null)
        {
            messageHandler(msg);
        }
    }


    private bool IsConnected()
    {
        return this.serverConnection != null && this.serverConnection.IsConnected();
    }

    #region HelperFunctionsForWriting
    void AppendTransform(NetworkOutMessage msg, Vector3 position, Quaternion rotation)
    {
        AppendVector3(msg, position);
        AppendQuaternion(msg, rotation);
    }

    void AppendVector3(NetworkOutMessage msg, Vector3 vector)
    {
        msg.Write(vector.x);
        msg.Write(vector.y);
        msg.Write(vector.z);
    }

    void AppendQuaternion(NetworkOutMessage msg, Quaternion rotation)
    {
        msg.Write(rotation.x);
        msg.Write(rotation.y);
        msg.Write(rotation.z);
        msg.Write(rotation.w);
    }

    #endregion HelperFunctionsForWriting

    #region HelperFunctionsForReading

    public Vector3 ReadVector3(NetworkInMessage msg)
    {
        return new Vector3(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
    }

    public Quaternion ReadQuaternion(NetworkInMessage msg)
    {
        return new Quaternion(msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat(), msg.ReadFloat());
    }

    #endregion HelperFunctionsForReading
}