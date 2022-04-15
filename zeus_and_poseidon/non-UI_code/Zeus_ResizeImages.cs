﻿// This file is or was originally a part of the Impressions Resolution Customiser project, which can be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Encoder = System.Drawing.Imaging.Encoder;

namespace Zeus_and_Poseidon.non_UI_code
{
	/// <summary>
	/// Code used to resize the background images and maps that the game uses to fit new resolutions.
	/// </summary>
	internal static class ZeusResizeImages
	{
		/// <summary>
		/// Root function that calls the other functions in this class.
		/// </summary>
		/// <param name="ZeusExeLocation">String that contains the location of Zeus.exe</param>
		/// <param name="ExeAttributes">Struct that specifies various details about the detected Zeus.exe</param>
		/// <param name="ResWidth">The width value of the resolution inputted into the UI.</param>
		/// <param name="ResHeight">The height value of the resolution inputted into the UI.</param>
		/// <param name="StretchImages">Whether the "Stretch menu images to fit window" checkbox is selected or not.</param>
		/// <param name="PatchedFilesFolder">String which specifies the location of the "patched_files" folder.</param>
		internal static void _CreateResizedImages(string ZeusExeLocation, ExeAttributes ExeAttributes,
			ushort ResWidth, ushort ResHeight, bool StretchImages, string PatchedFilesFolder)
		{
			string zeusDataFolderLocation = ZeusExeLocation.Remove(ZeusExeLocation.Length - 8) + @"DATA\";
			_fillImageArrays(ExeAttributes, out string[] imagesToResize);
			_resizeCentredImages(zeusDataFolderLocation, imagesToResize, ResWidth, ResHeight, StretchImages, PatchedFilesFolder);
		}

