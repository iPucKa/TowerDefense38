using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.TeleportationFeature
{
	public class RigidbodyTeleportingSystem : IInitializableSystem, IDisposableSystem
	{
		private ReactiveVariable<Vector3> _teleportPosition;
		private ReactiveEvent _teleportRequest;
		private ReactiveEvent _teleportEvent;
		private Rigidbody _rigidbody;		

		private ICompositCondition _canTeleport;
		private IDisposable _teleportRequestDisposable;

		public void OnInit(Entity entity)
		{
			_teleportPosition = entity.TeleportPosition;
			_rigidbody = entity.Rigidbody;
			_canTeleport = entity.CanTeleport;
			_teleportRequest = entity.TeleportingRequest;
			_teleportEvent = entity.TeleportingEvent;

			_teleportRequestDisposable = _teleportRequest.Subscribe(OnTeleportRequest);
		}

		private void OnTeleportRequest()
		{
			if (_canTeleport.Evaluate() == false)
			{
				Debug.Log("Не могу телепортироваться!");
				return; 
			}

			_rigidbody.position = _teleportPosition.Value;

			_teleportEvent.Invoke();
		}

		public void OnDispose()
		{
			_teleportRequestDisposable.Dispose();
		}
	}
}
