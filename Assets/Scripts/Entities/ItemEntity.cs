using UnityEngine;

using Survival.Items;

namespace Survival.Mobs
{
	public class ItemEntity : Entity
	{
		public Item item { get; protected set; }
		private Transform child;
		public float liveTime;
		public float pickedUpTime;
		private float step, offset;

		public ItemEntity() : base() { }

		public ItemEntity(World world, Item item, float x, float y, float z) : base(world)
		{
			this.item = item;

			gameObject = (GameObject)Object.Instantiate(Game.itemEntityPrefab, new Vector3(x, y, z), Quaternion.identity);
			gameObject.name = (item.GetType() == typeof(BlockItem) ? ((BlockItem)(item)).block.GetType().Name : item.GetType().Name) + " Entity";
			child = gameObject.transform.GetChild(0);
			child.GetComponent<SpriteRenderer>().sprite = item.sprite;

			world.itemEntities.Add(this);
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();

			Player closest = GetClosestPlayer(1.25f);
			liveTime += Time.deltaTime;

			// Can now be picked up.
			if (liveTime > 0.5f)
			{
				if (closest)
					if (closest.inventory.HasRoom(item) != null)
						if (pickedUpTime == 0f)
						{
							pickedUpTime = Time.realtimeSinceStartup;
							gameObject.GetComponent<Rigidbody>().isKinematic = true;
							closest.inventory.HasRoom(item).Add(item, 1);
						}
			}

			if (pickedUpTime > 0f)
			{
				if (Time.realtimeSinceStartup - pickedUpTime > 0f)
				{
					gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, closest.transform.position + new Vector3(0f, 0.75f, 0f), 0.3f);

					if (Time.realtimeSinceStartup - pickedUpTime > 0.1f)
					{
						Destroy();
						return;
					}
				}
			}

			// Should die now.
			if (liveTime > 300f || blockPosition.y <= 0)
			{
				Destroy();
				return;
			}

			offset = gameObject.transform.position.y - child.position.y + 0.3f;
			step += 0.01f;

			child.localPosition = new Vector3(0, Mathf.Sin(step) / 5 + offset, 0);
			gameObject.transform.LookAt(new Vector3(Camera.main.transform.position.x, gameObject.transform.position.y, Camera.main.transform.position.z));
		}

		public override void Destroy()
		{
			base.Destroy();

			item = null;
			world.itemEntities.Remove(this);
		}
	}
}
