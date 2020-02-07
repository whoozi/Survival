using UnityEngine;
using UnityEngine.Networking;

using Survival.Utility;

namespace Survival.Mobs
{
	public class Mob : NetworkBehaviour
	{
		public enum State
		{
			Idle, Move, Jump, Fall, Attack
		}

		public enum Direction
		{
			Up, Down, Left, Right
		}

		public State state { get; set; }
		public Direction direction { get; protected set; }
		public BlockPosition blockPosition { get; private set; }

		public int maxHealth = 20;
		public int health { get; protected set; }
		protected float hurtTime;
		protected float jumpVelocity;
		[System.NonSerialized]
		public bool justJumped, justLanded, canMove = true, isDead, isUsing;
		private Vector3 knockback;

		[System.NonSerialized]
		public Animator animator;
		protected CharacterController controller;
		protected SpriteRenderer spriteRenderer;
		protected World world;

		public virtual void Start()
		{
			blockPosition = new BlockPosition(0, 0, 0);

			world = Game.instance.world;
			animator = GetComponent<Animator>();
			controller = GetComponent<CharacterController>();
			spriteRenderer = GetComponent<SpriteRenderer>();

			if (!hasAuthority) return;

			health = maxHealth;
			if (world.isBuilding) gameObject.SetActive(false);
		}

		public virtual void FixedUpdate()
		{
			if (!hasAuthority) return;

			if (!gameObject.activeInHierarchy && !world.isBuilding) gameObject.SetActive(true);

			blockPosition = new BlockPosition(Mathf.Clamp(Mathf.RoundToInt(transform.position.x), 0, world.sizeX - 1), Mathf.Clamp(Mathf.RoundToInt(transform.position.y + 1), 0, world.sizeY - 1), Mathf.Clamp(Mathf.RoundToInt(transform.position.z), 0, world.sizeZ - 1));

			if (animator)
			{
				animator.SetInteger("State", (int)state);
				animator.SetFloat("Direction", (int)direction);
				animator.SetFloat("Fall Speed", controller.velocity.y);
			}

			health = Mathf.Clamp(health, 0, maxHealth);

			if (hurtTime > Time.deltaTime) spriteRenderer.material.color = Color.red;
			else spriteRenderer.material.color = new Color(world.block[blockPosition.x, blockPosition.y, blockPosition.z].sunLight, world.block[blockPosition.x, blockPosition.y, blockPosition.z].sunLight, world.block[blockPosition.x, blockPosition.y, blockPosition.z].sunLight, 1);

			if (knockback.magnitude > 0.2f) controller.Move(knockback);
			knockback = Vector3.Lerp(knockback, Vector3.zero, 0.2f);

			if (blockPosition.y <= 0) Damage(2, Vector3.zero);
		}

		public void Damage(int damage, Vector3 knockback)
		{
			if (hurtTime <= Time.deltaTime)
			{
				health = Mathf.Clamp(health - damage, 0, maxHealth);
				hurtTime = 0.2f;

				this.knockback += knockback;
				jumpVelocity = knockback.y * Time.deltaTime;

				Game.instance.hudMenu.UpdateHearts();
			}

			if (health <= 0) OnDeath();
		}

		public void OnDeath()
		{
			isDead = true;
			canMove = false;
		}
	}
}
