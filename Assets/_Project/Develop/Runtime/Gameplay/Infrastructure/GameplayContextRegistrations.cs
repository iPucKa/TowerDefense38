using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Mono;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI;
using Assets._Project.Develop.Runtime.Gameplay.Features.Enemies;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Gameplay.Features.StageFeature;
using Assets._Project.Develop.Runtime.Gameplay.States;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Meta.Features.Wallet;
using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.UI.Gameplay;
using Assets._Project.Develop.Runtime.Utilities.AssetsManagement;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagement;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagement;
using Assets._Project.Develop.Runtime.Utilities.DataManagement.DataProviders;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Infrastructure
{
	public class GameplayContextRegistrations
	{
		private static GameplayInputArgs _inputArgs;
		private static Camera _camera;

		public static void Process(DIContainer container, GameplayInputArgs args, Camera camera)
		{
			Debug.Log("Процесс регистрации сервисов на сцене геймплея");
			_inputArgs = args;
			_camera = camera;

			container.RegisterAsSingle(CreateGameplayPresentersFactory);
			container.RegisterAsSingle(CreateGameplayUIRoot).NonLazy();
			container.RegisterAsSingle(CreateGameplayPopupService);

			container.RegisterAsSingle(CreateEntitiesFactory);
			container.RegisterAsSingle(CreateEntitiesLifeContext);
			container.RegisterAsSingle(CreateMonoEntitiesFactory).NonLazy();
			container.RegisterAsSingle(CreateCollidersRegistryService);
			container.RegisterAsSingle(CreateBrainsFactory);
			container.RegisterAsSingle(CreateAIBrainsContext);
			container.RegisterAsSingle<IInputService>(CreateDesktopInput);
			container.RegisterAsSingle(CreateMouseTracktService);

			container.RegisterAsSingle(CreateMainHeroFactory);
			container.RegisterAsSingle(CreateEnemiesFactory);
			container.RegisterAsSingle(CreateStagesFactory);
			container.RegisterAsSingle(CreateStageProviderService);
			container.RegisterAsSingle(CreatePreparationTriggerService);
			container.RegisterAsSingle(CreateMineSetupOnPauseService);
			container.RegisterAsSingle(CreateMainHeroHolderService).NonLazy();
			container.RegisterAsSingle(CreateGameplayStatesFactory);
			container.RegisterAsSingle(CreateGameplayStatesContext);
		}

		//Способ создания контекста геймплея
		private static GameplayStatesContext CreateGameplayStatesContext(DIContainer c)
		{
			return new GameplayStatesContext(c.Resolve<GameplayStatesFactory>().CreateGameplayStateMachine(_inputArgs));
		}

		//Способ создания фаббрики состояний геймплея
		private static GameplayStatesFactory CreateGameplayStatesFactory(DIContainer c)
		{
			return new GameplayStatesFactory(c);
		}

		//Сервис создания сервиса хранения ГГ
		private static MainHeroHolderService CreateMainHeroHolderService(DIContainer c)
		{
			return new MainHeroHolderService(c.Resolve<EntitiesLifeContext>());
		}

		//Способ создания сервиса подготовки триггера
		private static PreparationTriggerService CreatePreparationTriggerService(DIContainer c)
		{
			return new PreparationTriggerService(
				c.Resolve<EntitiesFactory>(),
				c.Resolve<EntitiesLifeContext>());
		}

		//Способ создания сервиса паузы для установки мин по клику мышкой
		private static MineSetupOnPauseService CreateMineSetupOnPauseService(DIContainer c)
		{
			return new MineSetupOnPauseService(
				c.Resolve<EntitiesFactory>(),
				c.Resolve<IInputService>(),
				c.Resolve<MouseTrackService>(),
				c.Resolve<PlayerDataProvider>(),
				c.Resolve<ICoroutinesPerformer>(),
				c.Resolve<ConfigsProviderService>(),
				c.Resolve<WalletService>());
		}

		//Способ создания сервиса работы с волнами
		private static StageProviderService CreateStageProviderService(DIContainer c)
		{
			return new StageProviderService(
				c.Resolve<ConfigsProviderService>().GetConfig<LevelsListConfig>().GetBy(_inputArgs.LevelNumber),
				c.Resolve<StagesFactory>());
		}

		//Способ создания фабрики волны
		private static StagesFactory CreateStagesFactory(DIContainer c)
		{
			return new StagesFactory(c);
		}

		//Способ создания фабрики героя
		private static MainHeroFactory CreateMainHeroFactory(DIContainer c)
		{
			return new MainHeroFactory(c);
		}

		//Способ создания фабрики врагов
		private static EnemiesFactory CreateEnemiesFactory(DIContainer c)
		{
			return new EnemiesFactory(c);
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

		//Способ создания сервиса попапов геймплея
		private static GameplayPopupService CreateGameplayPopupService(DIContainer c)
		{
			return new GameplayPopupService(
				c.Resolve<ViewsFactory>(),
				c.Resolve<GameplayPresentersFactory>(),
				c.Resolve<GameplayUIRoot>());
		}		

		//Способ создания холста для геймплея
		private static GameplayUIRoot CreateGameplayUIRoot(DIContainer c)
		{
			ResourcesAssetsLoader resourcesAssetsLoader = c.Resolve<ResourcesAssetsLoader>();

			GameplayUIRoot gameplayUIRootPrefab = resourcesAssetsLoader
				.Load<GameplayUIRoot>("UI/Gameplay/GameplayUIRoot");

			return Object.Instantiate(gameplayUIRootPrefab);
		}

		//Способ создания фабрики всех презентеров геймплея
		private static GameplayPresentersFactory CreateGameplayPresentersFactory(DIContainer c)
			=> new GameplayPresentersFactory(c);		
	}
}
