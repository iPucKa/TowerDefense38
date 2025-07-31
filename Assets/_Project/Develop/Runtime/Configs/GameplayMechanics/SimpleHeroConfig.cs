using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.GameplayMechanics
{
	[CreateAssetMenu(menuName = "Configs/GameplayMechanics/NewSimpleHeroConfig", fileName = "SimpleHeroConfig")]
	public class SimpleHeroConfig : ScriptableObject
	{
		[field: SerializeField] public float MaxHealth { get; private set; }
		[field: SerializeField] public float MoveSpeed { get; private set; }
		[field: SerializeField] public float RotationSpeed { get; private set; }
		[field: SerializeField] public float Damage { get; private set; }
		[field: SerializeField] public float AttackProcessInitialTime { get; private set; }
		[field: SerializeField] public float AttackDelayTime { get; private set; }
		[field: SerializeField] public float AttackCooldown { get; private set; }
		[field: SerializeField] public float DeathProcessInitialTime { get; private set; }
		[field: SerializeField] public LayerMask LayerMask { get; private set; }
	}
}
