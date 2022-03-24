using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Units.NPCs
{
	[RequireComponent(typeof(NPCInputComponent), typeof(UnityEngine.AI.NavMeshAgent), typeof(NPCStateComponent))]
	public partial class NPCUnit : Unit
    {
		[SerializeField]
		private WeaponStyleType _weaponType;
		[SerializeField, Tooltip("Тип префаба, на основе которого создается экземпляр")]
		private BotPrefabType _botPrefabType;

		[SerializeField]
		private string[] _equipments;

		public BotPrefabType GetPrefabType => _botPrefabType;
		public string SetID
		{
			set => _id = value;
		}

		public Queue<string> SpeakingQueue { get; set; }
		public Queue<string> QuestQueue { get; set; }
		public LinkedList<IdentificatorParagraphData> ParagraphList { get; set; }
		public override bool CanSpeak => Target == null;
		public AIStateType AIState { get; set; }


		public void ConfigurationAIController()
		{
			Input.enabled = true;
		}

		protected override void FindNewTarget()
		{
			var player = _npcManager.GetPlayer;

			Target = _sqrFindTargetDistance <= (player.transform.position - transform.position).sqrMagnitude
				? player : null;
		}

		protected override void Start()
		{
			base.Start();
			var callback = gameObject.AddComponent<UnitAnimationCallbackComponent>();

			foreach (var pair in Constants.GetLayersInts)
			{
				Animator.SetLayerWeight(pair.Value, _weaponType == pair.Key ? 1f : 0f);
			}
		}

		private void OnValidate()
		{
			GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
		}
	}
}
