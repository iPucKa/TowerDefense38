using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack.Area
{
	public class AreaAttackSystem : IInitializableSystem, IDisposableSystem
	{
		private Buffer<Entity> _contacts;
		private ReactiveVariable<float> _damage;
		private ReactiveEvent _attackDelayEndEvent;

		private IDisposable _attackDelayEndDisposable;

		public void OnInit(Entity entity)
		{
			_contacts = entity.AreaContactEntitiesBuffer;
			_damage = entity.AreaContactDamage;
			_attackDelayEndEvent = entity.AttackDelayEndEvent;

			_attackDelayEndDisposable = _attackDelayEndEvent.Subscribe(OnAttackDelayEnd);
		}

		public void OnDispose()
		{
			_attackDelayEndDisposable.Dispose();
		}

		private void OnAttackDelayEnd()
		{
			//Debug.Log("Ищу кому бы нанести дамаг, число контактов: " + _contacts.Count);
			
			for (int i = 0; i < _contacts.Count; i++)
			{
				Entity contactEntity = _contacts.Items[i];

				DamageHandler.ApplyDamage(contactEntity, _damage.Value);								
			}
		}
	}
}
