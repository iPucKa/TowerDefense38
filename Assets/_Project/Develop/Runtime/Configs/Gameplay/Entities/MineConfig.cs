using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities
{
	[CreateAssetMenu(menuName = "Configs/Gameplay/Entities/NewMineConfig", fileName = "MineConfig")]
	public class MineConfig : EntityConfig
	{
		[field: SerializeField] public string PrefabPath { get; private set; } = "Entities/Mine";
		[field: SerializeField, Min(0)] public float DamageRadius { get; private set; } = 5;
		[field: SerializeField, Min(0)] public float AreaContactDamage { get; private set; } = 50;
		[field: SerializeField, Min(0)] public float AttackDelayTime { get; private set; } = 0.05f;
		[field: SerializeField, Min(0)] public float DeathProcessTime { get; private set; } = 0.05f;
	}
}
