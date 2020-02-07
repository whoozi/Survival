using UnityEngine;

namespace Survival.Utility
{
	public class FollowCamera : MonoBehaviour
	{
		public Transform target;
		public Vector3 positionOffset;
		public Vector3 lookOffset;

		public void Update()
		{
			if (!target) return;

			transform.position = new Vector3(target.position.x + positionOffset.x, target.position.y + positionOffset.y, target.position.z + positionOffset.z);
			transform.LookAt(target.position + lookOffset);
        }
	}
}
