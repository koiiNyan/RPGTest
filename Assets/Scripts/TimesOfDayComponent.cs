using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class TimesOfDayComponent : MonoBehaviour
    {
		private const float c_SecsInDay = 86400f;
		private const float _speed = 360f / c_SecsInDay;//0.00416f

		/// <summary>
		/// 600 - сутки сменяются за 2.4 минуты
		/// 360 - сутки сменяются за 4 минуты
		/// 240 - сутки сменяются за 6 минут
		/// 180 - сутки сменяются за 8 минут
		/// 120 - сутки сменяются за 12 минут
		/// 60 - сутки сменяются за 24 минуты
		/// 1 - сутки сменяются за 24 часа
		/// </summary>
		[SerializeField, Tooltip("Отношение виртуальных минут к игровым"), Range(600f, 1f)]
		private float _ratio = 600f;
		
		[Space, SerializeField]
		private Gradient _dayColor;
		[SerializeField]
		private Gradient _nightColor;
		[SerializeField]
		private Gradient _fogColor;

		[Space, SerializeField]
		private AnimationCurve _dayIntensity;
		[SerializeField]
		private AnimationCurve _nightIntensity;
		[SerializeField]
		private AnimationCurve _fogIntensity;

		[Space, SerializeField]
		private Light _dayLight;
		[SerializeField]
		private Light _nightLight;

		[Space, SerializeField]
		private Material _skyBox;


        private void Start()
        {
			_skyBox = new Material(_skyBox);
			RenderSettings.skybox = _skyBox;
        }

        private void Update()
		{
			transform.Rotate(transform.forward * _speed * _ratio * 4f * TimeAssistant.GameDeltaTime);
			var percent = transform.eulerAngles.z / 360f;
			_dayLight.color = _dayColor.Evaluate(percent);
			_nightLight.color = _nightColor.Evaluate(percent);

			_dayLight.intensity = _dayIntensity.Evaluate(percent);
			_nightLight.intensity = _nightIntensity.Evaluate(percent);

			_dayLight.enabled = _dayLight.intensity > 0f;
			_nightLight.enabled = _nightLight.intensity > 0f;

			_skyBox.SetFloat("_CubemapTransition", _nightIntensity.Evaluate(percent));
			RenderSettings.fogColor = _fogColor.Evaluate(percent);
			RenderSettings.fogEndDistance = _fogIntensity.Evaluate(percent);
		}
	}
}
