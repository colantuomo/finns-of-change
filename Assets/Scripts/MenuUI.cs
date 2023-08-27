using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [SerializeField]
    private RectTransform _blackPanel;
    public void StartGame()
    {
        _blackPanel.GetComponent<Image>().DOFade(1, 1.2f).OnComplete(() =>
        {
            SceneManager.LoadScene("Game");
        });
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
