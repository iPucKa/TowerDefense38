using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI.States;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.GameplayMechanics
{
	public class TestGameplay : MonoBehaviour
	{
		private DIContainer _container;
		private EntitiesFactory _entitiesFactory;
		private BrainsFactory _brainsFactory;

		private Entity _ghost;
		private Entity _entityBlinckedHero;
		private Entity _entityHero;

		private Camera _camera;

		private bool _isRunning;

		public void Initialize(DIContainer container)
		{
			_camera = Camera.main;
			_container = container;
			_entitiesFactory = _container.Resolve<EntitiesFactory>();
			_brainsFactory = _container.Resolve<BrainsFactory>();
		}

		public void Run()
		{
			//_ghost = _entitiesFactory.CreateGhost(Vector3.zero + Vector3.forward * 3);
			//_brainsFactory.CreateGhostBrain(_ghost);

			//_entityBlinckedHero = _entitiesFactory.CreateBlinckedHero(Vector3.zero);
			//добавить мозг Герою
			
			//_entityHero = _entitiesFactory.CreateHero(Vector3.zero + Vector3.right * 5);
			//добавить мозг Герою

			_isRunning = true;
		}

		private void Update()
		{
			if (_isRunning == false)
				return;

			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				Debug.Log("РЕЖИМ СВОБОДНОЙ ТЕЛЕПОРТАЦИИ");

				_brainsFactory.CreateMainHeroRandomTeleportingBrain(_entityBlinckedHero);
			}

			if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				Debug.Log("РЕЖИМ ТЕЛЕПОРТАЦИИ К ЦЕЛИ");

				_brainsFactory.CreateMainHeroToTargetTeleportingBrain(_entityBlinckedHero, new MinHealthDamageableTargetSelector(_entityBlinckedHero));
			}

			if (Input.GetKeyDown(KeyCode.Alpha3))
			{
				Debug.Log("РЕЖИМ УПРАВЛЕНИЯ ДВИЖЕНИЕМ");

				_brainsFactory.CreateSimpleHeroBrain(_entityHero, _camera);
			}
		}
	}
}
