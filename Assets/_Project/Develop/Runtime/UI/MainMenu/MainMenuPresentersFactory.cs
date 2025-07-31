using Assets._Project.Develop.Runtime.Gameplay.Progress;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.ConfigsManagement;
using Assets._Project.Develop.Runtime.Utilities.CoroutinesManagement;
using Assets._Project.Develop.Runtime.Utilities.SceneManagement;

namespace Assets._Project.Develop.Runtime.UI.MainMenu
{
	public class MainMenuPresentersFactory
	{
		private readonly DIContainer _container;

		public MainMenuPresentersFactory(DIContainer container)
		{
			_container = container;
		}

		public MainMenuScreenPresenter CreateMainMenuScreen(MainMenuScreenView view)
		{
			return new MainMenuScreenPresenter(
				view,
				_container.Resolve<ProjectPresentersFactory>(),
				_container.Resolve<ProgressRestoreService>(),
				_container.Resolve<ConfigsProviderService>(),
				_container.Resolve<SceneSwitcherService>(),
				_container.Resolve<ICoroutinesPerformer>());
		}
	}
}
