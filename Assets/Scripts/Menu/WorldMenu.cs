using System;
using System.IO;

using UnityEngine;
using UnityEngine.UI;

using Survival.Utility;

namespace Survival.Menu
{
	public class WorldMenu : MonoBehaviour
	{
		private string[] files;

		public VerticalLayoutGroup content;
		public Button createWorldButton;
		public Text worldNameText, worldSizeText;
		public InputField worldNameField;
		public GameObject worldInfoPrefab;

		private void Start()
		{
			createWorldButton.onClick.AddListener(delegate { Game.instance.HostServer(new Save.InfoSave("Alpha 1.0.0", worldNameText.text, UnityEngine.Random.seed, 64, 128, 64, World.Difficulty.Normal, DateTime.Now.ToString())); });
		}

		private void FixedUpdate()
		{
			createWorldButton.interactable = (worldNameText.text != "" && !HasSameName(worldNameText.text));
		}

		private bool HasSameName(string name)
		{
			for (int i = 0; i < content.transform.childCount; i++)
				if (name == content.transform.GetChild(i).GetChild(1).GetComponent<Text>().text) return true;

			return false;
		}

		public void RefreshList()
		{
			for (int i = 0; i < content.transform.childCount; i++) Destroy(content.transform.GetChild(i).gameObject);

			if (!Directory.Exists(Game.savePath + "worlds")) Directory.CreateDirectory(Game.savePath + "worlds");
			
			files = Directory.GetFiles(Game.savePath + "worlds/", "info.dat", SearchOption.AllDirectories);

			for (int i = 0; i < files.Length; i++)
			{
				Save.InfoSave worldInfoSave = Serializer.Deserialize<Save.InfoSave>(files[i]);

				Transform worldInfo = Instantiate(worldInfoPrefab).GetComponent<Transform>();
				worldInfo.SetParent(content.transform, false);
				worldInfo.GetChild(1).GetComponent<Text>().text = worldInfoSave.name;
				worldInfo.GetChild(2).GetComponent<Text>().text = worldInfoSave.sizeX.ToString() + " x " + worldInfoSave.sizeY.ToString() + " x " + worldInfoSave.sizeZ.ToString();
				worldInfo.GetChild(3).GetComponent<Text>().text = worldInfoSave.lastPlayed;
				worldInfo.GetComponent<Button>().onClick.AddListener(delegate { Game.instance.HostServer(worldInfoSave); });
			}
		}
	}
}
