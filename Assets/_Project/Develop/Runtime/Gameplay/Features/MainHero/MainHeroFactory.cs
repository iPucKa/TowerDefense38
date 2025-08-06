using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI;
using Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MainHero
{
	public class MainHeroFactory
	{
		private readonly DIContainer _container;
		private readonly EntitiesFactory _entitiesFactory;
		private readonly BrainsFactory _brainsFactory;
		private readonly EntitiesLifeContext _entitiesLifeContext;

		public MainHeroFactory(DIContainer container)
		{
			_container = container;
			_entitiesFactory = _container.Resolve<EntitiesFactory>();
			_brainsFactory = _container.Resolve<BrainsFactory>();
			_entitiesLifeContext = _container.Resolve<EntitiesLifeContext>();
		}

		public Entity Create(EntityConfig config, Vector3 position, float maxHelth)
		{
			Entity entity;

			switch (config)
			{
				case FortressConfig fortressConfig:
					entity = _entitiesFactory.CreateFortress(position, fortressConfig, maxHelth);
					
					entity
						.AddIsMainHero()
						.AddTeam(new ReactiveVariable<Teams>(Teams.MainHero));

					_brainsFactory.CreateFortressBrain(entity);
					break;				

				default:
					throw new ArgumentException($"Not support {config.GetType()} type config");
			}

			_entitiesLifeContext.Add(entity);

			return entity;
		}
	}
}
