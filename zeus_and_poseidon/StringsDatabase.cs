// This file is or was originally a part of the Impressions Resolution Customiser project, which can be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//

using System.Globalization;
using Zeus_and_Poseidon.Models;
using Zeus_and_Poseidon.ViewModels;

namespace Zeus_and_Poseidon
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

		internal static string[] _ApplyAnimationFixHelpClickMessage { get; private set; }

		internal static string _MainExeIntegrityCouldNotFindExe { get; private set; }
		internal static string _MainExeIntegrityDataCorruptionDetected { get; private set; }

		internal static string _ZeusEngTextEditFileNotFound { get; private set; }
		internal static string _ZeusEngTextEditExceptionOccurred { get; private set; }

		internal static string[] _ZeusExeAttributesExeKnownModified { get; private set; }
		internal static string _ZeusExeAttributesExeKnownModifiedMessageTitle { get; private set; }
		internal static string[] _ZeusExeAttributesExeKnownModifiedAndOutdated { get; private set; }
		internal static string _ZeusExeAttributesExeKnownModifiedAndOutdatedMessageTitle { get; private set; }
		internal static string[] _ZeusExeAttributesExeKnownOutdated { get; private set; }
		internal static string _ZeusExeAttributesExeKnownOutdatedMessageTitle { get; private set; }
		internal static string[] _ZeusExeAttributesExeNotRecognised { get; private set; }
		internal static string _ZeusExeAttributesExeNotRecognisedMessageTitle { get; private set; }

		internal static string _ZeusMakeChangesExeNotFound { get; private set; }
		internal static string _ZeusMakeChangesPathTooLongException { get; private set; }
		internal static string _ZeusMakeChangesIoException { get; private set; }
		internal static string _ZeusMakeChangesUnauthorizedAccessException { get; private set; }
		internal static string _ZeusMakeChangesExeSuccessfullyPatched { get; private set; }
		internal static string _ZeusMakeChangesUnhandledException { get; private set; }
		internal static string _ZeusMakeChangesUnhandledExceptionTitle { get; private set; }

		internal static string _EmperorResizeImagesOceanPatternNotFound { get; private set; }
		internal static string _ZeusResizeImagesJpegEncoderNotFound { get; private set; }
		internal static string _ZeusResizeImagesCouldNotFindImageMessageStart { get; private set; }


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

			_SelectExeClickTitle = "Please select the Zeus.exe you want to patch.";

			_HelpMeClickMessage = new[] {
				"Help menu for the Zeus & Poseidon Resolution Customiser utility",
				"",
				"",
				"This program modifies a supplied copy of Zeus.exe to allow you to change what resolution the game runs at when the \"1024x768\" option " +
				"is selected in the game's display menu. It also allows applying a number of patches to the game and resizing the game's background images.",
				"",
				"This program requires an unmodified copy of the game's EXE to work. It can be supplied in three ways. You can place this program in the same location " +
				"you have installed the game. Alternatively, you can copy the required game files into the \"base_files\" folder. Finally, you can use the file picker " +
				"button to choose where the game's files are. In any case, the modified files will be placed in the \"patched_files\" folder afterwards.",
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
				"Select Zeus.exe: This button opens a file picker that lets you specify the location of a Zeus.exe that you want to modify.",
				"",
				"Generate EXE: Once you have provided all the required information and selected the desired options, click on this button to generate a patched Zeus.exe " +
				"and optionally resized images. All of these will be placed in the \"patched_files\" folder next to this program."			};

			_ApplyAnimationFixHelpClickMessage = new [] {
				"Apply Animation Fixes:",
				"If enabled, this tickbox tells this program to fix the slow animation bugs that gods and urchin quays experience on modern CPUs."
			};

			_ApplyWindowFixHelpClickMessage = new [] {
				"Apply Windowed Mode Fixes:",
				"This tickbox tells this program to fix a bug in Zeus which means that the game can't be switched into windowed mode."
			};

			_PatchEngTextHelpClickMessage = new [] {
				"Patch Zeus_Text.eng:",
				"The game populates the Display Settings menu using strings from Zeus_Text.eng.",
				"By default, the string for the modified resolution option says \"1024 x 768 resolution\" and the menu option will continue " +
				"saying this even after the EXE has been patched to use a different resolution. When this option is ticked, the string read from " +
				"this file will be modified to instead identify the setting as the patched resolution (e.g. it will say \"1920 x 1080 resolution\" " +
				"if you typed in those numbers).",
				"To use this option, you need to have a copy of \"Zeus_Text.eng\" in the same folder as the \"Zeus.exe\" file " +
				"you selected for patching."
			};

			_ResizeImagesHelpClickMessage = new [] {
				"Resize Images:",
				"This tickbox tells this program to resize the various JPEGs the game uses as background images. This resizing is required for these images" +
				"to display properly at the new resolutions. All of the images need to be in a \"DATA\" folder that is in the same place as the selected Zeus.exe.",
				"Since this is the most computationally intensive operation this program does, it is recommended that you only keep this option enabled if you need the" +
				"resized images. If you already have images of the correct dimensions, feel free to disable this option."
			};

			_StretchImagesHelpClickMessage = new [] {
				"Stretch menu images to fit window:",
				"By default, this program keeps menu images at their original sizes and adds a black background around the images to " +
				"fill the gaps between it and the game window's edges. This option changes that behaviour and tells this program to stretch the images to fit the window " +
				"instead.",
				"Note that this option can only be selected if the \"Resize Images\" tickbox is checked."
			};

			_MainExeIntegrityCouldNotFindExe = "Could not find the location of the Resolution Customiser EXE. " +
			                                   "This is required to perform integrity testing on the EXE. The program will now exit.";
			_MainExeIntegrityDataCorruptionDetected = "Data corruption has been detected in the Resolution Customiser EXE. " +
			                                          "The program will now exit. Please re-download a fresh copy.";

			_ZeusEngTextEditFileNotFound = "You selected the \"Patch Zeus_Text.eng\" option but there is no " +
			                                  "\"Zeus_Text.eng\" file in the folder containing Zeus.exe.\n" +
			                                  "Please correct this problem then try again.";
			_ZeusEngTextEditExceptionOccurred = "An exception occurred while trying to patch Zeus_Text.eng:";

			_ZeusExeAttributesExeKnownModified = new [] {
				"The code for determining Zeus' version and language detected that you are using a copy of the game " +
				"that is known to have been modified. Modified copies of the game are not supported.",
				"",
				"Only the following unmodified distributions and languages are currently supported:",
				"- English GOG version with Poseidon expansion (Zeus.exe CRC: 0x90b9cf84)",
				"- English Steam version with Poseidon expansion (Zeus.exe CRC: 0x90b9cf84)",
				"",
				"Please remove any modifications you have applied to the game and try again."
			};
			_ZeusExeAttributesExeKnownModifiedMessageTitle = "Modified version of Zeus.exe detected";
			_ZeusExeAttributesExeKnownModifiedAndOutdated = new [] {
				"The code for determining Zeus' version and language detected that you are using a copy of the game " +
				"that is both outdated and known to have been modified. Both of those are not supported.",
				"",
				"Only the following unmodified distributions and languages are currently supported:",
				"- English GOG version with Poseidon expansion (Zeus.exe CRC: 0x90b9cf84)",
				"- English Steam version with Poseidon expansion (Zeus.exe CRC: 0x90b9cf84)",
				"",
				"Please remove any modifications you have applied to the game, then install the latest patch before trying again."
			};
			_ZeusExeAttributesExeKnownModifiedAndOutdatedMessageTitle = "Modified and outdated version of Zeus.exe detected";
			_ZeusExeAttributesExeKnownOutdated = new [] {
				"The code for determining Zeus' version and language detected that you are using a copy of the game " +
				"that is known to be outdated. Outdated copies of the game are not supported.",
				"",
				"Only the following unmodified distributions and languages are currently supported:",
				"- English GOG version with Poseidon expansion (Zeus.exe CRC: 0x90b9cf84)",
				"- English Steam version with Poseidon expansion (Zeus.exe CRC: 0x90b9cf84)",
				"",
				"Please update the game to the latest patch before trying again."
			};
			_ZeusExeAttributesExeKnownOutdatedMessageTitle = "Outdated version of Zeus.exe detected";
			_ZeusExeAttributesExeNotRecognised = new [] {
				"The code for determining Zeus' version and language could not work out what edition of the game you are using.",
				"",
				"Only the following unmodified distributions and languages are currently supported:",
				"- English GOG version with Poseidon expansion (Zeus.exe CRC: 0x90b9cf84)",
				"- English Steam version with Poseidon expansion (Zeus.exe CRC: 0x90b9cf84)",
				"",
				"If you are using one of the listed versions, please ensure that the EXE has not been modified.",
				"If you are not, please do request that support be added, especially if you can provide info on how I can get a copy of your version."
			};
			_ZeusExeAttributesExeNotRecognisedMessageTitle = "Unrecognised Zeus.exe detected";

			_ZeusMakeChangesExeNotFound = "Zeus.exe does not exist in either the selected location, or either of the automatically scanned locations.\n" +
			                                 "Please ensure that you have done one of the following:\n" +
			                                 "- Selected the correct location using the \"Select Zeus.exe\" button,\n" +
			                                 "- Placed this program in the folder you installed Zeus, or\n" +
			                                 "- Placed the correct files in the \"base_files\" folder.";
			_ZeusMakeChangesPathTooLongException = "A PathTooLong Exception occurred while trying to work on the \"patched_files\" folder next to this program's EXE. " +
			                                          "Please exit and move the program somewhere with a shorter path length (the Downloads folder is a good choice).";
			_ZeusMakeChangesIoException = "An IO Exception occurred while trying to work on the \"patched_files\" folder next to this program's EXE. " +
			                                 "Please close any other programs using that folder, make sure the folder and it's contents are not marked Read-only and/or " +
			                                 "manually delete any files or folders named \"patched_files\".";
			_ZeusMakeChangesUnauthorizedAccessException = "This program must be run from a location that it is allowed to write files to. Please exit and move " +
			                                                 "the program somewhere that you have write permissions available (the Downloads folder is a good choice).";
			_ZeusMakeChangesExeSuccessfullyPatched = "Your patched Zeus.exe has been successfully created in";
			_ZeusMakeChangesUnhandledException = "An unhandled exception occurred";
			_ZeusMakeChangesUnhandledExceptionTitle = "Unhandled exception occurred";

			_ZeusResizeImagesJpegEncoderNotFound = "Could not resize any of the game's images because the program could not find a " +
			                                       "JPEG Encoder available on your PC. Since Windows comes with such a codec by default, this " +
			                                       "could indicate a serious problem with your PC that can only be fixed by reinstalling Windows.";
			_ZeusResizeImagesCouldNotFindImageMessageStart = "Could not find the following images";
		}
	}
}
