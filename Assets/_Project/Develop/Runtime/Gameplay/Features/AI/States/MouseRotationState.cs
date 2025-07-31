using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities.StateMachineCore;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI.States
{
	public class MouseRotationState : State, IUpdatableState
	{
		private Transform _transform;
		private ReactiveVariable<Vector3> _rotationDirection;
		private ReactiveVariable<bool> _isAttackKeyPressed;
		
		private IInputService _inputService;
		private MouseTrackService _mouseTrackService;

		public MouseRotationState(
			Entity entity,
			IInputService inputService,
			MouseTrackService mouseTrackService)
		{
			_transform = entity.Transform;
			_rotationDirection = entity.RotationDirection;
			_isAttackKeyPressed = entity.IsAttackKeyPressed;
			_inputService = inputService;
			_mouseTrackService = mouseTrackService;
		}

		public void Update(float deltaTime)
		{
			_rotationDirection.Value = (_mouseTrackService.Position - _transform.position).normalized;

			if (_isAttackKeyPressed.Value == false)
				_isAttackKeyPressed.Value = _inputService.IsAttackButtonPressed;
		}		
	}
}
