using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct RelayEvent
{
    public string name;
    public UnityEvent relayEvent; 
}

public class AniEventRelay : MonoBehaviour
{
    [SerializeField]
    RelayEvent[] relayEvents;

    public void CallRelayEvent(string name)
    {
        foreach (RelayEvent relayEvent in relayEvents)
        {
            if (relayEvent.name == name)
            {
                relayEvent.relayEvent.Invoke();
            }
        }
    }
}