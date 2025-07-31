using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities.StateMachineCore;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI.States
{
	public class AttackByMouseKeyState : State, IUpdatableState
	{
		private readonly IInputService _inputService;
		private ReactiveEvent _attackRequest;
		private ReactiveVariable<bool> _isAttackKeyPressed;

		public AttackByMouseKeyState(
			Entity entity,
			IInputService inputService)
		{
			_attackRequest = entity.StartAttackRequest;
			_isAttackKeyPressed = entity.IsAttackKeyPressed;
			_inputService = inputService;
		}		

		public void Update(float deltaTime)
		{
			//if (_isAttackKeyPressed.Value == false)
			//	return;			

			if (_isAttackKeyPressed.Value)
			{
				_attackRequest.Invoke();
				_isAttackKeyPressed.Value = false;
			}

			if (_isAttackKeyPressed.Value == false)
				_isAttackKeyPressed.Value = _inputService.IsAttackButtonPressed;
		}

		public override void Exit()
		{
			base.Exit();

			_isAttackKeyPressed.Value = false;
		}
	}
}
