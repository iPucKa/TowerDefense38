using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.Gameplay.Features.Attack;
using Assets._Project.Develop.Runtime.Gameplay.Features.Attack.Explosion;
using Assets._Project.Develop.Runtime.Gameplay.Features.ContactTakeDamage;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle;
using Assets._Project.Develop.Runtime.Gameplay.Features.MovementFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.Sensors;
using Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagement;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore
{
	public class EntitiesFactory
	{
		private readonly DIContainer _container;
		private readonly ConfigsProviderService _configProviderService;
		private readonly EntitiesLifeContext _entitiesLifeContext;
		private readonly CollidersRegistryService _collidersRegistryService;
		private readonly MouseTrackService _mouseTrackService;
		private readonly MonoEntitiesFactory _monoEntitiesFactory;

		public EntitiesFactory(DIContainer container)
		{
			_container = container;
			_configProviderService = _container.Resolve<ConfigsProviderService>();
			_entitiesLifeContext = _container.Resolve<EntitiesLifeContext>();
			_monoEntitiesFactory = _container.Resolve<MonoEntitiesFactory>();
			_collidersRegistryService = _container.Resolve<CollidersRegistryService>();
			_mouseTrackService = _container.Resolve<MouseTrackService>();
		}

		public Entity CreateFortress(Vector3 position, FortressConfig config, float maxHelth)
		{
			Entity entity = CreateEmpty();

			_monoEntitiesFactory.Create(entity, position, "Entities/Fortress");

			entity
				.AddMaxHealth(new ReactiveVariable<float>(maxHelth))
				.AddCurrentHealth(new ReactiveVariable<float>(maxHelth))
				.AddIsDead()
				.AddInDeathProcess()
				.AddDeathProcessInitialTime(new ReactiveVariable<float>(config.DeathProcessTime))
				.AddDeathProcessCurrentTime()
				.AddTakeDamageRequest()
				.AddTakeDamageEvent()

				.AddIsAttackKeyPressed()                                                            // ЗАПРОС НА НАЧАЛО АТАКИ

				.AddStartAttackRequest()
				.AddStartAttackEvent()
				.AddAttackProcessInitialTime(new ReactiveVariable<float>(config.AttackProcessTime))
				.AddAttackProcessCurrentTime()
				.AddInAttackProcess()
				.AddEndAttackEvent()
				.AddAttackDelayTime(new ReactiveVariable<float>(config.AttackDelayTime))
				.AddAttackDelayEndEvent()

				.AddAreaContactDamage(new ReactiveVariable<float>(config.ExplosionDamage))         // ДАМАГ ПО ПЛОЩАДИ от БОМБ
				.AddAreaContactRadius(new ReactiveVariable<float>(config.ExplosionRadius))

				.AddAttackCanceledEvent()
				.AddAttackCooldownInitialTime(new ReactiveVariable<float>(config.AttackCooldown))
				.AddAttackCooldownCurrentTime()
				.AddInAttackCooldown();

			ICompositCondition mustDie = new CompositCondition()
				.Add(new FuncCondition(() => entity.CurrentHealth.Value <= 0));

			ICompositCondition mustSelfRelease = new CompositCondition()
				.Add(new FuncCondition(() => entity.IsDead.Value))
				.Add(new FuncCondition(() => entity.InDeathProcess.Value == false));

			ICompositCondition canApplyDamage = new CompositCondition()
				.Add(new FuncCondition(() => entity.IsDead.Value == false));

			ICompositCondition canStartAttack = new CompositCondition()								// Можно ли начинать бомбить
				.Add(new FuncCondition(() => entity.IsDead.Value == false))
				.Add(new FuncCondition(() => entity.InAttackProcess.Value == false))
				.Add(new FuncCondition(() => entity.InAttackCooldown.Value == false));

			ICompositCondition mustCancelAttack = new CompositCondition(LogicOperations.Or)
				.Add(new FuncCondition(() => entity.IsDead.Value));

			entity
				.AddMustDie(mustDie)
				.AddMustSelfRelease(mustSelfRelease)
				.AddCanApplyDamage(canApplyDamage)
				.AddCanStartAttack(canStartAttack)													// Участвует в состоянии AttackByMouseKeyState - формирует запрос на НАЧАЛО АТАКИ по клику мышкой (и переключает состояние IsAttackKeyPressed)
				.AddMustCancelAttack(mustCancelAttack);

			entity				
				.AddSystem(new StartAttackSystem())													// тут проверяется условие canStartAttack и формирует событие НАЧАЛА АТАКИ
				.AddSystem(new ExplosionSystem(_mouseTrackService, _collidersRegistryService))		// СОЗДАЕТ БОМБУ при событии начала атаки

				.AddSystem(new AttackCancelSystem())
				.AddSystem(new AttackProcessTimerSystem())
				.AddSystem(new AttackDelayEndTriggerSystem())
				.AddSystem(new EndAttackSystem())
				.AddSystem(new AttackCooldownTimerSystem())

				.AddSystem(new ApplyDamageSystem())

				.AddSystem(new DeathSystem())
				.AddSystem(new DisableCollidersOnDeathSystem())
				.AddSystem(new DeathProcessTimerSystem())
				.AddSystem(new SelfReleaseSystem(_entitiesLifeContext));

			return entity;
		}

		public Entity CreateAgroEnemy(Vector3 position, AgroEnemyConfig config)
		{
			Entity entity = CreateEmpty();

			_monoEntitiesFactory.Create(entity, position, "Entities/Ghost");

			entity
				.AddMoveDirection()
				.AddMoveSpeed(new ReactiveVariable<float>(config.MoveSpeed))
				.AddIsMoving()
				.AddRotationDirection()
				.AddRotationSpeed(new ReactiveVariable<float>(config.RotationSpeed))

				.AddMaxHealth(new ReactiveVariable<float>(config.MaxHealth))
				.AddCurrentHealth(new ReactiveVariable<float>(config.MaxHealth))
				.AddIsDead()
				.AddInDeathProcess()

				.AddDeathProcessInitialTime(new ReactiveVariable<float>(config.DeathProcessTime))
				.AddDeathProcessCurrentTime()
				.AddTakeDamageRequest()
				.AddTakeDamageEvent()
				.AddContactsDetectingMask(Layers.CharactersMask)

				.AddAttackProcessInitialTime(new ReactiveVariable<float>(0))
				.AddAttackProcessCurrentTime()
				.AddInAttackProcess()
				.AddStartAttackRequest()
				.AddStartAttackEvent()
				.AddEndAttackEvent()
				.AddAttackDelayTime(new ReactiveVariable<float>(0))
				.AddAttackDelayEndEvent()
				.AddAttackCanceledEvent()

				.AddAreaContactDamage(new ReactiveVariable<float>(config.ExplosionDamage))
				.AddAreaContactRadius(new ReactiveVariable<float>(config.ExplosionRadius))

				.AddAreaContactCollidersBuffer(new Buffer<Collider>(64))
				.AddAreaContactEntitiesBuffer(new Buffer<Entity>(64))
				.AddIsTouchMainHero()																//Компонент на касание ГГ
				.AddCurrentTarget();

			ICompositCondition canMove = new CompositCondition()
				.Add(new FuncCondition(() => entity.IsDead.Value == false));

			ICompositCondition canRotate = new CompositCondition()
				.Add(new FuncCondition(() => entity.IsDead.Value == false));

			ICompositCondition mustDie = new CompositCondition()
				.Add(new FuncCondition(() => entity.CurrentHealth.Value <= 0));

			ICompositCondition mustSelfRelease = new CompositCondition()
				.Add(new FuncCondition(() => entity.IsDead.Value))
				.Add(new FuncCondition(() => entity.InDeathProcess.Value == false));

			ICompositCondition canApplyDamage = new CompositCondition()
				.Add(new FuncCondition(() => entity.IsDead.Value == false));

			ICompositCondition canStartAttack = new CompositCondition()
				.Add(new FuncCondition(() => entity.IsDead.Value == false))
				.Add(new FuncCondition(() => entity.InAttackProcess.Value == false))
				.Add(new FuncCondition(() => entity.IsTouchMainHero.Value));

			ICompositCondition mustCancelAttack = new CompositCondition(LogicOperations.Or)
				.Add(new FuncCondition(() => entity.IsDead.Value));

			entity
				.AddCanMove(canMove)
				.AddCanRotate(canRotate)
				.AddMustDie(mustDie)
				.AddCanStartAttack(canStartAttack)													// Участвует в состоянии AttackTriggerState - формирует запрос на НАЧАЛО АТАКИ
				.AddCanApplyDamage(canApplyDamage)
				.AddMustSelfRelease(mustSelfRelease)
				.AddMustCancelAttack(mustCancelAttack);

			entity
				.AddSystem(new RigidbodyMovementSystem())
				.AddSystem(new RigidbodyRotationSystem())

				.AddSystem(new AreaContactsDetectingSystem())
				.AddSystem(new AreaContactsEntitiesFilterSystem(_collidersRegistryService))

				.AddSystem(new MainHeroTouchAreaDetectorSystem())                                   // ПРОВЕРКА на касание ГГ в ОБЛАСТИ переключается поле _isTouchMainHero
				.AddSystem(new StartAttackSystem())                                                 // тут проверяется условие canStartAttack и формирует событие НАЧАЛА АТАКИ

				.AddSystem(new DealDamageOnAreaByEventSystem())										//ВЗРЫВАЕТСЯ и НАНОСИТ УРОН СУЩНОСТЯМ ПО ПЛОЩАДИ СРАЗУ ПО СОБЫТИЮ КОНЦА АТАКИ

				.AddSystem(new AttackCancelSystem())
				.AddSystem(new AttackProcessTimerSystem())
				.AddSystem(new AttackDelayEndTriggerSystem())				
				.AddSystem(new EndAttackSystem())

				.AddSystem(new ApplyDamageSystem())

				.AddSystem(new DeathSystem())
				.AddSystem(new DisableCollidersOnDeathSystem())
				.AddSystem(new DeathProcessTimerSystem())
				.AddSystem(new SelfReleaseSystem(_entitiesLifeContext));

			return entity;
		}

		public Entity CreateGhost(Vector3 position, GhostConfig config)
		{
			Entity entity = CreateEmpty();

			_monoEntitiesFactory.Create(entity, position, "Entities/Ghost");

			entity
				.AddMoveDirection()
				.AddMoveSpeed(new ReactiveVariable<float>(config.MoveSpeed))
				.AddIsMoving()
				.AddRotationDirection()
				.AddRotationSpeed(new ReactiveVariable<float>(config.RotationSpeed))
				.AddMaxHealth(new ReactiveVariable<float>(config.MaxHealth))
				.AddCurrentHealth(new ReactiveVariable<float>(config.MaxHealth))
				.AddIsDead()
				.AddInDeathProcess()
				.AddDeathProcessInitialTime(new ReactiveVariable<float>(config.DeathProcessTime))
				.AddDeathProcessCurrentTime()
				.AddTakeDamageRequest()
				.AddTakeDamageEvent()
				.AddContactsDetectingMask(Layers.CharactersMask)
				.AddContactCollidersBuffer(new Buffer<Collider>(64))
				.AddContactEntitiesBuffer(new Buffer<Entity>(64))
				.AddBodyContactDamage(new ReactiveVariable<float>(config.BodyContactDamage));

			ICompositCondition canMove = new CompositCondition()
				.Add(new FuncCondition(() => entity.IsDead.Value == false));

			ICompositCondition canRotate = new CompositCondition()
				.Add(new FuncCondition(() => entity.IsDead.Value == false));

			ICompositCondition mustDie = new CompositCondition()
				.Add(new FuncCondition(() => entity.CurrentHealth.Value <= 0));

			ICompositCondition mustSelfRelease = new CompositCondition()
				.Add(new FuncCondition(() => entity.IsDead.Value))
				.Add(new FuncCondition(() => entity.InDeathProcess.Value == false));

			ICompositCondition canApplyDamage = new CompositCondition()
				.Add(new FuncCondition(() => entity.IsDead.Value == false));

			entity
				.AddCanMove(canMove)
				.AddCanRotate(canRotate)
				.AddMustDie(mustDie)
				.AddMustSelfRelease(mustSelfRelease)
				.AddCanApplyDamage(canApplyDamage);

			entity
				.AddSystem(new RigidbodyMovementSystem())
				.AddSystem(new RigidbodyRotationSystem())
				.AddSystem(new BodyContactsDetectingSystem())
				.AddSystem(new BodyContactsEntitiesFilterSystem(_collidersRegistryService))
				.AddSystem(new DealDamageOnContactSystem())
				.AddSystem(new ApplyDamageSystem())
				.AddSystem(new DeathSystem())
				.AddSystem(new DisableCollidersOnDeathSystem())
				.AddSystem(new DeathProcessTimerSystem())
				.AddSystem(new SelfReleaseSystem(_entitiesLifeContext));			

			return entity;
		}		

		public Entity CreateProjectile(Vector3 position, Vector3 direction, float damage, Entity owner)
		{
			Entity entity = CreateEmpty();

			_monoEntitiesFactory.Create(entity, position, "Entities/Projectile");

			entity
				.AddMoveDirection(new ReactiveVariable<Vector3>(direction))
				.AddMoveSpeed(new ReactiveVariable<float>(10))
				.AddIsMoving()
				.AddRotationDirection(new ReactiveVariable<Vector3>(direction))
				.AddRotationSpeed(new ReactiveVariable<float>(9999))
				.AddIsDead()
				.AddContactsDetectingMask(Layers.CharactersMask | Layers.EnvironmentMask) // Чтобы получить маску из нескольких слоев воспользуемся побитовым ИЛИ
				.AddContactCollidersBuffer(new Buffer<Collider>(64))
				.AddContactEntitiesBuffer(new Buffer<Entity>(64))
				.AddBodyContactDamage(new ReactiveVariable<float>(damage))
				.AddDeathMask(Layers.EnvironmentMask)
				.AddIsTouchDeathMask()
				.AddIsTouchAnotherTeam()
				.AddTeam(new ReactiveVariable<Teams>(owner.Team.Value));

			ICompositCondition canMove = new CompositCondition()
				.Add(new FuncCondition(() => entity.IsDead.Value == false));

			ICompositCondition canRotate = new CompositCondition()
				.Add(new FuncCondition(() => entity.IsDead.Value == false));

			ICompositCondition mustDie = new CompositCondition()
				.Add(new FuncCondition(() => entity.IsTouchDeathMask.Value))
				.Add(new FuncCondition(() => entity.IsTouchAnotherTeam.Value));

			ICompositCondition mustSelfRelease = new CompositCondition()
				.Add(new FuncCondition(() => entity.IsDead.Value));

			entity
				.AddCanMove(canMove)
				.AddCanRotate(canRotate)
				.AddMustDie(mustDie)
				.AddMustSelfRelease(mustSelfRelease);

			entity
				.AddSystem(new RigidbodyMovementSystem())
				.AddSystem(new RigidbodyRotationSystem())
				.AddSystem(new BodyContactsDetectingSystem())
				.AddSystem(new BodyContactsEntitiesFilterSystem(_collidersRegistryService))
				.AddSystem(new DealDamageOnContactSystem())
				.AddSystem(new DeathMaskTouchDetectorSystem())
				.AddSystem(new AnotherTeamTouchDetectorSystem())
				.AddSystem(new DeathSystem())
				.AddSystem(new DisableCollidersOnDeathSystem())
				.AddSystem(new SelfReleaseSystem(_entitiesLifeContext));

			_entitiesLifeContext.Add(entity);

			return entity;
		}

		public Entity CreateContactTrigger(Vector3 position)
		{
			Entity entity = CreateEmpty();

			_monoEntitiesFactory.Create(entity, position, "Entities/ContactTrigger");

			entity
				.AddContactsDetectingMask(Layers.CharactersMask)
				.AddContactCollidersBuffer(new Buffer<Collider>(64))
				.AddContactEntitiesBuffer(new Buffer<Entity>(64));

			entity
				.AddSystem(new BodyContactsDetectingSystem())
				.AddSystem(new BodyContactsEntitiesFilterSystem(_collidersRegistryService));

			_entitiesLifeContext.Add(entity);

			return entity;
		}

		public Entity CreateMine(Vector3 position)
		{
			Entity entity = CreateEmpty();

			MineConfig config = _configProviderService.GetConfig<MineConfig>();

			_monoEntitiesFactory.Create(entity, position, "Entities/Mine");

			entity
				.AddIsDead()
				.AddContactsDetectingMask(Layers.CharactersMask)
				.AddAreaContactCollidersBuffer(new Buffer<Collider>(64))
				.AddAreaContactEntitiesBuffer(new Buffer<Entity>(64))			
				
				.AddAreaContactDamage(new ReactiveVariable<float>(config.AreaContactDamage))
				.AddAreaContactRadius(new ReactiveVariable<float>(config.DamageRadius))

				.AddTeam(new ReactiveVariable<Teams>(Teams.MainHero));

			ICompositCondition mustSelfRelease = new CompositCondition()
				.Add(new FuncCondition(() => entity.IsDead.Value));

			entity				
				.AddMustSelfRelease(mustSelfRelease);

			entity

				.AddSystem(new AreaContactsDetectingSystem())
				.AddSystem(new AreaContactsEntitiesFilterSystem(_collidersRegistryService))
				.AddSystem(new DealDamageOnAreaContactSystem())                                     //В АПДЕЙТЕ ПРОВЕРЯЕТ ОКРУЖАЮЩИЙ БУФЕР и НАНОСИТ УРОН СУЩНОСТЯМ ПО ПЛОЩАДИ				

				.AddSystem(new DisableCollidersOnDeathSystem())
				.AddSystem(new SelfReleaseSystem(_entitiesLifeContext));

			_entitiesLifeContext.Add(entity);

			return entity;
		}		

		private Entity CreateEmpty() => new Entity();
	}
}