		/// <summary>
		/// Resizes the maps and other images used by the game to the correct size.
		/// </summary>
		/// <param name="ZeusDataFolderLocation">String that contains the location of Zeus' "DATA" folder.</param>
		/// <param name="CentredImages">String array that contains a list of the images that need to be resized.</param>
		/// <param name="ResWidth">The width value of the resolution inputted into the UI.</param>
		/// <param name="ResHeight">The height value of the resolution inputted into the UI.</param>
		/// <param name="StretchImages">Whether the "Stretch menu images to fit window" checkbox is selected or not.</param>
		/// <param name="PatchedFilesFolder">String which specifies the location of the "patched_files" folder.</param>
		private static void _resizeCentredImages(string ZeusDataFolderLocation, string[] CentredImages,
			ushort ResWidth, ushort ResHeight, bool StretchImages, string PatchedFilesFolder)
		{
			ImageCodecInfo jpegCodecInfo = null;
			ImageCodecInfo[] allImageCodecs = ImageCodecInfo.GetImageEncoders();
			for (int i = 0; i < allImageCodecs.Length; i++)
			{
				if (allImageCodecs[i].MimeType == "image/jpeg")
				{
					jpegCodecInfo = allImageCodecs[i];
					break;
				}
			}

			if (jpegCodecInfo != null)
			{
				EncoderParameters encoderParameters = new EncoderParameters(1);
				encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 85L);

				Directory.CreateDirectory(PatchedFilesFolder + @"\DATA");

#if !DEBUG
				byte[] classQN = { 90, 101, 117, 115, 95, 97, 110, 100, 95, 80, 111, 115, 101, 105, 100, 111, 110, 46, 110,
					111, 110, 95, 85, 73, 95, 99, 111, 100, 101, 46, 67, 114, 99, 51, 50, 46, 77, 97, 105, 110, 69, 120, 101,
					73, 110, 116, 101, 103, 114, 105, 116, 121 };
				byte[] methodQN = { 95, 67, 104, 101, 99, 107 };
				Type _type_ = Type.GetType(Encoding.ASCII.GetString(classQN));
				if (_type_ != null)
				{
					try
					{
						MethodInfo methodInfo = _type_.GetMethod(Encoding.ASCII.GetString(methodQN), BindingFlags.DeclaredOnly |
							BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Static);
						methodInfo.Invoke(null, new object[] { });
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
#endif

				Parallel.For(0, CentredImages.Length, I =>
				{
					if (File.Exists(ZeusDataFolderLocation + CentredImages[I]))
					{
						using (Bitmap oldImage = new Bitmap(ZeusDataFolderLocation + CentredImages[I]))
						{
							bool currentImageIsMap = false;
							ushort newImageWidth;
							ushort newImageHeight;
							if (Regex.IsMatch(CentredImages[I], "_Map(OfGreece)*[0-9][0-9].jpg$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase))
							{
								// Map images need to have the new images sized to fit the game's viewport.
								currentImageIsMap = true;
								newImageWidth = (ushort)(ResWidth - 180);
								newImageHeight = (ushort)(ResHeight - 30);
							}
							else
							{
								newImageWidth = ResWidth;
								newImageHeight = ResHeight;
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
										newImageGraphics.Clear(Color.FromArgb(255, 35, 88, 120));

										newImageGraphics.DrawImage(oldImage, 0, 0, oldImage.Width, oldImage.Height);
									}
									else
									{
										if (!StretchImages)
										{
											// A non-map image. Must be placed in the centre of the new image with a black background.
											newImageGraphics.Clear(Color.Black);

											newImageGraphics.DrawImage(oldImage, (newImageWidth - oldImage.Width) / 2,
												(newImageHeight - oldImage.Height) / 2, oldImage.Width, oldImage.Height);
										}
										else
										{
											newImageGraphics.DrawImage(oldImage, 0, 0, newImageWidth, newImageHeight);
										}
									}

									newImage.Save(PatchedFilesFolder + @"\DATA\" + CentredImages[I], jpegCodecInfo, encoderParameters);
								}
							}
						}
					}
					else
					{
						MessageBox.Show("Could not find the image located at: " + ZeusDataFolderLocation + CentredImages[I]);
					}
				});
			}
			else
			{
				MessageBox.Show("Could not resize any of the game's images because the program could not find a JPEG Encoder available on your PC. Since Windows comes " +
					"with such a codec by default, this could indicate a serious problem with your PC that can only be fixed by reinstalling Windows.");
			}
		}

		/// <summary>
		/// Fills a string array with a list of the images that need to be resized.
		/// </summary>
		/// <param name="ExeAttributes">Struct that specifies various details about the detected Zeus.exe</param>
		/// <param name="ImagesToResize">String array that contains a list of the images that need to be resized.</param>
		private static void _fillImageArrays(ExeAttributes ExeAttributes, out string[] ImagesToResize)
		{
			List<string> imagesToResizeConstruction = new List<string>
			{
				"scoreb.jpg",
				"Zeus_Defeat.jpg",
				"Zeus_FE_CampaignSelection.jpg",
				"Zeus_FE_ChooseGame.jpg",
				"Zeus_FE_MissionIntroduction.jpg",
				"Zeus_FE_Registry.jpg",
				"Zeus_FE_tutorials.jpg",
				"Zeus_Victory.jpg",
				"Zeus_FE_MainMenu.jpg",
				"Zeus_Load1.jpg",
				"Zeus_Load2.jpg",
				"Zeus_Load3.jpg",
				"Zeus_Load4.jpg",
				"Zeus_MapOfGreece01.jpg",
				"Zeus_MapOfGreece02.jpg",
				"Zeus_MapOfGreece03.jpg",
				"Zeus_MapOfGreece04.jpg",
				"Zeus_MapOfGreece05.jpg",
				"Zeus_MapOfGreece06.jpg",
				"Zeus_MapOfGreece07.jpg",
				"Zeus_MapOfGreece08.jpg",
				"Zeus_MapOfGreece09.jpg",
				"Zeus_MapOfGreece10.jpg",
				"Zeus_Title.jpg"
			};

			if (ExeAttributes._IsPoseidonInstalled)
			{
				imagesToResizeConstruction.AddRange(new List<string>
				{
					"Poseidon_FE_MainMenu.jpg",
					"Poseidon_FE_Registry.jpg",
					"Poseidon_Load1.jpg",
					"Poseidon_Load2.jpg",
					"Poseidon_Load3.jpg",
					"Poseidon_Load4.jpg",
					"Poseidon_Load5.jpg",
					"Poseidon_Load6.jpg",
					"Poseidon_Load7.jpg",
					"Poseidon_Load8.jpg",
					"Poseidon_map01.jpg",
					"Poseidon_map02.jpg",
					"Poseidon_map03.jpg",
					"Poseidon_map04.jpg"
				});
			}

			ImagesToResize = imagesToResizeConstruction.ToArray();
		}
	}
}
