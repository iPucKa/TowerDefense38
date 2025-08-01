using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI.States;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Gameplay.Features.TeamsFeature;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities.Timer;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI
{
	public class BrainsFactory
	{
		private readonly DIContainer _container;
		private readonly TimerServiceFactory _timerServiceFactory;
		private readonly AIBrainsContext _brainsContext;
		private readonly IInputService _inputService;
		private readonly EntitiesLifeContext _entitiesLifeContext;

		public BrainsFactory(DIContainer container)
		{
			_container = container;
			_timerServiceFactory = _container.Resolve<TimerServiceFactory>();
			_brainsContext = _container.Resolve<AIBrainsContext>();
			_inputService = _container.Resolve<IInputService>();
			_entitiesLifeContext = _container.Resolve<EntitiesLifeContext>();
		}

		public StateMachineBrain CreateFortressBrain(Entity entity)
		{
			AIStateMachine behaviour = CreateByPlayerClickAttackStateMachine(entity);			

			StateMachineBrain brain = new StateMachineBrain(behaviour);

			_brainsContext.SetFor(entity, brain);

			return brain;
		}

		public StateMachineBrain CreateAgroEnemyBrain(Entity entity, ITargetSelector targetSelector)
		{
			AIStateMachine movementState = CreateMoveToTargetStateMachine(entity, targetSelector);

			AttackTriggerState attackTriggerState = new AttackTriggerState(entity);

			ICondition fromMovementToAttackCondition = entity.CanStartAttack;

			AIStateMachine rootStateMachine = new AIStateMachine();

			rootStateMachine.AddState(movementState);
			rootStateMachine.AddState(attackTriggerState);

			rootStateMachine.AddTransition(movementState, attackTriggerState, fromMovementToAttackCondition);

			StateMachineBrain brain = new StateMachineBrain(rootStateMachine);

			_brainsContext.SetFor(entity, brain);

			return brain;
		}

		public StateMachineBrain CreateMainHeroBrain(Entity entity, ITargetSelector targetSelector)
		{
			AIStateMachine combatState = CreateAutoAttackStateMachine(entity);

			PlayerInputMovementState movementState = new PlayerInputMovementState(entity, _inputService);

			ReactiveVariable<Entity> currentTarget = entity.CurrentTarget;

			ICompositCondition fromMovementToCombatStateCondition = new CompositCondition()
				.Add(new FuncCondition(() => currentTarget.Value != null))
				.Add(new FuncCondition(() => _inputService.Direction == Vector3.zero));

			ICompositCondition fromCombatToMovementStateCondition = new CompositCondition(LogicOperations.Or)
				.Add(new FuncCondition(() => currentTarget.Value == null))
				.Add(new FuncCondition(() => _inputService.Direction != Vector3.zero));

			AIStateMachine behaviour = new AIStateMachine();

			behaviour.AddState(combatState);
			behaviour.AddState(movementState);

			behaviour.AddTransition(movementState, combatState, fromMovementToCombatStateCondition);
			behaviour.AddTransition(combatState, movementState, fromCombatToMovementStateCondition);

			FindTargetState findTargetState = new FindTargetState(targetSelector, _entitiesLifeContext, entity);
			AIParallelState parallelState = new AIParallelState(findTargetState, behaviour);

			AIStateMachine rootStateMachine = new AIStateMachine();
			rootStateMachine.AddState(parallelState);

			StateMachineBrain brain = new StateMachineBrain(rootStateMachine);

			_brainsContext.SetFor(entity, brain);
			return brain;
		}

		public StateMachineBrain CreateSimpleHeroBrain(Entity entity, Camera camera)
		{
			AIStateMachine combatState = CreatePlayerAttackStateMachine(entity, camera);

			PlayerInputMovementState movementState = new PlayerInputMovementState(entity, _inputService);			

			ICompositCondition fromMovementToCombatStateCondition = new CompositCondition()
				.Add(entity.CanStartAttack)
				.Add(new FuncCondition(() => _inputService.Direction == Vector3.zero));

			ICompositCondition fromCombatToMovemenStateCondition = new CompositCondition()
				.Add(entity.CanMove)
				.Add(new FuncCondition(() => _inputService.Direction != Vector3.zero));

			AIStateMachine behaviour = new AIStateMachine();
			behaviour.AddState(combatState);
			behaviour.AddState(movementState);

			behaviour.AddTransition(combatState, movementState, fromCombatToMovemenStateCondition);
			behaviour.AddTransition(movementState, combatState, fromMovementToCombatStateCondition);

			StateMachineBrain brain = new StateMachineBrain(behaviour);

			_brainsContext.SetFor(entity, brain);

			return brain;
		}

		public StateMachineBrain CreateMainHeroRandomTeleportingBrain(Entity entity)
		{
			AIStateMachine stateMachine = CreateRandomTeleportStateMachine(entity);
			StateMachineBrain brain = new StateMachineBrain(stateMachine);

			_brainsContext.SetFor(entity, brain);

			return brain;
		}

		public StateMachineBrain CreateMainHeroToTargetTeleportingBrain(Entity entity, ITargetSelector targetSelector)
		{
			AIStateMachine stateMachine = CreateTeleportToTargetStateMachine(entity, targetSelector);
			StateMachineBrain brain = new StateMachineBrain(stateMachine);

			_brainsContext.SetFor(entity, brain);

			return brain;
		}

		public StateMachineBrain CreateGhostBrain(Entity entity)
		{
			AIStateMachine stateMachine = CreateRandomMovementStateMachine(entity);
			StateMachineBrain brain = new StateMachineBrain(stateMachine);

			_brainsContext.SetFor(entity, brain);

			return brain;
		}

		private AIStateMachine CreateRandomMovementStateMachine(Entity entity)
		{
			List<IDisposable> disposables = new List<IDisposable>();

			RandomMovementState randomMovementState = new RandomMovementState(entity, 0.5f);

			EmptyState emptyState = new EmptyState();

			TimerService movementTimer = _timerServiceFactory.Create(2f);
			disposables.Add(movementTimer);
			disposables.Add(randomMovementState.Entered.Subscribe(movementTimer.Restart));

			TimerService idleTimer = _timerServiceFactory.Create(3f);
			disposables.Add(idleTimer);
			disposables.Add(emptyState.Entered.Subscribe(idleTimer.Restart));

			FuncCondition movementTimerEndedCondition = new FuncCondition(() => movementTimer.IsOver);
			FuncCondition idleTimerEndedCondition = new FuncCondition(() => idleTimer.IsOver);

			AIStateMachine stateMachine = new AIStateMachine(disposables);

			stateMachine.AddState(randomMovementState);
			stateMachine.AddState(emptyState);

			stateMachine.AddTransition(randomMovementState, emptyState, movementTimerEndedCondition);
			stateMachine.AddTransition(emptyState, randomMovementState, idleTimerEndedCondition);

			return stateMachine;
		}

		private AIStateMachine CreateAutoAttackStateMachine(Entity entity)
		{
			RotateToTargetState rotateToTargetState = new RotateToTargetState(entity);

			AttackTriggerState attackTriggerState = new AttackTriggerState(entity);

			ICondition canAttack = entity.CanStartAttack;
			Transform transform = entity.Transform;
			ReactiveVariable<Entity> currentTarget = entity.CurrentTarget;

			ICompositCondition fromRotateToAttackCondition = new CompositCondition()
				.Add(canAttack)
				.Add(new FuncCondition(() =>
				{
					Entity target = currentTarget.Value;

					if (target == null)
						return false;

					float angleToTarget = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(target.Transform.position - transform.position));
					return angleToTarget < 3f;
				}));

			ReactiveVariable<bool> inAttackProcess = entity.InAttackProcess;

			ICondition fromAttackToRotateStateCondition = new FuncCondition(() => inAttackProcess.Value == false);

			AIStateMachine stateMachine = new AIStateMachine();

			stateMachine.AddState(rotateToTargetState);
			stateMachine.AddState(attackTriggerState);

			stateMachine.AddTransition(rotateToTargetState, attackTriggerState, fromRotateToAttackCondition);
			stateMachine.AddTransition(attackTriggerState, rotateToTargetState, fromAttackToRotateStateCondition);

			return stateMachine;
		}

		private AIStateMachine CreateRandomTeleportStateMachine(Entity entity)
		{
			RandomTeleportingState randomTeleportingState = new RandomTeleportingState(entity, entity.TeleportCooldownInitialTime.Value);

			EmptyState emptyState = new EmptyState();

			ICondition canTeleport = entity.CanTeleport;

			ICompositCondition fromEmptyToTeleportingStateCondition = new CompositCondition()
				.Add(canTeleport);

			ICompositCondition fromTeleportingToEmptyStateCondition = new CompositCondition(LogicOperations.Or)
				//.Add(new FuncCondition(() => canTeleport.Evaluate()==false));
				.Add(new FuncCondition(() => entity.IsDead.Value == true))
				.Add(new FuncCondition(() => entity.InTeleportCooldown.Value == true))
				.Add(new FuncCondition(() => entity.CurrentEnergy.Value < entity.TeleportByEnergyValue.Value));

			AIStateMachine stateMachine = new AIStateMachine();

			stateMachine.AddState(randomTeleportingState);
			stateMachine.AddState(emptyState);

			stateMachine.AddTransition(emptyState, randomTeleportingState, fromEmptyToTeleportingStateCondition);
			stateMachine.AddTransition(randomTeleportingState, emptyState, fromTeleportingToEmptyStateCondition);

			return stateMachine;
		}

		private AIStateMachine CreateTeleportToTargetStateMachine(Entity entity, ITargetSelector targetSelector)
		{
			TeleportToTargetState teleportToTargetState = new TeleportToTargetState(entity, entity.TeleportCooldownInitialTime.Value);

			EmptyState emptyState = new EmptyState();

			ICondition canTeleport = entity.CanTeleport;

			float energyEconomyValue = 0.4f * entity.MaxEnergy.Value;

			ICompositCondition fromEmptyToTeleportingStateCondition = new CompositCondition()
				.Add(canTeleport)
				.Add(new FuncCondition(() => entity.CurrentEnergy.Value >= energyEconomyValue));

			ICompositCondition fromTeleportingToEmptyStateCondition = new CompositCondition(LogicOperations.Or)
				.Add(new FuncCondition(() => entity.IsDead.Value == true))
				.Add(new FuncCondition(() => entity.InTeleportCooldown.Value == true))
				.Add(new FuncCondition(() => entity.CurrentEnergy.Value < energyEconomyValue));

			AIStateMachine stateMachine = new AIStateMachine();

			stateMachine.AddState(teleportToTargetState);
			stateMachine.AddState(emptyState);

			stateMachine.AddTransition(emptyState, teleportToTargetState, fromEmptyToTeleportingStateCondition);
			stateMachine.AddTransition(teleportToTargetState, emptyState, fromTeleportingToEmptyStateCondition);

			FindTargetState findTargetState = new FindTargetState(targetSelector, _entitiesLifeContext, entity);
			AIParallelState parallelState = new AIParallelState(findTargetState, stateMachine);

			AIStateMachine rootStateMachine = new AIStateMachine();
			rootStateMachine.AddState(parallelState);

			return rootStateMachine;
		}

		private AIStateMachine CreatePlayerAttackStateMachine(Entity entity, Camera camera)
		{
			MouseTrackService mouseTrackService = new MouseTrackService(camera, _inputService);
			
			MouseRotationState mouseRotationState = new MouseRotationState(entity, _inputService, mouseTrackService);

			AttackByMouseKeyState attackByKeyState = new AttackByMouseKeyState(entity, _inputService);

			ReactiveVariable<bool> inAttackProcess = entity.InAttackProcess;

			ICompositCondition fromAttackToMouseRotationStateCondition = new CompositCondition()
				.Add(entity.CanRotate)
				.Add(new FuncCondition(() => inAttackProcess.Value == false));

			ICompositCondition fromMouseRotationToAttackStateCondition = new CompositCondition()
				.Add(entity.CanStartAttack)
				.Add(new FuncCondition(() => entity.IsAttackKeyPressed.Value == true));

			AIStateMachine stateMachine = new AIStateMachine();

			stateMachine.AddState(attackByKeyState);
			stateMachine.AddState(mouseRotationState);

			stateMachine.AddTransition(attackByKeyState, mouseRotationState, fromAttackToMouseRotationStateCondition);
			stateMachine.AddTransition(mouseRotationState, attackByKeyState, fromMouseRotationToAttackStateCondition);

			return stateMachine;
		}

		private AIStateMachine CreateByPlayerClickAttackStateMachine(Entity entity)
		{
			IReadOnlyList<Entity> entities = _entitiesLifeContext.Entities;

			EmptyState emptyState = new EmptyState();
			AttackByMouseKeyState attackByKeyState = new AttackByMouseKeyState(entity, _inputService);			

			ICompositCondition fromEmptyToAttackStateCondition = new CompositCondition()
				.Add(entity.CanStartAttack)
				.Add(new FuncCondition(() =>
				{
					foreach (Entity entity in entities)
					{
						if (entity.TryGetTeam(out ReactiveVariable<Teams> team))
						{
							if (team.Value == Teams.Enemies)							
								return true;							
						}
					}					
					return false;
				}));

			ICompositCondition fromAttackToEmptyStateCondition = new CompositCondition()		
				.Add(new FuncCondition(() =>
				{
					foreach (Entity entity in entities)
					{
						if (entity.TryGetTeam(out ReactiveVariable<Teams> team))
						{
							if (team.Value == Teams.Enemies)
								return false;							
						}						
					}
					return true;
				}));				

			AIStateMachine stateMachine = new AIStateMachine();

			stateMachine.AddState(emptyState);
			stateMachine.AddState(attackByKeyState);

			stateMachine.AddTransition(emptyState, attackByKeyState, fromEmptyToAttackStateCondition);
			stateMachine.AddTransition(attackByKeyState, emptyState, fromAttackToEmptyStateCondition);

			return stateMachine;
		}

		private AIStateMachine CreateMoveToTargetStateMachine(Entity entity, ITargetSelector targetSelector)
		{
			EmptyState emptyState = new EmptyState();
			RotateToTargetState rotateToTargetState = new RotateToTargetState(entity);
			MoveToTargetState moveToTargetState = new MoveToTargetState(entity);

			ReactiveVariable<Vector3> rotationDirection = entity.RotationDirection;
			ReactiveVariable<Vector3> moveDirection = entity.MoveDirection;
			ReactiveVariable<Entity> currentTarget = entity.CurrentTarget;
			Transform transform = entity.Transform;

			ICondition canMove = entity.CanMove;
			ICondition canRotate = entity.CanRotate;

			ICompositCondition fromEmptyStateToRotateCondition = new CompositCondition()
				.Add(canRotate)
				.Add(new FuncCondition(() => entity.CurrentTarget.Value !=null));

			ICompositCondition fromRotateToEmptyStateCondition = new CompositCondition()				
				.Add(new FuncCondition(() => entity.CurrentTarget.Value == null));

			ICompositCondition fromRotateToMoveCondition = new CompositCondition()
				.Add(canMove)
				.Add(new FuncCondition(() => entity.CurrentTarget.Value != null))
				.Add(new FuncCondition(() =>
				{
					Entity target = currentTarget.Value;

					if (target == null)
						return false;

					float angleToTarget = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(target.Transform.position - transform.position));
					return angleToTarget < 2f;
				}));

			ICompositCondition fromMoveToRotateCondition = new CompositCondition()
				.Add(canRotate)
				.Add(new FuncCondition(() => entity.CurrentTarget.Value != null))
				.Add(new FuncCondition(() =>
				{
					Entity target = currentTarget.Value;

					if (target == null)
						return false;

					float angleToTarget = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(target.Transform.position - transform.position));
					return angleToTarget > 5f;
				}));

			ICompositCondition fromMoveToEmptyStateCondition = new CompositCondition()
				.Add(new FuncCondition(() => entity.CurrentTarget.Value == null));

			AIStateMachine behaviour = new AIStateMachine();
			behaviour.AddState(emptyState);
			behaviour.AddState(rotateToTargetState);
			behaviour.AddState(moveToTargetState);
						
			behaviour.AddTransition(emptyState, rotateToTargetState, fromEmptyStateToRotateCondition);
			behaviour.AddTransition(rotateToTargetState, moveToTargetState, fromRotateToMoveCondition);
			behaviour.AddTransition(rotateToTargetState, emptyState, fromRotateToEmptyStateCondition);
			behaviour.AddTransition(moveToTargetState, rotateToTargetState, fromMoveToRotateCondition);
			behaviour.AddTransition(moveToTargetState, emptyState, fromMoveToEmptyStateCondition);

			FindTargetState findTargetState = new FindTargetState(targetSelector, _entitiesLifeContext, entity);
			AIParallelState parallelState = new AIParallelState(findTargetState, behaviour);

			AIStateMachine stateMachine = new AIStateMachine();
			stateMachine.AddState(parallelState);

			return stateMachine;
		}
	}
}
