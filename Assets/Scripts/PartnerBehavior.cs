using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PartnerBehavior : MonoBehaviour
{

    [SerializeField]
    private Transform _placeholder;
    [SerializeField]
    private Fish_SO _playerFish;
    private Fish_SO _currentFish;

    [SerializeField]
    private List<Fish_SO> _allFishes = new();

    private Vector3 _targetToFollow = Vector3.zero;
    private SpriteRenderer _spriteRenderer;
    private Vector3 _defaultScale;
    private float _speed = 1f;

    private void Start()
    {
        GameEvents.Singleton.OnUpdatePlayerStats += OnUpdatePlayerStats;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _defaultScale = _placeholder.transform.localScale;
        UpdateSpeed();
        SetRandomSprite();
    }

    private void OnUpdatePlayerStats(Fish_SO obj)
    {
        UpdateSpeed();
    }

    private void UpdateSpeed()
    {
        _speed = _playerFish.Speed - 0.1f;
    }

    private void Update()
    {
        if (_targetToFollow != Vector3.zero)
        {
            MoveTowardsTarget(_targetToFollow);
            RotateToTarget(_targetToFollow);
        }
    }

    private void SetRandomSprite()
    {
        var random = new System.Random();
        int index = random.Next(_allFishes.Count);
        _currentFish = _allFishes[index];
        _spriteRenderer.sprite = _currentFish.Sprite;
    }

    public Fish_SO Stats()
    {
        return _currentFish;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.TryGetComponent(out FishController fish))
        {
            if (_targetToFollow != Vector3.zero || GameEvents.Singleton.CurrentGameState != GameStates.Playing) return;
            EnablePlaceholder();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.TryGetComponent(out FishController fish))
        {
            DisablePlaceholder();
        }
    }

    private void EnablePlaceholder()
    {
        _placeholder.gameObject.SetActive(true);
        _placeholder.transform.localScale = Vector3.zero;
        _placeholder.transform.DOScale(_defaultScale, .5f).SetEase(Ease.OutBounce);
    }

    private void DisablePlaceholder()
    {
        _placeholder.transform.DOScale(Vector3.zero, .1f).OnComplete(() =>
        {
            _placeholder.gameObject.SetActive(false);
        });
    }

    public void SetTargetToFollow(Vector3 target)
    {
        _targetToFollow = target;
        DisablePlaceholder();
    }

    private void MoveTowardsTarget(Vector3 target)
    {
        transform.position = Vector2.MoveTowards(transform.position, target, Time.deltaTime * _speed);
    }

    private void RotateToTarget(Vector3 targetPos)
    {
        Vector3 targetDirection = targetPos - transform.position;
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 55f * Time.deltaTime);
        if (angle < -90f || angle > 90f)
        {
            transform.DOScaleX(-1f, .1f);
            _spriteRenderer.flipX = true;
            _spriteRenderer.flipY = true;
        }
        else
        {
            transform.DOScaleX(1f, .1f);
            _spriteRenderer.flipX = false;
            _spriteRenderer.flipY = false;
        }
    }
}
