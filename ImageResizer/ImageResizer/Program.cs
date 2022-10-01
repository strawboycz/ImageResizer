using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace ImageResizer
{
	internal class Program
	{
		static void Main(string[] args)
		{
			string Path = args[0];
			if (CheckForDirectoryAndCommand(args)) return;
			if (CheckIfFileExists(Path)) return;
			if (CheckIfCommandIsValid(args)) return;

			if (args[1] == "-r" || args[1] == "--resize")
			{
				if (CheckForValidWitdth(args)) return;
				string width = args[2];
				if (FormatWidth(ref width)) return;
				resizeImage(Path,width);
			}
		}

		private static bool FormatWidth(ref string width)
		{
			if (width.StartsWith("-x="))
				width = width.Substring(3);
			else if (width.StartsWith("--width="))
				width = width.Substring(8);
			else
			{
				Console.WriteLine("Width parameter is provided in an invalid format.");
				return true;
			}

			return false;
		}

		private static bool CheckForValidWitdth(string[] args)
		{
			if (args.Length < 3)
			{
				Console.WriteLine("A valid width needs to be provided.");
				return true;
			}

			return false;
		}

		private static bool CheckIfCommandIsValid(string[] args)
		{
			if (!(args[1].StartsWith("-") || args[1].StartsWith("--")))
			{
				Console.WriteLine("A valid command needs to be provided.");
				return true;
			}

			return false;
		}

		private static bool CheckIfFileExists(string Path)
		{
			if (!System.IO.Directory.Exists(Path))
			{
				Console.WriteLine($"\"{Path}\" either does not exist or is not a directory.");
				return true;
			}

			return false;
		}

		private static bool CheckForDirectoryAndCommand(string[] args)
		{
			if (args.Length < 2)
			{
				Console.WriteLine("A directory and a command need to be provided.");
				return true;
			}

			return false;
		}

		static void resizeImage(string Path,string width)
		{

			DirectoryInfo dir = new DirectoryInfo(Path);
			List<FileInfo> fileInfos = ReturnFileInfos(dir);
			if (CheckIfAnyImagesExist(fileInfos)) return;

			
			List<string> newFileNames = new List<string>();
			
			fileInfos.ForEach(info => newFileNames.Add($"{Path}\\{info.Name.Remove(info.Name.LastIndexOf('.'),4)}.{width}.jpg"));
			fileInfos.Where(info => newFileNames.Contains(info.FullName)).ToList().ForEach(info => info.Delete());

			fileInfos = dir.GetFiles("*.jpg").ToList();
			foreach (var info in fileInfos)
			{
				ResizeAndSaveImage(width, info, newFileNames, fileInfos);
			}
			


				
				

				
			

		}

		private static void ResizeAndSaveImage(string width, FileInfo info, List<string> newFileNames, List<FileInfo> fileInfos)
		{
			Image image = Image.Load(info.FullName);
			int height = 0;
			image.Mutate(x => x.Resize(Convert.ToInt32(width), height));
			image.Save(newFileNames[fileInfos.IndexOf(info)], new JpegEncoder());
		}

		private static bool CheckIfAnyImagesExist(List<FileInfo> fileInfos)
		{
			if (fileInfos.Count == 0)
			{
				Console.WriteLine("No images found in the provided directory.");
				return true;
			}

			return false;
		}

		private static List<FileInfo> ReturnFileInfos(DirectoryInfo dir)
		{
			return dir.GetFiles("*.jpg").ToList();
		}
	}
}
