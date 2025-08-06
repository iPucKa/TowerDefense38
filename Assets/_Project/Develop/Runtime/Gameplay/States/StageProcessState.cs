using Assets._Project.Develop.Runtime.Gameplay.Features.Attack.Explosion;
using Assets._Project.Develop.Runtime.Gameplay.Features.StageFeature;
using Assets._Project.Develop.Runtime.Utilities.StateMachineCore;

namespace Assets._Project.Develop.Runtime.Gameplay.States
{
	public class StageProcessState : State, IUpdatableState
	{
		private readonly StageProviderService _stageProviderService;
		private readonly ExplosionService _explosionService;

		public StageProcessState(StageProviderService stageProviderService, ExplosionService explosionService)
		{
			_stageProviderService = stageProviderService;
			_explosionService = explosionService;
		}

		public override void Enter()
		{
			base.Enter();

			_stageProviderService.SwitchToNext();
			_stageProviderService.StartCurrent();
		}

		public void Update(float deltaTime)
		{
			_stageProviderService.UpdateCurrent(deltaTime);
			_explosionService.Update(deltaTime);
		}

		public override void Exit()
		{
			base.Exit();

			_stageProviderService.CleanupCurrent();
			_explosionService.Cleanup();
		}
	}
}
