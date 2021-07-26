using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace zeus_and_poseidon
{
	class Zeus_ResizeImages
	{
		internal static void CreateResizedImages(string ZeusExeLocation, ushort _resWidth, ushort _resHeight, string PatchedFilesFolder)
		{
			string _zeusDataFolderLocation = ZeusExeLocation.Remove(8) + @"DATA\";
			_fillImageArrays(_zeusDataFolderLocation, out string[] _imagesToResize);
			_resizeCentredImages(_zeusDataFolderLocation, _imagesToResize, _resWidth, _resHeight, PatchedFilesFolder);
		}

		private static void _resizeCentredImages(string _zeusDataFolderLocation, string[] _centredImages, ushort _resWidth, ushort _resHeight, string _patchedFilesFolder)
		{
			Parallel.For(0, _centredImages.Length, _i =>
			{
				using (Bitmap _oldImage = new Bitmap(_centredImages[_i]))
				{
					int _newWidth = _resWidth > _oldImage.Width ? _resWidth : _oldImage.Width;
					int _newHeight = _resHeight > _oldImage.Height ? _resHeight : _oldImage.Height;

					using (Bitmap _newImage = new Bitmap(_newWidth, _newHeight))
					{
						using (Graphics _newImageGraphics = Graphics.FromImage(_newImage))
						{
							if (Regex.IsMatch(_centredImages[_i], "_Map(OfGreece)*[0-9][0-9].jpg$", RegexOptions.CultureInvariant & RegexOptions.IgnoreCase))
							{
								// This file is one of the maps. Must be placed in the top-left corner of the new image.

							}
							else
							{
								// A non-map image. Must be placed in the centre of the new image.
								using (Brush _fillColour = new SolidBrush(Color.Black))
								{
									_newImageGraphics.FillRectangle(_fillColour, 0, 0, _newWidth, _newHeight);
								}
								int _oldImgTopLeftCornerPosX = _resWidth > _oldImage.Width ? (_resWidth - _oldImage.Width) / 2 : 0;
								int _oldImgTopLeftCornerPosY = _resHeight > _oldImage.Height ? (_resHeight - _oldImage.Height) / 2 : 0;

								_newImageGraphics.DrawImage(_oldImage, new Rectangle(_oldImgTopLeftCornerPosX, _oldImgTopLeftCornerPosY, 
									_oldImage.Width, _oldImage.Height));
							}
						}
						EncoderParameters _encoderParameters = new EncoderParameters(1);
						_encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 85);
						_newImage.Save(_patchedFilesFolder + _centredImages[_i], ImageCodecInfo, _encoderParameters);
					}
				}
			});
		}

		private static void _fillImageArrays(string _zeusDataFolderLocation, out string[] _imagesToResize)
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
