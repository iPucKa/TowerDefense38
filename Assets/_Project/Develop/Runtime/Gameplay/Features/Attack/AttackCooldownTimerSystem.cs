using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
	public class AttackCooldownTimerSystem : IInitializableSystem, IUpdatableSystem, IDisposableSystem
	{
		private ReactiveVariable<float> _currentTime;
		private ReactiveVariable<float> _initialTime;
		private ReactiveVariable<bool> _inAttackCoolDown;

		private ReactiveEvent _endAttackEvent;
		private IDisposable _endAttackEventDisposable;

		public void OnInit(Entity entity)
		{
			_currentTime = entity.AttackCooldownCurrentTime;
			_initialTime = entity.AttackCooldownInitialTime;
			_inAttackCoolDown = entity.InAttackCooldown;
			_endAttackEvent = entity.EndAttackEvent;

			_endAttackEventDisposable = _endAttackEvent.Subscribe(OnEndAttack);
		}

		public void OnUpdate(float deltaTime)
		{
			if (_inAttackCoolDown.Value == false)
				return;

			_currentTime.Value -= deltaTime;

			if (CooldownIsOver())
			{
				_inAttackCoolDown.Value = false;
				Debug.Log("КУЛДАУН ЗАКОНЧИЛСЯ");
			}
		}

		public void OnDispose()
		{
			_endAttackEventDisposable.Dispose();
		}

		private void OnEndAttack()
		{
			Debug.Log("КУЛДАУН НАЧАЛСЯ");
			_currentTime.Value = _initialTime.Value;
			_inAttackCoolDown.Value = true;
		}

		private bool CooldownIsOver() => _currentTime.Value <= 0;
	}
}
