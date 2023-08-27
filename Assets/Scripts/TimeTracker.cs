using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeTracker : MonoBehaviour
{
    private int minuteCounter = 0;
    private float elapsedTime = 0f;
    [SerializeField]
    private float updateInterval = 60f;

    private void Start()
    {
        GameEvents.Singleton.OnResetTimer += OnResetTimer;
    }

    private void OnResetTimer()
    {
        minuteCounter = 0;
        elapsedTime = 0f;
    }

    void Update()
    {
        CountTime();
    }


    private void CountTime()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= updateInterval)
        {
            minuteCounter++;
            elapsedTime = 0f;
            GameEvents.Singleton.MinutePassed(minuteCounter);
        }
    }
}
