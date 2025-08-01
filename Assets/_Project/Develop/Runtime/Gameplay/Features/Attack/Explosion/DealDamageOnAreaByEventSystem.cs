using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack.Explosion
{
	public class DealDamageOnAreaByEventSystem : IInitializableSystem, IDisposableSystem
	{
		private Entity _entity;
		private ReactiveVariable<float> _currentHealth;
		private Buffer<Entity> _contacts;
		private ReactiveVariable<float> _damage;

		private ReactiveEvent _attackDelayEndEvent;

		private IDisposable _attackDelayEndDisposable;

		private List<Entity> _processedEntities;		

		public void OnInit(Entity entity)
		{
			_entity = entity;

			_damage = entity.AreaContactDamage;

			_currentHealth = entity.CurrentHealth;

			_contacts = entity.AreaContactEntitiesBuffer;

			_processedEntities = new List<Entity>(_contacts.Items.Length);

			_attackDelayEndEvent = entity.AttackDelayEndEvent;								

			_attackDelayEndDisposable = _attackDelayEndEvent.Subscribe(OnAttackDelayEnd);
		}

		private void OnAttackDelayEnd()
		{
			//обработка первого касания
			for (int i = 0; i < _contacts.Count; i++)
			{
				Entity contactEntity = _contacts.Items[i];

				if (_processedEntities.Contains(contactEntity) == false)
				{
					_processedEntities.Add(contactEntity);

					EntitiesHelper.TryTakeDamageFrom(_entity, contactEntity, _damage.Value);

					_currentHealth.Value = 0;													// ТУТ СРАЗУ ОБНУЛЯЮ ЗДОРОВЬЕ ПЕРЕДАННОЙ СУЩНОСТИ
				}
			}
		}

		public void OnDispose()
		{
			_attackDelayEndDisposable.Dispose();
		}
	}
}
