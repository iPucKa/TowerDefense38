using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Gameplay.Features.StageFeature;
using Assets._Project.Develop.Runtime.Gameplay.Infrastructure;
using Assets._Project.Develop.Runtime.Gameplay.Progress;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Meta.Features.LevelsProgression;
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
			return new PauseForMineSetupState(_container.Resolve<MineSetupOnPauseService>());
		}

		public StageProcessState CreateStageProcessState()
		{
			return new StageProcessState(_container.Resolve<StageProviderService>());
		}

		public WinState CreateWinState(GameplayInputArgs inputArgs)
		{
			return new WinState(
				_container.Resolve<IInputService>(),
				_container.Resolve<LevelsProgressionService>(),
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

		//public GameplayStateMachine CreateGameplayStateMachine(GameplayInputArgs inputArgs)
		//{
		//	PreparationTriggerService preparationTriggerService = _container.Resolve<PreparationTriggerService>();
		//	StageProviderService stageProviderService = _container.Resolve<StageProviderService>();
		//	MainHeroHolderService mainHeroHolderService = _container.Resolve<MainHeroHolderService>();

		//	GameplayStateMachine coreLoopState = CreateCoreLoopState();

		//	DefeatState defeatState = CreateDefeatState();
		//	WinState winState = CreateWinState(inputArgs);

		//	ICompositCondition coreLoopToWinStateCondition = new CompositCondition()
		//		.Add(new FuncCondition(() => preparationTriggerService.HasMainHeroContact.Value))
		//		.Add(new FuncCondition(() => stageProviderService.CurrentStageResult.Value == StageResults.Completed))
		//		.Add(new FuncCondition(() => stageProviderService.HasNextStage() == false));

		//	ICompositCondition coreLoopToDefeatStateCondition = new CompositCondition()
		//		.Add(new FuncCondition(() =>
		//		{
		//			if (mainHeroHolderService.MainHero != null)
		//				return mainHeroHolderService.MainHero.IsDead.Value;

		//			return false;
		//		}));

		//	GameplayStateMachine gameplayCycle = new GameplayStateMachine();

		//	gameplayCycle.AddState(coreLoopState);
		//	gameplayCycle.AddState(winState);
		//	gameplayCycle.AddState(defeatState);

		//	gameplayCycle.AddTransition(coreLoopState, winState, coreLoopToWinStateCondition);
		//	gameplayCycle.AddTransition(coreLoopState, defeatState, coreLoopToDefeatStateCondition);

		//	return gameplayCycle;
		//}

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

		//public GameplayStateMachine CreateCoreLoopState()
		//{
		//	PreparationTriggerService preparationTriggerService = _container.Resolve<PreparationTriggerService>();
		//	StageProviderService stageProviderService = _container.Resolve<StageProviderService>();

		//	PreparationState preparationState = CreatePreparationState();
		//	StageProcessState stageProcessState = CreateStageProcessState();

		//	ICompositCondition preparationToStageProcessCondition = new CompositCondition()
		//		.Add(new FuncCondition(() => preparationTriggerService.HasMainHeroContact.Value))
		//		.Add(new FuncCondition(() => stageProviderService.HasNextStage()));

		//	FuncCondition stageProcessToPreparationCondition = new FuncCondition(() => stageProviderService.CurrentStageResult.Value == StageResults.Completed);

		//	GameplayStateMachine coreLoopState = new GameplayStateMachine();

		//	coreLoopState.AddState(preparationState);
		//	coreLoopState.AddState(stageProcessState);

		//	coreLoopState.AddTransition(preparationState, stageProcessState, preparationToStageProcessCondition);
		//	coreLoopState.AddTransition(stageProcessState, preparationState, stageProcessToPreparationCondition);

		//	return coreLoopState;
		//}

		public GameplayStateMachine CreateCoreLoopState()
		{
			MineSetupOnPauseService mineSetupOnPauseService = _container.Resolve<MineSetupOnPauseService>();
			StageProviderService stageProviderService = _container.Resolve<StageProviderService>();

			PauseForMineSetupState pauseForMineSetupState = CreatePauseForMineSetupState();
			StageProcessState stageProcessState = CreateStageProcessState();

			ICompositCondition pauseToStageProcessCondition = new CompositCondition()
				.Add(new FuncCondition(() => mineSetupOnPauseService.IsMineSetuped))
				.Add(new FuncCondition(() => stageProviderService.HasNextStage()));

			ICompositCondition stageProcessToPauseCondition = new CompositCondition()
				.Add(new FuncCondition(() => stageProviderService.CurrentStageResult.Value == StageResults.Completed))
				.Add(new FuncCondition(() => stageProviderService.HasNextStage()));

			GameplayStateMachine coreLoopState = new GameplayStateMachine();

			//coreLoopState.AddState(preparationState);
			coreLoopState.AddState(pauseForMineSetupState);
			coreLoopState.AddState(stageProcessState);

			coreLoopState.AddTransition(pauseForMineSetupState, stageProcessState, pauseToStageProcessCondition);
			coreLoopState.AddTransition(stageProcessState, pauseForMineSetupState, stageProcessToPauseCondition);

			return coreLoopState;
		}
	}
}
