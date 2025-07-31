﻿using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Gameplay.States;
using Assets._Project.Develop.Runtime.Infrastructure;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Meta.Features.Wallet;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagement;
using Assets._Project.Develop.Runtime.Utilities.SceneManagement;
using System;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Infrastructure
{
	public class GameplayBootstrap : SceneBootstrap
	{
		private DIContainer _container;
		private GameplayInputArgs _inputArgs;

		private WalletService _walletService;

		private GameplayStatesContext _gameplayStatesContext;
		private EntitiesLifeContext _entitiesLifeContext;
		private AIBrainsContext _brainsContext;

		public override void ProcessRegistrations(DIContainer container, IInputSceneArgs sceneArgs = null)
		{
			_container = container;

			if (sceneArgs is not GameplayInputArgs gameplayInputArgs)
				throw new ArgumentException($"{nameof(sceneArgs)} is not match with {typeof(GameplayInputArgs)}");

			_inputArgs = gameplayInputArgs;
			Camera camera = Camera.main;

			GameplayContextRegistrations.Process(_container, _inputArgs, camera);
		}

		public override IEnumerator Initialize()
		{
			Debug.Log($"Вы попали на уровень {_inputArgs.LevelNumber}");

			Debug.Log("Инициализация геймплейной сцены");

			_walletService = _container.Resolve<WalletService>();

			_entitiesLifeContext = _container.Resolve<EntitiesLifeContext>();
			_brainsContext = _container.Resolve<AIBrainsContext>();

			_gameplayStatesContext = _container.Resolve<GameplayStatesContext>();

			//Пока висит загрузочная шторка создаем героя

			LevelConfig levelConfig = _container.Resolve<ConfigsProviderService>().GetConfig<LevelsListConfig>().GetBy(_inputArgs.LevelNumber);

			_container.Resolve<MainHeroFactory>().Create(Vector3.zero, levelConfig.FortressHealth);

			yield break;
		}

		public override void Run()
		{
			Debug.Log("Старт геймплейной сцены");

			_gameplayStatesContext.Run();
		}

		private void Update()
		{
			_brainsContext?.Update(Time.deltaTime);
			_entitiesLifeContext?.Update(Time.deltaTime);
			_gameplayStatesContext?.Update(Time.deltaTime);
		}
	}
}
