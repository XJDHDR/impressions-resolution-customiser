// This file is or was originally a part of the Impressions Resolution Customiser project, which can be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

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
		/// <param name="_ZeusExeLocation_">String that contains the location of Zeus.exe</param>
		/// <param name="_ExeAttributes_">Struct that specifies various details about the detected Zeus.exe</param>
		/// <param name="_ResWidth_">The width value of the resolution inputted into the UI.</param>
		/// <param name="_ResHeight_">The height value of the resolution inputted into the UI.</param>
		/// <param name="_PatchedFilesFolder_">String which specifies the location of the "patched_files" folder.</param>
		internal static void _CreateResizedImages(string _ZeusExeLocation_, ExeAttributes _ExeAttributes_,
			ushort _ResWidth_, ushort _ResHeight_, bool _StretchImages_, string _PatchedFilesFolder_)
		{
			string _zeusDataFolderLocation_ = _ZeusExeLocation_.Remove(_ZeusExeLocation_.Length - 8) + @"DATA\";
			_fillImageArrays(_ExeAttributes_, out string[] _imagesToResize_);
			_resizeCentredImages(_zeusDataFolderLocation_, _imagesToResize_, _ResWidth_, _ResHeight_, _StretchImages_, _PatchedFilesFolder_);
		}

		/// <summary>
		/// Resizes the maps and other images used by the game to the correct size.
		/// </summary>
		/// <param name="_ZeusDataFolderLocation_">String that contains the location of Zeus' "DATA" folder.</param>
		/// <param name="_CentredImages_">String array that contains a list of the images that need to be resized.</param>
		/// <param name="_ResWidth_">The width value of the resolution inputted into the UI.</param>
		/// <param name="_ResHeight_">The height value of the resolution inputted into the UI.</param>
		/// <param name="_PatchedFilesFolder_">String which specifies the location of the "patched_files" folder.</param>
		private static void _resizeCentredImages(string _ZeusDataFolderLocation_, string[] _CentredImages_,
			ushort _ResWidth_, ushort _ResHeight_, bool _StretchImages_, string _PatchedFilesFolder_)
		{
			ImageCodecInfo _jpegCodecInfo_ = null;
			ImageCodecInfo[] _allImageCodecs_ = ImageCodecInfo.GetImageEncoders();
			for (int _j_ = 0; _j_ < _allImageCodecs_.Length; _j_++)
			{
				if (_allImageCodecs_[_j_].MimeType == "image/jpeg")
				{
					_jpegCodecInfo_ = _allImageCodecs_[_j_];
					break;
				}
			}

			if (_jpegCodecInfo_ != null)
			{
				EncoderParameters _encoderParameters_ = new EncoderParameters(1);
				_encoderParameters_.Param[0] = new EncoderParameter(Encoder.Quality, 85L);

				Directory.CreateDirectory(_PatchedFilesFolder_ + @"\DATA");

				Parallel.For(0, _CentredImages_.Length, _I_ =>
				{
					if (File.Exists(_ZeusDataFolderLocation_ + _CentredImages_[_I_]))
					{
						using (Bitmap _oldImage_ = new Bitmap(_ZeusDataFolderLocation_ + _CentredImages_[_I_]))
						{
							bool _currentImageIsMap_ = false;
							ushort _newImageWidth_;
							ushort _newImageHeight_;
							if (Regex.IsMatch(_CentredImages_[_I_], "_Map(OfGreece)*[0-9][0-9].jpg$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase))
							{
								// Map images need to have the new images sized to fit the game's viewport.
								_currentImageIsMap_ = true;
								_newImageWidth_ = (ushort)(_ResWidth_ - 180);
								_newImageHeight_ = (ushort)(_ResHeight_ - 30);
							}
							else
							{
								_newImageWidth_ = _ResWidth_;
								_newImageHeight_ = _ResHeight_;
							}

							using (Bitmap _newImage_ = new Bitmap(_newImageWidth_, _newImageHeight_))
							{
								using (Graphics _newImageGraphics_ = Graphics.FromImage(_newImage_))
								{
									// Note to self: Don't simplify the DrawImage calls. Specifying the old image's width and height is required
									// to work around a quirk where the image's DPI is scaled to the screen's before insertion:
									// https://stackoverflow.com/a/41189062
									if (_currentImageIsMap_)
									{
										// This file is one of the maps. Must be placed in the top-left corner of the new image.
										// Also create the background colour that will be used to fill the spaces not taken by the original image.
										_newImageGraphics_.Clear(Color.FromArgb(255, 35, 88, 120));

										_newImageGraphics_.DrawImage(_oldImage_, 0, 0, _oldImage_.Width, _oldImage_.Height);
									}
									else
									{
										if (!_StretchImages_)
										{
											// A non-map image. Must be placed in the centre of the new image with a black background.
											_newImageGraphics_.Clear(Color.Black);

											_newImageGraphics_.DrawImage(_oldImage_, (_newImageWidth_ - _oldImage_.Width) / 2,
												(_newImageHeight_ - _oldImage_.Height) / 2, _oldImage_.Width, _oldImage_.Height);
										}
										else
										{
											_newImageGraphics_.DrawImage(_oldImage_, 0, 0, _newImageWidth_, _newImageHeight_);
										}
									}

									_newImage_.Save(_PatchedFilesFolder_ + @"\DATA\" + _CentredImages_[_I_], _jpegCodecInfo_, _encoderParameters_);
								}
							}
						}
					}
					else
					{
						MessageBox.Show("Could not find the image located at: " + _ZeusDataFolderLocation_ + _CentredImages_[_I_]);
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
		/// <param name="_ExeAttributes_">Struct that specifies various details about the detected Zeus.exe</param>
		/// <param name="_ImagesToResize_">String array that contains a list of the images that need to be resized.</param>
		private static void _fillImageArrays(ExeAttributes _ExeAttributes_, out string[] _ImagesToResize_)
		{
			List<string> _imagesToResizeConstruction_ = new List<string>
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

			if (_ExeAttributes_._IsPoseidonInstalled)
			{
				_imagesToResizeConstruction_.AddRange(new List<string>
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

			_ImagesToResize_ = _imagesToResizeConstruction_.ToArray();
		}
	}
}
