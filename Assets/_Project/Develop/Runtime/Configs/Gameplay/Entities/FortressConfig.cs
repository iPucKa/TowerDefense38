using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities
{
	[CreateAssetMenu(menuName = "Configs/Gameplay/Entities/NewFortressConfig", fileName = "FortressConfig")]
	public class FortressConfig : EntityConfig
	{
		[field: SerializeField] public string PrefabPath { get; private set; } = "Entities/Fortress";
		//[field: SerializeField, Min(0)] public float MaxHealth { get; private set; } = 100;
		[field: SerializeField, Min(0)] public float ExplosionDamage { get; private set; } = 50;
		[field: SerializeField, Min(0)] public float ExplosionRadius { get; private set; } = 2;
		[field: SerializeField, Min(0)] public float AttackProcessTime { get; private set; } = 1.5f;
		[field: SerializeField, Min(0)] public float AttackDelayTime { get; private set; } = 0.75f;
		[field: SerializeField, Min(0)] public float AttackCooldown { get; private set; } = 1f;
		[field: SerializeField, Min(0)] public float DeathProcessTime { get; private set; } = 2;
	}
}
