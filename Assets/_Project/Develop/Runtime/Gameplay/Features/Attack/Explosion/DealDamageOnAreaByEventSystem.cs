using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack.Explosion
{
	public class DealDamageOnAreaByEventSystem : IInitializableSystem, IDisposableSystem
	{
		private Entity _entity;
		private ReactiveEvent<float> _damageRequest;

		private ReactiveVariable<float> _currentHealth;
		private Buffer<Entity> _contacts;
		private ReactiveVariable<float> _damage;

		private ReactiveEvent _attackDelayEndEvent;

		private IDisposable _attackDelayEndDisposable;

		public void OnInit(Entity entity)
		{
			_entity = entity;
			_damageRequest = entity.TakeDamageRequest;

			_damage = entity.AreaContactDamage;

			_currentHealth = entity.CurrentHealth;

			_contacts = entity.AreaContactEntitiesBuffer;

			_attackDelayEndEvent = entity.AttackDelayEndEvent;

			_attackDelayEndDisposable = _attackDelayEndEvent.Subscribe(OnAttackDelayEnd);
		}

		private void OnAttackDelayEnd()
		{
			//обработка касания
			for (int i = 0; i < _contacts.Count; i++)
			{
				Entity contactEntity = _contacts.Items[i];

				EntitiesHelper.TryTakeDamageFrom(_entity, contactEntity, _damage.Value);

				_damageRequest?.Invoke(_currentHealth.Value);
			}
		}

		public void OnDispose()
		{
			_attackDelayEndDisposable.Dispose();
		}
	}
}
