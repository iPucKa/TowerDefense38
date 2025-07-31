﻿using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.ContactTakeDamage
{
	public class DealDamageOnContactSystem : IInitializableSystem, IUpdatableSystem
	{
		private Entity _entity;
		private Buffer<Entity> _contacts;
		private ReactiveVariable<float> _damage;

		private List<Entity> _processedEntities;

		public void OnInit(Entity entity)
		{
			_entity = entity;
			_contacts = entity.ContactEntitiesBuffer;
			_damage = entity.BodyContactDamage;

			_processedEntities = new List<Entity>(_contacts.Items.Length);
		}

		public void OnUpdate(float deltaTime)
		{
			//обработка первого касания
			for (int i = 0; i < _contacts.Count; i++)
			{
				Entity contactEntity = _contacts.Items[i];

				if (_processedEntities.Contains(contactEntity) == false)
				{
					_processedEntities.Add(contactEntity);

					//DamageHandler.ApplyDamage(contactEntity, _damage.Value);
					EntitiesHelper.TryTakeDamageFrom(_entity, contactEntity, _damage.Value);
				}
			}

			//обработка выхода из касания
			for (int i = _processedEntities.Count - 1; i >= 0; i--)
				if (ContainInContacts(_processedEntities[i]) == false)
					_processedEntities.Remove(_processedEntities[i]);
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
