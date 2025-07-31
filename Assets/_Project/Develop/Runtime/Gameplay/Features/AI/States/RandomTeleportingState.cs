using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities.StateMachineCore;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI.States
{
	public class RandomTeleportingState : State, IUpdatableState
	{
		private ReactiveVariable<Vector3> _teleportPosition;
		private ReactiveVariable<float> _teleportRadius;
		private ReactiveEvent _teleportRequest;

		private float _cooldownBetweenTeleportPositionGeneration;
		private float _time;

		public RandomTeleportingState(
			Entity entity,
			float cooldownBetweenTeleportPositionGeneration)
		{
			_teleportRequest = entity.TeleportingRequest;
			_teleportPosition = entity.TeleportPosition;
			_teleportRadius = entity.TeleportRadius;

			_cooldownBetweenTeleportPositionGeneration = cooldownBetweenTeleportPositionGeneration;
		}

		public override void Enter()
		{
			base.Enter();

			_time = 0;
		}		

		public void Update(float deltaTime)
		{
			_time += deltaTime;

			if (_time >= _cooldownBetweenTeleportPositionGeneration)
			{
				GenerateTeleportPosition();
				_teleportRequest.Invoke();
				_time = 0;
			}
		}

		private void GenerateTeleportPosition()
		{
			Vector2 point = Random.insideUnitCircle * _teleportRadius.Value;

			_teleportPosition.Value = new Vector3(point.x, 0f, point.y);
		}
	}
}
