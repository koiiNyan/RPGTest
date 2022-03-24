using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using Zenject;

namespace RPG.Units.Player
{
    public class CameraComponent : MonoBehaviour
    {
		private CameraPositionType _cameraPositionType;

		[Inject]
		private ScriptableObjects.GameSettings _settings;
		[Inject]
        private PlayerControls _controls;
		private Unit _target;

		private Transform _camera;

		//Текущее положение поворота вокруг оси OX
		private float _angleX;
		//Текущее положение поворота вокруг оси OY
		private float _angleY;
		private Vector3 _pivotEulers;
		//Поворот пивота по вертикале
		private Quaternion _pivotTargetRotation;
		//Поворот точки по горизонтале
		private Quaternion _transformTargetRotation;
		//Поворот камеры относительно пивота по-умолчанию
		private Quaternion _defaultCameraRotation;


		[SerializeField, Range(-90f, 0f), Tooltip("Минимальный наклон камеры по вертикале")]
		private float _minY = -45f;
		[SerializeField, Range(0f, 90f), Tooltip("Максимальный наклон камеры по вертикале")]
		private float _maxY = 30f;
		[Space, SerializeField, Range(0.5f, 10f)]
		private float _moveSpeed = 5f;
		[SerializeField, Range(0.5f, 10f)]
		private float _rotateSpeed = 0.5f;
		[SerializeField, Range(0.5f, 10f)]
		private float _lockCameraSpeed = 1.5f;
		[SerializeField, Range(10f, 0f), Tooltip("Сглаживание вращения камеры")]
		private float _smoothing = 5f;
		[Space, SerializeField]
		private TransformData[] _positions;

		public Transform PivotTransform { get; private set; }

		public CameraPositionType CameraPosition 
		{
			get => _cameraPositionType;
			set
			{
				_cameraPositionType = value;
				_positions.First(t => t.Type == value).SetLocalTransformData(_camera);
			}
		}

		private void Start()
		{
			_target = transform.parent.GetComponent<Unit>();
			PivotTransform = transform.GetChild(0);
			_pivotEulers = PivotTransform.eulerAngles;

			_camera = GetComponentInChildren<Camera>().transform;
			_defaultCameraRotation = _camera.localRotation;

			transform.parent = null;

			_target.OnTargetLostHandler += () => _camera.localRotation = _defaultCameraRotation;
		}

		private void LateUpdate()
		{
			transform.position = Vector3.Lerp(transform.position, _target.transform.position, TimeAssistant.GameDeltaTime * _moveSpeed);
			if(_target.Target == null) FreeCamera();
			else LockCamera();
		}

		private void FreeCamera()
		{
			var delta = _controls.Camera.Delta.ReadValue<Vector2>();

			_angleX += delta.x * _rotateSpeed * _settings.MouseSensitivity;
			_angleY -= delta.y * _rotateSpeed * _settings.MouseSensitivity;
			_angleY = Mathf.Clamp(_angleY, _minY, _maxY);

			_pivotTargetRotation = Quaternion.Euler(_angleY, _pivotEulers.y, _pivotEulers.z);
			_transformTargetRotation = Quaternion.Euler(0f, _angleX, 0f);

			PivotTransform.localRotation = Quaternion.Slerp(PivotTransform.localRotation, _pivotTargetRotation, _smoothing * TimeAssistant.GameDeltaTime);
			transform.localRotation = Quaternion.Slerp(transform.localRotation, _transformTargetRotation, _smoothing * TimeAssistant.GameDeltaTime);
		}

		private void LockCamera()
		{
			var rotation = Quaternion.LookRotation(_target.Target.GetTargetPoint.position - _camera.position);
			_camera.rotation = Quaternion.Slerp(_camera.rotation, rotation, _lockCameraSpeed * TimeAssistant.GameDeltaTime);

			rotation = Quaternion.LookRotation(_target.Target.GetTargetPoint.position - PivotTransform.position);
			PivotTransform.rotation = Quaternion.Slerp(PivotTransform.rotation, rotation, _lockCameraSpeed * TimeAssistant.GameDeltaTime);
		}

		private void OnDrawGizmos()
		{
			var target = _target == null ? transform.parent.GetComponent<Unit>() : _target;
			Gizmos.DrawSphere(target.transform.position, 0.1f);

			Gizmos.color = Color.green;
			Gizmos.DrawSphere(transform.position, 0.15f);

			var pivot = PivotTransform == null ? transform.GetChild(0) : PivotTransform;
			Gizmos.color = Color.red;
			Gizmos.DrawRay(pivot.position, pivot.forward);

			var camera = _camera == null ? GetComponentInChildren<Camera>().transform : _camera;
			Gizmos.color = Color.yellow;
			Gizmos.DrawRay(camera.position, camera.forward);
		}
	}
}
