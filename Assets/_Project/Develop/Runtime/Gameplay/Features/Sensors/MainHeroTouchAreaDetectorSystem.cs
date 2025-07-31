using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.MainHero;
using Assets._Project.Develop.Runtime.Utilities;
using Assets._Project.Develop.Runtime.Utilities.Reactive;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.Sensors
{
	public class MainHeroTouchAreaDetectorSystem : IInitializableSystem, IUpdatableSystem
	{
		private Buffer<Entity> _contacts;
		private ReactiveVariable<bool> _isTouchMainHero;

		public void OnInit(Entity entity)
		{
			_contacts = entity.AreaContactEntitiesBuffer;
			_isTouchMainHero = entity.IsTouchMainHero;
		}

		public void OnUpdate(float deltaTime)
		{
			for (int i = 0; i < _contacts.Count; i++)
			{
				Entity contact = _contacts.Items[i];

				if (contact.HasComponent<IsMainHero>())
				{
					_isTouchMainHero.Value = true;
					return;
				}
			}

			_isTouchMainHero.Value = false;
		}
	}
}
