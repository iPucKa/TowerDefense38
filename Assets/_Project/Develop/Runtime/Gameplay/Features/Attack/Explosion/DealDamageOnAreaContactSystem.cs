using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack.Explosion
{
	public class DealDamageOnAreaContactSystem : IInitializableSystem, IUpdatableSystem
	{
		private Entity _entity;
		private Buffer<Entity> _contacts;
		private ReactiveVariable<float> _damage;
		private ReactiveVariable<bool> _isDead;

		private List<Entity> _damagedEntities = new();

		public void OnInit(Entity entity)
		{
			_entity = entity;

			_damage = entity.AreaContactDamage; 			

			_contacts = entity.AreaContactEntitiesBuffer;

			_isDead = entity.IsDead;

			_damagedEntities = new List<Entity>(_contacts.Items.Length);
		}

		public void OnUpdate(float deltaTime)
		{
			//обработка касания
			for (int i = 0; i < _contacts.Count; i++)
			{
				Entity contactEntity = _contacts.Items[i];

				if (EntitiesHelper.TryTakeDamageFrom(_entity, contactEntity, _damage.Value))
					_damagedEntities.Add(contactEntity);										
			}

			if (_damagedEntities.Count != 0)
				_isDead.Value = true;			
		}		
	}
}