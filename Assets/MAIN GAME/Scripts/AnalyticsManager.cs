using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager instance;

    public bool isEnable;

    [HideInInspector] public EventType eventType;

    public enum EventType
    {
        StartEvent,
        EndEvent
    }

    private void Awake()
    {
        instance = this;
    }
}
