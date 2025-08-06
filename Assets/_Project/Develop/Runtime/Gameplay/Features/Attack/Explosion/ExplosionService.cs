using Assets._Project.Develop.Runtime.Configs.Gameplay;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilities;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagement;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack.Explosion
{
	public class ExplosionService
	{
		private readonly IInputService _inputService;
		private readonly MouseTrackService _mouseTrackService;
		private readonly CollidersRegistryService _colllidersRegistryService;
		private readonly ConfigsProviderService _configProviderService;

		private readonly float _damage;
		private readonly float _radius;

		private Buffer<Collider> _contacts = new(64);
		private Buffer<Entity> _contactsEntities = new(64);

		private ReactiveVariable<bool> _isAttackKeyPressed = new();		

		public ExplosionService(
			IInputService inputService,
			MouseTrackService mouseTrackService,
			CollidersRegistryService colllidersRegistryService,
			ConfigsProviderService configProviderService)
		{
			_inputService = inputService;
			_mouseTrackService = mouseTrackService;
			_colllidersRegistryService = colllidersRegistryService;
			_configProviderService = configProviderService;

			_damage = _configProviderService.GetConfig<GameplayConfig>().ExplosionDamageByBomb;
			_radius = _configProviderService.GetConfig<GameplayConfig>().ExplosionRadiusByBomb;
		}

		public void Update(float deltaTime)
		{
			if (_isAttackKeyPressed.Value == false)
				_isAttackKeyPressed.Value = _inputService.IsAttackButtonPressed;

			if (_isAttackKeyPressed.Value == true)
			{
				Detonate();

				_isAttackKeyPressed.Value = false;
			}		
		}

		private void Detonate()
		{
			// КАПСУЛА В ПОЗИЦИИ МЫШКИ, собираю информацию по коллайдерам

			_contacts.Count = Physics.OverlapCapsuleNonAlloc(
				_mouseTrackService.Position,
				_mouseTrackService.Position + Vector3.up * 3,
				_radius,
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

			//Запрос на нанесение урона КОМАНДЕ ВРАГОВ
			for (int i = 0; i < _contactsEntities.Count; i++)
			{
				Entity contactEntity = _contactsEntities.Items[i];

				EntitiesHelper.TryApplyDamageToEnemyTeam(contactEntity, _damage);
			}
		}

		public void Cleanup()
		{
			_isAttackKeyPressed.Value = false;			
		}
	}
}
