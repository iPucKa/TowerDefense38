using Assets._Project.Develop.Runtime.Gameplay.Features.StageFeature;
using Assets._Project.Develop.Runtime.Utilities.StateMachineCore;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.States
{
	public class PreparationState : State, IUpdatableState
	{
		private readonly PreparationTriggerService _preparationTriggerService;

		public PreparationState(PreparationTriggerService preparationTriggerService)
		{
			_preparationTriggerService = preparationTriggerService;
		}

		public override void Enter()
		{
			base.Enter();

			Vector3 nextStageTriggerPosition = Vector3.zero + Vector3.forward * 4;
			_preparationTriggerService.Create(nextStageTriggerPosition);
		}

		public void Update(float deltaTime)
		{
			_preparationTriggerService.Update(deltaTime);
		}

		public override void Exit()
		{
			base.Exit();

			_preparationTriggerService.Cleanup();
		}
	}
}
