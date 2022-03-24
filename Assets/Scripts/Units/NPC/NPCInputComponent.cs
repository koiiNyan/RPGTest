using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Units.NPCs
{
	[SaveData("ai", 1)]
    public partial class NPCInputComponent : UnitInputComponent, IStateRecorder
    {
		private Vector3 _startPoint;
		private int _currentPatrollingPointIndex;
		private NavMeshAgent _agent;

		[SerializeField, SQRFloat, Range(0.1f, 20f)]
		private float _arrivalDistance = 1.5f;
		[SerializeField, Range(0f, 30f)]
		private float _patrollingIdleDelay = 2f;
		[SerializeField, Range(0f, 15f)]
		private float _partollingIdleRandomStep = 0f;

		[Space, SerializeField]
		private Vector3[] _patrollingPoints;

		public bool Activity 
		{ 
			get => _agent.enabled;
			set
			{
				_agent.enabled = value;
				if (_agent.enabled) _agent.isStopped = ForceDisable;
			}
		}

		public bool ForceDisable { get; set; }

		public AIStateType StateType { get; private set; }

		public float Delay { get; set; }

		public void SetTarget(Unit target)
		{
#if UNITY_EDITOR
			if (target == null) Editor.EditorExtensions.ConsoleLog($"<b>{name}</b>: Target can not be null", Editor.PriorityMessageType.Critical);
#endif
			if (target == null) return;
			Owner.Target = target;
			StateType = AIStateType.Pursuit;
		}

		/// <summary>
		/// Проверка на сброс состояния бота
		/// </summary>
		/// <param name="distance">Дистанция до игрока</param>
		/// <returns>Произошел-ли сброс игрока, как цели для преследования</returns>
		public bool CheckResetState(float distance)
		{
			//Либо последняя точка для патруля, либо стартовая точка
			var point = _patrollingPoints.Length != 0 ? _patrollingPoints[_currentPatrollingPointIndex] : _startPoint;

			//Если бот ушел далеко - возвращаем его
			if(distance * distance < (transform.position - point).sqrMagnitude)
			{
				StateType = _patrollingPoints.Length != 0 ? AIStateType.Patrolling : StateType = AIStateType.Idle;
				Owner.Target = null;
				return true;
			}

			return false;
		}

		private void UpdateState()
		{
			if (!Activity && !ForceDisable) return;

			Vector3 point;
			switch (StateType)
			{
				case AIStateType.Idle:
					point = _startPoint;
					point.y = transform.position.y;
					var dist = (transform.position - point).sqrMagnitude;

					if (dist > _arrivalDistance) _agent.destination = _startPoint;
					break;
				case AIStateType.Patrolling:
					point = _patrollingPoints[_currentPatrollingPointIndex];
					point.y = transform.position.y;
					var distance = (transform.position - point).sqrMagnitude;

					if (distance < _arrivalDistance)
					{
						_currentPatrollingPointIndex = (_currentPatrollingPointIndex + 1) % _patrollingPoints.Length;
						Delay = Mathf.Clamp(_patrollingIdleDelay + Random.Range(-_partollingIdleRandomStep, _partollingIdleRandomStep), 0f, float.MaxValue);
					}
					if (Delay <= 0f) _agent.destination = _patrollingPoints[_currentPatrollingPointIndex];
					break;
				case AIStateType.Pursuit:
					_agent.destination = Owner.Target.transform.position;
					break;
			}
		}

		public void OnUpdate(float delta)
		{
			if (Delay > 0f) Delay = Delay - delta;

			UpdateState();
			Owner.Move.UpdateAnimationStates(_agent.velocity);
			if(StateType == AIStateType.Pursuit && (transform.position - _agent.destination).sqrMagnitude < _agent.stoppingDistance * _agent.stoppingDistance)
			{
				CallSimpleHandle(nameof(MainEventHandler));
			}
		}

		protected override void Awake()
		{
			base.Awake();
			_agent = this.FindComponent<NavMeshAgent>();

			if (_patrollingPoints != null && _patrollingPoints.Length > 1)
			{
				StateType = AIStateType.Patrolling;
				_startPoint = transform.position;
			}
			else
			{
				StateType = AIStateType.Idle;
				_startPoint = _patrollingPoints.Length == 0 ? transform.position : _patrollingPoints[0];
				_patrollingPoints = new Vector3[0];
			}
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere(transform.position, _arrivalDistance);

			if (_patrollingPoints == null) return;

			Gizmos.color = Color.green;
			var scale = new Vector3(.2f, 3f, .2f);

			foreach (var point in _patrollingPoints)
			{
				var position = point;
				position.y = position.y + scale.y / 2f;
				Gizmos.DrawCube(position, scale);
			}
		}
	}
}
