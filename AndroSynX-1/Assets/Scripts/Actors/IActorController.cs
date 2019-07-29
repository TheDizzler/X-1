using AtomosZ.AndroSyn.InputProcessing;

namespace AtomosZ.AndroSyn.Actors
{
	public interface IActorController
	{
		void OnActorControl(Actors.Actor actor);
		/// <summary>
		/// Called during Update, after the Character's internal state has been updated.
		/// Set inputs for the characters CommandQueue
		/// </summary>
		void UpdateCommands();
		/// <summary>
		/// Called during FixedUpdate, after the Character's internal state has been updated.
		/// Set inputs for the characters CommandQueue.
		/// </summary>
		void FixedUpdateCommands();
	}
}