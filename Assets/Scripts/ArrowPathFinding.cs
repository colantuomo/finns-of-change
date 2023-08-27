using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPathFinding : MonoBehaviour
{

    [SerializeField]
    private List<Transform> _allNextGenSpots = new();
    [SerializeField]
    private Transform _player;
    [SerializeField]
    private float _heightOffset = 0.2f, _arrowDuration = 10f;

    private Vector3 _targetToLook;
    private Vector3 _scale;
    private bool _canUsePathFinding = true;

    private void Start()
    {
        GameEvents.Singleton.OnShowPathDirection += OnShowPathDirection;
        GameEvents.Singleton.OnMinutePassed += OnMinutePassed;
        GameEvents.Singleton.OnReachedNextGenSelection += OnReachedNextGenSelection;
        _targetToLook = Vector3.zero;
        _scale = transform.localScale;
        HideArrow();
    }

    private void OnReachedNextGenSelection(Transform obj)
    {
        HideArrow();
    }

    private void OnMinutePassed(int minute)
    {
        _canUsePathFinding = true;
    }

    private void OnShowPathDirection()
    {
        if (!_canUsePathFinding) return;
        _canUsePathFinding = false;
        SetSpotToPoint();
        ShowArrow();
    }

    private void SetSpotToPoint()
    {
        _targetToLook = Vector3.zero;
        float nearDistance = Mathf.Infinity;
        _allNextGenSpots.ForEach((spot) =>
        {
            var distance = Vector3.Distance(transform.position, spot.position);
            if (!spot.gameObject.activeSelf) return;
            if (distance < nearDistance)
            {
                nearDistance = distance;
                _targetToLook = spot.position;
            }
        });
    }

    private void Update()
    {
        transform.position = new Vector2(_player.position.x, _player.position.y + _heightOffset);
        RotateToTarget();
    }

    private void RotateToTarget()
    {
        if (_targetToLook == Vector3.zero) return;

        Vector3 targetDirection = _targetToLook - transform.position;
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 60f * Time.deltaTime);
    }

    private void ShowArrow()
    {
        transform.DOKill();
        transform.DOScale(_scale, .2f).SetEase(Ease.OutBounce).OnComplete(() =>
        {
            HideArrow(_arrowDuration);
        });
    }

    private Tweener HideArrow(float duration = 0f)
    {
        transform.DOKill();
        return transform.DOScale(Vector3.zero, duration).OnComplete(() =>
        {
            _targetToLook = Vector3.zero;
        });
    }
}
