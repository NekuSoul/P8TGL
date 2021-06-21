using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using P8TGL.GameListXml;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace P8TGL
{
	class Program
	{
		private static void Main(string[] args)
		{
			if (args.Length is 0)
			{
				Console.WriteLine("No parameter given. Include path to gamelist.xml");
				return;
			}

			var path = args[0];
			var gamelistPath = Path.Combine(path, "gamelist.xml");
			var cartPath = Path.Combine(path, "carts");
			var imagePath = Path.Combine(path, "images");

			Directory.CreateDirectory(imagePath);

			// Read gamelist.xml
			var serializer = new XmlSerializer(typeof(GameList));
			GameList list;

			if (File.Exists(gamelistPath))
			{
				using var reader = XmlReader.Create(gamelistPath);
				list = (GameList)serializer.Deserialize(reader);
			}
			else
			{
				list = new GameList { Games = new List<Game>() };
			}

			if (list is null)
				throw new NullReferenceException();

			var existingGameNames = list.Games
				.Where(game => game.Path.EndsWith(".p8.png", StringComparison.InvariantCultureIgnoreCase))
				.Select(g => Path.GetFileName(g.Path))
				.ToArray();

			// Add new entries
			foreach (var file in Directory.EnumerateFiles(cartPath))
			{
				if (!file.EndsWith(".p8.png", StringComparison.InvariantCultureIgnoreCase))
					continue;

				var gameName = Path.GetFileName(file);

				if (existingGameNames.Contains(gameName))
					continue;

				list.Games.Add(new Game { Path = $@"./carts/{gameName}" });
			}

			// Update existing entries
			foreach (var game in list.Games.Where(game => game.Path.EndsWith(".p8.png", StringComparison.InvariantCultureIgnoreCase)))
			{
				GenerateThumbnail(Path.Combine(path, game.Path), Path.Combine(imagePath, Path.GetFileName(game.Path)!));
				game.Name = GetPrettyCartName(Path.GetFileName(game.Path));
				game.Image = $@"./images/{Path.GetFileName(game.Path)}";
			}

			// Write gamelist.xml
			var settings = new XmlWriterSettings
			{
				Indent = true,
				NewLineOnAttributes = true
			};

			using (var writer = XmlWriter.Create(gamelistPath, settings))
			{
				serializer.Serialize(writer, list);
			}

			Console.WriteLine("Done!");
		}

		private static string GetPrettyCartName(string path)
		{
			var fileName = Path.GetFileName(path);
			var baseName = Regex.Match(fileName, @"(.*?)(\-\d)?\.").Groups[1].Value;
			var prettyName = baseName.Replace("_", " ").ToUpperInvariant();
			return prettyName;
		}

		private static void GenerateThumbnail(string inputPath, string outputPath)
		{
			if (File.Exists(outputPath))
				return;

			var image = Image.Load(File.OpenRead(inputPath));
			var thumbnail = image.Clone(context => context.Crop(new Rectangle(16, 24, 128, 128)));
			thumbnail.Save(File.Create(outputPath), PngFormat.Instance);
		}
	}
}
