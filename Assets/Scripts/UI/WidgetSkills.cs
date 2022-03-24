using Newtonsoft.Json.Linq;

using RPG.ScriptableObjects;
using RPG.ScriptableObjects.Contexts;
using RPG.ScriptableObjects.Effects;
using RPG.UI.Blocks;
using RPG.Units.Player;

using System.Threading.Tasks;

using UnityEngine;

using Zenject;

namespace RPG.UI
{
    public class WidgetSkills : MonoBehaviour, IClosableWidget
    {
        [Inject]
        private PlayerUnit _player;

        [SerializeField]
        private SkillTreeBlock _skillTree;
        [SerializeField]
        private SelectSkillBlock _selectSkill;

        public event SimpleHandle OnCloseWidgetEventHandler;

		private void OnEnable()
		{
            if(_skillTree.Progress == null) _skillTree.Progress = _player.State as ILevelProgress;
        }

		private void Start()
		{
            _skillTree.OnSkillUpEventHandler += OnUpdatePlayer;
            _skillTree.OnSkillDownEventHandler += OnRegressPlayer;
            _selectSkill.OnSkillSelectedEventHandler += OnUpdatePlayer;
            _skillTree.OnEndDragSkillEventHandler += _selectSkill.OnSelectSkill;
        }

        private void OnUpdatePlayer(InputSetType input, SkillData skill)
        {
            if(skill.Statuses.Count != 0) _player.State.Statuses.AddStatuses(skill.Statuses);
            if(input != InputSetType.None && !string.IsNullOrEmpty(skill.EffectID))
            {
                _player.SkillSet.Remove(input);

                var effect = Assistants.ConstructEntityExtensions.CreateCommandEffect<Commands.NonTargetEffect>(skill.EffectID);
                effect.Source = _player;
                _player.SkillSet.Add(input, effect);
			}
        }

        private void OnRegressPlayer(SkillData skill)
        {
            _player.State.Statuses.RemoveStatuses(skill.Statuses);
        }

        public void OnClose_UnityEvent() => OnCloseWidgetEventHandler?.Invoke();

		private void OnDestroy()
		{
            _skillTree.OnSkillUpEventHandler -= OnUpdatePlayer;
            _skillTree.OnSkillDownEventHandler -= OnRegressPlayer;
            _selectSkill.OnSkillSelectedEventHandler -= OnUpdatePlayer;
            _skillTree.OnEndDragSkillEventHandler -= _selectSkill.OnSelectSkill;
        }
    }
}
