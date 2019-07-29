using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionLock : MonoBehaviour
{
	[SerializeField] private Vector3 lockToPosition = Vector3.zero;

	public void OnDrawGizmos()
	{
		transform.localPosition = lockToPosition;
	}
}
