using System.Collections.Generic;
using System.Xml.Serialization;

namespace P8TGL.GameListXml
{
	[XmlRoot(ElementName = "gameList")]
	public class GameList
	{
		[XmlElement(ElementName = "game")]
		public List<Game> Games { get; set; }

		[XmlElement(ElementName="folder")]
		public List<Folder> Folders { get; set; }
	}
}
