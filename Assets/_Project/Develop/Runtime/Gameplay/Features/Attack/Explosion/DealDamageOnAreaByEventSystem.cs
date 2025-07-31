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
		private readonly Entity _owner;

		private Entity _entity;
		private ReactiveVariable<float> _currentHealth;
		private Buffer<Entity> _contacts;
		private ReactiveVariable<float> _damage;

		private ReactiveEvent _attackDelayEndEvent;

		private IDisposable _attackDelayEndDisposable;

		private List<Entity> _processedEntities;

		public DealDamageOnAreaByEventSystem(Entity owner)
		{
			_owner = owner;
		}

		public void OnInit(Entity entity)
		{
			_entity = entity;

			_damage = entity.AreaContactDamage;

			_currentHealth = entity.CurrentHealth;

			_contacts = entity.AreaContactEntitiesBuffer;

			_processedEntities = new List<Entity>(_contacts.Items.Length);

			_attackDelayEndEvent = _owner.AttackDelayEndEvent;								// ПО СОБЫТИЮ ОТ ВЛАДЕЛЬЦА ИНИЦИАТОРА ВЗРЫВА

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

					if (_currentHealth != null)
						_currentHealth.Value = 0;                                            // ТУТ СРАЗУ ОБНУЛЯЮ ЗДОРОВЬЕ ПЕРЕДАННОЙ СУЩНОСТИ (если есть компонент здоровья)
				}
			}

			//обработка выхода из касания
			for (int i = _processedEntities.Count - 1; i >= 0; i--)
				if (ContainInContacts(_processedEntities[i]) == false)
					_processedEntities.Remove(_processedEntities[i]);
		}

		public void OnDispose()
		{
			_attackDelayEndDisposable.Dispose();
		}

		public bool ContainInContacts(Entity entity)
		{
			for (int i = 0; i < _contacts.Count; i++)
				if (_contacts.Items[i] == entity)
					return true;

			return false;
		}
	}
}
