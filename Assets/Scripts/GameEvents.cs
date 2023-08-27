using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum GameStates
{
    Playing,
    ChoosingNextGen
}

public class GameEvents : MonoBehaviour
{
    public static GameEvents Singleton { get; private set; }
    public GameStates CurrentGameState { get; private set; }

    private void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Singleton = this;
        }
    }

    public event Action<Transform> OnReachedNextGenSelection;
    public void ReachedNextGenSelection(Transform nextGen)
    {
        OnReachedNextGenSelection?.Invoke(nextGen);
    }

    public event Action<GameStates> OnGameStateChanged;
    public void GameStateChanged(GameStates state)
    {
        CurrentGameState = state;
        OnGameStateChanged?.Invoke(state);
    }

    public event Action<Fish_SO> OnUpdatePlayerStats;
    public void UpdatePlayerStats(Fish_SO fish)
    {
        OnUpdatePlayerStats?.Invoke(fish);
    }

    public event Action<int> OnMinutePassed;
    public void MinutePassed(int minute)
    {
        OnMinutePassed?.Invoke(minute);
    }

    public event Action OnResetTimer;
    public void ResetTimer()
    {
        OnResetTimer?.Invoke();
    }

    public event Action OnShowPathDirection;
    public void ShowPathDirection()
    {
        OnShowPathDirection?.Invoke();
    }

    public event Action OnPlayerDied;
    public void PlayerDied()
    {
        OnPlayerDied?.Invoke();
    }

    public event Action OnResetGame;
    public void ResetGame()
    {
        OnResetGame?.Invoke();
    }

    public event Action OnEndGame;
    public void EndGame()
    {
        OnEndGame?.Invoke();
    }

    public event Action OnPitchAudioDown;
    public void PitchAudioDown()
    {
        OnPitchAudioDown?.Invoke();
    }

    public event Action OnPlayerGotHit;
    public void PlayerGotHit()
    {
        OnPlayerGotHit?.Invoke();
    }
}
