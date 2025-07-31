using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature
{
	public class MouseTrackService
	{
		private readonly Camera _camera;
		private readonly IInputService _inputService;

		private Vector3 _mousePosition;

		public MouseTrackService(
			Camera camera,
			IInputService inputService)
		{
			_camera = camera;
			_inputService = inputService;
		}

		public Vector3 Position
		{
			get
			{
				// Получаю луч от камеры через позицию мыши
				Ray ray = _camera.ScreenPointToRay(_inputService.PointerPosition);

				// Создаю плоскость на уровне пола
				Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

				// Определяю точку пересечения луча с плоскостью
				if (groundPlane.Raycast(ray, out float point))
					_mousePosition = ray.GetPoint(point);

				return _mousePosition;
			}
		}
	}
}
