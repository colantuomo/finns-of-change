using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextGenSelection : MonoBehaviour
{
    private List<Transform> options = new List<Transform>();
    [SerializeField]
    private OptionSelection _optionSelectionA, _optionSelectionB;
    private void Start()
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            var option = transform.GetChild(i);
            options.Add(option);
        }
        DisableOptions();
        options[2].gameObject.SetActive(true);
        _optionSelectionA.OnOptionSelected += OnOptionSelected;
        _optionSelectionB.OnOptionSelected += OnOptionSelected;
    }

    private void OnOptionSelected(float value, OptionTypes option)
    {
        DisableOptions();
        GameManager.Singleton.UpdatePlayerStats(value, option);
        transform.DOScale(Vector3.zero, 1f).SetEase(Ease.InBounce).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.TryGetComponent(out FishController fish))
        {
            if (!fish.HasParner()) return;

            GameEvents.Singleton.ResetTimer();
            GameEvents.Singleton.GameStateChanged(GameStates.ChoosingNextGen);
            GameEvents.Singleton.ReachedNextGenSelection(transform);
        }
    }

    private void EnableOptions()
    {
        options.ForEach((option) =>
        {
            option.gameObject.SetActive(true);
        });
    }

    private void DisableOptions()
    {
        options.ForEach((option) =>
        {
            option.gameObject.SetActive(false);
        });
    }

    public void ShowOptionsPartnerBased(Fish_SO player, Fish_SO partner)
    {
        var speed = partner.Speed;
        var awareness = partner.Awareness;
        options[0].GetComponent<OptionSelection>().SetOption(speed, OptionTypes.Speed);
        options[1].GetComponent<OptionSelection>().SetOption(awareness, OptionTypes.Awareness);
        EnableOptions();
    }
}
