using System.Xml.Serialization;

namespace P8TGL.GameListXml
{
	[XmlRoot(ElementName="folder")]
	public class Folder {
		[XmlElement(ElementName="path")]
		public string Path { get; set; }
		[XmlElement(ElementName="name")]
		public string Name { get; set; }
		[XmlElement(ElementName="hidden")]
		public string Hidden { get; set; }
	}
}
