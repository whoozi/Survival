using System.Xml.Serialization;
using System.IO;

namespace Survival.Utility
{
	public static class Serializer
	{
		public static void Serialize(object obj, string path)
		{
			if (!Directory.Exists(path)) Directory.CreateDirectory(path.Remove(path.Length - Path.GetFileName(path).Length));

			XmlSerializer serializer = new XmlSerializer(obj.GetType());
			using (StreamWriter stream = new StreamWriter(path, false, System.Text.Encoding.UTF8)) serializer.Serialize(stream, obj);
		}

		public static T Deserialize<T>(string path)
		{
			if (!Directory.Exists(path.Remove(path.Length - Path.GetFileName(path).Length))) return default(T);

			XmlSerializer serializer = new XmlSerializer(typeof(T));
			using (FileStream stream = new FileStream(path, FileMode.Open)) return (T)serializer.Deserialize(stream);
		}
	}
}
