using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fish", menuName = "ScriptableObjects/Fish/Create new", order = 1)]
public class Fish_SO : ScriptableObject
{
    public float Speed;
    public float Awareness;

    [Header("Sprites")]
    public Sprite Sprite;

    [Header("Player Use Only!")]
    [SerializeField]
    private float _defaultPlayerSpeed = 1f;
    public int Generation = 0;
    [SerializeField]
    private float _defaultPlayerAwareness = 0;
    private readonly int _defaultPlayerGen = 0;

    public void Reset()
    {
        Speed = _defaultPlayerSpeed;
        Awareness = _defaultPlayerAwareness;
        Generation = _defaultPlayerGen;
    }
}
