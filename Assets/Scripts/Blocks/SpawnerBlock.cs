using System.Xml.Serialization;

using Survival.Mobs;

namespace Survival.Blocks
{
	[System.Serializable]
	public class SpawnerBlock : Block
	{
		public override ID id
		{
			get { return ID.Spawner; }
		}

		public override string name
		{
			get { return mob.name + " Spawner"; }
		}

		public override bool canOcclude
		{
			get { return true; }
		}

		public override bool hasCollider
		{
			get { return true; }
		}

		[XmlIgnore]
		public Mob mob;

		public SpawnerBlock() : base() { }

		public SpawnerBlock(Mob mob) : this()
		{
			this.mob = mob;
		}
	}
}
