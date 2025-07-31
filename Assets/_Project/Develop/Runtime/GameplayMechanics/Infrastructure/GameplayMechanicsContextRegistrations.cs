using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.AssetsManagement;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.GameplayMechanics.Infrastructure
{
	public class GameplayMechanicsContextRegistrations
	{
		private static Camera _camera;

		public static void Process(DIContainer container, Camera camera)
		{
			Debug.Log("Процесс регистрации сервисов на сцене геймплейных механик");

			_camera = camera;

			container.RegisterAsSingle(CreateEntitiesFactory);
			container.RegisterAsSingle(CreateEntitiesLifeContext);
			container.RegisterAsSingle(CreateMonoEntitiesFactory).NonLazy();
			container.RegisterAsSingle(CreateCollidersRegistryService);
			container.RegisterAsSingle(CreateBrainsFactory);
			container.RegisterAsSingle(CreateAIBrainsContext);
			container.RegisterAsSingle<IInputService>(CreateDesktopInput);
			container.RegisterAsSingle(CreateMouseTracktService);
		}

		private static MouseTrackService CreateMouseTracktService(DIContainer c)
		{
			return new MouseTrackService(
				_camera, 
				c.Resolve<IInputService>());
		}

		//Способ создания сервиса ввода с клавиатуры
		private static DesktopInput CreateDesktopInput(DIContainer c)
		{
			return new DesktopInput();
		}		

		//Способ создания Сервиса жизненного цикла мозгов
		private static AIBrainsContext CreateAIBrainsContext(DIContainer c)
		{
			return new AIBrainsContext();
		}

		//Способ создания Фабрики мозгов
		private static BrainsFactory CreateBrainsFactory(DIContainer c)
		{
			return new BrainsFactory(c);
		}

		//Способ создания связи сущности и коллайдера
		private static CollidersRegistryService CreateCollidersRegistryService(DIContainer c)
		{
			return new CollidersRegistryService();
		}

		//Способ создания фабрики прослоек между сущностью и Unity
		private static MonoEntitiesFactory CreateMonoEntitiesFactory(DIContainer c)
		{
			return new MonoEntitiesFactory(
				c.Resolve<ResourcesAssetsLoader>(),
				c.Resolve<EntitiesLifeContext>(),
				c.Resolve<CollidersRegistryService>());
		}

		//Способ создания Сервиса жизненного цикла сущностей
		private static EntitiesLifeContext CreateEntitiesLifeContext(DIContainer c)
		{
			return new EntitiesLifeContext();
		}

		//Способ создания фабрики сущностей
		private static EntitiesFactory CreateEntitiesFactory(DIContainer c)
		{
			return new EntitiesFactory(c);
		}
	}
}
