using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private enum State { Patrulla, Persigue, Ataca, Busca }
    private State _state;

    [Header("Referencias")]
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Transform _player;
    [SerializeField] private Animator _animator;
    [SerializeField] private Transform _eyes;

    [Header("Patrulla")]
    [SerializeField] private float _radiusPatrol = 12f;
    [SerializeField] private float _pointPatrol = 0.6f;
    [SerializeField] private float _timeWait = 1.0f;

    [Header("Rangos")]
    [SerializeField] private float _rangeChase = 10f;
    [SerializeField] private float _rangeAttack = 2f;
    [SerializeField] private float _hysteresisFactor = 1.1f;

    [Header("Visión")]
    [SerializeField] private float _angleVision = 90f;
    [SerializeField] private float _heightVision = 1.6f;
    [SerializeField] private LayerMask _visionMask;

    [Header("Combate")]
    [SerializeField] private float _cooldownAttack = 1.0f;

    [Header("Debug")]
    [SerializeField] private bool _drawGizmos = true;

    [Header ("Memoria")]
    [SerializeField] private float _timeMemory = 4f;
    private float _memoryTimer;
    private Vector3 _lastKnownPosition;

    private Vector3 _currentPatrolTarget;
    private float _waitTimer;
    private float _nextAttackTime;

    void Reset()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        if (!_agent) _agent = GetComponent<NavMeshAgent>();
        if (!_player) _player = GameObject.FindGameObjectWithTag("Player")?.transform;

        PickNewPatrolPoint();
        _state = State.Patrulla;
    }

    void Update()
    {
        if (!_player || !_agent) return;

        float disToPlayer = Vector3.Distance(transform.position, _player.position);
        bool canSee = CanSeePlayer();

        if (canSee)
        {
            _lastKnownPosition = _player.position;
            _memoryTimer = _timeMemory;
        }

        switch (_state)
        {
            case State.Patrulla:

                PatrolTick();

                if (disToPlayer <= _rangeChase && canSee)
                {
                    _state = State.Persigue;
                }

                break;

            case State.Persigue:

                _agent.stoppingDistance = _rangeAttack * 0.9f;
                _agent.SetDestination(_player.position);
                _agent.acceleration = 13;

                if (disToPlayer <= _rangeAttack && canSee)
                {
                    _state = State.Ataca;
                    _agent.ResetPath();
                }

                else if (!canSee || disToPlayer > _rangeChase * _hysteresisFactor)
                {
                    _state = State.Busca;
                }
                break;

            case State.Ataca:

                FaceTarget(_player.position);

                if (Time.time >= _nextAttackTime)
                {
                    DoAttack(disToPlayer);
                    _nextAttackTime = Time.time + _cooldownAttack;
                }

                if (disToPlayer > _rangeAttack * _hysteresisFactor || !canSee)
                {
                    _state = State.Persigue;
                }

                break;

            case State.Busca:
                _agent.stoppingDistance = 0.5f;
                _agent.SetDestination(_lastKnownPosition);

                if (canSee && disToPlayer <= _rangeChase * _hysteresisFactor)
                {
                    _state = State.Persigue;
                }
                _memoryTimer -= Time.deltaTime;

                if (_memoryTimer <= 0)
                {
                    _state = State.Patrulla;
                    PickNewPatrolPoint();
                }

                break;
        }   
    }

    void PatrolTick()
    {
        _agent.stoppingDistance = 0f;

        if (!_agent.hasPath)
        {
            _agent.SetDestination(_currentPatrolTarget);
        }

        if (Vector3.Distance(transform.position, _currentPatrolTarget) <= _pointPatrol)
        {
            _waitTimer += Time.deltaTime;

            if (_waitTimer >= _timeWait)
            {
                _waitTimer = 0f;
                PickNewPatrolPoint();
            }
        }
    }

    void PickNewPatrolPoint()
    {
        if (TryGetRandomPoint(transform.position, _radiusPatrol, out var point))
        {
            _currentPatrolTarget = point;
            _agent.SetDestination(_currentPatrolTarget);
        }
        else
        {
            _currentPatrolTarget = transform.position;
        }
    }

    bool TryGetRandomPoint(Vector3 center, float radius, out Vector3 result)
    {
        for (int i = 0; i < 20; i++)
        {
            Vector3 random = center + Random.insideUnitSphere * radius;

            if (NavMesh.SamplePosition(random, out var hit, 2f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }

        result = center;
        return false;
    }

    bool CanSeePlayer()
    {
        if (!_player) return false;

        Vector3 eyePos = _eyes ? _eyes.position : transform.position + Vector3.up * _heightVision;
        Vector3 targetPos = _player.position + Vector3.up * 1f;

        Vector3 dir = (targetPos - eyePos).normalized;
        float dist = Vector3.Distance(eyePos, targetPos);

        float angle = Vector3.Angle(transform.forward, dir);
        if (angle > _angleVision * 0.5f)
            return false;

        if (Physics.Raycast(eyePos, dir, out RaycastHit hit, dist, _visionMask))
        {
            return hit.transform == _player;
        }

        return false;
    }

    void DoAttack(float distToPlayer)
    {
        if (distToPlayer <= _rangeAttack + 0.3f)
        {
            Debug.Log("Atacando");
        }
    }

    void FaceTarget(Vector3 targetPos)
    {
        Vector3 dir = targetPos - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion look = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * 10f);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!_drawGizmos) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _radiusPatrol);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _rangeChase);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _rangeAttack);

    
        Vector3 eyePos = transform.position + Vector3.up * _heightVision;

        Vector3 left = Quaternion.Euler(0, -_angleVision * 0.5f, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0, _angleVision * 0.5f, 0) * transform.forward;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(eyePos, eyePos + left * _rangeChase);
        Gizmos.DrawLine(eyePos, eyePos + right * _rangeChase);
    }
}
