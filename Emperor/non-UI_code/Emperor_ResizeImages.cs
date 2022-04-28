﻿// This file is or was originally a part of the Impressions Resolution Customiser project, which can be found here:
// ReSharper disable RedundantUsingDirective
// https://github.com/XJDHDR/impressions-resolution-customiser
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//

using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Encoder = System.Drawing.Imaging.Encoder;

namespace Emperor.non_UI_code
{
	/// <summary>
	/// Code used to resize the background images and maps that the game uses to fit new resolutions.
	/// </summary>
	internal class EmperorResizeImages
	{
		/// <summary>
		/// Whether the "Stretch menu images to fit window" checkbox is selected or not.
		/// </summary>
		private readonly bool stretchImages;

		/// <summary>
		/// The width value of the resolution inputted into the UI.
		/// </summary>
		private readonly ushort resWidth;

		/// <summary>
		/// The height value of the resolution inputted into the UI.
		/// </summary>
		private readonly ushort resHeight;

		/// <summary>
		/// The width of the city viewport calculated by the resolution editing code.
		/// </summary>
		private readonly ushort viewportWidth;

		/// <summary>
		/// The height of the city viewport calculated by the resolution editing code.
		/// </summary>
		private readonly ushort viewportHeight;

		/// <summary>
		/// String that contains the location of Emperor's "DATA" folder.
		/// </summary>
		private readonly string emperorDataFolderLocation;

		/// <summary>
		/// Whether the "Stretch menu images to fit window" checkbox is selected or not.
		/// </summary>
		private readonly string emperorMapTemplateImageLocation;

		/// <summary>
		/// String which specifies the location of the "patched_files" folder.
		/// </summary>
		private readonly string patchedImagesFolder;

		/// <summary>
		/// String array that contains a list of the images that need to be resized.
		/// </summary>
		private readonly string[] imagesToResize;

		/// <summary>
		/// Info about the JPEG codec that will be used to create the new resized images.
		/// </summary>
		private readonly ImageCodecInfo jpegCodecInfo;

		/// <summary>
		/// The parameters that will be used to create the resized images. Set to Quality and 85%.
		/// </summary>
		private readonly EncoderParameters encoderParameters;

		/// <summary>
		/// ConcurrentBag used to hold all of the missing image error messages generated by the threads.
		/// </summary>
		private readonly ConcurrentBag<string> allErrorMessages;


		/// <summary>
		/// Used to supply all of the info required to resize the game's images.
		/// </summary>
		/// <param name="ResWidth">The width value of the resolution inputted into the UI.</param>
		/// <param name="ResHeight">The height value of the resolution inputted into the UI.</param>
		/// <param name="ViewportWidth">The width of the city viewport calculated by the resolution editing code.</param>
		/// <param name="ViewportHeight">The height of the city viewport calculated by the resolution editing code.</param>
		/// <param name="StretchImages">Whether the "Stretch menu images to fit window" checkbox is selected or not.</param>
		/// <param name="EmperorExeDirectory">String that contains the location of Emperor's EXE.</param>
		/// <param name="PatchedFilesFolder">String which specifies the location of the "patched_files" folder.</param>
		/// <param name="JpegCodecFound">Returns True if a JPEG encoder is present on the PC. False if there is none available.</param>
		internal EmperorResizeImages(ushort ResWidth, ushort ResHeight, ushort ViewportWidth, ushort ViewportHeight,
			bool StretchImages, string EmperorExeDirectory, string PatchedFilesFolder, out bool JpegCodecFound)
		{
			resWidth = ResWidth;
			resHeight = ResHeight;
			viewportWidth = ViewportWidth;
			viewportHeight = ViewportHeight;
			stretchImages = StretchImages;
			emperorDataFolderLocation = $"{EmperorExeDirectory}/DATA";
			patchedImagesFolder = $"{PatchedFilesFolder}/DATA";

			emperorMapTemplateImageLocation = $"{AppDomain.CurrentDomain.BaseDirectory}ocean_pattern/ocean_pattern.png";
			if (!File.Exists(emperorMapTemplateImageLocation))
			{
				MessageBox.Show("Could not find \"ocean_pattern\\ocean_pattern.png\". A fallback colour will " +
				                "be used to create the maps instead. Please check if the ocean_pattern image was successfully " +
				                "extracted from this program's downloaded archive and is in the correct place.");
			}

			imagesToResize = new[]
			{
				"China_Defeat.jpg",
				"China_editor_splash.jpg",
				"China_FE_CampaignSelection.jpg",
				"China_FE_ChooseGame.jpg",
				"China_FE_HighScores.jpg",
				"China_FE_MainMenu.jpg",
				"China_FE_MissionIntroduction.jpg",
				"China_FE_OpenPlay.jpg",
				"China_FE_Registry.jpg",
				"China_FE_tutorials.jpg",
				"China_Load1.jpg",
				"China_Load2.jpg",
				"China_Load3.jpg",
				"China_Load4.jpg",
				"China_Load5.jpg",
				"China_MapOfChina01.jpg",
				"China_MapOfChina02.jpg",
				"China_MapOfChina03.jpg",
				"China_MapOfChina04.jpg",
				"China_Victory.jpg",
				"scoreb.jpg"
			};

			jpegCodecInfo = null;
			ImageCodecInfo[] allImageCodecs = ImageCodecInfo.GetImageEncoders();
			for (int i = 0; i < allImageCodecs.Length; i++)
			{
				if (allImageCodecs[i].MimeType == "image/jpeg")
				{
					jpegCodecInfo = allImageCodecs[i];
					break;
				}
			}
			if (jpegCodecInfo == null)
			{
				MessageBox.Show("Could not resize any of the game's images because the program could not find a " +
				                "JPEG Encoder available on your PC. Since Windows comes with such a codec by default, this " +
				                "could indicate a serious problem with your PC that can only be fixed by reinstalling Windows.");
				encoderParameters = null;
				allErrorMessages = null;
				JpegCodecFound = false;
				return;
			}

			encoderParameters = new EncoderParameters(1);
			encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 85L);

