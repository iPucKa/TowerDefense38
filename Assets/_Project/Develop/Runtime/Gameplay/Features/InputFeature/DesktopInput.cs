using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature
{
	public class DesktopInput : IInputService
	{
		private const string HorizontalAxisName = "Horizontal";
		private const string VerticalAxisName = "Vertical";
		private const int LeftMouseButton = 0;

		public bool IsEnabled { get; set; } = true;

		public Vector3 Direction
		{
			get
			{
				if (IsEnabled == false)
					return Vector3.zero;

				return new Vector3(Input.GetAxisRaw(HorizontalAxisName), 0, Input.GetAxisRaw(VerticalAxisName));
			}
		}

		public Vector3 PointerPosition => Input.mousePosition;

		public bool IsAttackButtonPressed => Input.GetMouseButtonDown(LeftMouseButton);
	}
}
