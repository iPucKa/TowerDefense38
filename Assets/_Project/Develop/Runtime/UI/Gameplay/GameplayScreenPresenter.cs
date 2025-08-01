using Assets._Project.Develop.Runtime.UI.Core;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.UI.Gameplay
{
	public class GameplayScreenPresenter : IPresenter
	{
		private const string WinMessage = "YOU WIN";
		private const string DefeatMessage = "TRY AGAIN";

		private readonly GameplayScreenView _screen;
		private readonly ProjectPresentersFactory _projectPresentersFactory;
		private readonly GameplayPopupService _popupService;

		private readonly List<IPresenter> _childPresenters = new();

		public GameplayScreenPresenter(
			GameplayScreenView screen,
			ProjectPresentersFactory projectPresentersFactory,
			GameplayPopupService popupService)
		{
			_screen = screen;
			_projectPresentersFactory = projectPresentersFactory;
			_popupService = popupService;
		}

		public void Initialize()
		{
			//_rule.IsMatch += OnGameWin;
			//_rule.IsNotMatch += OnGameDefeat;

			foreach (IPresenter presenter in _childPresenters)
				presenter.Initialize();
		}

		public void Dispose()
		{
			//_rule.IsMatch -= OnGameWin;
			//_rule.IsNotMatch -= OnGameDefeat;

			foreach (IPresenter presenter in _childPresenters)
				presenter.Dispose();

			_childPresenters.Clear();
		}		

		private void OnGameWin() => _popupService.OpenEndGamePopup(WinMessage);

		private void OnGameDefeat() => _popupService.OpenEndGamePopup(DefeatMessage);
	}
}
