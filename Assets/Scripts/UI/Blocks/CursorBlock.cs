using RPG.Assistants;
using RPG.Units;
using RPG.Units.NPCs;
using RPG.Units.Player;

using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

using Zenject;

namespace RPG.UI.Blocks
{
    public class CursorBlock : MonoBehaviour
    {
		[Inject]
		private PlayerUnit _player;

		private Image _image;

        [SerializeField]
        private Color _defaultColor;
        [SerializeField]
        private Color _friendlyColor;
        [SerializeField]
        private Color _enemyColor;
        [SerializeField]
        private Color _lootColor;

		[Space, SerializeField]
		private TextMeshProUGUI _focusText;
		[SerializeField]
		private TextMeshProUGUI _healthText;
		[SerializeField]
		private TextMeshProUGUI _infoText;

		[Space, SerializeField, SQRFloat, Range(100f, 1000f)]
		private float _maxDistance = 500f;
		[SerializeField, SQRFloat, Range(0.1f, 50f)]
		private float _interactDistance = 4f;

		public bool CanInteract { get; private set; }
		public NPCUnit LastFocusUnit { get; private set; }

		private void LateUpdate()
		{
			var ray = Camera.main.ScreenPointToRay(transform.position);
			NPCUnit unit;
			if (Physics.Raycast(ray, out var hit, _maxDistance))
			{
				unit = hit.transform.GetComponent<NPCUnit>();
				if (unit == null || unit.Equals(_player))
				{
					ClearFocusUI();
					return;
				}
			}
			else
			{
				ClearFocusUI();
				return;
			}

			_focusText.text = unit.State.DisplayName;
			_healthText.text = string.Concat(Mathf.Round(unit.State.Parameters.Health), '/', unit.State.Parameters[StatsType.Health]);

			switch (unit.State.Side)
			{
				case SideType.Friendly:
					_focusText.color = _image.color = _friendlyColor;
					CheckInteract(unit);
					break;
				case SideType.Enemy:
					_focusText.color = _image.color = _enemyColor;
					break;
			}

			LastFocusUnit = unit;
		}

		private void CheckInteract(Unit unit)
		{
			var distance = (unit.transform.position - _player.transform.position).sqrMagnitude;
			if (distance <= _interactDistance && _player.CanSpeak && unit.CanSpeak)
			{
				_infoText.text = "Interact";//todo вынести в локали
				CanInteract = true;
			}
			else
			{
				_infoText.text = "";
				CanInteract = false;
			}
		}

		private void ClearFocusUI()
		{
			_focusText.text = _healthText.text = string.Empty;
			_focusText.color = _image.color = _defaultColor;
			_infoText.text = "";
			CanInteract = false;
			LastFocusUnit = null;
		}

		private void Start()
		{
			_image = GetComponent<Image>();
		}
	}
}