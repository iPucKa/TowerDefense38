using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI.States
{
	public class NearestDamageableTargetSelector : ITargetSelector
	{
		private Entity _source;
		private Transform _sourceTransform;

		public NearestDamageableTargetSelector(Entity entity)
		{
			_source = entity;
			_sourceTransform = entity.Transform;
		}

		public Entity SelectTargetFrom(IEnumerable<Entity> targets)
		{
			IEnumerable<Entity> selectedTargets = SelectorHelper.InitialFilteredTargetsFrom(targets, _source);

			if (selectedTargets.Any() == false)
				return null;

			Entity closestTarget = selectedTargets.First();
			float minDistance = GetDistanceTo(closestTarget);

			foreach (Entity target in selectedTargets)
			{
				float distance = GetDistanceTo(target);

				if (distance < minDistance)
				{
					minDistance = distance;
					closestTarget = target;
				}
			}

			return closestTarget;
		}

		private float GetDistanceTo(Entity target) => (_sourceTransform.position - target.Transform.position).magnitude;
	}
}
