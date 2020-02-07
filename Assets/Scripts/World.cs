using System.Collections.Generic;
using System.Threading;

using UnityEngine;
using UnityEngine.Rendering;

using Survival.Blocks;
using Survival.Mobs;
using Survival.Utility;

namespace Survival
{
	public class World
	{
		public enum Difficulty
		{
			Normal, Expert
		}

		public string name;
		public int seed, sizeX, sizeY, sizeZ;
		public Difficulty difficulty;
		public Block[,,] block;
		public List<Slice> slices = new List<Slice>();
		public List<ItemEntity> itemEntities = new List<ItemEntity>();
		public bool isBuilding = false, shouldUpdateSunlight = true, isUpdatingSunlight = false;

		public World(string name, int seed, int sizeX, int sizeY, int sizeZ)
		{
			this.name = name;
			this.seed = Random.seed = seed;
			this.sizeX = sizeX;
			this.sizeY = sizeY;
			this.sizeZ = sizeZ;
			block = new Block[sizeX, sizeY, sizeZ];
		}

		public void Build()
		{
			isBuilding = true;

			for (int y = 0; y < sizeY; y++) slices.Add(new Slice(0, 0, y));

			for (int x = 0; x < sizeX; x++)
				for (int z = 0; z < sizeZ; z++)
					for (int y = 0; y < sizeY; y++)
						block[x, y, z] = Block.air;

			Vector3 woolBlockStart = new Vector3(Random.Range(5, sizeX - 5), 0, Random.Range(5, sizeZ - 5));

			for (int x = 0; x < sizeX; x++)
			{
				for (int z = 0; z < sizeZ; z++)
				{
					for (int y = 0; y < sizeY; y++)
					{
						if (y == 0)
							if (Random.Range(0, 2) == 0) SetBlock<GrassBlock>(x, y, z);
							else SetBlock<SandBlock>(x, y, z);

						if (y == 1)
						{
							if (Random.Range(0, 20) == 0) SetBlock<GrassBlock>(x, y, z);
							else SetBlock<Block>(x, y, z);

							if (block[x, y, z].id == Block.ID.Air && Block.cactus.CanStay(this, x, y, z))
								if (Random.Range(0, 100) == 0)
								{
									SetBlock<CactusBlock>(x, y, z);

									if (Random.Range(0, 10) < 3 && Block.cactus.CanStay(this, x, y + 1, z)) SetBlock<CactusBlock>(x, y + 1, z);
								}

							if (block[x, y, z].id == Block.ID.Air && Block.flower.CanStay(this, x, y, z))
								if (Random.Range(0, 10) == 0)
									SetBlock<FlowerBlock>(x, y, z, (FlowerBlock.Color)Random.Range(0, 2));
						}

						if ((x >= woolBlockStart.x && x <= woolBlockStart.x + 5) && (y > 0 && y <= 5) && (z >= woolBlockStart.z && z <= woolBlockStart.z + 5)) SetBlock<WoolBlock>(x, y, z, (WoolBlock.Color)Random.Range(0, 9));
					}
				}
			}

			shouldUpdateSunlight = true;
			isBuilding = false;
		}

		public void FixedUpdate()
		{
			RaycastHit hit = default(RaycastHit);
			if (Game.localPlayer) Physics.Linecast(Camera.main.transform.position, Game.localPlayer.transform.position + new Vector3(0, 1.3f, 0), out hit, ~Player.ignoreLayers);

			if (shouldUpdateSunlight)
			{
				isUpdatingSunlight = true;
				new Thread(UpdateSunlight).Start();
				shouldUpdateSunlight = false;
			}

			// Show and hide slices obstructing the view.
			for (int y = 0; y < slices.Count; y++)
			{
				if (hit.collider != null)
				{
					if (slices[y].height > (Game.localPlayer.blockPosition.y + 1) + Game.visibleSliceOffset)
					{
						if (slices[y].meshRenderer.shadowCastingMode == ShadowCastingMode.On)
						{
							slices[y].meshRenderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;

							for (int c = 0; c < slices[y].meshRenderer.transform.childCount; c++)
								slices[y].meshRenderer.transform.GetChild(c).gameObject.layer = LayerMask.NameToLayer("Hidden");

							slices[(Game.localPlayer.blockPosition.y + 1) + Game.visibleSliceOffset].isDirty = true;
						}
					}
					else
					{
						if (slices[y].meshRenderer.shadowCastingMode == ShadowCastingMode.ShadowsOnly)
						{
							slices[y].meshRenderer.shadowCastingMode = ShadowCastingMode.On;

							for (int c = 0; c < slices[y].meshRenderer.transform.childCount; c++)
								slices[y].meshRenderer.transform.GetChild(c).gameObject.layer = 0;

							slices[(Game.localPlayer.blockPosition.y + 1) + Game.visibleSliceOffset].isDirty = true;
						}
					}
				}
				else
				{
					if (slices[y].meshRenderer.shadowCastingMode == ShadowCastingMode.ShadowsOnly)
					{
						slices[y].meshRenderer.shadowCastingMode = ShadowCastingMode.On;

						for (int c = 0; c < slices[y].meshRenderer.transform.childCount; c++)
							slices[y].meshRenderer.transform.GetChild(c).gameObject.layer = 0;
					}
				}

				if (Game.localPlayer)
				{
					if (Game.localPlayer.blockPosition != null ? (slices[y].height < Game.localPlayer.blockPosition.y + 10 && slices[y].height > Game.localPlayer.blockPosition.y - 10) : false)
					{
						if (!isUpdatingSunlight)
						{
							for (int x = 0; x < sizeX; x++)
								for (int z = 0; z < sizeZ; z++)
									block[x, y, z].FixedUpdate(this, x, y, z);
						}

						if (slices[y].isDirty)
						{
							slices[y].Build(this);
							slices[y].isDirty = false;
						}
					}
				}
			}

			for (int i = 0; i < itemEntities.Count; i++) itemEntities[i].FixedUpdate();
		}

