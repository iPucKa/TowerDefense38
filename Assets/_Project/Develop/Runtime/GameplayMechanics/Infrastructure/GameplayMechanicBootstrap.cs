using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI;
using Assets._Project.Develop.Runtime.Infrastructure;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.SceneManagement;
using System.Collections;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.GameplayMechanics.Infrastructure
{	
	public class GameplayMechanicBootstrap : SceneBootstrap
	{
		private DIContainer _container;

		[SerializeField] private TestGameplay _testGameplay;

		private EntitiesLifeContext _entitiesLifeContext;
		private AIBrainsContext _brainsContext;

		public override void ProcessRegistrations(DIContainer container, IInputSceneArgs sceneArgs = null)
		{
			_container = container;	
			//Camera camera = Camera.main;

			//GameplayMechanicsContextRegistrations.Process(_container, camera);
		}

		public override IEnumerator Initialize()
		{
			Debug.Log("Инициализация сцены геймплейных механик");

			_entitiesLifeContext = _container.Resolve<EntitiesLifeContext>();
			_brainsContext = _container.Resolve<AIBrainsContext>();

			_testGameplay.Initialize(_container);

			yield break;
		}

		public override void Run()
		{
			Debug.Log("Старт сцены геймплейных механик");
			_testGameplay.Run();
		}

		private void Update()
		{
			_brainsContext?.Update(Time.deltaTime);
			_entitiesLifeContext?.Update(Time.deltaTime);			
		}
	}
}
