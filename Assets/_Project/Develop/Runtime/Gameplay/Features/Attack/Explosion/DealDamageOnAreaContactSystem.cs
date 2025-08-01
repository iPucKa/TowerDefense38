using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack.Explosion
{
	public class DealDamageOnAreaContactSystem : IInitializableSystem, IUpdatableSystem
	{
		private Entity _entity;
		private Buffer<Entity> _contacts;
		private ReactiveVariable<float> _damage;
		private ReactiveVariable<bool> _isDead;	

		public void OnInit(Entity entity)
		{
			_entity = entity;

			_damage = entity.AreaContactDamage; 			

			_contacts = entity.AreaContactEntitiesBuffer;

			_isDead = entity.IsDead;
		}

		public void OnUpdate(float deltaTime)
		{
			//обработка касания
			for (int i = 0; i < _contacts.Count; i++)
			{
				Entity contactEntity = _contacts.Items[i];

				EntitiesHelper.TryTakeDamageFrom(_entity, contactEntity, _damage.Value);

				_isDead.Value = true;				
			}
		}			
	}
}
