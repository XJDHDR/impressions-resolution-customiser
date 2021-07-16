

using Soft160.Data.Cryptography;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace common_and_non_UI_code
{
	public static class Zeus_HexEditing
	{
		public static void HexEditZeusExe(string ZeusExeContFolder, ushort ResWidth, ushort ResHeight)
		{
			if (!File.Exists(ZeusExeContFolder + "/Zeus.exe"))
			{
				MessageBox.Show("Zeus.exe does not exist in the resource_files folder. Please ensure ");
			}
			else
			{
				byte[] _zeusExeData = File.ReadAllBytes(ZeusExeContFolder + "/Zeus.exe");

				uint _fileHash = CRC.Crc32(_zeusExeData, 0, _zeusExeData.Length);

				switch (_fileHash)
				{
					case 0x90B9CF84:		// English GOG version
						_hexEditZeusExe(ResWidth, ResHeight, GameVersion.GOG_English, ref _zeusExeData);
						break;

					default:				// Unrecognised EXE
						MessageBox.Show("Zeus.exe was not recognised. Only the English GOG version of this game is supported.");
						return;
				}
			}
		}

		private static void _hexEditZeusExe(ushort _resWidth, ushort _resHeight, GameVersion _gameVersion, ref byte[] _zeusExeData)
		{
			_fillHexEditOffsetTable(_gameVersion, out HexEditingOffsetTable _hexEditingOffsetTable);

			byte[] _resWidthBytes = BitConverter.GetBytes(_resWidth);
			byte[] _resHeightBytes = BitConverter.GetBytes(_resHeight);

			// Set the resolution to the desired amount
			_zeusExeData[_hexEditingOffsetTable._resWidth]		= _resWidthBytes[0];
			_zeusExeData[_hexEditingOffsetTable._resWidth + 1]  = _resWidthBytes[1];
			_zeusExeData[_hexEditingOffsetTable._resHeight]		= _resHeightBytes[0];
			_zeusExeData[_hexEditingOffsetTable._resHeight + 1] = _resHeightBytes[1];

			// Fix the game's opening screen
			_zeusExeData[_hexEditingOffsetTable._openingScreenWidth]		= _resWidthBytes[0];
			_zeusExeData[_hexEditingOffsetTable._openingScreenWidth + 1]	= _resWidthBytes[1];
			_zeusExeData[_hexEditingOffsetTable._openingScreenHeight]		= _resHeightBytes[0];
			_zeusExeData[_hexEditingOffsetTable._openingScreenHeight + 1]	= _resHeightBytes[1];


		}

		private static bool _fillHexEditOffsetTable(GameVersion _gameVersion, out HexEditingOffsetTable _hexEditingOffsetTable)
		{
			switch ((byte)_gameVersion)
			{
				case 1:			// English GOG version
					_hexEditingOffsetTable = new HexEditingOffsetTable(0x10BA29, 0x10BA2E, 0x1051FA, 0x105212, 0x18EF7C, 0x19EBFB, 0x18E2F7);
					return true;

				default:		// Unrecognised EXE
					_hexEditingOffsetTable = new HexEditingOffsetTable(0, 0, 0, 0, 0, 0, 0);
					return false;
			}
		}

		private enum GameVersion
		{
			GOG_English = 1
		}

		private readonly struct HexEditingOffsetTable
		{
			internal readonly uint _resWidth;
			internal readonly uint _resHeight;
			internal readonly uint _openingScreenWidth;
			internal readonly uint _openingScreenHeight;

			internal readonly uint _fixMenuTextWidth;
			internal readonly uint _fixMenuBackgroundWidth;
			internal readonly uint _fixSidebarBottomWidth;

			public HexEditingOffsetTable(uint resWidth, uint resHeight, uint openingScreenWidth, uint openingScreenHeight, uint fixMenuTextWidth, 
				uint fixMenuBackgroundWidth, uint fixSidebarBottomWidth)
			{
				_resWidth = resWidth;
				_resHeight = resHeight;
				_openingScreenWidth = openingScreenWidth;
				_openingScreenHeight = openingScreenHeight;
				_fixMenuTextWidth = fixMenuTextWidth;
				_fixMenuBackgroundWidth = fixMenuBackgroundWidth;
				_fixSidebarBottomWidth = fixSidebarBottomWidth;
			}
		}
	}
}
