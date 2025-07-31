using Assets._Project.Develop.Runtime.Configs.Gameplay.Stages;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.Enemies;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using System;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.StageFeature
{
	public class StagesFactory
	{
		private readonly DIContainer _container;

		public StagesFactory(DIContainer container)
		{
			_container = container;
		}

		public IStage Create(StageConfig stageConfig)
		{
			switch (stageConfig)
			{
				case ClearAllEnemyStageConfig clearAllEnemyStageConfig:
					return new ClearAllEnemiesStage(
						clearAllEnemyStageConfig,
						_container.Resolve<EnemiesFactory>(),
						_container.Resolve<EntitiesLifeContext>());

				default:
					throw new ArgumentException($"Not supported {stageConfig.GetType()} type config");
			}
		}
	}
}
