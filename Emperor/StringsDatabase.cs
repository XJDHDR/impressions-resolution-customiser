// This file is or was originally a part of the Impressions Resolution Customiser project, which can be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//

using System.Globalization;
using Emperor.Models;
using Emperor.ViewModels;

namespace Emperor
{
	internal static class StringsDatabase
	{
		// ==== Properties ====
		internal static string _GenerateExeClickWidthTryParseFailed { get; private set; }
		internal static string _GenerateExeClickHeightTryParseFailed { get; private set; }
		internal static string _GenerateExeClickWidthLessThanMinimum { get; private set; }
		internal static string _GenerateExeClickHeightLessThanMinimum { get; private set; }
		internal static string _GenerateExeClickWidthMoreThanMaximum { get; private set; }
		internal static string _GenerateExeClickHeightMoreThanMaximum { get; private set; }
		internal static string _GenerateExeClickWidthNotDivisibleBy4 { get; private set; }

		internal static string _SelectExeClickTitle { get; private set; }

		internal static string[] _HelpMeClickMessage { get; private set; }

		internal static string[] _ApplyWindowFixHelpClickMessage { get; private set; }

		internal static string[] _PatchEngTextHelpClickMessage { get; private set; }

		internal static string[] _ResizeImagesHelpClickMessage { get; private set; }

		internal static string[] _StretchImagesHelpClickMessage { get; private set; }

		internal static string[] _IncreaseSpriteLimitsHelpClickMessage { get; private set; }

		internal static string _MainExeIntegrityCouldNotFindExe { get; private set; }
		internal static string _MainExeIntegrityDataCorruptionDetected { get; private set; }

		internal static string _EmperorEngTextEditFileNotFound { get; private set; }
		internal static string _EmperorEngTextEditExceptionOccurred { get; private set; }

		internal static string[] _EmperorExeAttributesExeKnownModified { get; private set; }
		internal static string _EmperorExeAttributesExeKnownModifiedMessageTitle { get; private set; }
		internal static string[] _EmperorExeAttributesExeKnownModifiedAndOutdated { get; private set; }
		internal static string _EmperorExeAttributesExeKnownModifiedAndOutdatedMessageTitle { get; private set; }
		internal static string[] _EmperorExeAttributesExeKnownOutdated { get; private set; }
		internal static string _EmperorExeAttributesExeKnownOutdatedMessageTitle { get; private set; }
		internal static string[] _EmperorExeAttributesExeNotRecognised { get; private set; }
		internal static string _EmperorExeAttributesExeNotRecognisedMessageTitle { get; private set; }

		internal static string _EmperorMakeChangesExeNotFound { get; private set; }
		internal static string _EmperorMakeChangesPathTooLongException { get; private set; }
		internal static string _EmperorMakeChangesIoException { get; private set; }
		internal static string _EmperorMakeChangesUnauthorizedAccessException { get; private set; }
		internal static string _EmperorMakeChangesExeSuccessfullyPatched { get; private set; }
		internal static string _EmperorMakeChangesUnhandledException { get; private set; }
		internal static string _EmperorMakeChangesUnhandledExceptionTitle { get; private set; }

		internal static string _EmperorResizeImagesOceanPatternNotFound { get; private set; }
		internal static string _EmperorResizeImagesJpegEncoderNotFound { get; private set; }
		internal static string _EmperorResizeImagesCouldNotFindImageMessageStart { get; private set; }


		// ==== Constructor ====
		static StringsDatabase() =>
			changeLanguage(CultureInfo.CurrentUICulture.ThreeLetterISOLanguageName);


		// ==== Public methods ====
		[ExecuteFromViewModelConstructor]
		// ReSharper disable once UnusedMember.Local
		private static void registerForChangeLanguageEvent() =>
			MainWindowVm.Instance.LanguageChangedEventHandler += changeLanguage;


		// ==== Private methods ====
		private static void changeLanguage(string ThreeLetterIsoLanguageCode)
		{
			switch (ThreeLetterIsoLanguageCode)
			{
				case "eng":
					// English
					setLanguageStringsToEnglishText();
					break;

			/*	case "fra":
					// French
					uiLanguage = SupportedLanguages.French;
					break;

				case "ita":
					// Italian
					uiLanguage = SupportedLanguages.Italian;
					break;
*/
				default:
					// Unknown language. Default to English.
					setLanguageStringsToEnglishText();
					break;
			}
		}

