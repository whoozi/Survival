using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

using UnityEngine;

using Survival.Items;
using Survival.Mobs;

namespace Survival.Blocks
{
	[Serializable,
		XmlInclude(typeof(GrassBlock)), XmlInclude(typeof(DirtBlock)), XmlInclude(typeof(SandBlock)), XmlInclude(typeof(CactusBlock)), XmlInclude(typeof(FlowerBlock)), XmlInclude(typeof(WoolBlock)),
		XmlInclude(typeof(TorchBlock)), XmlInclude(typeof(SpawnerBlock))]
	public class Block
	{
		public enum ID
		{
			Air, Grass, Dirt, Sand, Cactus, Wool, Flower, Torch, Spawner
		}

		public enum Type
		{
			Cube, Plane, Other
		}

		public virtual ID id
		{
			get { return ID.Air; }
		}

		public virtual Type type
		{
			get { return Type.Cube; }
		}

		public virtual Rect textureCoords
		{
			get { return new Rect(0, 16, 16, 16); }
		}

		public virtual Sprite sprite
		{
			get { return Sprite.Create((Texture2D)Game.blockMaterial.mainTexture, new Rect(textureCoords.x, textureCoords.y - textureCoords.height, textureCoords.width, textureCoords.height), Vector2.one / 2, 10); }
		}

		public virtual string name
		{
			get { return "Air"; }
		}

		public virtual bool canOcclude
		{
			get { return false; }
		}

		public virtual bool hasCollider
		{
			get { return false; }
		}

		public virtual bool canActivate
		{
			get { return false; }
		}

		public virtual float durability
		{
			get { return 1f; }
		}

		public virtual Item dropItem
		{
			get { return new BlockItem(this); }
		}

		[NonSerialized, XmlIgnore]
		public GameObject gameObject = null;
		[NonSerialized, XmlIgnore]
		public float sunLight = 0f, breakage = 0f;

		public Block() { }

		public virtual void FixedUpdate(World world, int x, int y, int z) { }

		public virtual void OnActivate(Mob mob, Item item) { }

		public virtual void OnTouch(Mob mob, Vector3 direction) { }

		public virtual void OnDestroy(World world, int x, int y, int z, ToolItem tool)
		{
			world.SetBlock<Block>(x, y, z);
			new ItemEntity(world, dropItem, x, y, z);
		}

		public virtual bool CanStay(World world, int x, int y, int z)
		{
			return ((x >= 0 && x < world.sizeX - 1) && (y >= 0 && y < world.sizeY - 1) && (z >= 0 && z < world.sizeZ - 1));
		}

		public void Damage(World world, int x, int y, int z, ToolItem tool)
		{
			if (tool.IsEffectiveAgainst(this))
			{
				breakage += tool.breakSpeed;

				if (breakage > durability) OnDestroy(world, x, y, z, tool);
			}
		}

		public Block Clone()
		{
			using (MemoryStream memoryStream = new MemoryStream(10))
			{
				IFormatter formatter = new BinaryFormatter();
				formatter.Serialize(memoryStream, this);
				memoryStream.Seek(0, SeekOrigin.Begin);
				return formatter.Deserialize(memoryStream) as Block;
			}
		}

		public static readonly Block air = new Block();
		public static readonly GrassBlock grass = new GrassBlock();
		public static readonly DirtBlock dirt = new DirtBlock();
		public static readonly SandBlock sand = new SandBlock();
		public static readonly CactusBlock cactus = new CactusBlock();
		public static readonly WoolBlock wool = new WoolBlock();
		public static readonly FlowerBlock flower = new FlowerBlock();
		public static readonly TorchBlock torch = new TorchBlock();
		public static readonly SpawnerBlock spawner = new SpawnerBlock();
	}
}
