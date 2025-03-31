using GeneralScripts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Enemies
{
    public class SharkEnemy : Enemy
    {
        private static readonly int IsAttackingAnimationId = Animator.StringToHash("isAttacking");

        private SharkState _state = SharkState.MovingToTarget;
        private Vector3 _attackPoint;
        private float _attackCooldown;
        private float _takenDamageOnLastAttack;
        
        [Space]
        [SerializeField] private float minWanderingTime;
        [SerializeField] private float maxWanderingTime;
        private float _currWanderingTime;

        private Vector3 _wanderingWaypoint;
        [Space]
        [SerializeField] private int maxWanderingDistance;
        
        [Space]
        [SerializeField] private float escapeDistance;

        private void Start()
        {
            agent.speed = stats.moveSpeed;
        }
        
        private void Update()
        {
            ChoseBehaviour();
        }

        private void LateUpdate()
        {
            LookForward();
            SmoothModel();
            ControlAnimation();
        }

        private void ChoseBehaviour()
        {
            switch (_state)
            {
                case SharkState.Wandering:
                    Wander();
                    break;
                case SharkState.MovingToTarget:
                    ChoseTarget();
                    MoveToTarget();
                    break;
                case SharkState.Attacking:
                    Attack();
                    break;
                case SharkState.MovingFromRaft:
                    MoveFromRaft();
                    break;
            }
        }

        private void Wander()
        {
            if (_currWanderingTime > 0)
            {
                _currWanderingTime -= Time.deltaTime;
            }
            else
            {
                _state = SharkState.MovingToTarget;
                return;
            }
            
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                _wanderingWaypoint = transform.position +
                    new Vector3(Random.Range(-maxWanderingDistance, maxWanderingDistance), 0,
                    Random.Range(-maxWanderingDistance, maxWanderingDistance));
                
                agent.SetDestination(_wanderingWaypoint);
            }
        }

        private void MoveToTarget()
        {
            if (_currWanderingTime > 0f)
                _state = SharkState.Wandering;

            if (Target.IsDestroyed() || !Target)
            {
                _currWanderingTime = Random.Range(minWanderingTime, maxWanderingTime);
                _state = SharkState.Wandering;
                return;
            }
            
            Vector3 closestPoint = Target.transform.position;
            float minDist = Mathf.Infinity;
            
            Vector3[] points = new Vector3[8];
            points[0] = Target.transform.position + new Vector3(2, 0, 2);
            points[1] = Target.transform.position + new Vector3(-2, 0, 2);
            points[2] = Target.transform.position + new Vector3(2, 0, -2);
            points[3] = Target.transform.position + new Vector3(-2, 0, -2);
            points[4] = Target.transform.position + new Vector3(0, 0, 2);
            points[5] = Target.transform.position + new Vector3(-2, 0, 0);
            points[6] = Target.transform.position + new Vector3(0, 0, -2);
            points[7] = Target.transform.position + new Vector3(2, 0, 0);
            
            foreach (var point in points)
            {
                float dist = Vector3.Distance(transform.position, point);
                if (dist < minDist)
                {
                    minDist = dist;
                    closestPoint = point;
                }
            }
            
            _attackPoint = closestPoint;
            
            if (Vector3.Distance(transform.position, _attackPoint) > stats.attackRange)
            {
                agent.SetDestination(_attackPoint);
            }
            else
            {
                agent.ResetPath();
                transform.SetParent(Target);
                _takenDamageOnLastAttack = 0f;
                _state = SharkState.Attacking;
            }
        }

        private void Attack()
        {
            if (Target.IsDestroyed() || !Target || _takenDamageOnLastAttack / stats.maxHealth >= 0.6f)
            {
                _state = SharkState.MovingFromRaft;
                _currWanderingTime = Random.Range(minWanderingTime, maxWanderingTime);
                return;
            }
            
            transform.LookAt(Target);
            
            if (_attackCooldown > 0)
            {
                _attackCooldown -= Time.deltaTime;
                return;
            }

            if (Target.TryGetComponent(out IDamageable damageableTarget))
            {
                damageableTarget.TakeDamage(stats.damage);
                _attackCooldown = stats.attackSpeed / 60f;
            }
            else
            {
                _state = SharkState.MovingFromRaft;
                _currWanderingTime = Random.Range(minWanderingTime, maxWanderingTime);
            }
        }

        private void MoveFromRaft()
        {
            Vector3 escapeDirection = (transform.position - BuildingsManager.transform.position).normalized;
            _wanderingWaypoint = transform.position + escapeDirection * escapeDistance;
            _wanderingWaypoint.y = 0;
            
            agent.SetDestination(_wanderingWaypoint);
            
            _state = SharkState.Wandering;
        }

        private void ControlAnimation()
        {
            if (_state != SharkState.Attacking)
            {
                animator.SetBool(IsAttackingAnimationId, false);
                animator.speed = agent.speed / 1.5f;
            }
            else
            {
                animator.SetBool(IsAttackingAnimationId, true);
                animator.speed = 1;
            }
        }

        private void LookForward()
        {
            Vector3 direction = agent.velocity;
            if (direction.magnitude > 0.1f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5);
        }

        public override void TakeDamage(float damage)
        {
            _state = SharkState.MovingToTarget;
            _takenDamageOnLastAttack += damage;
            base.TakeDamage(damage);
        }
    }

    public enum SharkState
    {
        Wandering,
        MovingToTarget,
        Attacking,
        MovingFromRaft
    }
}