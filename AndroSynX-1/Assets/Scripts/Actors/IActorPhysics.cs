using UnityEngine;

namespace AtomosZ.AndroSyn.Actors
{
	public interface IActorPhysics
	{
		bool isGrounded { get; set;}
		Vector2 desiredVelocity { get; set; }
		bool isFacingRight { get; set;}
		Vector2 up { get; set; }

		Vector2 Facing();
		Vector2 GetVelocity();
		Vector3 GetTotalAffectingGravity();
		void ChangeUpDirection(Vector2 newUp);
		void UpdateInternalStateFromPhysicsResult();
		void ApplyToPhysics();
		bool CheckForCollision(Vector2 normalizedDirection);
	}
}