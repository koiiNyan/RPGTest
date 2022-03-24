using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using RPG.Assistants;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using Zenject;

namespace RPG.Units
{
    [SaveData("unit", 255)]
    [RequireComponent(typeof(UnitInputComponent), typeof(Rigidbody), typeof(Animator))]
    public abstract partial class Unit : MonoBehaviour, IStateRecorder
    {
        private const float с_blendCoef = 2f;

        [SerializeField]
        protected string _id;

        protected internal Rigidbody RigidBody;
        protected internal Animator Animator;
        protected internal UnitSpaceComponent Space;
        protected internal UnitStateComponent State;
        protected internal UnitInputComponent Input;
        protected internal UnitMoveComponent Move;
        protected internal WeaponSetComponent Set;

        //private BaseTriggerComponent[] _colliders;

        [Inject]
        protected Managers.UnitManager _npcManager;

        public SimpleHandle OnTargetLostHandler;

        [SerializeField, SQRFloat, Tooltip("Квадрат максимального расстояния до поиска цели")]
        protected float _sqrFindTargetDistance = 500f;
        [SerializeField, ReadOnly]
        private Transform _targetPoint;

        public bool Enable
        {
            get => enabled;
            set
            {
                enabled = GetComponent<Collider>().enabled = Input.enabled = Move.enabled = State.enabled = Space.enabled = value;
			}
		}

        public ulong NumberID { get; set; }
        public Unit Target { get; set; }
        public Transform GetTargetPoint => _targetPoint;

        public string GetID => _id;
        public abstract bool CanSpeak { get; }

        protected virtual void Start()
        {
            Animator = this.FindComponent<Animator>();
            Input = this.FindComponent<UnitInputComponent>();
            Move = this.FindComponent<UnitMoveComponent>();
            State = this.FindComponent<UnitStateComponent>();
            Space = this.FindComponent<UnitSpaceComponent>();
            RigidBody = this.FindComponent<Rigidbody>();
            Set = GetComponent<WeaponSetComponent>();

            //_colliders = GetComponentsInChildren<BaseTriggerComponent>();
            //foreach (var collider in _colliders) collider.Construct(this, _stats);

            if (Input == null) return;

            BindingEvents();
        }

        private void OnFalling(bool isFalling)
        {
            if (isFalling && !Animator.GetBool("Falling")) Animator.SetTrigger("Fall");
            Animator.SetBool("Falling", isFalling);
		}


        private void OnMainAction()//todo
        {
            if (Move.LockMovement || !Set.InArmed) return;

            Move.InCrouch = Move.InSprint = false;

            Animator.SetTrigger("MainAction");
            Move.LockMovement = true;
        }

        private void OnAdditionalAction()
        {
            //if (_inAnimation || _stats._currentCalldown > 0f) return; todo
            //_animator.SetTrigger("AdditionalAction");
            //_inAnimation = true;
            //_stats._currentCalldown = _stats._calldownShieldAttack;
		}

        private void OnTargetUpdate()
        {
            if(Target != null)
            {
                Target = null;
                OnTargetLostHandler?.Invoke();//todo
                return;
			}

            FindNewTarget();
        }

        protected abstract void FindNewTarget();

        protected virtual void BindingEvents(bool unbind = false)
        {
            if (unbind)
            {
                Input.MainEventHandler -= OnMainAction;
                Input.AdditionalEventHandler -= OnAdditionalAction;
                Input.TargetEventHandler -= OnTargetUpdate;
                Space.OnFallingEvent -= OnFalling;
                return;
            }

            Input.MainEventHandler += OnMainAction;
            Input.AdditionalEventHandler += OnAdditionalAction;
            Input.TargetEventHandler += OnTargetUpdate;
            Space.OnFallingEvent += OnFalling;
        }

        private void OnDestroy()
		{
            BindingEvents(unbind: true);
		}

#if UNITY_EDITOR

        [ContextMenu("Update Internal States")]
        protected virtual void UpdateInternalStates()
        {
            _targetPoint = GetComponentsInChildren<Transform>().First(t => t.name == Editor.EditorConstants.FocusTargetPointName);

            var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

            if (agent != null)
            {
                var @params = Editor.EditorConstants.MeshAgentObstacleAvoidance;
                agent.baseOffset = @params.x;
                agent.radius = @params.y;
                agent.height = @params.z;
            }
        }

        [ContextMenu("Configuration Skeleton")]
        private void ConfigurationSkeleton()
        {
            var components = GetComponentsInChildren<MonoBehaviour>(true);

            foreach(var component in components)
            {
                var methods = component.GetType().GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);

                foreach(var method in methods)
                {
                    if (method.GetCustomAttributes(typeof(ConfigurationSkeletonAttribute), true).Length == 0) continue;

                    var parameters = method.GetParameters();
                    if (parameters == null || parameters.Length == 0) method?.Invoke(component, null);
				}
			}
  		}

        [ContextMenu("Parse unit data")]
        private void ParseUnitData()
        {
            gameObject.AddComponent<Editor.UnitParses>();
            Debug.Log($"Remember to remove the <b>{nameof(Editor.UnitParses)}</b> component from the <b>{name}</b> GameObject before saving the scene!");
		}
#endif
	}
}
