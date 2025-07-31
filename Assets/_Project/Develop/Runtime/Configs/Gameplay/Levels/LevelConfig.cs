using Assets._Project.Develop.Runtime.Configs.Gameplay.Stages;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Levels
{
	[CreateAssetMenu(menuName = "Configs/Gameplay/Levels/NewLevelConfig", fileName = "LevelConfig")]
	public class LevelConfig : ScriptableObject
	{
		//добавить позже настройки для уровня
		[SerializeField] private List<StageConfig> _stageConfigs;
		[field: SerializeField, Min(0)] public float FortressHealth { get; private set; } = 100;

		public IReadOnlyList<StageConfig> StageConfigs => _stageConfigs;
	}
}
