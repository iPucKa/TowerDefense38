﻿using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay.Levels
{
	[CreateAssetMenu(menuName = "Configs/Gameplay/Levels/NewLevelsListConfig", fileName = "LevelsListConfig")]
	public class LevelsListConfig : ScriptableObject
	{
		[SerializeField] private List<LevelConfig> _levels = new();

		public IReadOnlyList<LevelConfig> Levels => _levels;

		public LevelConfig GetBy(int levelNumber)
		{
			int levelIndex = levelNumber - 1;

			return _levels[levelIndex];
		}
	}
}