			allErrorMessages = new ConcurrentBag<string>();

			JpegCodecFound = true;
		}


		/// <summary>
		/// Resizes the maps and other images used by the game to the correct size.
		/// </summary>
		internal void _CreateResizedImages()
		{
			Directory.CreateDirectory(patchedImagesFolder);

			#if !DEBUG
			unsafe
			{
				byte* classQn = stackalloc byte[] { 69, 109, 112, 101, 114, 111, 114, 46, 110, 111, 110, 95, 85, 73, 95, 99, 111, 100, 101, 46, 67, 114, 99,
					51, 50, 46, 77, 97, 105, 110, 69, 120, 101, 73, 110, 116, 101, 103, 114, 105, 116, 121 };
				byte* methodQn = stackalloc byte[] { 95, 67, 104, 101, 99, 107 };
				Type type = Type.GetType(classQn->ToString());
				if (type != null)
				{
					try
					{
						MethodInfo methodInfo = type.GetMethod(methodQn->ToString(), BindingFlags.DeclaredOnly |
							BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Static);
						if (methodInfo != null)
							methodInfo.Invoke(null, new object[] { });

						else
						{
							Application.Current.Shutdown();
							return;
						}
					}
					catch (Exception)
					{
						Application.Current.Shutdown();
						return;
					}
				}
				else
				{
					Application.Current.Shutdown();
					return;
				}
			}
			#endif

			Parallel.For(0, imagesToResize.Length, resizeIndividualImage);

			// If the ConcurrentBag has entries in it, create a message listing all of these missing images.
			if (allErrorMessages.IsEmpty == false)
			{
				StringBuilder messageText = new StringBuilder();
				messageText.Append("Could not find the following images :\n\n");

				while (allErrorMessages.IsEmpty == false)
				{
					if (allErrorMessages.TryTake(out string extractedText))
					{
						messageText.Append(extractedText);
					}
				}

				MessageBox.Show(messageText.ToString());
			}
		}


		private void resizeIndividualImage(int I)
		{
			string imageToResizeFullPath = $"{emperorDataFolderLocation}/{imagesToResize[I]}";
			if (!File.Exists(imageToResizeFullPath))
			{
				// Drop the missing image's name into a ConcurrentBag to put in an error message later.
				allErrorMessages.Add($"{imageToResizeFullPath}\n");
				return;
			}

			using (Bitmap oldImage = new Bitmap(imageToResizeFullPath))
			{
				bool currentImageIsMap = false;
				ushort newImageWidth;
				ushort newImageHeight;
				if (Regex.IsMatch(imagesToResize[I], "^China_MapOfChina0[1-4].jpg$",
					    RegexOptions.CultureInvariant | RegexOptions.IgnoreCase))
				{
					// Map images need to have the new images sized to fit the game's viewport.
					currentImageIsMap = true;
					newImageWidth = viewportWidth;
					newImageHeight = viewportHeight;
				}
				else
				{
					newImageWidth = resWidth;
					newImageHeight = resHeight;
				}

				using (Bitmap newImage = new Bitmap(newImageWidth, newImageHeight))
				{
					using (Graphics newImageGraphics = Graphics.FromImage(newImage))
					{
						// Note to self: Don't simplify the DrawImage calls. Specifying the old image's width and height is required
						// to work around a quirk where the image's DPI is scaled to the screen's before insertion:
						// https://stackoverflow.com/a/41189062
						if (currentImageIsMap)
						{
							// This file is one of the maps. Must be placed in the top-left corner of the new image.
							// Also create the background colour that will be used to fill the spaces not taken by the original image.
							if (File.Exists(emperorMapTemplateImageLocation))
							{
								// Note to self: Don't try and make this more efficient. Only one thread can access a bitmap at a time, even if just reading.
								using (Bitmap mapBackgroundImage = new Bitmap(emperorMapTemplateImageLocation))
								{
									newImageGraphics.DrawImage(mapBackgroundImage, 0, 0, mapBackgroundImage.Width, mapBackgroundImage.Height);
								}
							}
							else
							{
								newImageGraphics.Clear(Color.FromArgb(255, 35, 88, 120));
							}

							newImageGraphics.DrawImage(oldImage, 0, 0, oldImage.Width, oldImage.Height);
						}
						else
						{
							if (!stretchImages)
							{
								// A non-map image. Must be placed in the centre of the new image with a black background.
								newImageGraphics.Clear(Color.Black);

								newImageGraphics.DrawImage(oldImage, (newImageWidth - oldImage.Width) / 2,
									(newImageHeight - oldImage.Height) / 2, oldImage.Width, oldImage.Height);
							}
							else
							{
								// A non-map image. Stretch it to fit the new window's dimensions.
								newImageGraphics.DrawImage(oldImage, 0, 0, newImageWidth, newImageHeight);
							}
						}

						newImage.Save($"{patchedImagesFolder}/{imagesToResize[I]}", jpegCodecInfo, encoderParameters);
					}
				}
			}
		}
	}
}
