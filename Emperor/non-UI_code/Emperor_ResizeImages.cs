// This file is or was originally a part of the Impressions Resolution Customiser project, which can be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace Emperor.non_UI_code
{
	/// <summary>
	/// Code used to resize the background images and maps that the game uses to fit new resolutions.
	/// </summary>
	internal static class EmperorResizeImages
	{
		/// <summary>
		/// Root function that calls the other functions in this class.
		/// </summary>
		/// <param name="_EmperorExeLocation_">String that contains the location of Emperor.exe</param>
		/// <param name="_ResWidth_">The width value of the resolution inputted into the UI.</param>
		/// <param name="_ResHeight_">The height value of the resolution inputted into the UI.</param>
		/// <param name="_ViewportWidth_">The width of the city viewport calculated by the resolution editing code.</param>
		/// <param name="_ViewportHeight_">The height of the city viewport calculated by the resolution editing code.</param>
		/// <param name="_StretchImages_">Whether the "Stretch menu images to fit window" checkbox is selected or not.</param>
		/// <param name="_PatchedFilesFolder_">String which specifies the location of the "patched_files" folder.</param>
		internal static void _CreateResizedImages(string _EmperorExeLocation_, ushort _ResWidth_, ushort _ResHeight_,
			ushort _ViewportWidth_, ushort _ViewportHeight_, bool _StretchImages_, string _PatchedFilesFolder_)
		{
			string _emperorDataFilesFolderLocation_ = _EmperorExeLocation_.Remove(_EmperorExeLocation_.Length - 11) + @"DATA\";
			_fillImageArrays(out string[] _imagesToResize_);
			_resizeCentredImages(_emperorDataFilesFolderLocation_, _imagesToResize_, _ResWidth_, _ResHeight_, _ViewportWidth_, _ViewportHeight_,
				_StretchImages_, _PatchedFilesFolder_);
		}

		/// <summary>
		/// Resizes the maps and other images used by the game to the correct size.
		/// </summary>
		/// <param name="_EmperorDataFolderLocation_">String that contains the location of Emperor's "DATA" folder.</param>
		/// <param name="_CentredImages_">String array that contains a list of the images that need to be resized.</param>
		/// <param name="_ResWidth_">The width value of the resolution inputted into the UI.</param>
		/// <param name="_ResHeight_">The height value of the resolution inputted into the UI.</param>
		/// <param name="_ViewportWidth_">The width of the city viewport calculated by the resolution editing code.</param>
		/// <param name="_ViewportHeight_">The height of the city viewport calculated by the resolution editing code.</param>
		/// <param name="_StretchImages_">Whether the "Stretch menu images to fit window" checkbox is selected or not.</param>
		/// <param name="_PatchedFilesFolder_">String which specifies the location of the "patched_files" folder.</param>
		private static void _resizeCentredImages(string _EmperorDataFolderLocation_, string[] _CentredImages_, ushort _ResWidth_, ushort _ResHeight_,
			ushort _ViewportWidth_, ushort _ViewportHeight_, bool _StretchImages_, string _PatchedFilesFolder_)
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
				string _emperorMapTemplateImageLocation_ = AppDomain.CurrentDomain.BaseDirectory + @"ocean_pattern\ocean_pattern.png";

				if (!File.Exists(_emperorMapTemplateImageLocation_))
				{
					MessageBox.Show("Could not find \"ocean_pattern\\ocean_pattern.png\". A fallback colour will be used to create the maps instead. " +
						"Please check if the ocean_pattern image was successfully extracted from this program's downloaded archive and is in the correct place.");
				}

				Parallel.For(0, _CentredImages_.Length, _I_ =>
				{
					if (File.Exists(_EmperorDataFolderLocation_ + _CentredImages_[_I_]))
					{
						using (Bitmap _oldImage_ = new Bitmap(_EmperorDataFolderLocation_ + _CentredImages_[_I_]))
						{
							bool _currentImageIsMap_ = false;
							ushort _newImageWidth_;
							ushort _newImageHeight_;
							if (Regex.IsMatch(_CentredImages_[_I_], "^China_MapOfChina0[1-4].jpg$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase))
							{
								// Map images need to have the new images sized to fit the game's viewport.
								_currentImageIsMap_ = true;
								_newImageWidth_ = _ViewportWidth_;
								_newImageHeight_ = _ViewportHeight_;
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
										if (File.Exists(_emperorMapTemplateImageLocation_))
										{
											// Note to self: Don't try and make this more efficient. Only one thread can access a bitmap at a time, even if just reading.
											using (Bitmap _mapBackgroundImage_ = new Bitmap(_emperorMapTemplateImageLocation_))
											{
												_newImageGraphics_.DrawImage(_mapBackgroundImage_, 0, 0, _mapBackgroundImage_.Width, _mapBackgroundImage_.Height);
											}
										}
										else
										{
											_newImageGraphics_.Clear(Color.FromArgb(255, 35, 88, 120));
										}

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
											// A non-map image. Stretch it to fit the new window's dimensions.
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
						MessageBox.Show("Could not find the image located at: " + _EmperorDataFolderLocation_ + _CentredImages_[_I_]);
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
		/// <param name="_ImagesToResize_">String array that contains a list of the images that need to be resized.</param>
		private static void _fillImageArrays(out string[] _ImagesToResize_)
		{
			_ImagesToResize_ = new[]
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
		}
	}
}
