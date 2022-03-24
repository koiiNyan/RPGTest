using RPG.Units;
using RPG.Units.Player;

using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

using Zenject;

namespace RPG.UI.Blocks
{
    public class ConditionBlock : MonoBehaviour
    {
        [Inject]
        private PlayerUnit _player;

        [SerializeField]
        private FillWidget _hp;
        [SerializeField]
        private FillWidget _mp;
        [SerializeField]
        private FillWidget _sp;


		private void Update()
		{
            _hp.Text.text = Mathf.RoundToInt(_player.State.Parameters.Health).ToString();
            _hp.Fill.fillAmount = _player.State.Parameters.Health / _player.State.Parameters[StatsType.Health];

            if (_player.State.Parameters.TryGetParameter(StatsType.Mana, out var container))
            {
                _mp.Text.text = Mathf.RoundToInt(_player.State.Parameters.Mana).ToString();
                _mp.Fill.fillAmount = _player.State.Parameters.Mana / container;
            }

            if (_player.State.Parameters.TryGetParameter(StatsType.Stamina, out container))
            {
                _sp.Text.text = Mathf.RoundToInt(_player.State.Parameters.Stamina).ToString();
                _sp.Fill.fillAmount = _player.State.Parameters.Stamina / container;
            }
        }

        [System.Serializable]
        private struct FillWidget
        {
            public Image Fill;
            public TextMeshProUGUI Text;
		}
    }
}
