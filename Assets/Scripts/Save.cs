using System;
using System.Xml.Serialization;

using UnityEngine;

using Survival.Blocks;
using Survival.Items;
using Survival.Utility;

namespace Survival
{
	public class Save
	{
		[XmlRoot("info")]
		public class InfoSave
		{
			public string version;
			public string name;
			public int seed, sizeX, sizeY, sizeZ;
			public World.Difficulty difficulty;
			public string lastPlayed;

			public InfoSave() { }

			public InfoSave(string version, string name, int seed, int sizeX, int sizeY, int sizeZ, World.Difficulty difficulty, string lastPlayed)
			{
				this.version = version;
				this.name = name;
				this.seed = seed;
				this.sizeX = sizeX;
				this.sizeY = sizeY;
				this.sizeZ = sizeZ;
				this.difficulty = difficulty;
				this.lastPlayed = lastPlayed;
			}
		}

		[XmlRoot("world")]
		public class WorldSave
		{
			public class ItemEntitySave
			{
				public Item item;
				[XmlAttribute]
				public float x, y, z, t;

				public ItemEntitySave() { }

				public ItemEntitySave(Item item, float x, float y, float z, float t)
				{
					this.item = item;
					this.x = x;
					this.y = y;
					this.z = z;
					this.t = t;
				}
			}

			[XmlElement("b")]
			public Block[] blocks;
			[XmlElement("i")]
			public ItemEntitySave[] items;

			public WorldSave() { }

			public WorldSave(Block[] blocks, ItemEntitySave[] items)
			{
				this.blocks = blocks;
				this.items = items;
			}

			public static void Serialize(World world, string path)
			{
				WorldSave worldSave = new WorldSave(new Block[world.sizeX * world.sizeY * world.sizeZ], new ItemEntitySave[world.itemEntities.Count]);

				Application.CaptureScreenshot(path + "worlds/" + world.name + "/thumbnail.png");

				int index = 0;

				for (int x = 0; x < world.sizeX; x++)
					for (int y = 0; y < world.sizeY; y++)
						for (int z = 0; z < world.sizeZ; z++)
						{
							worldSave.blocks[index] = world.block[x, y, z];
							index++;
						}

				for (int i = 0; i < world.itemEntities.Count; i++) worldSave.items[i] = new ItemEntitySave(world.itemEntities[i].item, world.itemEntities[i].gameObject.transform.position.x, world.itemEntities[i].gameObject.transform.position.y, world.itemEntities[i].gameObject.transform.position.z, world.itemEntities[i].liveTime);

				Serializer.Serialize(worldSave, path + "worlds/" + world.name + "/world.dat");
				Serializer.Serialize(new InfoSave("1.0.0a", world.name, world.seed, world.sizeX, world.sizeY, world.sizeZ, world.difficulty, DateTime.Now.ToString()), path + "worlds/" + world.name + "/info.dat");
			}

			public static World Deserialize(InfoSave worldInfoSave, string path)
			{
				WorldSave worldSave = Serializer.Deserialize<WorldSave>(path + "worlds/" + worldInfoSave.name + "/world.dat");

				World world = new World(worldInfoSave.name, worldInfoSave.seed, worldInfoSave.sizeX, worldInfoSave.sizeY, worldInfoSave.sizeZ);

				int index = 0;

				if (worldSave.blocks != null)
					for (int x = 0; x < world.sizeX; x++)
						for (int y = 0; y < world.sizeY; y++)
							for (int z = 0; z < world.sizeZ; z++)
							{
								world.block[x, y, z] = worldSave.blocks[index];
								index++;
							}

				for (int y = 0; y < world.sizeY; y++) world.slices.Add(new Slice(0, 0, y));
				if (worldSave.items != null) for (int i = 0; i < worldSave.items.Length; i++) new Mobs.ItemEntity(world, worldSave.items[i].item, worldSave.items[i].x, worldSave.items[i].y, worldSave.items[i].z).liveTime = worldSave.items[i].t;

				return world;
			}
		}

		[XmlRoot("character")]
		public class CharacterSave
		{
			public Inventory inventory;
		}
	}
}
