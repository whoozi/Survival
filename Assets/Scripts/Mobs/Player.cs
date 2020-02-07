using UnityEngine;

using Survival.Blocks;
using Survival.Items;
using Survival.Utility;

namespace Survival.Mobs
{
	public class Player : Mob
	{
		public static LayerMask ignoreLayers;

		public Inventory inventory = new Inventory();
		[System.NonSerialized]
		public int currentSlot;
		private Vector3 oldMovement;
		private float timeUsed;
		private bool canSwing;

		public override void Start()
		{
			base.Start();

			ignoreLayers = (1 << LayerMask.NameToLayer("Mob") | 1 << LayerMask.NameToLayer("Item") | 1 << LayerMask.NameToLayer("UI"));

			Game.players.Add(this);

			if (isLocalPlayer)
			{
				Game.localPlayer = this;
				Camera.main.GetComponent<FollowCamera>().target = transform;
				inventory.slots = new Inventory.Slot[36];

				for (int i = 0; i < inventory.slots.Length; i++)
					inventory.slots[i] = new Inventory.Slot(null, 0);

				inventory.slots[0].Add(Item.pickaxe, 1);
				inventory.slots[1].Add(Item.sword, 1);
			}
		}

		private void OnDestroy()
		{
			Game.players.Remove(this);
		}

		private void Update()
		{
			if (!isLocalPlayer) return;

			if (isDead) return;

			if (controller.isGrounded)
			{
				if (Input.GetKey(KeyCode.Mouse0) && !Input.GetKey(KeyCode.Mouse1))
				{
					Item current = inventory.slots[currentSlot].item != null ? inventory.slots[currentSlot].item : Item.fist;

					if (current.autoSwing)
					{
						if (canMove)
							if (Time.realtimeSinceStartup - timeUsed > 0.25f || canSwing)
							{
								current.Use(world, this, inventory.slots[currentSlot]);
								timeUsed = Time.realtimeSinceStartup;
								canSwing = false;
							}
					}
					else
					{
						if (canMove && canSwing)
						{
							current.Use(world, this, inventory.slots[currentSlot]);
							canSwing = false;
						}
					}
				}
				else if (!Input.GetKey(KeyCode.Mouse1)) canSwing = true;

				if (Input.GetKey(KeyCode.Mouse1) && !Input.GetKey(KeyCode.Mouse0))
				{
					Item current = inventory.slots[currentSlot].item != null ? inventory.slots[currentSlot].item : Item.fist;

					if (current.autoSwing)
					{
						if (canMove)
							if (Time.realtimeSinceStartup - timeUsed > 0.25f || canSwing)
							{
								current.UseAlt(world, this, inventory.slots[currentSlot]);
								timeUsed = Time.realtimeSinceStartup;
								canSwing = false;
							}
					}
					else
					{
						if (canMove && canSwing)
						{
							current.UseAlt(world, this, inventory.slots[currentSlot]);
							canSwing = false;
						}
					}
				}
				else if (!Input.GetKey(KeyCode.Mouse0)) canSwing = true;
			}

			if (canSwing)
			{
				currentSlot -= (int)(Input.GetAxisRaw("Mouse Wheel"));

				if (currentSlot > 8) currentSlot = 0;
				else if (currentSlot < 0) currentSlot = 8;

				if (Input.GetKeyDown(KeyCode.Alpha1)) currentSlot = 0;
				if (Input.GetKeyDown(KeyCode.Alpha2)) currentSlot = 1;
				if (Input.GetKeyDown(KeyCode.Alpha3)) currentSlot = 2;
				if (Input.GetKeyDown(KeyCode.Alpha4)) currentSlot = 3;
				if (Input.GetKeyDown(KeyCode.Alpha5)) currentSlot = 4;
				if (Input.GetKeyDown(KeyCode.Alpha6)) currentSlot = 5;
				if (Input.GetKeyDown(KeyCode.Alpha7)) currentSlot = 6;
				if (Input.GetKeyDown(KeyCode.Alpha8)) currentSlot = 7;
				if (Input.GetKeyDown(KeyCode.Alpha9)) currentSlot = 8;
			}
		}

		public override void FixedUpdate()
		{
			if (!isLocalPlayer) return;

			base.FixedUpdate();

			// Movement
			Vector3 movement = new Vector3(SimpleInput.GetKeyCodeAxis(Game.instance.controls.moveLeft, Game.instance.controls.moveRight), 0, SimpleInput.GetKeyCodeAxis(Game.instance.controls.moveDown, Game.instance.controls.moveUp));
			movement = Camera.main.transform.TransformDirection(movement);
			movement.y = 0;
			movement.Normalize();

			if (hurtTime > 0) hurtTime -= Time.deltaTime;

			if (controller.isGrounded)
			{
				if (canMove)
				{
					if (movement.z > 0) direction = Direction.Up;
					if (movement.z < 0) direction = Direction.Down;
					if (movement.x < 0) direction = Direction.Left;
					if (movement.x > 0) direction = Direction.Right;

					if (movement != Vector3.zero)
					{
						state = State.Move;
						movement *= 4f;
					}
					else state = State.Idle;

					if (hurtTime <= Time.deltaTime && !justJumped)
					{
						if (Input.GetKey(Game.instance.controls.jump))
						{
							justJumped = true;
							jumpVelocity = 0.25f;
						}
					}
				}

				if (!justJumped) jumpVelocity = 0f;

				oldMovement = movement;
			}
			else
			{
				if (controller.velocity.y > 0) state = State.Jump;
				else
				{
					state = State.Fall;
					justJumped = false;
				}

				if (hurtTime > Time.deltaTime) oldMovement = Vector3.zero;

				jumpVelocity -= 0.008f;
			}

			controller.Move(((controller.isGrounded ? (canMove ? movement : Vector3.zero) : oldMovement) + Physics.gravity) * Time.deltaTime + new Vector3(0, jumpVelocity, 0));

			animator.SetFloat("Attack Speed", inventory.slots[currentSlot].item != null ? inventory.slots[currentSlot].item.swingSpeed : Item.fist.swingSpeed);

			// Mouse
			Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			Physics.Raycast(mouseRay, out Game.mouseHit, 100, ~(ignoreLayers | 1 << LayerMask.NameToLayer("Hidden")));
		}

		private void OnControllerColliderHit(ControllerColliderHit hit)
		{
			if (!isLocalPlayer) return;

			Block hitBlock = null;

			if (world.block[(int)hit.transform.position.x, (int)hit.transform.position.y, (int)hit.transform.position.z].id != Block.ID.Air)
				hitBlock = world.block[(int)hit.transform.position.x, (int)hit.transform.position.y, (int)hit.transform.position.z];

			if (hitBlock != null) hitBlock.OnTouch(this, hit.moveDirection);
		}
	}
}
