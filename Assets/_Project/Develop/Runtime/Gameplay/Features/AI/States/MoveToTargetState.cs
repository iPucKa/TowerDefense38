using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities.StateMachineCore;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI.States
{
	public class MoveToTargetState : State, IUpdatableState
	{
		private ReactiveVariable<Vector3> _movementDirection;
		private ReactiveVariable<Vector3> _rotationDirection;

		public MoveToTargetState(Entity entity)
		{
			_movementDirection = entity.MoveDirection;
			_rotationDirection = entity.RotationDirection;			
		}

		public override void Enter()
		{
			base.Enter();
			
			_movementDirection.Value = _rotationDirection.Value;			
		}

		public void Update(float deltaTime)
		{
			
		}

		public override void Exit()
		{
			base.Exit();

			_movementDirection.Value = Vector3.zero;
		}
	}
}
