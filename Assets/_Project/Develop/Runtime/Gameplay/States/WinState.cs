using Assets._Project.Develop.Runtime.Configs.Gameplay;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Infrastructure;
using Assets._Project.Develop.Runtime.Gameplay.Progress;
using Assets._Project.Develop.Runtime.Meta.Features.GameProgress;
using Assets._Project.Develop.Runtime.Meta.Features.LevelsProgression;
using Assets._Project.Develop.Runtime.Meta.Features.Wallet;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagement;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagement;
using Assets._Project.Develop.Runtime.Utilities.DataManagement.DataProviders;
using Assets._Project.Develop.Runtime.Utilities.SceneManagement;
using Assets._Project.Develop.Runtime.Utilities.StateMachineCore;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.States
{
	public class WinState : EndGameState, IUpdatableState
	{
		private readonly PlayerDataProvider _playerDataProvider;
		private readonly SceneSwitcherService _sceneSwitcherService;
		private readonly ICoroutinesPerformer _coroutinesPerformer;
		private readonly WalletService _walletService;
		private readonly ProgressService _progressService;
		private readonly ConfigsProviderService _configProviderService;

		public WinState(
			IInputService inputService,
			LevelsProgressionService levelsProgressionService,
			GameplayInputArgs gameplayInputArgs,
			PlayerDataProvider playerDataProvider,
			SceneSwitcherService sceneSwitcherService,
			ICoroutinesPerformer coroutinesPerformer,
			WalletService walletService,
			ProgressService progressService,
			ConfigsProviderService configProviderService) : base(inputService)
		{
			_playerDataProvider = playerDataProvider;
			_sceneSwitcherService = sceneSwitcherService;
			_coroutinesPerformer = coroutinesPerformer;
			_walletService = walletService;
			_progressService = progressService;
			_configProviderService = configProviderService;
		}

		public override void Enter()
		{
			base.Enter();

			Debug.Log("ПОБЕДА!");

			int winValue = _configProviderService.GetConfig<GameplayConfig>().WinValue;
			_walletService.Add(CurrencyTypes.Gold, winValue);
			
			_progressService.Increase(GameProgressTypes.Win);
			//_levelsProgressionService.AddLevelToCompleted(_gameplayInputArgs.LevelNumber);
			_coroutinesPerformer.StartPerform(_playerDataProvider.SaveAsync());
		}

		public void Update(float deltaTime)
		{
			if (Input.GetKeyDown(KeyCode.Q))
			{
				_coroutinesPerformer.StartPerform(_sceneSwitcherService.ProcessSwitchTo(Scenes.MainMenu));
			}
		}
	}
}
