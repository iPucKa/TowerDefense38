using Assets._Project.Develop.Runtime.Configs.Gameplay;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities;
using Assets._Project.Develop.Runtime.Configs.Gameplay.Levels;
using Assets._Project.Develop.Runtime.Configs.Meta.Progress;
using Assets._Project.Develop.Runtime.Configs.Meta.Wallet;
using Assets._Project.Develop.Runtime.Utilities.AssetsManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Utilities.ConfigsManagement
{
	public class ResourcesConfigsLoader : IConfigsLoader
	{
		private readonly ResourcesAssetsLoader _resources;

		private readonly Dictionary<Type, string> _configsResourcesPath = new()
		{
			{typeof(GameplayConfig), "Configs/Gameplay/GameplayConfig"},
			{typeof(StartWalletConfig), "Configs/Wallet/StartWalletConfig"},
			{typeof(StartProgressConfig), "Configs/Gameplay/StartProgressConfig"},
			{typeof(CurrencyIconsConfig), "Configs/Wallet/CurrencyIconsConfig"},
			{typeof(ProgressIconsConfig), "Configs/Gameplay/ProgressIconsConfig"},
			//{typeof(GameplayMechanicsConfig), "Configs/GameplayMechanics/GameplayMechanicsConfig"},
			//{typeof(TeleportedEntityConfig), "Configs/GameplayMechanics/TeleportedEntityConfig"},
			//{typeof(SimpleHeroConfig), "Configs/GameplayMechanics/SimpleHeroConfig"},
			{typeof(HeroConfig), "Configs/Gameplay/Entities/HeroConfig"},
			{typeof(FortressConfig), "Configs/Gameplay/Entities/FortressConfig"},
			{typeof(MineConfig), "Configs/Gameplay/Entities/MineConfig"},
			{typeof(LevelsListConfig), "Configs/Gameplay/Levels/LevelsListConfig"},
		};

		public ResourcesConfigsLoader(ResourcesAssetsLoader resources)
		{
			_resources = resources;
		}

		public IEnumerator LoadAsync(Action<Dictionary<Type, object>> onConfigsLoaded)
		{
			Dictionary<Type, object> loadedConfigs = new();

			foreach (KeyValuePair<Type, string> configResourcesPath in _configsResourcesPath)
			{
				ScriptableObject config = _resources.Load<ScriptableObject>(configResourcesPath.Value);
				loadedConfigs.Add(configResourcesPath.Key, config);
				yield return null;
			}

			onConfigsLoaded?.Invoke(loadedConfigs);
		}
	}
}
