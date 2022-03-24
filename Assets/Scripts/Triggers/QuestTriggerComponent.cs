using RPG.Units.Player;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Triggers
{
	public class QuestTriggerComponent : TriggerComponent
	{
		[SerializeField]
		private bool _dontDestroy;

		[SerializeField]
		private string _contextID;
		[SerializeField]
		private string _paragraphID;
		[SerializeField]
		private QuestTriggerComponent[] _dependencies;

		public string GetContextID => _contextID;
		public string GetParagraphID => _paragraphID;


		public void UpdateTriggerState()
		{
			foreach (var dependence in _dependencies) dependence.Enable = true;

			if (_dontDestroy) Enable = false;
			else Destroy(gameObject);
		}

		protected override void OnTriggerEnter(Collider other)
		{
			var player = other.GetComponent<PlayerUnit>();

#if UNITY_EDITOR
			if(player == null)
			Editor.EditorExtensions.ConsoleLog($"Layers are not configured correctly, the trigger must have a <b>{Editor.EditorConstants.PlayerTriggerLayer}</b> layer, and the player must have a <b>{Editor.EditorConstants.PlayerLayer}</b> layer", Editor.PriorityMessageType.Critical);
#endif

			_manager.UpdateQuestState(new IdentificatorParagraphData(_contextID, _paragraphID));
		}
	}
}