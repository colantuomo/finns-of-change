using Cinemachine;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Fish_SO _playerFish;
    [SerializeField]
    private CinemachineVirtualCamera _mainCam;
    [SerializeField]
    private int _minutesToDie;
    private AudioSource _audioSource;
    [SerializeField]
    private AudioClip _deathAudio, _nextGenAudio;
    public static GameManager Singleton { get; private set; }

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
        ResetPlayerStats();
    }

    private void Start()
    {
        GameEvents.Singleton.OnMinutePassed += OnMinutePassed;
        GameEvents.Singleton.OnResetGame += OnResetGame;
        GameEvents.Singleton.OnPlayerDied += OnPlayerDied;
        GameEvents.Singleton.OnPitchAudioDown += OnPitchAudioDown;
        GameEvents.Singleton.OnUpdatePlayerStats += OnUpdatePlayerStats;
        GameEvents.Singleton.OnEndGame += OnEndGame;
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnEndGame()
    {
        _audioSource.DOFade(0f, 1f);
    }

    private void OnUpdatePlayerStats(Fish_SO obj)
    {
        _audioSource.PlayOneShot(_nextGenAudio);
    }

    private void OnPitchAudioDown()
    {
        PitchAudioDown();
    }

    private void OnPlayerDied()
    {
        _audioSource.PlayOneShot(_deathAudio);
        _audioSource.DOPitch(0f, 2f).OnComplete(() =>
        {
            OnResetGame();
        });
    }

    private void PitchAudioDown()
    {
        DOVirtual.Float(0.8f, 1, 0.7f, (f) =>
        {
            _audioSource.DOPitch(f, 0);
        });

    }

    private void OnResetGame()
    {
        ReloadCurrentScene();
    }

    private void OnDisable()
    {
        GameEvents.Singleton.OnMinutePassed -= OnMinutePassed;
        GameEvents.Singleton.OnResetGame -= OnResetGame;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadCurrentScene();
        }
    }

    private void ReloadCurrentScene()
    {
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    private void OnMinutePassed(int minute)
    {
        if (minute >= _minutesToDie)
        {
            GameEvents.Singleton.PlayerDied();
        }
    }

    private void ResetPlayerStats()
    {
        _playerFish.Reset();
    }

    public void UpdatePlayerStats(float value, OptionTypes option)
    {
        switch (option)
        {
            case OptionTypes.Speed:
                _playerFish.Speed += value;
                break;
            case OptionTypes.Awareness:
                _playerFish.Awareness += value;
                var nextZoom = _mainCam.m_Lens.OrthographicSize + value;
                DOVirtual.Float(_mainCam.m_Lens.OrthographicSize, nextZoom, 0.5f, (f) =>
                {
                    _mainCam.m_Lens.OrthographicSize = f;
                });
                break;
        }
        _playerFish.Generation++;
        GameEvents.Singleton.UpdatePlayerStats(_playerFish);
        GameEvents.Singleton.GameStateChanged(GameStates.Playing);
    }
}
