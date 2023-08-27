using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{

    [SerializeField]
    private float _searchPlayerRadius = 1f;
    [SerializeField]
    private LayerMask _playerMask;

    private SpriteRenderer _spriteRenderer;
    private bool _canAttack = true;

    private Vector3 _localScale = Vector3.one;
    private float _speed = 1.5f;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _localScale = transform.localScale;
        GameEvents.Singleton.OnUpdatePlayerStats += OnUpdatePlayerStats;
    }

    private void OnUpdatePlayerStats(Fish_SO fish)
    {
        _speed = fish.Speed - 0.5f;
    }

    private void Update()
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position, _searchPlayerRadius, _playerMask);
        if (player != null && _canAttack && IsPlaying())
        {
            MoveTowardsTarget(player.transform.position);
            RotateToTarget(player.transform.position);
        }
    }

    private bool IsPlaying()
    {
        return GameEvents.Singleton.CurrentGameState == GameStates.Playing;
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
            transform.DOScaleX(-_localScale.x, .2f);
            _spriteRenderer.flipX = true;
            _spriteRenderer.flipY = true;
        }
        else
        {
            transform.DOScaleX(_localScale.x, .2f);
            _spriteRenderer.flipX = false;
            _spriteRenderer.flipY = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _searchPlayerRadius);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!IsPlaying()) return;
        if (collision.transform.TryGetComponent(out FishController player) && _canAttack)
        {
            _canAttack = false;
            transform.DOMove(player.transform.position, 0.2f).SetEase(Ease.InBounce).OnComplete(() =>
            {
                player.TakeDamage();
                DOVirtual.Float(0, 1, 2f, (f) => { }).OnComplete(() =>
                {
                    _canAttack = true;
                });
            });
        }
    }
}
