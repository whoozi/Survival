using UnityEngine;

using Survival.Utility;

namespace Survival.Mobs
{
	public class Entity
	{
		protected World world;
		public GameObject gameObject { get; protected set; }
		public BlockPosition blockPosition { get; protected set; }

		public Entity()
		{
			blockPosition = new BlockPosition(0, 0, 0);
		}

		public Entity(World world) : this()
		{
			this.world = world;
		}

		public virtual void FixedUpdate()
		{
			if (gameObject && blockPosition != null)
			{
				blockPosition = new BlockPosition(Mathf.Clamp(Mathf.RoundToInt(gameObject.transform.position.x), 0, world.sizeX - 1), Mathf.Clamp(Mathf.RoundToInt(gameObject.transform.position.y + 1), 0, world.sizeY - 1), Mathf.Clamp(Mathf.RoundToInt(gameObject.transform.position.z), 0, world.sizeZ - 1));

				if (gameObject.GetComponentInChildren<SpriteRenderer>()) gameObject.GetComponentInChildren<SpriteRenderer>().material.color = new Color(world.block[blockPosition.x, blockPosition.y, blockPosition.z].sunLight, world.block[blockPosition.x, blockPosition.y, blockPosition.z].sunLight, world.block[blockPosition.x, blockPosition.y, blockPosition.z].sunLight, 1);
			}
		}

		public Player GetClosestPlayer(float distance)
		{
			Player closest = null;

			for (int i = 0; i < Game.players.Count; i++)
			{
				if (Vector3.Distance(Game.players[i].transform.position + new Vector3(0f, 0.75f, 0f), gameObject.transform.position) <= distance)
				{
					if (!closest) closest = Game.players[i];
					else if (Vector3.Distance(closest.transform.position + new Vector3(0f, 0.75f, 0f), gameObject.transform.position) > Vector3.Distance(Game.players[i].transform.position + new Vector3(0f, 0.75f, 0f), gameObject.transform.position)) closest = Game.players[i];
				}
			}

			return closest;
		}

		public virtual void Destroy()
		{
			if (gameObject) Object.Destroy(gameObject);
			blockPosition = null;
		}
	}
}
