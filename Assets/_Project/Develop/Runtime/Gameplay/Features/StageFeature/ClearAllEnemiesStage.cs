using Assets._Project.Develop.Runtime.Configs.Gameplay.Stages;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.Enemies;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.StageFeature
{
	public class ClearAllEnemiesStage : IStage
	{
		private readonly ClearAllEnemyStageConfig _config;
		private readonly EnemiesFactory _enemiesFactory;
		private readonly EntitiesLifeContext _entitiesLifeContext;
		private ReactiveEvent _completed = new();

		private Dictionary<Entity, IDisposable> _spawnedEnemiesToRemoveReason = new();

		private bool _inProcess;

		public ClearAllEnemiesStage(
			ClearAllEnemyStageConfig config,
			EnemiesFactory enemiesFactory,
			EntitiesLifeContext entitiesLifeContext)
		{
			_config = config;
			_enemiesFactory = enemiesFactory;
			_entitiesLifeContext = entitiesLifeContext;
		}

		public IReadOnlyEvent Completed => _completed;

		public void Cleanup()
		{
			foreach (KeyValuePair<Entity, IDisposable> item in _spawnedEnemiesToRemoveReason)
			{
				item.Value.Dispose();
				_entitiesLifeContext.Release(item.Key);
			}

			_spawnedEnemiesToRemoveReason.Clear();

			_inProcess = false;
		}

		public void Dispose()
		{
			foreach (KeyValuePair<Entity, IDisposable> item in _spawnedEnemiesToRemoveReason)
			{
				item.Value.Dispose();
			}

			_spawnedEnemiesToRemoveReason.Clear();

			_inProcess = false;
		}

		public void Start()
		{
			if (_inProcess)
				throw new InvalidOperationException("Game mode already started");

			SpawnEnemies();

			_inProcess = true;
		}

		public void Update(float deltaTime)
		{
			if (_inProcess == false)
				return;

			if (_spawnedEnemiesToRemoveReason.Count == 0)
				ProcessEnd();
		}

		private void ProcessEnd()
		{
			_inProcess = false;
			_completed.Invoke();
		}

		private void SpawnEnemies()
		{
			foreach (EnemyItemConfig enemyItemConfig in _config.EnemyItems)
				SpawnEnemy(enemyItemConfig);
		}

		private void SpawnEnemy(EnemyItemConfig enemyItemConfig)
		{
			Vector3 spawnPosition = GetSpawnPointOnCircle(Vector3.zero, enemyItemConfig.SpawnRadius);

			Entity spawnedEnemy = _enemiesFactory.Create(spawnPosition, enemyItemConfig.EnemyConfig);

			IDisposable removeReason = spawnedEnemy.IsDead.Subscribe((oldValue, isDead) =>
			{
				if (isDead)
				{
					IDisposable disposable = _spawnedEnemiesToRemoveReason[spawnedEnemy];
					disposable.Dispose();
					_spawnedEnemiesToRemoveReason.Remove(spawnedEnemy);
				}
			});

			_spawnedEnemiesToRemoveReason.Add(spawnedEnemy, removeReason);
		}

		private Vector3 GetSpawnPointOnCircle(Vector3 center, float radius)
		{
			float angle = Random.Range(0f, Mathf.PI * 2);
			Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
			
			return center + offset;
		}
	}
}
