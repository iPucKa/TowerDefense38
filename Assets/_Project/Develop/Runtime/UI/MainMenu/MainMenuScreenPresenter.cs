using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Gameplay.Infrastructure;
using Assets._Project.Develop.Runtime.Gameplay.Progress;
using Assets._Project.Develop.Runtime.UI.Core;
using Assets._Project.Develop.Runtime.UI.Progress;
using Assets._Project.Develop.Runtime.UI.Wallet;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagement;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagement;
using Assets._Project.Develop.Runtime.Utilities.SceneManagement;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.UI.MainMenu
{
	public class MainMenuScreenPresenter : IPresenter
	{
		private readonly MainMenuScreenView _screen;
		private readonly ProjectPresentersFactory _projectPresentersFactory;
		private readonly ProgressRestoreService _progressRestoreService;
		
		private readonly ConfigsProviderService _configsProviderService;
		private readonly SceneSwitcherService _sceneSwitcherService;
		private readonly ICoroutinesPerformer _coroutinesPerformer;

		//private GameplayInputArgs _args;

		private readonly List<IPresenter> _childPresenters = new();

		public MainMenuScreenPresenter(
			MainMenuScreenView screen,
			ProjectPresentersFactory projectPresentersFactory,
			ProgressRestoreService progressRestoreService,
			ConfigsProviderService configsProviderService,
			SceneSwitcherService sceneSwitcherService,
			ICoroutinesPerformer coroutinesPerformer)
		{
			_screen = screen;
			_projectPresentersFactory = projectPresentersFactory;
			_progressRestoreService = progressRestoreService;

			_configsProviderService = configsProviderService;
			_sceneSwitcherService = sceneSwitcherService;
			_coroutinesPerformer = coroutinesPerformer;
		}

		public void Initialize()
		{
			_screen.ResetProgressButtonClicked += OnResetProgressButtonClicked;
			_screen.PlayGameButtonClicked += OnPlayGameButtonClicked;

			CreateWallet();

			CreateProgressBar();

			foreach (IPresenter presenter in _childPresenters)
				presenter.Initialize();
		}

		public void Dispose()
		{
			_screen.ResetProgressButtonClicked -= OnResetProgressButtonClicked;
			_screen.PlayGameButtonClicked -= OnPlayGameButtonClicked;

			foreach (IPresenter presenter in _childPresenters)
				presenter.Dispose();

			_childPresenters.Clear();
		}

		private void CreateWallet()
		{
			WalletPresenter walletPresenter = _projectPresentersFactory.CreateWalletPresenter(_screen.WalletView);

			_childPresenters.Add(walletPresenter);
		}

		private void CreateProgressBar()
		{
			ProgressBarPresenter progressPresenter = _projectPresentersFactory.CreateProgressBarPresenter(_screen.ProgressView);

			_childPresenters.Add(progressPresenter);
		}

		private void OnResetProgressButtonClicked() => _progressRestoreService.SetInitialValues();

		private void OnPlayGameButtonClicked() 
		{
			LevelsListConfig levelsListConfig = _configsProviderService.GetConfig<LevelsListConfig>();

			int levelIndex = Random.Range(1, levelsListConfig.Levels.Count);

			_coroutinesPerformer.StartPerform(_sceneSwitcherService.ProcessSwitchTo(Scenes.Gameplay, new GameplayInputArgs(levelIndex)));
		}		
	}
}
