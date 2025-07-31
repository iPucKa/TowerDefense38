﻿using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Entities
{
	[CreateAssetMenu(menuName = "Configs/Gameplay/Entities/NewAgroEnemyConfig", fileName = "AgroEnemyConfig")]
	public class AgroEnemyConfig : EntityConfig
	{
		[field: SerializeField] public string PrefabPath { get; private set; } = "Entities/Ghost";
		[field: SerializeField, Min(0)] public float MoveSpeed { get; private set; } = 9;
		[field: SerializeField, Min(0)] public float RotationSpeed { get; private set; } = 900;
		[field: SerializeField, Min(0)] public float MaxHealth { get; private set; } = 100;
		[field: SerializeField, Min(0)] public float ExplosionDamage { get; private set; } = 50;
		[field: SerializeField, Min(0)] public float ExplosionRadius { get; private set; } = 2;
		[field: SerializeField, Min(0)] public float DeathProcessTime { get; private set; } = 2;
	}
}
