using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _genText, _dayText;
    [SerializeField]
    private int _minutePreDeath = 5;
    [SerializeField]
    private RectTransform _blackScreenPanel, _endMenuText;

    [SerializeField]
    private Image _fishLifeImage;
    [SerializeField]
    private List<Sprite> _lifeImages = new();

    private Vector3 _localTextDayScale;
    private int _currentFishSprite = 0;

    private void Start()
    {
        GameEvents.Singleton.OnMinutePassed += OnMinutePassed;
        GameEvents.Singleton.OnUpdatePlayerStats += OnUpdatePlayerStats;
        GameEvents.Singleton.OnResetTimer += OnResetTimer;
        GameEvents.Singleton.OnPlayerDied += OnPlayerDied;
        GameEvents.Singleton.OnEndGame += OnEndGame;
        GameEvents.Singleton.OnPlayerGotHit += OnPlayerGotHit;
        _localTextDayScale = _dayText.rectTransform.localScale;
    }

    private void OnPlayerGotHit()
    {
        _fishLifeImage.transform.DOShakeScale(.1f);
        _currentFishSprite++;
        if (_currentFishSprite + 1 == _lifeImages.Count)
        {
            _fishLifeImage.DOColor(Color.red, 1f);
        }
        if (_currentFishSprite > _lifeImages.Count - 1) return;
        _fishLifeImage.sprite = _lifeImages[_currentFishSprite];
    }

    private void OnEndGame()
    {
        GameEvents.Singleton.ResetTimer();
        _blackScreenPanel.GetComponent<Image>().DOFade(1, 1f);
        _blackScreenPanel.GetComponent<Image>().DOColor(Color.white, 1f);
        DOVirtual.Float(0, 1f, 3f, (e) => { }).OnComplete(() =>
        {
            LoadEndScene();
        });
    }

    private void LoadEndScene()
    {
        //_endMenuText.gameObject.SetActive(true);
        //_endMenuText.DOScale(Vector3.one, 5f).SetEase(Ease.OutElastic);
        SceneManager.LoadScene("Menu Final");
    }

    private void OnPlayerDied()
    {
        _blackScreenPanel.GetComponent<Image>().DOFade(1, 2f);
    }

    private void OnResetTimer()
    {
        OnMinutePassed(0);
        ResetDayTextSize();
    }

    private void OnUpdatePlayerStats(Fish_SO fish)
    {
        _genText.text = "Gen: " + fish.Generation;
    }

    private void OnMinutePassed(int minute)
    {
        if (minute == _minutePreDeath)
        {
            _dayText.rectTransform.DOScale(Vector3.one + Vector3.one, 1f);
            _dayText.DOColor(Color.red, .5f);
        }
        else if (minute < _minutePreDeath)
        {
            TriggerOnMinutePassTextEffect();
        }
        _dayText.text = "Day: " + minute.ToString();
    }

    private void ResetDayTextSize()
    {
        _dayText.DOColor(Color.white, .2f);
        _dayText.rectTransform.DOScale(_localTextDayScale, .2f);
    }

    private void TriggerOnMinutePassTextEffect()
    {
        _dayText.rectTransform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), .5f).SetEase(Ease.OutBounce).OnComplete(() =>
        {
            _dayText.rectTransform.DOScale(_localTextDayScale, .5f);
        });
    }

    public void GoToMainMenu()
    {
        print("Main Menu Works?");
        SceneManager.LoadScene("Menu Initial");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