		private void UpdateSunlight()
		{
			for (int i = 0; i < 10; i++)
			{
				bool anyChange = false;

				for (int x = 0; x < sizeX; x++)
				{
					for (int z = 0; z < sizeZ; z++)
					{
						bool isLit = true;

						for (int y = sizeY - 1; y >= 0; y--)
						{
							bool isChanged = false;

							if (block[x, y, z].canOcclude)
							{
								if (block[x, y, z].sunLight != 0f)
								{
									block[x, y, z].sunLight = 0f;
									isChanged = true;
								}

								if (block[x, y + 1, z].sunLight != 0 && slices[y + 1].isDirty) slices[y].isDirty = true;

								isLit = false;
							}
							else
							{
								if (isLit)
								{
									if (block[x, y, z].sunLight != 1f)
									{
										block[x, y, z].sunLight = 1f;
										isChanged = true;
									}
								}
								else
								{
									float newSunlight = 0.9f;

									Block north = null, south = null, west = null, east = null, up = null, down = null;

									if (z > 0) south = block[x, y, z - 1];
									if (z < sizeX - 1) north = block[x, y, z + 1];

									if (x > 0) west = block[x - 1, y, z];
									if (x < sizeX - 1) east = block[x + 1, y, z];

									if (y > 0) down = block[x, y - 1, z];
									if (y < sizeY - 1) up = block[x, y + 1, z];

									if (north != null && south != null && west != null && east != null)
									{
										if (north.sunLight < 1f && south.sunLight < 1f && west.sunLight < 1f && east.sunLight < 1f)
										{
											float[] sunLights = new float[6];

											if (north.sunLight < 1f && !north.canOcclude) sunLights[0] = north.sunLight;
											if (south.sunLight < 1f && !south.canOcclude) sunLights[1] = south.sunLight;
											if (west.sunLight < 1f && !west.canOcclude) sunLights[2] = west.sunLight;
											if (east.sunLight < 1f && !east.canOcclude) sunLights[3] = east.sunLight;
											if (up != null && up.sunLight < 1f && !up.canOcclude) sunLights[4] = up.sunLight;
											if (down != null && down.sunLight < 1f && !down.canOcclude) sunLights[5] = down.sunLight;

											float max = Mathf.Max(sunLights);
											newSunlight = Mathf.Round((max = Mathf.Clamp(max - 0.1f, 0, 1)) * 10f) / 10f;
										}
									}

									if (block[x, y, z].sunLight != newSunlight)
									{
										block[x, y, z].sunLight = newSunlight;
										isChanged = true;
									}
								}
							}

							if (isChanged)
							{
								slices[y].isDirty = true;
								anyChange = true;
							}
						}
					}
				}

				if (!anyChange) break;
			}

			isUpdatingSunlight = false;
		}

		public T SetBlock<T>(int x, int y, int z, params object[] options) where T : Block
		{
			T newBlock = (T)System.Activator.CreateInstance(typeof(T), options);

			if (block[x, y, z].gameObject != null) Object.Destroy(block[x, y, z].gameObject);
			block[x, y, z] = newBlock;

			slices[y].isDirty = true;
			shouldUpdateSunlight = true;

			return (T)block[x, y, z];
		}

		public Block SetBlock(Block block, int x, int y, int z)
		{
			if (this.block[x, y, z].gameObject != null) Object.Destroy(this.block[x, y, z].gameObject);
			this.block[x, y, z] = block;

			slices[y].isDirty = true;
			shouldUpdateSunlight = true;

			return this.block[x, y, z];
		}

		public bool IsPlayerLocatedAt(int x, int y, int z)
		{
			for (int i = 0; i < Game.players.Count; i++)
				if (Game.players[i].blockPosition == new BlockPosition(x, y, z)) return true;

			return false;
		}
	}
}
