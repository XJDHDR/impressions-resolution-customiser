// This code is part of the Impressions Resolution Customiser project
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace zeus_and_poseidon
{
	/// <summary>
	/// Code used to resize the background images and maps that the game uses to fit new resolutions.
	/// </summary>
	class Zeus_ResizeImages
	{
		/// <summary>
		/// Root function that calls the other functions in this class.
		/// </summary>
		/// <param name="ZeusExeLocation">String that contains the location of Zeus.exe</param>
		/// <param name="ResWidth">The width value of the resolution inputted into the UI.</param>
		/// <param name="ResHeight">The height value of the resolution inputted into the UI.</param>
		/// <param name="PatchedFilesFolder">String which specifies the location of the "patched_files" folder.</param>
		internal static void CreateResizedImages(string ZeusExeLocation, ushort ResWidth, ushort ResHeight, string PatchedFilesFolder)
		{
			string _zeusDataFolderLocation = ZeusExeLocation.Remove(ZeusExeLocation.Length - 8) + @"DATA\";
			_fillImageArrays(out string[] _imagesToResize);
			_resizeCentredImages(_zeusDataFolderLocation, _imagesToResize, ResWidth, ResHeight, PatchedFilesFolder);
		}

		/// <summary>
		/// Root function that calls the other functions in this class.
		/// </summary>
		/// <param name="_zeusDataFolderLocation">String that contains the location of Zeus' "DATA" folder.</param>
		/// <param name="_centredImages">String array that contains a list of the images that need to be resized.</param>
		/// <param name="_resWidth">The width value of the resolution inputted into the UI.</param>
		/// <param name="_resHeight">The height value of the resolution inputted into the UI.</param>
		/// <param name="_patchedFilesFolder">String which specifies the location of the "patched_files" folder.</param>
		private static void _resizeCentredImages(string _zeusDataFolderLocation, string[] _centredImages, ushort _resWidth, ushort _resHeight, string _patchedFilesFolder)
		{
			ImageCodecInfo _jpegCodecInfo = null;
			ImageCodecInfo[] _allImageCodecs = ImageCodecInfo.GetImageEncoders();
			for (int _j = 0; _j < _allImageCodecs.Length; _j++)
			{
				if (_allImageCodecs[_j].MimeType == "image/jpeg")
				{
					_jpegCodecInfo = _allImageCodecs[_j];
					break;
				}
			}

			if (_jpegCodecInfo != null)
			{
				EncoderParameters _encoderParameters = new EncoderParameters(1);
				_encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 85L);

				Directory.CreateDirectory(_patchedFilesFolder + @"\DATA");

				//Parallel.For(0, _centredImages.Length, _i =>
				for (byte _i = 0; _i < _centredImages.Length; _i++)
				{
					if (File.Exists(_zeusDataFolderLocation + _centredImages[_i]))
					{
						using (Bitmap _oldImage = new Bitmap(_zeusDataFolderLocation + _centredImages[_i]))
						{
							bool _currentImageIsMap = false;
							ushort _newImageWidth;
							ushort _newImageHeight;
							if (Regex.IsMatch(_centredImages[_i], "_Map(OfGreece)*[0-9][0-9].jpg$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase))
							{
								// Map images need to have the new images sized to fit the game's viewport.
								_currentImageIsMap = true;
								_newImageWidth = (ushort)(_resWidth - 180);
								_newImageHeight = (ushort)(_resHeight - 30);
							}
							else
							{
								_newImageWidth = _resWidth;
								_newImageHeight = _resHeight;
							}

							using (Bitmap _newImage = new Bitmap(_newImageWidth, _newImageHeight))
							{
								using (Graphics _newImageGraphics = Graphics.FromImage(_newImage))
								{
									// Note to self: Don't simplify the DrawImage calls. Specifying the old image's width and height is required
									// to work around a bug/quirk where the image's DPI is scaled to the screen's before insertion:
									// https://stackoverflow.com/a/41189062
									if (_currentImageIsMap)
									{
										// This file is one of the maps. Must be placed in the top-left corner of the new image.
										// Also create the background colour that will be used to fill the spaces not taken by the original image.
										_newImageGraphics.Clear(Color.FromArgb(255, 35, 88, 120));

										_newImageGraphics.DrawImage(_oldImage, 0, 0, _oldImage.Width, _oldImage.Height);
									}
									else
									{
										// A non-map image. Must be placed in the centre of the new image with a black background.
										_newImageGraphics.Clear(Color.Black);

										_newImageGraphics.DrawImage(_oldImage, (_newImageWidth - _oldImage.Width) / 2, 
											(_newImageHeight - _oldImage.Height) / 2, _oldImage.Width, _oldImage.Height);
									}

									_newImage.Save(_patchedFilesFolder + @"\DATA\" + _centredImages[_i], _jpegCodecInfo, _encoderParameters);
								}
							}
						}
					}
					else
					{
						MessageBox.Show("Could not find the image located at: " + _zeusDataFolderLocation + _centredImages[_i]);
					}
				//});
				}
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
		/// <param name="_imagesToResize">String array that contains a list of the images that need to be resized.</param>
		private static void _fillImageArrays(out string[] _imagesToResize)
		{
			_imagesToResize = new string[]
			{
				"Poseidon_FE_MainMenu.jpg",
				"Poseidon_FE_Registry.jpg",
				"scoreb.jpg",
				"Zeus_Defeat.jpg",
				"Zeus_FE_CampaignSelection.jpg",
				"Zeus_FE_ChooseGame.jpg",
				"Zeus_FE_MissionIntroduction.jpg",
				"Zeus_FE_Registry.jpg",
				"Zeus_FE_tutorials.jpg",
				"Zeus_Victory.jpg",
				"Poseidon_map01.jpg",
				"Poseidon_map02.jpg",
				"Poseidon_map03.jpg",
				"Poseidon_map04.jpg",
				"Zeus_MapOfGreece01.jpg",
				"Zeus_MapOfGreece02.jpg",
				"Zeus_MapOfGreece03.jpg",
				"Zeus_MapOfGreece04.jpg",
				"Zeus_MapOfGreece05.jpg",
				"Zeus_MapOfGreece06.jpg",
				"Zeus_MapOfGreece07.jpg",
				"Zeus_MapOfGreece08.jpg",
				"Zeus_MapOfGreece09.jpg",
				"Zeus_MapOfGreece10.jpg"
			};
		}
	}
}