		private static void setLanguageStringsToEnglishText()
		{
			_GenerateExeClickWidthTryParseFailed = "An error occurred while trying to convert the typed in Horizontal Resolution " +
			                                       "from a string. Make sure you only typed in digits.";
			_GenerateExeClickHeightTryParseFailed = "An error occurred while trying to convert the typed in Vertical Resolution " +
			                                        "from a string. Make sure you only typed in digits.";
			_GenerateExeClickWidthLessThanMinimum = "The desired Horizontal Resolution is less than 800, which is the absolute minimum width " +
			                                        "the game's window can be. Please type in a number which is at least 800.";
			_GenerateExeClickHeightLessThanMinimum = "The desired Vertical Resolution is less than 600, which is the absolute minimum height " +
			                                         "the game's window can be. Please type in a number which is at least 600.";
			_GenerateExeClickWidthMoreThanMaximum = $"The desired Horizontal Resolution is greater than {CentralManager.MAX_RESOLUTION_WIDTH.ToString()}, " +
			                                        $"which is not allowed. Please type in a number which is less than {CentralManager.MAX_RESOLUTION_WIDTH.ToString()}.";
			_GenerateExeClickHeightMoreThanMaximum = $"The desired Vertical Resolution is greater than {CentralManager.MAX_RESOLUTION_HEIGHT.ToString()}, " +
			                                         $"which is not allowed. Please type in a number which is less than {CentralManager.MAX_RESOLUTION_HEIGHT.ToString()}.";
			_GenerateExeClickWidthNotDivisibleBy4 = "The desired Horizontal Resolution is not divisible by 4. Please type in a number which is.";

			_SelectExeClickTitle = "Please select the Emperor.exe you want to patch.";

			_HelpMeClickMessage = new[] {
				"Help menu for the Emperor Resolution Customiser utility",
				"",
				"",
				"This program modifies a supplied copy of Emperor.exe to allow you to change what resolution the game runs at when the \"1024x768\" option " +
				"is selected in the game's display menu. It also allows applying a number of patches to the game and resizing the game's background images.",
				"",
				"This program requires an unmodified copy of the game's EXE to work. It can be supplied in three ways. You can place this program in the same location " +
				"you have installed the game. Alternatively, you can copy the required game files into the \"base_files\" folder. Finally, you can use the file picker " +
				"button to choose where the game's files are. In any case, the modified files will be placed in the \"patched_files\" folder afterwards. ",
				"Do note that any existing \"patched_files\" folder and it's contents will be deleted before generating the new patched files.",
				"",
				"",
				"Resolution Width: This text box allows you to specify the horizontal component of your desired resolution. If your screen is in landscape, " +
				$"this is the bigger number. Note that this number must be divisible by 4 as well as between 800 and {CentralManager.MAX_RESOLUTION_WIDTH.ToString()}, both inclusive.",
				"",
				"Resolution Height: This text box allows you to specify the vertical component of your desired resolution. If your screen is in landscape, " +
				$"this is the smaller number. Note that this number must be between 600 and {CentralManager.MAX_RESOLUTION_HEIGHT.ToString()}, both inclusive.",
				"",
				"",
				"Select Emperor.exe: This button opens a file picker that lets you specify the location of a Emperor.exe that you want to modify.",
				"",
				"Generate EXE: Once you have provided all the required information and selected the desired options, click on this button to generate a patched Emperor.exe " +
				"and optionally resized images. All of these will be placed in the \"patched_files\" folder next to this program."
			};

			_ApplyWindowFixHelpClickMessage = new [] {
				"Apply Windowed Mode Fixes:",
				"This tickbox tells this program to fix a bug in Emperor which means that the game can't be switched into windowed mode.",
			};

			_PatchEngTextHelpClickMessage = new [] {
				"Patch EmperorText.eng:",
				"The game populates the Display Settings menu using strings from EmperorText.eng.",
				"By default, the string for the modified resolution option says \"1024 x 768 resolution\" and the menu option will continue " +
				"saying this even after the EXE has been patched to use a different resolution. When this option is ticked, the string read from " +
				"this file will be modified to instead identify the setting as the patched resolution (e.g. it will say \"1920 x 1080 resolution\" " +
				"if you typed in those numbers).",
				"To use this option, you need to have a copy of \"EmperorText.eng\" in the same folder as the \"Emperor.exe\" file " +
				"you selected for patching."
			};

			_ResizeImagesHelpClickMessage = new [] {
				"Resize Images:",
				"This tickbox tells this program to resize the various JPEGs the game uses as background images. This resizing is required for these images " +
				"to display properly at the new resolutions. All of the images need to be in a \"DATA\" folder that is in the same place as the selected Emperor.exe. ",
				"Since this is the most computationally intensive operation this program does, it is recommended that you only keep this option enabled if you need the " +
				"resized images. If you already have images of the correct dimensions, feel free to disable this option.",
			};

			_StretchImagesHelpClickMessage = new [] {
				"Stretch menu images to fit window:",
				"By default, this program keeps menu images at their original sizes and adds a black background around the images to " +
				"fill the gaps between it and the game window's edges. This option changes that behaviour and tells this program to stretch the images to fit the window ",
				"instead.",
				"Note that this option can only be selected if the \"Resize Images\" tickbox is checked.",
			};

			_IncreaseSpriteLimitsHelpClickMessage = new [] {
				"Double Sprite Limits:",
				"This tickbox tells the program to increase the game's sprite limit from 4000 to 8000. This is essentially the exact same patch created " +
				"by Vadim_Panenko on Mod DB, except that it will work with any of the custom resolutions this program supports. There are two things you must be aware of:",
				"1. The game might experience a bit of slowdown after building more buildings than the original limit permitted. Vadim also reported that the game " +
				"\"flies out\" if the limit is raised further than double.",
				"2. For obvious reasons, loading a save where this limit has been exceeded on an EXE that doesn't have this raised limit can cause issues.",
				"For these reasons, this option is disabled by default and it is recommended that you only enable it if you need it.",
			};

			_MainExeIntegrityCouldNotFindExe = "Could not find the location of the Resolution Customiser EXE. " +
			                                   "This is required to perform integrity testing on the EXE. The program will now exit.";
			_MainExeIntegrityDataCorruptionDetected = "Data corruption has been detected in the Resolution Customiser EXE. " +
			                                          "The program will now exit. Please re-download a fresh copy.";

			_EmperorEngTextEditFileNotFound = "You selected the \"Patch EmperorText.eng\" option but there is no " +
			                                  "\"EmperorText.eng\" file in the folder containing Emperor.exe.\n" +
			                                  "Please correct this problem then try again.";
			_EmperorEngTextEditExceptionOccurred = "An exception occurred while trying to patch EmperorText.eng:";

			_EmperorExeAttributesExeKnownModified = new [] {
				"The code for determining Emperor's version and language detected that you are using a copy of the game " +
				"that is known to have been modified. Modified copies of the game are not supported.",
				"",
				"Only the following unmodified distributions and languages are currently supported:",
				"- English GOG version (Emperor.exe CRC: 0xfd9cf46f)",
				"- English CD (Sierra and Sold Out versions) with the v1.1 patch installed (Emperor.exe CRC: 0xa8a1ae71)",
				"- French CD version with the v1.1 patch installed (Emperor.exe CRC: 0x46119d54)",
				"- Italian CD version with the v1.1 patch installed (Emperor.exe CRC: 0xbbf34fc6)",
				"",
				"Please remove any modifications you have applied to the game and try again."
			};
			_EmperorExeAttributesExeKnownModifiedMessageTitle = "Modified version of Emperor.exe detected";
			_EmperorExeAttributesExeKnownModifiedAndOutdated = new [] {
				"The code for determining Emperor's version and language detected that you are using a copy of the game " +
				"that is both outdated and known to have been modified. Both of those are not supported.",
				"",
				"Only the following unmodified distributions and languages are currently supported:",
				"- English GOG version (Emperor.exe CRC: 0xfd9cf46f)",
				"- English CD (Sierra and Sold Out versions) with the v1.1 patch installed (Emperor.exe CRC: 0xa8a1ae71)",
				"- French CD version with the v1.1 patch installed (Emperor.exe CRC: 0x46119d54)",
				"- Italian CD version with the v1.1 patch installed (Emperor.exe CRC: 0xbbf34fc6)",
				"",
				"Please remove any modifications you have applied to the game, then install the latest patch before trying again."
			};
			_EmperorExeAttributesExeKnownModifiedAndOutdatedMessageTitle = "Modified and outdated version of Emperor.exe detected";
			_EmperorExeAttributesExeKnownOutdated = new [] {
				"The code for determining Emperor's version and language detected that you are using a copy of the game " +
				"that is known to be outdated. Outdated copies of the game are not supported.",
				"",
				"Only the following unmodified distributions and languages are currently supported:",
				"- English GOG version (Emperor.exe CRC: 0xfd9cf46f)",
				"- English CD (Sierra and Sold Out versions) with the v1.1 patch installed (Emperor.exe CRC: 0xa8a1ae71)",
				"- French CD version with the v1.1 patch installed (Emperor.exe CRC: 0x46119d54)",
				"- Italian CD version with the v1.1 patch installed (Emperor.exe CRC: 0xbbf34fc6)",
				"",
				"Please update the game to the latest patch before trying again."
			};
			_EmperorExeAttributesExeKnownOutdatedMessageTitle = "Outdated version of Emperor.exe detected";
			_EmperorExeAttributesExeNotRecognised = new [] {
				"The code for determining Emperor's version and language could not work out what edition of the game you are using.",
				"",
				"Only the following unmodified distributions and languages are currently supported:",
				"- English GOG version (Emperor.exe CRC: 0xfd9cf46f)",
				"- English CD (Sierra and Sold Out versions) with the v1.1 patch installed (Emperor.exe CRC: 0xa8a1ae71)",
				"- French CD version with the v1.1 patch installed (Emperor.exe CRC: 0x46119d54)",
				"- Italian CD version with the v1.1 patch installed (Emperor.exe CRC: 0xbbf34fc6)",
				"",
				"If you are using one of the listed versions, please ensure that the EXE has not been modified.",
				"If you are not, please do request that support be added, especially if you can provide info on how I can get a copy of your version."
			};
			_EmperorExeAttributesExeNotRecognisedMessageTitle = "Unrecognised Emperor.exe detected";

			_EmperorMakeChangesExeNotFound = "Emperor.exe does not exist in either the selected location or either of the automatically scanned locations.\n" +
			                                "Please ensure that you have done one of the following:\n" +
			                                "- Selected the correct location using the \"Select Emperor.exe\" button,\n" +
			                                "- Placed this program in the folder you installed Emperor, or\n" +
			                                "- Placed the correct files in the \"base_files\" folder.";
			_EmperorMakeChangesPathTooLongException = "A PathTooLong Exception occurred while trying to work on the \"patched_files\" folder next to this program's EXE. " +
			                                          "Please exit and move the program somewhere with a shorter path length (the Downloads folder is a good choice).";
			_EmperorMakeChangesIoException = "An IO Exception occurred while trying to work on the \"patched_files\" folder next to this program's EXE. " +
			                                 "Please close any other programs using that folder, make sure the folder and it's contents are not marked Read-only and/or " +
			                                 "manually delete any files or folders named \"patched_files\".";
			_EmperorMakeChangesUnauthorizedAccessException = "This program must be run from a location that it is allowed to write files to. Please exit and move " +
			                                                 "the program somewhere that you have write permissions available (the Downloads folder is a good choice).";
			_EmperorMakeChangesExeSuccessfullyPatched = "Your patched Emperor.exe has been successfully created in";
			_EmperorMakeChangesUnhandledException = "An unhandled exception occurred";
			_EmperorMakeChangesUnhandledExceptionTitle = "Unhandled exception occurred";

			_EmperorResizeImagesOceanPatternNotFound = "Could not find \"ocean_pattern\\ocean_pattern.png\". A fallback colour will " +
			                                           "be used to create the maps instead. Please check if the ocean_pattern image was successfully " +
			                                           "extracted from this program's downloaded archive and is in the correct place.";
			_EmperorResizeImagesJpegEncoderNotFound = "Could not resize any of the game's images because the program could not find a " +
			                                          "JPEG Encoder available on your PC. Since Windows comes with such a codec by default, this " +
			                                          "could indicate a serious problem with your PC that can only be fixed by reinstalling Windows.";
			_EmperorResizeImagesCouldNotFindImageMessageStart = "Could not find the following images";
		}
	}
}
