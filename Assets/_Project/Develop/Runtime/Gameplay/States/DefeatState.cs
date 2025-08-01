using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Progress;
using Assets._Project.Develop.Runtime.Meta.Features.GameProgress;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagement;
using Assets._Project.Develop.Runtime.Utilities.DataManagement.DataProviders;
using Assets._Project.Develop.Runtime.Utilities.SceneManagement;
using Assets._Project.Develop.Runtime.Utilities.StateMachineCore;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.States
{
	public class DefeatState : EndGameState, IUpdatableState
	{
		public event Action Defeat;

		private readonly ICoroutinesPerformer _coroutinesPerformer;
		private readonly PlayerDataProvider _playerDataProvider;
		private readonly SceneSwitcherService _sceneSwitcherService;
		private readonly ProgressService _progressService;

		public DefeatState(
			IInputService inputService,
			SceneSwitcherService sceneSwitcherService,
			ICoroutinesPerformer coroutinesPerformer,
			PlayerDataProvider playerDataProvider,
			ProgressService progressService) : base(inputService)
		{
			_sceneSwitcherService = sceneSwitcherService;
			_coroutinesPerformer = coroutinesPerformer;
			_progressService = progressService;
			_playerDataProvider = playerDataProvider;
		}

		public override void Enter()
		{
			base.Enter();

			Debug.Log("ПОРАЖЕНИЕ!");
			
			Defeat?.Invoke();

			_progressService.Increase(GameProgressTypes.Defeat);
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
