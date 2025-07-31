using Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore
{
	public class EntitiesHelper
	{
		public static bool TryTakeDamageFrom(Entity source, Entity damagable, float damage)
		{
			if (damagable.TryGetTakeDamageRequest(out ReactiveEvent<float> takeDamageRequest) == false)
				return false;

			if (source.TryGetTeam(out ReactiveVariable<Teams> sourceTeam)
				&& damagable.TryGetTeam(out ReactiveVariable<Teams> damagableTeam))
			{
				if (sourceTeam.Value == damagableTeam.Value)
					return false;
			}

			takeDamageRequest.Invoke(damage);
			return true;
		}
	}
}
