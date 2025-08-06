﻿using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI.States;
using Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Enemies
{
	public class EnemiesFactory
	{
		private readonly DIContainer _container;
		private readonly EntitiesFactory _entitiesFactory;
		private readonly BrainsFactory _brainsFactory;
		private readonly EntitiesLifeContext _entitiesLifeContext;

		public EnemiesFactory(DIContainer container)
		{
			_container = container;
			_entitiesFactory = _container.Resolve<EntitiesFactory>();
			_brainsFactory = _container.Resolve<BrainsFactory>();
			_entitiesLifeContext = _container.Resolve<EntitiesLifeContext>();
		}

		public Entity Create(Vector3 position, EntityConfig config)
		{
			Entity entity;

			switch (config)
			{
				case GhostConfig ghostConfig:
					entity = _entitiesFactory.CreateGhost(position, ghostConfig);
					_brainsFactory.CreateGhostBrain(entity);
					break;

				case AgroEnemyConfig agroEnemyConfig:
					entity = _entitiesFactory.CreateEnemy(position, agroEnemyConfig);
					_brainsFactory.CreateAgroEnemyBrain(entity, new MainHeroTargetSelector(entity));
					break;				

				default:
					throw new ArgumentException($"Not support {config.GetType()} type config");
			}

			entity				
				.AddTeam(new ReactiveVariable<Teams>(Teams.Enemies));

			_entitiesLifeContext.Add(entity);

			return entity;
		}
	}
}
