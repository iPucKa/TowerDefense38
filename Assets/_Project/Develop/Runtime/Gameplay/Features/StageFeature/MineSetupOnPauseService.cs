using Assets._Project.Develop.Runtime.Configs.Gameplay;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Meta.Features.Wallet;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagement;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagement;
using Assets._Project.Develop.Runtime.Utilities.DataManagement.DataProviders;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.StageFeature
{
	public class MineSetupOnPauseService
	{
		private readonly EntitiesFactory _entitiesFactory;

		private readonly IInputService _inputService;
		private readonly MouseTrackService _mouseTrackService;
		private readonly ICoroutinesPerformer _coroutinesPerformer;
		private readonly PlayerDataProvider _playerDataProvider;
		private readonly ConfigsProviderService _configProviderService;
		private readonly WalletService _walletService;

		private readonly int _valueForMineSetup;

		private ReactiveVariable<bool> _isAttackKeyPressed = new();		
		private List<Entity> _mines = new();

		public MineSetupOnPauseService(
			EntitiesFactory entitiesFactory,
			IInputService inputService,
			MouseTrackService mouseTrackService,
			PlayerDataProvider playerDataProvider,
			ICoroutinesPerformer coroutinesPerformer,
			ConfigsProviderService configProviderService,
			WalletService walletService)
		{
			_entitiesFactory = entitiesFactory;
			_inputService = inputService;
			_mouseTrackService = mouseTrackService;
			_playerDataProvider = playerDataProvider;
			_coroutinesPerformer = coroutinesPerformer;
			_configProviderService = configProviderService;
			_walletService = walletService;

			_valueForMineSetup = _configProviderService.GetConfig<GameplayConfig>().MineCostSetupValue;
		}

		public bool IsMineSetuped => _walletService.Enough(CurrencyTypes.Gold, _valueForMineSetup) == false;

		public void Update(float deltaTime)
		{
			if (_isAttackKeyPressed.Value == false)
				_isAttackKeyPressed.Value = _inputService.IsAttackButtonPressed;			

			if (_isAttackKeyPressed.Value == true)
			{
				if (_walletService.Enough(CurrencyTypes.Gold, _valueForMineSetup))
				{
					Entity mine = _entitiesFactory.CreateMine(_mouseTrackService.Position);
					_mines.Add(mine);

					_walletService.Spend(CurrencyTypes.Gold, _valueForMineSetup);
					_coroutinesPerformer.StartPerform(_playerDataProvider.SaveAsync());

					_isAttackKeyPressed.Value = false;
				}
				else
				{
					Debug.Log("Нужно больше золота!");
				}
			}			
		}

		public void Cleanup()
		{
			_isAttackKeyPressed.Value = false;
			_mines.Clear();
		}
	}
}
