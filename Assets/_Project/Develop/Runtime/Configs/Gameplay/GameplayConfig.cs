using UnityEngine;

namespace Assets._Project.Develop.Runtime.Configs.Gameplay
{
	[CreateAssetMenu(menuName = "Configs/Gameplay/GameplayConfig", fileName = "GameplayConfig")]
	public class GameplayConfig : ScriptableObject
	{
		[field: SerializeField, Min(0)] public int ValueToResetProgress { get; private set; } = 50;
		[field: SerializeField, Min(0)] public int WinValue { get; private set; } = 10;
		[field: SerializeField, Min(0)] public int DefeatValue { get; private set; } = 15;
		[field: SerializeField, Min(0)] public int MineCostSetupValue { get; private set; } = 5;

		//public int GetWinValue => WinValue;

		//public int GetDefeatValue => DefeatValue;
	}
}
