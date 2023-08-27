using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class FishController : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3f, _rotationSpeed = 60f, _findPartnerRadius = 2f;
    [SerializeField] private LayerMask _partnerLayerMask;

    private PartnerBehavior _currentPartner;
    private SpriteRenderer _spriteRenderer;

    private Fish _fish;

    private bool _canMove = false;
    [SerializeField]
    private int _life = 3;

    [SerializeField]
    private CinemachineVirtualCamera _mainCam;
    private CinemachineBasicMultiChannelPerlin _mainCamMultiChannelPerlin;

    private void Start()
    {

        GameEvents.Singleton.OnReachedNextGenSelection += OnReachedNextGenSelection;
        GameEvents.Singleton.OnUpdatePlayerStats += OnUpdatePlayerStats;
        GameEvents.Singleton.OnPlayerDied += OnPlayerDied;

        _mainCamMultiChannelPerlin = _mainCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _fish = GetComponent<Fish>();
        UpdateStats();
    }

    private void OnDisable()
    {
        GameEvents.Singleton.OnReachedNextGenSelection -= OnReachedNextGenSelection;
        GameEvents.Singleton.OnUpdatePlayerStats -= OnUpdatePlayerStats;
        GameEvents.Singleton.OnPlayerDied -= OnPlayerDied;
    }

    private void OnPlayerDied()
    {
        Die();
    }

    private void OnUpdatePlayerStats(Fish_SO fishSO)
    {
        UpdateStats();
        GoToNextGeneration();
    }

    private void GoToNextGeneration()
    {
        if (_fish.Stats.Generation > _fish.Evolutions.Count - 1) return;
        _spriteRenderer.sprite = _fish.Evolutions[_fish.Stats.Generation];
    }

    private void UpdateStats()
    {
        _speed = _fish.Stats.Speed;
    }

    private void OnReachedNextGenSelection(Transform nextGenNest)
    {
        transform.DOMove(nextGenNest.position, 3f).SetEase(Ease.OutCirc);
        transform.DORotate(Vector3.zero, 3f);

        if (_currentPartner == null) return;

        var partnerFish = _currentPartner.GetComponent<PartnerBehavior>().Stats();

        _currentPartner.SetTargetToFollow(Vector3.zero);
        _currentPartner.transform.DOMove(nextGenNest.position, 4f).SetEase(Ease.OutCirc);
        _currentPartner.transform.DORotate(Vector3.zero, 3f).OnComplete(() =>
        {
            nextGenNest.GetComponent<NextGenSelection>().ShowOptionsPartnerBased(_fish.Stats, partnerFish);
            Destroy(_currentPartner.gameObject);
            _currentPartner = null;
        });
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !_canMove)
        {
            GameEvents.Singleton.ResetTimer();
            _canMove = true;
        }
        if (!_canMove) return;
        if (GameEvents.Singleton.CurrentGameState != GameStates.Playing) return;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var mousePos = new Vector2(worldPosition.x, worldPosition.y);
        MoveTowardsTarget(mousePos);
        RotateToTarget(mousePos);
        BondWithPartner();
        ShowPathDirection();
    }

    private void SendPositionToPartner()
    {
        if (_currentPartner == null) return;
        _currentPartner.SetTargetToFollow(transform.position);
    }

    private void BondWithPartner()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (_currentPartner != null)
            {
                _currentPartner.SetTargetToFollow(Vector3.zero);
                _currentPartner = null;
            }
            else
            {
                var collider = Physics2D.OverlapCircle(transform.position, _findPartnerRadius, _partnerLayerMask);
                if (collider.transform.TryGetComponent(out PartnerBehavior partner))
                {
                    _currentPartner = partner;
                }
            }
        }
        SendPositionToPartner();
    }

    private void ShowPathDirection()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameEvents.Singleton.ShowPathDirection();
        }
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
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _findPartnerRadius);
    }

    public bool HasParner()
    {
        return _currentPartner != null;
    }

    public void TakeDamage()
    {
        _life -= 1;
        CameraShake();
        GameEvents.Singleton.PlayerGotHit();
        if (_life <= 0)
        {
            GameEvents.Singleton.PlayerDied();
        }
        else
        {
            transform.DOKill();
            transform.DOShakeScale(1f);
            _spriteRenderer.DOColor(Color.red, 0.2f).SetEase(Ease.InFlash).OnComplete(() =>
            {
                _spriteRenderer.DOColor(Color.white, 0.1f);
            });
            GameEvents.Singleton.PitchAudioDown();
        }
    }

    private void Die()
    {
        print("Die?");
        GameEvents.Singleton.GameStateChanged(GameStates.ChoosingNextGen);
        transform.DORotate(-transform.rotation.eulerAngles, 1f).OnComplete(() =>
        {
            GameEvents.Singleton.ResetGame();
        });
        transform.DOMoveY(transform.position.y + 1f, 3f);
    }

    private async void CameraShake()
    {
        _mainCamMultiChannelPerlin.m_AmplitudeGain = 3.0f;
        await Task.Delay(80);
        _mainCamMultiChannelPerlin.m_AmplitudeGain = 0f;
    }

    public Fish GetFish()
    {
        return _fish;
    }
}
