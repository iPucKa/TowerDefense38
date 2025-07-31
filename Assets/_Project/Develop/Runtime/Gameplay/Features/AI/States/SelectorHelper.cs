using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI.States
{
	public class SelectorHelper
	{
		public static IEnumerable<Entity> InitialFilteredTargetsFrom(IEnumerable<Entity> targets, Entity source)
		{
			IEnumerable<Entity> selectedTargets = targets.Where(target =>
			{
				bool result = target.HasComponent<TakeDamageRequest>();
				if (target.TryGetCanApplyDamage(out ICompositCondition canApplyDamage))
				{
					result = result && canApplyDamage.Evaluate();
				}

				if (source.TryGetTeam(out ReactiveVariable<Teams> sourceTeam)
				&& target.TryGetTeam(out ReactiveVariable<Teams> targetTeam))
				{
					result = result && (sourceTeam.Value != targetTeam.Value);
				}

				result = result && (target != source);
				return result;
			});

			if (selectedTargets.Any() == false)
				return null;			

			return selectedTargets;
		}
	}
}
