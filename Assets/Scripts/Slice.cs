using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering;

using Survival.Blocks;

namespace Survival
{
	public class Slice
	{
		public static float pixelSize = 1.0f / Game.blockMaterial.mainTexture.width;

		public int x, z, height;
		public bool isDirty = true;

		public MeshFilter meshFilter;
		public MeshRenderer meshRenderer;

		public Slice(int x, int z, int height)
		{
			this.x = x;
			this.z = z;
			this.height = height;

			GameObject gameObject = new GameObject("Slice " + x + " " + z + " " + height);
			gameObject.hideFlags = HideFlags.HideInHierarchy;
			meshFilter = gameObject.AddComponent<MeshFilter>();
			meshRenderer = gameObject.AddComponent<MeshRenderer>();
			meshRenderer.material = Game.blockMaterial;
		}

		public void Build(World world)
		{
			List<Vector3> vertices = new List<Vector3>();
			List<Color> colors = new List<Color>();
			List<Vector3> normals = new List<Vector3>();
			List<Vector2> uvs = new List<Vector2>();
			List<int> triangles = new List<int>();

			Vector3[] face = new Vector3[4]
			{
				new Vector3(-0.5f, 0.5f, -0.5f),
				new Vector3(0.5f, 0.5f, -0.5f),
				new Vector3(-0.5f, -0.5f, -0.5f),
				new Vector3(0.5f, -0.5f, -0.5f)
			};

			for (int x = this.x * 64; x < (this.x * 64) + 64; x++)
			{
				for (int z = this.z * 64; z < (this.z * 64) + 64; z++)
				{
					if (world.block[x, height, z].hasCollider)
					{
						if (world.block[x, height, z].gameObject == null)
						{
							world.block[x, height, z].gameObject = new GameObject(world.block[x, height, z].name);
							world.block[x, height, z].gameObject.transform.position = new Vector3(x, height, z);
							world.block[x, height, z].gameObject.transform.SetParent(meshFilter.transform);

							world.block[x, height, z].gameObject.AddComponent<BoxCollider>();
						}
					}

					if (world.block[x, height, z].id != Block.ID.Air)
					{
						int vertexCount = vertices.Count;
						Quaternion q = Quaternion.identity;
						Color c = Color.white;
						Vector2 ul = new Vector2(0f, 32f), lr = new Vector2(16f, 16f);
						float sunlight = 0f;

						switch (world.block[x, height, z].type)
						{
							case Block.Type.Cube:
								switch (world.block[x, height, z].id)
								{
									case Block.ID.Grass:
									case Block.ID.Dirt:
									case Block.ID.Sand:
									case Block.ID.Wool:
										for (int i = 0; i < 4; i++)
										{
											sunlight = 0f;

											switch (i)
											{
												case 0: // front
													if (z > 0)
													{
														if (!world.block[x, height, z - 1].canOcclude) sunlight = world.block[x, height, z - 1].sunLight;
														else continue;
													}

													q = Quaternion.identity;
													c = new Color(sunlight, sunlight, sunlight, 1);
													break;

												case 1: // left side
													if (x > 0)
													{
														if (!world.block[x - 1, height, z].canOcclude) sunlight = world.block[x - 1, height, z].sunLight;
														else continue;
													}

													q = Quaternion.Euler(0, 90, 0);
													c = new Color(sunlight, sunlight, sunlight, 1);
													break;

												case 2: // right side
													if (x < world.sizeX - 1)
													{
														if (!world.block[x + 1, height, z].canOcclude) sunlight = world.block[x + 1, height, z].sunLight;
														else continue;
													}

													q = Quaternion.Euler(0, -90, 0);
													c = new Color(sunlight, sunlight, sunlight, 1);
													break;

												case 3: // top
													if (height < world.sizeY - 1)
													{
														if (world.slices[height + 1].meshRenderer.shadowCastingMode == ShadowCastingMode.On || !world.block[x, height + 1, z].canOcclude)
															if (!world.block[x, height + 1, z].canOcclude) sunlight = world.block[x, height + 1, z].sunLight;
															else continue;
													}

													q = Quaternion.Euler(90, 0, 0);
													c = new Color(sunlight, sunlight, sunlight, 1);
													break;
											} // End of face switch.

											switch (world.block[x, height, z].id)
											{
												case Block.ID.Grass:
													switch (i)
													{
														case 0: // front
															ul = new Vector2(16f, 48f);
															lr = new Vector2(32f, 32f);
															break;

														case 1: // left side
															ul = new Vector2(16f, 48f);
															lr = new Vector2(32f, 32f);
															break;

														case 2: // right side
															ul = new Vector2(16f, 48f);
															lr = new Vector2(32f, 32f);
															break;

														case 3: // top
															ul = new Vector2(0f, 48f);
															lr = new Vector2(16f, 32f);
															break;
													}
													break;

												case Block.ID.Dirt:
													ul = new Vector2(0f, 32f);
													lr = new Vector2(16f, 16f);
													break;

												case Block.ID.Sand:
													ul = new Vector2(16f, 32f);
													lr = new Vector2(32f, 16f);
													break;

												case Block.ID.Wool:
													switch (((WoolBlock)world.block[x, height, z]).color)
													{
														case WoolBlock.Color.White:
															ul = new Vector2(0f, 16f);
															lr = new Vector2(16f, 0f);
															break;

														case WoolBlock.Color.Yellow:
															ul = new Vector2(16f, 16f);
															lr = new Vector2(32f, 0f);
															break;

														case WoolBlock.Color.Green:
															ul = new Vector2(112, 32);
															lr = new Vector2(128, 16);
															break;

														case WoolBlock.Color.Orange:
															ul = new Vector2(32f, 16f);
															lr = new Vector2(48f, 0f);
															break;

														case WoolBlock.Color.Red:
															ul = new Vector2(48f, 16f);
															lr = new Vector2(64f, 0f);
															break;

														case WoolBlock.Color.Pink:
															ul = new Vector2(64f, 16f);
															lr = new Vector2(80f, 0f);
															break;

														case WoolBlock.Color.Purple:
															ul = new Vector2(80f, 16f);
															lr = new Vector2(96f, 0f);
															break;

														case WoolBlock.Color.Blue:
															ul = new Vector2(96f, 16f);
															lr = new Vector2(112f, 0f);
															break;

														case WoolBlock.Color.Black:
															ul = new Vector2(112f, 16f);
															lr = new Vector2(128f, 0f);
															break;
													}
													break;

												default:
													continue;
											} // End of texture switch.

											vertices.Add((q * face[0]) + new Vector3(x, height, z));
											vertices.Add((q * face[1]) + new Vector3(x, height, z));
											vertices.Add((q * face[2]) + new Vector3(x, height, z));
											vertices.Add((q * face[3]) + new Vector3(x, height, z));

											colors.Add(c);
											colors.Add(c);
											colors.Add(c);
											colors.Add(c);

											normals.Add(q * new Vector3(0, 0, -1));
											normals.Add(q * new Vector3(0, 0, -1));
											normals.Add(q * new Vector3(0, 0, -1));
											normals.Add(q * new Vector3(0, 0, -1));

											ul *= pixelSize;
											lr *= pixelSize;

											uvs.Add(new Vector2(ul.x, ul.y));
											uvs.Add(new Vector2(lr.x, ul.y));
											uvs.Add(new Vector2(ul.x, lr.y));
											uvs.Add(new Vector2(lr.x, lr.y));

											triangles.Add(vertexCount + 0);
											triangles.Add(vertexCount + 1);
											triangles.Add(vertexCount + 2);
											triangles.Add(vertexCount + 1);
											triangles.Add(vertexCount + 3);
											triangles.Add(vertexCount + 2);

											vertexCount = vertices.Count;
										}
										break;
								}
								break; // End of Cube generation.

							case Block.Type.Plane:
								vertices.Add((q * new Vector3(-0.5f, 0.5f, 0)) + new Vector3(x, height, z));
								vertices.Add((q * new Vector3(0.5f, 0.5f, 0)) + new Vector3(x, height, z));
								vertices.Add((q * new Vector3(-0.5f, -0.5f, 0)) + new Vector3(x, height, z));
								vertices.Add((q * new Vector3(0.5f, -0.5f, 0)) + new Vector3(x, height, z));

								c = new Color(world.block[x, height, z].sunLight, world.block[x, height, z].sunLight, world.block[x, height, z].sunLight, 1);

								colors.Add(c);
								colors.Add(c);
								colors.Add(c);
								colors.Add(c);

								normals.Add(q * new Vector3(0, 0, -1));
								normals.Add(q * new Vector3(0, 0, -1));
								normals.Add(q * new Vector3(0, 0, -1));
								normals.Add(q * new Vector3(0, 0, -1));

								switch (world.block[x, height, z].id)
								{
									case Block.ID.Cactus:
										ul = new Vector2(32f, 32f);
										lr = new Vector2(48f, 16f);
										break;

									case Block.ID.Flower:
										switch (((FlowerBlock)world.block[x, height, z]).color)
										{
											case FlowerBlock.Color.Red:
												ul = new Vector2(48f, 32f);
												lr = new Vector2(64f, 16f);
												break;

											case FlowerBlock.Color.White:
												ul = new Vector2(64f, 32f);
												lr = new Vector2(80f, 16f);
												break;
										}
										break;

									case Block.ID.Torch:
										ul = new Vector2(32f, 48f);
										lr = new Vector2(48f, 32f);
										break;
								} // End of texture switch.

								ul *= pixelSize;
								lr *= pixelSize;

								uvs.Add(new Vector2(ul.x, ul.y));
								uvs.Add(new Vector2(lr.x, ul.y));
								uvs.Add(new Vector2(ul.x, lr.y));
								uvs.Add(new Vector2(lr.x, lr.y));

								triangles.Add(vertexCount + 0);
								triangles.Add(vertexCount + 1);
								triangles.Add(vertexCount + 2);
								triangles.Add(vertexCount + 1);
								triangles.Add(vertexCount + 3);
								triangles.Add(vertexCount + 2);
								break; // End of plane generation.
						}
					}
				}
			}

			meshFilter.mesh.Clear();
			meshFilter.mesh.MarkDynamic();

			meshFilter.mesh.vertices = vertices.ToArray();
			meshFilter.mesh.colors = colors.ToArray();
			meshFilter.mesh.normals = normals.ToArray();
			meshFilter.mesh.uv = uvs.ToArray();
			meshFilter.mesh.triangles = triangles.ToArray();
		}
	}
}
