using AtomosZ.AndroSyn.Gadgets;
using UnityEngine;

namespace AtomosZ.AndroSyn.Actors.State
{
	public class ElevatorState : MonoBehaviour, IMovementState, IActionState
	{
		public MovementStateType movementStateType
		{
			get => MovementStateType.ELEVATOR;
			set => throw new System.NotImplementedException();
		}

		public ActionStateType actionStateType
		{
			get => ActionStateType.ELEVATOR;
			set => throw new System.NotImplementedException();
		}

		public Elevator elevator;

		private Actor actor;
		private int oldZDepth;


		public void SetActor(Actor owner)
		{
			actor = owner;
		}

		
		public void EnterState(ActionStateType previousState)
		{
			// do nothing
		}
		
		public void EnterState(MovementStateType previousState)
		{
			oldZDepth = actor.SetZDepth(-10);
			actor.EnterElevator();
			elevator.Entered(actor);
		}

		public ActionStateType ExitState(ActionStateType nextState)
		{
			return actionStateType;
		}

		public MovementStateType ExitState(MovementStateType nextState)
		{
			actor.SetZDepth(oldZDepth);
			return movementStateType;
		}


		public ActionStateType UpdateState()
		{
			return elevator.PassengerUpdate(actor.commandList);
		}

		public MovementStateType FixedUpdateState()
		{
			MovementStateType nextMovement = elevator.PassengerFixedUpdate(actor.commandList);

			return nextMovement;
		}
	}
}