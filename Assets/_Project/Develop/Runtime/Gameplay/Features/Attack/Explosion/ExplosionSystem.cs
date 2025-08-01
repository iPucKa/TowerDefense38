using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilities;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack.Explosion
{
	public class ExplosionSystem : IInitializableSystem, IDisposableSystem
	{
		//private readonly EntitiesFactory _entitiesFactory;
		private readonly MouseTrackService _mouseTrackService; 
		private readonly CollidersRegistryService _colllidersRegistryService;

		private Entity _entity;
		private ReactiveVariable<float> _damage;
		private ReactiveVariable<float> _radius;

		private ReactiveEvent _startAttackEvent;

		private IDisposable _startAttackDisposable;

		private Buffer<Collider> _contacts = new(64);
		private Buffer<Entity> _contactsEntities = new(64);
		
		private List<Entity> _processedEntities = new();

		public ExplosionSystem(
			MouseTrackService mouseTrackService,
			CollidersRegistryService colllidersRegistryService)
		{
			_mouseTrackService = mouseTrackService;
			_colllidersRegistryService = colllidersRegistryService;
		}

		public void OnInit(Entity entity)
		{
			_entity = entity;
			_startAttackEvent = entity.StartAttackEvent;

			_damage = entity.AreaContactDamage;
			_radius = entity.AreaContactRadius;

			_processedEntities = new List<Entity>(_contacts.Items.Length);

			_startAttackDisposable = _startAttackEvent.Subscribe(OnAttackStarted);
		}

		private void OnAttackStarted()
		{
			//_entitiesFactory.CreateBomb(_mouseTrackService.Position, _damage.Value, _entity);

			// КАПСУЛА В ПОЗИЦИИ МЫШКИ, собираю информацию по коллайдерам

			_contacts.Count = Physics.OverlapCapsuleNonAlloc(
				_mouseTrackService.Position,
				_mouseTrackService.Position + Vector3.up * 3,
				_radius.Value,
				_contacts.Items,
				Layers.CharactersMask,
				QueryTriggerInteraction.Ignore);

			//Из массива коллайдеров отбираю массив именно тех, кто сущности

			_contactsEntities.Count = 0;

			for (int i = 0; i < _contacts.Count; i++)
			{
				Collider collider = _contacts.Items[i];

				Entity contactEntity = _colllidersRegistryService.GetBy(collider);
				if (contactEntity != null)
				{
					_contactsEntities.Items[_contactsEntities.Count] = contactEntity;
					_contactsEntities.Count++;
				}
			}

			Debug.Log("Контакты сущностей: " + _contactsEntities.Count);

			//Запрос на нанесение урона НЕДРУЖЕСТВЕННОЙ КОМАНДЕ
			for (int i = 0; i < _contactsEntities.Count; i++)
			{
				Entity contactEntity = _contactsEntities.Items[i];

				EntitiesHelper.TryTakeDamageFrom(_entity, contactEntity, _damage.Value);
			}
		}

		public void OnDispose()
		{
			_startAttackDisposable.Dispose();
		}		
	}
}
