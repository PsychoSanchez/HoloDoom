using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemotePlayerHealth : OverridableMonoBehaviour
{
    public long Id { get; private set; }

    public void SetId(long id)
    {
        Id = id;
    }

    public void TakeDamage(int amt)
    {
        CustomMessages.Instance.SendRemoteUserRecieveDamage(this.Id, amt);
    }
}
