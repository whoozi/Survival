using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

using Survival.Menu;
using Survival.Mobs;
using Survival.Utility;

namespace Survival
{
	public class Game : Singleton<Game>
	{
		[Serializable]
		public class Controls
		{
			public KeyCode moveUp = KeyCode.W;
			public KeyCode moveDown = KeyCode.S;
			public KeyCode moveLeft = KeyCode.A;
			public KeyCode moveRight = KeyCode.D;
			public KeyCode jump = KeyCode.Space;
			public KeyCode dropItem = KeyCode.G;
		}

		[Serializable]
		public class Graphics
		{
			public bool verticalSync = true;

			public void Update()
			{
				QualitySettings.vSyncCount = Convert.ToInt16(verticalSync);
			}
		}

		public static string savePath;
		public static Player localPlayer = null;
		public static List<Player> players = new List<Player>();
		public static GameObject itemEntityPrefab = null;
		public static Material blockMaterial = null, spriteMaterial = null;
		public static Sprite[] itemSprites = null;
		public static RaycastHit mouseHit;
		public static int visibleSliceOffset = 0;

		public World world;
		public Controls controls;
		public Graphics graphics;
		public MainMenu mainMenu;
		public WorldMenu worldMenu;
		public DebugMenu debugMenu;
		public HUDMenu hudMenu;

		private void Start()
		{
			savePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/My Games/Survival/";
			if (itemEntityPrefab == null) itemEntityPrefab = Resources.Load<GameObject>("Prefabs/Item Entity");
			if (blockMaterial == null) blockMaterial = Resources.Load<Material>("Materials/Block");
			if (spriteMaterial == null) spriteMaterial = Resources.Load<Material>("Materials/Sprite");
			if (itemSprites == null) itemSprites = Resources.LoadAll<Sprite>("Sprites/Items");

			SetActiveMenu(mainMenu.gameObject);
			graphics.Update();
		}

		private void Update()
		{
			if (!EventSystem.current.currentSelectedGameObject)
			{
				if (Input.GetKeyDown(KeyCode.V))
				{
					graphics.verticalSync = !graphics.verticalSync;
					graphics.Update();
				}

				if (Input.GetKeyDown(KeyCode.UpArrow)) visibleSliceOffset = Mathf.Clamp(visibleSliceOffset + 1, -1, 3);
				if (Input.GetKeyDown(KeyCode.DownArrow)) visibleSliceOffset = Mathf.Clamp(visibleSliceOffset - 1, -1, 3);
			}
		}

		private void FixedUpdate()
		{
			if (world != null && !world.isBuilding)
				world.FixedUpdate();
		}

		private void OnApplicationQuit()
		{
			// Save World
			if (world != null)
				Save.WorldSave.Serialize(world, savePath);
		}

		public void HostServer(Save.InfoSave worldInfoSave)
		{
			if (File.Exists(savePath + "worlds/" + worldInfoSave.name + "/" + "world.dat") && File.Exists(savePath + "worlds/" + worldInfoSave.name + "/" + "info.dat"))
			{
				// Load World
				world = Save.WorldSave.Deserialize(worldInfoSave, savePath);
			}
			else
			{
				// Generate World
				world = new World(worldInfoSave.name, worldInfoSave.seed, worldInfoSave.sizeX, worldInfoSave.sizeY, worldInfoSave.sizeZ);
				world.Build();
			}

			SetActiveMenu(hudMenu.gameObject);
			NetworkManager.singleton.StartHost();
		}

		public void JoinServer(string address, int port)
		{
			SetActiveMenu(hudMenu.gameObject);
			NetworkManager.singleton.networkAddress = address;
			NetworkManager.singleton.networkPort = port;
			NetworkManager.singleton.StartClient();
		}

		public void SetActiveMenu(GameObject menu)
		{
			mainMenu.gameObject.SetActive(false);
			worldMenu.gameObject.SetActive(false);
			hudMenu.gameObject.SetActive(false);

			menu.gameObject.SetActive(true);
		}

		public IEnumerator CaptureScreen()
		{
			Canvas canvas = GameObject.Find("Canvas").GetComponent<Canvas>();

			yield return null;
			canvas.enabled = false;

			yield return new WaitForEndOfFrame();

			Application.CaptureScreenshot(savePath + "worlds/" + world.name + "/thumbnail.png");

			canvas.enabled = true;
		}

		public void Quit()
		{
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
			Application.Quit();
#endif
		}
	}
}
