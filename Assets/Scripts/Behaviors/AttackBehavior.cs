using UnityEngine;

using Survival.Mobs;

namespace Survival.Behaviors
{
	public class AttackBehavior : StateMachineBehaviour
	{
		private Mob mob;

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			mob = animator.GetComponent<Mob>();
			mob.canMove = false;
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			mob.canMove = true;
			if (mob.transform.childCount > 0 && mob.transform.GetChild(0).childCount > 0) Destroy(mob.transform.GetChild(0).GetChild(0).gameObject);
		}
	}
}
