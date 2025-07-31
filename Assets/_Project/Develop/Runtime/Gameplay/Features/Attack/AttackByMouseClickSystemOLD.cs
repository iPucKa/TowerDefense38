using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
	public class AttackByMouseClickSystemOLD : IInitializableSystem, IDisposableSystem
	{
		private ReactiveEvent _attackKeyPressedEvent;
		private ReactiveEvent _startAttackReqest;

		private ICompositCondition _canAttack;

		private IDisposable _attackKeyPressedDisposable;

		public void OnInit(Entity entity)
		{
			_attackKeyPressedEvent = entity.AttackKeyPressedEvent;

			_startAttackReqest = entity.StartAttackRequest;
			_canAttack = entity.CanStartAttack;

			_attackKeyPressedDisposable = _attackKeyPressedEvent.Subscribe(OnAttackKeyPressed);
		}

		private void OnAttackKeyPressed()
		{
			if (_canAttack.Evaluate() == false)
				return;

			_startAttackReqest.Invoke();
		}

		public void OnDispose()
		{
			_attackKeyPressedDisposable.Dispose();
		}
	}
}
