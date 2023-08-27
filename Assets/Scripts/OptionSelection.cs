using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum OptionTypes
{
    Speed,
    Awareness
}

public class OptionSelection : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _optionText;
    private float _value;
    private OptionTypes _type;

    public void SetOption(float value, OptionTypes type)
    {
        _optionText.text = Sign(value) + " \n" + GetOptionText(type);
        _value = value;
        _type = type;
    }

    private string Sign(float value)
    {
        return value < 0 ? "-" : "+";
    }

    private string GetOptionText(OptionTypes type)
    {
        return type == OptionTypes.Speed ? "Speed" : "Awareness";
    }

    public event Action<float, OptionTypes> OnOptionSelected;
    public void OptionSelected()
    {
        OnOptionSelected?.Invoke(_value, _type);
    }
}
