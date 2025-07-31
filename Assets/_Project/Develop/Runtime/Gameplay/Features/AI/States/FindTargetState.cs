﻿using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities.StateMachineCore;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI.States
{
	public class FindTargetState : State, IUpdatableState
	{
		private ITargetSelector _targetSelector; //алгоритм отбора цели
		private EntitiesLifeContext _entitiesLifeContext;
		private ReactiveVariable<Entity> _currentTarget;

		public FindTargetState(
			ITargetSelector targetSelector,
			EntitiesLifeContext entitiesLifeContext,
			Entity entity)
		{
			_targetSelector = targetSelector;
			_entitiesLifeContext = entitiesLifeContext;
			_currentTarget = entity.CurrentTarget;
		}

		public void Update(float deltaTime)
		{
			_currentTarget.Value = _targetSelector.SelectTargetFrom(_entitiesLifeContext.Entities);
		}
	}
}
