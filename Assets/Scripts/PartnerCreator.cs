using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartnerCreator : MonoBehaviour
{
    [SerializeField]
    private Fish_SO _fish;
    [SerializeField]
    private Transform _eyePosition, _finPosition;

    private void Start()
    {
        //var fin = Instantiate(_fish.Fin, _finPosition.position, Quaternion.identity);
        //var eye = Instantiate(_fish.Eye, _eyePosition.position, Quaternion.identity);
        //fin.SetParent(transform);
        //eye.SetParent(transform);
    }
}
