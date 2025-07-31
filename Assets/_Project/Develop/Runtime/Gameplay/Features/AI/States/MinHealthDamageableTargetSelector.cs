using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI.States
{
	public class MinHealthDamageableTargetSelector : ITargetSelector
	{
		private Entity _source;

		public MinHealthDamageableTargetSelector(Entity entity)
		{
			_source = entity;
		}

		public Entity SelectTargetFrom(IEnumerable<Entity> targets)
		{
			IEnumerable<Entity> selectedTargets = SelectorHelper.InitialFilteredTargetsFrom(targets, _source);

			Entity minHealthTarget = selectedTargets.First();
			float minHealth = GetHealth(minHealthTarget);

			foreach (Entity target in selectedTargets)
			{
				float currentHealth = GetHealth(target);

				if (currentHealth < minHealth)
				{
					minHealth = currentHealth;
					minHealthTarget = target;
				}
			}

			return minHealthTarget;
		}

		private float GetHealth(Entity target) => target.CurrentHealth.Value;
	}
}
