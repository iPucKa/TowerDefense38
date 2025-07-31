using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack.Explosion
{
	public class BombSetupSystem : IInitializableSystem, IDisposableSystem
	{
		private readonly EntitiesFactory _entitiesFactory;
		private readonly MouseTrackService _mouseTrackService;

		private Entity _entity;
		private ReactiveVariable<float> _damage;

		private ReactiveEvent _startAttackEvent;

		private IDisposable _startAttackDisposable;

		public BombSetupSystem(
			EntitiesFactory entitiesFactory, 
			MouseTrackService mouseTrackService)
		{
			_entitiesFactory = entitiesFactory;
			_mouseTrackService = mouseTrackService;
		}

		public void OnInit(Entity entity)
		{
			_entity = entity;
			_startAttackEvent = entity.StartAttackEvent;

			_damage = entity.AreaContactDamage;                                             // ТУТ УКАЗАТЬ ДАМАГ ОТ ВЗРЫВА БОМБЫ

			_startAttackDisposable = _startAttackEvent.Subscribe(OnAttackStarted);
		}

		private void OnAttackStarted()
		{
			_entitiesFactory.CreateBomb(_mouseTrackService.Position, _damage.Value, _entity);
		}

		public void OnDispose()
		{
			_startAttackDisposable.Dispose();
		}
	}
}
