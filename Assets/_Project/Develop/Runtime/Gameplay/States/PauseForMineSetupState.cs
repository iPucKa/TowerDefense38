using Assets._Project.Develop.Runtime.Gameplay.Features.StageFeature;
using Assets._Project.Develop.Runtime.Utilities.StateMachineCore;

namespace Assets._Project.Develop.Runtime.Gameplay.States
{
	public class PauseForMineSetupState : State, IUpdatableState
	{
		private readonly MineSetupOnPauseService _mineSetupOnPauseService;

		public PauseForMineSetupState(MineSetupOnPauseService mineSetupOnPauseService)
		{
			_mineSetupOnPauseService = mineSetupOnPauseService;
		}

		public void Update(float deltaTime)
		{
			_mineSetupOnPauseService.Update(deltaTime);
		}

		public override void Exit()
		{
			base.Exit();

			_mineSetupOnPauseService.Cleanup();
		}
	}
}
