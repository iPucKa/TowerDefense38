using Assets._Project.Develop.Runtime.Gameplay.Features.Attack.Explosion;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Gameplay.Features.StageFeature;
using Assets._Project.Develop.Runtime.Gameplay.Infrastructure;
using Assets._Project.Develop.Runtime.Gameplay.Progress;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Meta.Features.Wallet;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagement;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagement;
using Assets._Project.Develop.Runtime.Utilities.DataManagement.DataProviders;
using Assets._Project.Develop.Runtime.Utilities.SceneManagement;

namespace Assets._Project.Develop.Runtime.Gameplay.States
{
	public class GameplayStatesFactory
	{
		private readonly DIContainer _container;

		public GameplayStatesFactory(DIContainer container)
		{
			_container = container;
		}

		public PreparationState CreatePreparationState()
		{
			return new PreparationState(_container.Resolve<PreparationTriggerService>());
		}

		public PauseForMineSetupState CreatePauseForMineSetupState()
		{
			return new PauseForMineSetupState(_container.Resolve<MineSetupService>());
		}

		public StageProcessState CreateStageProcessState()
		{
			return new StageProcessState(
				_container.Resolve<StageProviderService>(),
				_container.Resolve<ExplosionService>());
		}

		public WinState CreateWinState(GameplayInputArgs inputArgs)
		{
			return new WinState(
				_container.Resolve<IInputService>(),
				inputArgs,
				_container.Resolve<PlayerDataProvider>(),
				_container.Resolve<SceneSwitcherService>(),
				_container.Resolve<ICoroutinesPerformer>(),
				_container.Resolve<WalletService>(),
				_container.Resolve<ProgressService>(),
				_container.Resolve<ConfigsProviderService>());
		}

		public DefeatState CreateDefeatState()
		{
			return new DefeatState(
				_container.Resolve<IInputService>(),
				_container.Resolve<SceneSwitcherService>(),
				_container.Resolve<ICoroutinesPerformer>(),
				_container.Resolve<PlayerDataProvider>(),
				_container.Resolve<ProgressService>());
		}		

		public GameplayStateMachine CreateGameplayStateMachine(GameplayInputArgs inputArgs)
		{			
			StageProviderService stageProviderService = _container.Resolve<StageProviderService>();
			MainHeroHolderService mainHeroHolderService = _container.Resolve<MainHeroHolderService>();

			GameplayStateMachine coreLoopState = CreateCoreLoopState();

			DefeatState defeatState = CreateDefeatState();
			WinState winState = CreateWinState(inputArgs);

			ICompositCondition coreLoopToWinStateCondition = new CompositCondition()
				.Add(new FuncCondition(() => stageProviderService.CurrentStageResult.Value == StageResults.Completed))
				.Add(new FuncCondition(() => stageProviderService.HasNextStage() == false))
				.Add(new FuncCondition(() =>
				{
					if (mainHeroHolderService.MainHero != null)
						if (mainHeroHolderService.MainHero.IsDead.Value == false)
							return true;

					return false;
				}));

			ICompositCondition coreLoopToDefeatStateCondition = new CompositCondition()
				.Add(new FuncCondition(() =>
				{
					if (mainHeroHolderService.MainHero != null)
						return mainHeroHolderService.MainHero.IsDead.Value;

					return false;
				}));

			GameplayStateMachine gameplayCycle = new GameplayStateMachine();

			gameplayCycle.AddState(coreLoopState);
			gameplayCycle.AddState(winState);
			gameplayCycle.AddState(defeatState);

			gameplayCycle.AddTransition(coreLoopState, winState, coreLoopToWinStateCondition);
			gameplayCycle.AddTransition(coreLoopState, defeatState, coreLoopToDefeatStateCondition);

			return gameplayCycle;
		}		

		public GameplayStateMachine CreateCoreLoopState()
		{
			ExplosionService explosionService = _container.Resolve<ExplosionService>();
			MineSetupService mineSetupOnPauseService = _container.Resolve<MineSetupService>();

			StageProviderService stageProviderService = _container.Resolve<StageProviderService>();

			PauseForMineSetupState pauseForMineSetupState = CreatePauseForMineSetupState();
			StageProcessState stageProcessState = CreateStageProcessState();

			ICompositCondition fromPauseToStageProcessCondition = new CompositCondition()
				.Add(new FuncCondition(() => mineSetupOnPauseService.IsMineSetuped))
				.Add(new FuncCondition(() => stageProviderService.HasNextStage()));

			ICompositCondition fromProcessToPauseCondition = new CompositCondition()
				.Add(new FuncCondition(() => stageProviderService.CurrentStageResult.Value == StageResults.Completed))
				.Add(new FuncCondition(() => stageProviderService.HasNextStage()));

			GameplayStateMachine coreLoopState = new GameplayStateMachine();

			//coreLoopState.AddState(preparationState);
			coreLoopState.AddState(pauseForMineSetupState);
			coreLoopState.AddState(stageProcessState);

			coreLoopState.AddTransition(pauseForMineSetupState, stageProcessState, fromPauseToStageProcessCondition);
			coreLoopState.AddTransition(stageProcessState, pauseForMineSetupState, fromProcessToPauseCondition);

			return coreLoopState;
		}
	}
}
