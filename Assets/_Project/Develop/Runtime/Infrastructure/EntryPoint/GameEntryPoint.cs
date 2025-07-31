using Assets._Project.Develop.Runtime.Gameplay.Infrastructure;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagement;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagement;
using Assets._Project.Develop.Runtime.Utilities.DataManagement.DataProviders;
using Assets._Project.Develop.Runtime.Utilities.LoadingScreen;
using Assets._Project.Develop.Runtime.Utilities.SceneManagement;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Infrastructure.EntryPoint
{
	public class GameEntryPoint : MonoBehaviour
	{
		private void Awake()
		{
			Debug.Log("����� �������, ����� ��������");
			SetupAppSettings();

			Debug.Log("������� ����������� �������� ����� �������");

			DIContainer projectContainer = new DIContainer();
			ProjectContextRegistrations.Process(projectContainer);

			projectContainer.Initialize();
			projectContainer.Resolve<ICoroutinesPerformer>().StartPerform(Initialize(projectContainer));
		}

		private void SetupAppSettings()
		{
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = 60;
		}

		private IEnumerator Initialize(DIContainer container)
		{
			//Debug.Log("����������� ����� ��������");
			ILoadingScreen loadingScreen = container.Resolve<ILoadingScreen>();
			SceneSwitcherService sceneSwitcherService = container.Resolve<SceneSwitcherService>();
			PlayerDataProvider playerDataProvider = container.Resolve<PlayerDataProvider>();

			loadingScreen.Show();

			Debug.Log("���������� ������������� ��������");

			yield return container.Resolve<ConfigsProviderService>().LoadAsync();

			// ���������� ���������� ��� ��������� ��������
			bool isPlayerDataSaveExists = false;

			yield return playerDataProvider.ExistsAsync(result => isPlayerDataSaveExists = result);

			if (isPlayerDataSaveExists)
				yield return playerDataProvider.LoadAsync();
			else
				playerDataProvider.Reset();

			yield return new WaitForSeconds(1f);

			Debug.Log("����������� ������������� ��������");

			//Debug.Log("����������� ����� ��������");
			loadingScreen.Hide();

			//Debug.Log("���������� ������� �� �����-�� �����");
			//yield return sceneSwitcherService.ProcessSwitchTo(Scenes.GameplayMechanic);
			//yield return sceneSwitcherService.ProcessSwitchTo(Scenes.Gameplay, new GameplayInputArgs(1));
			yield return sceneSwitcherService.ProcessSwitchTo(Scenes.MainMenu);
		}
	}
}