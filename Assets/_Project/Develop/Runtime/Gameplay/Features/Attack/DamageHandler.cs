using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Attack
{
	public static class DamageHandler
	{
		public static void ApplyDamage(Entity contactEntity, float damage)
		{
			if (contactEntity.HasComponent<TakeDamageRequest>())
				contactEntity.TakeDamageRequest.Invoke(damage);
		}
	}
}
