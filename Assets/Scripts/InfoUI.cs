using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoUI : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            transform.GetComponent<RectTransform>().DOScale(Vector3.zero, 0.5f).SetEase(Ease.InCirc);
        }
    }
}
