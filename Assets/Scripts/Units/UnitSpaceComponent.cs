using RPG.Assistants;
using RPG.Triggers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace RPG.Units
{
    public class UnitSpaceComponent : UnitComponent
    {
        private LinkedList<string> _obstacles = new LinkedList<string>();
        private UnitStateComponent _stats;
        private Coroutine _fallingCor;
        private Rigidbody _rigidBody;

        [SerializeField, ReadOnly]
        private UnitTriggerComponent _trigger;

        [Space, SerializeField, Range(0f, 5f)]
        private float _fallDelayStart = 0.5f;
        [SerializeField, Range(0f, 10f), Tooltip("Расстояние болезненного падения")]
        private float _distanceHardFall = 2.5f;

        public event SimpleHandle<bool> OnFallingEvent;


        private void UpdateAir(Collider collider, bool enter)
        {
            if (!collider.CompareTag(Constants.FloorTag)) return;
            
            //Сохраняем объект, с которым коллизимся
            if (enter) _obstacles.AddLast(collider.name);
            //Удаляем объект с которым больше не коллизимся
            else _obstacles.Remove(collider.name);

            _stats.InAir = _obstacles.Count == 0;

            //Если мы в воздухе - запускаем таймер начала падения
            if (_stats.InAir) _fallingCor = StartCoroutine(Falling());
            //Иначе мы на земле - останавливаем таймер
            else
            {
                Owner.Move.LockMovement = false;
                if (_fallingCor != null) StopCoroutine(_fallingCor);
                OnFallingEvent?.Invoke(false);
            }
		}

		private void FixedUpdate()
		{
            var velocity = _rigidBody.velocity;
            velocity.y = 0f;
            if (Physics.Raycast(transform.position, transform.forward, out var hit, 1f))
            {
                if (hit.collider.gameObject.layer != Constants.ObstacleLayerInt) return;
                //_stats._angleHill = Vector3.SignedAngle(hit.normal, Vector3.up, transform.right); todo
                //transform.eulerAngles = new Vector3(-_stats._angleHill, transform.eulerAngles.y, transform.eulerAngles.z);
                //Debug.Log(_stats._angleHill);
            }
		}

		private void OnDrawGizmos()
		{
            if (_rigidBody == null) return;
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 1f);
		}

		private IEnumerator Falling()
        {
            Owner.Move.LockMovement = true;
            Owner.Move.InCrouch = Owner.Move.InSprint = false;
            yield return new WaitForSeconds(_fallDelayStart);

            if (_stats.InAir) OnFallingEvent?.Invoke(true);
		}

        private void Start()
        {
            _rigidBody = this.FindComponent<Rigidbody>();
            _stats = this.FindComponent<UnitStateComponent>();
            _trigger.OnTriggerCollisionEventHandler += UpdateAir;

            if (_trigger == null)
            {
                _trigger = this.FindComponentInChildren<UnitTriggerComponent>();
#if UNITY_EDITOR
                if (_trigger.gameObject.layer != LayerMask.NameToLayer(Editor.EditorConstants.TriggerLayer))
                    Editor.EditorExtensions.ConsoleLog($"Not found Air Collider with layer <b>{LayerMask.NameToLayer(Editor.EditorConstants.TriggerLayer)}</b> on GameObject: <b>{name}</b>", Editor.PriorityMessageType.Critical);
#endif
            }
        }

#if UNITY_EDITOR
        #region ContextMenu
        [ContextMenu("Create or Find Colliders"), ConfigurationSkeleton]
        private void Construct()
        {
            var triggers = GetComponentsInChildren<UnitTriggerComponent>().Where(t => t.name == Editor.EditorConstants.AirColliderName).ToArray();

            if (triggers.Length == 1)
            {
                _trigger = triggers[0];
                return;
            }
            else if (triggers.Length > 1)
            {
                Editor.EditorExtensions.ConsoleLog($"Unit <b>{name}</b> contains air colliders, but their number is not equal to one. Current count: <b>{triggers.Length}</b>", Editor.PriorityMessageType.Critical);
                return;
            }

            var GO = new GameObject(Editor.EditorConstants.AirColliderName);
            GO.transform.parent = transform;
            GO.transform.localPosition = new Vector3();
            GO.transform.localRotation = new Quaternion();
            GO.transform.localScale = Vector3.one;
            GO.layer = LayerMask.NameToLayer(Editor.EditorConstants.TriggerLayer);

            var box = GO.AddComponent<BoxCollider>();
            box.isTrigger = true;
            box.center = Editor.EditorConstants.AirColliderBound.center;
            box.size = Editor.EditorConstants.AirColliderBound.size;

            _trigger = GO.AddComponent<UnitTriggerComponent>();
            Editor.EditorExtensions.ConsoleLog($"Unit <b>{name}</b>: An air collider was created", Editor.PriorityMessageType.Notification);

            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        }

		#endregion
#endif
	}
}
