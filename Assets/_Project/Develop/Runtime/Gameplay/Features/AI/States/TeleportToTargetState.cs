using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities.StateMachineCore;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI.States
{
	public class TeleportToTargetState : State, IUpdatableState
	{
		private ReactiveVariable<Entity> _currentTarget;
		private Transform _transform;

		private ReactiveVariable<Vector3> _teleportPosition;
		private ReactiveVariable<float> _teleportRadius;
		private ReactiveEvent _teleportRequest;

		private float _cooldownBetweenTeleportPositionGeneration;
		private float _time;

		public TeleportToTargetState(
			Entity entity,
			float cooldownBetweenTeleportPositionGeneration)
		{
			_currentTarget = entity.CurrentTarget;
			_transform = entity.Transform;

			_teleportRequest = entity.TeleportingRequest;
			_teleportPosition = entity.TeleportPosition;
			_teleportRadius = entity.TeleportRadius;

			_cooldownBetweenTeleportPositionGeneration = cooldownBetweenTeleportPositionGeneration;
		}		

		public void Update(float deltaTime)
		{
			_time += deltaTime;

			if (_currentTarget.Value != null)
			{
				if (_time >= _cooldownBetweenTeleportPositionGeneration)
				{
					GetTeleportPosition();

					_teleportRequest.Invoke();
					_time = 0;
				}
			}						
		}

		private void GetTeleportPosition()
		{
			Vector3 direction = (_currentTarget.Value.Transform.position - _transform.position).normalized;

			Vector3 newPosition = _transform.position + direction * _teleportRadius.Value;

			_teleportPosition.Value = newPosition;
		}
	}
}
