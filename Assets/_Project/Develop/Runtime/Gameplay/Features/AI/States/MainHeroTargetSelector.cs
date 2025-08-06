using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI.States
{
	public class MainHeroTargetSelector : ITargetSelector
	{
		private Entity _source;
		//private Transform _sourceTransform;

		public MainHeroTargetSelector(Entity entity)
		{
			_source = entity;
			//_sourceTransform = entity.Transform;
		}

		public Entity SelectTargetFrom(IEnumerable<Entity> targets)
		{
			IEnumerable<Entity> selectedTargets = SelectorHelper.InitialFilteredTargetsFrom(targets, _source);

			if (selectedTargets == null)
				return null;

			Entity mainHero = selectedTargets.First();

			foreach (Entity target in selectedTargets)
			{
				if (target.HasComponent<IsMainHero>())				
					mainHero = target;				
			}

			return mainHero;
		}
	}
}
