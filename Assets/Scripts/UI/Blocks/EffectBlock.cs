using RPG.Assistants;
using RPG.Commands;
using RPG.UI.Elements;
using RPG.Units.Player;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace RPG.UI.Blocks
{
    public class EffectBlock : MonoBehaviour
    {
        private GameObjectPool<EffectVisualElement> _pool;

        [SerializeField]
        private EffectVisualElement _prefab;

        [Inject]
        private PlayerUnit _player;

        public void ClearVisual() => _pool.DisableAllElements();

        private void OnEffectStateChange(BaseCommandEffect effect)
        {
            var element = _pool.GetOrCreateElement(out var isNewElement);

            if(isNewElement) element.transform.SetParent(transform, false);
            element.Effect = effect;
        }

        private void LateUpdate()
        {
            foreach(var element in _pool)
            {
                if (!element.gameObject.activeSelf) continue;
                element.Filling = element.Effect.CurrentDuration / element.Effect.Duration;

                if (element.Filling <= 0f) element.gameObject.SetActive(false);
            }
        }

		private void Awake()
		{
            _pool = new GameObjectPool<EffectVisualElement>(_prefab);
		}

		private async void Start()
        {
            await Task.Yield();
            _player.State.Effects.OnEffectEventHandler += OnEffectStateChange;
        }

        private void OnDestroy()
        {
            _player.State.Effects.OnEffectEventHandler -= OnEffectStateChange;
            _pool.OnDestroy();
        }
    }
}
