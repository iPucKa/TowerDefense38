using Assets._Project.Develop.Runtime.Configs.Gameplay.Entities;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI;
using Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagement;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.MainHero
{
	public class MainHeroFactory
	{
		private readonly DIContainer _container;
		private readonly EntitiesFactory _entitiesFactory;
		private readonly BrainsFactory _brainsFactory;
		private readonly ConfigsProviderService _configProviderService;
		private readonly EntitiesLifeContext _entitiesLifeContext;

		public MainHeroFactory(DIContainer container)
		{
			_container = container;
			_entitiesFactory = _container.Resolve<EntitiesFactory>();
			_brainsFactory = _container.Resolve<BrainsFactory>();
			_configProviderService = _container.Resolve<ConfigsProviderService>();
			_entitiesLifeContext = _container.Resolve<EntitiesLifeContext>();
		}

		public Entity Create(Vector3 position, float maxHelth)
		{
			FortressConfig fortressConfig = _configProviderService.GetConfig<FortressConfig>();

			Entity entity = _entitiesFactory.CreateFortress(position, fortressConfig, maxHelth);

			entity
				.AddIsMainHero()
				.AddTeam(new ReactiveVariable<Teams>(Teams.MainHero));

			_brainsFactory.CreateFortressBrain(entity);

			_entitiesLifeContext.Add(entity);

			return entity;
		}
	}
}
