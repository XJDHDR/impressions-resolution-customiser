// This file is or was originally a part of the Impressions Resolution Customiser project, which can be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Input;

namespace Zeus_and_Poseidon.ViewModels
{
	public class MainWindowVm : INotifyPropertyChanged
	{
		public static MainWindowVm Instance;

		public bool ApplyAnimationFixIsChecked { get; set; } = true;
		public bool ApplyWindowModeFixesIsChecked { get; set; } = true;
		public bool PatchEngTextIsChecked { get; set; } = true;

		public bool ResizeImagesIsChecked
		{
			get => resizeImagesIsChecked;
			set
			{
				setField(ref resizeImagesIsChecked, value, "ResizeImagesIsChecked");
				switch (value)
				{
					case true:
						StretchImagesIsEnabled = true;
						break;

					case false:
						StretchImagesIsEnabled = false;
						StretchImagesIsChecked = false;
						break;
				}
			}
		}
		public bool StretchImagesIsChecked
		{
			get => stretchImagesIsChecked;
			set => setField(ref stretchImagesIsChecked, value, "StretchImagesIsChecked");
		}
		public bool StretchImagesIsEnabled
		{
			get => stretchImagesIsEnabled;
			set => setField(ref stretchImagesIsEnabled, value, "StretchImagesIsEnabled");
		}

		public string ResolutionHeight { get; set; } = "768";
		public string ResolutionWidth { get; set; } = "1024";

		public string WindowTitle
		{
			get => windowTitle;
			private set => setField(ref windowTitle, value, "WindowTitle");
		}
		public string ResolutionHeightText
		{
			get => resolutionHeightText;
			private set => setField(ref resolutionHeightText, value, "ResolutionHeightText");
		}
		public string ResolutionWidthText
		{
			get => resolutionWidthText;
			private set => setField(ref resolutionWidthText, value, "ResolutionWidthText");
		}
		public string ApplyAnimationFixCheckboxText
		{
			get => applyAnimationFixCheckboxText;
			private set => setField(ref applyAnimationFixCheckboxText, value, "ApplyAnimationFixCheckboxText");
		}
		public string ApplyWindowModeFixesCheckboxText
		{
			get => applyWindowModeFixesCheckboxText;
			private set => setField(ref applyWindowModeFixesCheckboxText, value, "ApplyWindowModeFixesCheckboxText");
		}
		public string PatchEngTextCheckboxText
		{
			get => patchEngTextCheckboxText;
			private set => setField(ref patchEngTextCheckboxText, value, "PatchEngTextCheckboxText");
		}
		public string ResizeImagesCheckboxText
		{
			get => resizeImagesCheckboxText;
			private set => setField(ref resizeImagesCheckboxText, value, "ResizeImagesCheckboxText");
		}
		public string StretchImagesCheckboxText
		{
			get => stretchImagesCheckboxText;
			private set => setField(ref stretchImagesCheckboxText, value, "StretchImagesCheckboxText");
		}
		public string SelectExeButtonText
		{
			get => selectExeButtonText;
			private set => setField(ref selectExeButtonText, value, "SelectExeButtonText");
		}
		public string GenerateExeButtonText
		{
			get => generateExeButtonText;
			private set => setField(ref generateExeButtonText, value, "GenerateExeButtonText");
		}

		public ICommand GenerateExeEvent { get; } = new RelayCommand(_ => Instance.GenerateExeButtonClickedEventHandler?.Invoke());
		public ICommand HelpMeEvent { get; } = new RelayCommand(_ => Instance.HelpMeButtonClickedEventHandler?.Invoke());
		public ICommand SelectExeEvent { get; } = new RelayCommand(_ => Instance.SelectExeButtonClickedEventHandler?.Invoke());
		public ICommand SwitchLanguageToEnglishEvent { get; } = new RelayCommand(_ =>
		{
			Instance.setLanguage("eng");
			Instance.LanguageChangedEventHandler?.Invoke("eng");
		});

		public ICommand ApplyAnimationFixHelpEvent { get; } = new RelayCommand(_ => Instance.ApplyAnimationFixHelpButtonClickedEventHandler?.Invoke());
		public ICommand ApplyWindowFixHelpEvent { get; } = new RelayCommand(_ => Instance.ApplyWindowFixHelpButtonClickedEventHandler?.Invoke());
		public ICommand PatchEngTextHelpEvent { get; } = new RelayCommand(_ => Instance.PatchEngTextHelpButtonClickedEventHandler?.Invoke());
		public ICommand ResizeImagesHelpEvent { get; } = new RelayCommand(_ => Instance.ResizeImagesHelpButtonClickedEventHandler?.Invoke());
		public ICommand StretchImagesHelpEvent { get; } = new RelayCommand(_ => Instance.StretchImagesHelpButtonClickedEventHandler?.Invoke());


		// ==== Fields ====
		private bool resizeImagesIsChecked = true;
		private bool stretchImagesIsChecked;
		private bool stretchImagesIsEnabled = true;

		private string windowTitle;
		private string resolutionHeightText;
		private string resolutionWidthText;
		private string applyAnimationFixCheckboxText;
		private string applyWindowModeFixesCheckboxText;
		private string patchEngTextCheckboxText;
		private string resizeImagesCheckboxText;
		private string stretchImagesCheckboxText;
		private string selectExeButtonText;
		private string generateExeButtonText;


		// ==== Events ====
		public event PropertyChangedEventHandler PropertyChanged;

		public delegate void VoidReturnNoArgumentsEvent();
		public event VoidReturnNoArgumentsEvent GenerateExeButtonClickedEventHandler;
		public event VoidReturnNoArgumentsEvent HelpMeButtonClickedEventHandler;
		public event VoidReturnNoArgumentsEvent SelectExeButtonClickedEventHandler;

		public event VoidReturnNoArgumentsEvent ApplyWindowFixHelpButtonClickedEventHandler;
		public event VoidReturnNoArgumentsEvent ApplyAnimationFixHelpButtonClickedEventHandler;
		public event VoidReturnNoArgumentsEvent PatchEngTextHelpButtonClickedEventHandler;
		public event VoidReturnNoArgumentsEvent ResizeImagesHelpButtonClickedEventHandler;
		public event VoidReturnNoArgumentsEvent StretchImagesHelpButtonClickedEventHandler;

		public delegate void VoidReturnOneStringArgumentEvent(string String1);
		public event VoidReturnOneStringArgumentEvent LanguageChangedEventHandler;


		// ==== Constructor ====
		public MainWindowVm()
		{
			Instance = this;

			// Get all assemblies in program, and iterate through them.
			Assembly[] allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < allAssemblies.Length; ++i)
			{
				// Get all types in current assembly, and iterate through them.
				Type[] allTypesInAssembly = allAssemblies[i].GetTypes();
				for (int j = 0; j < allTypesInAssembly.Length; ++j)
				{
					// If the current type is not a class, skip to the next one.
					if (!allTypesInAssembly[j].IsClass)
						continue;

					// Get all methods in current class, and iterate through them.
					MethodInfo[] allMethodsInType = allTypesInAssembly[j].GetMethods(BindingFlags.NonPublic | BindingFlags.Static);
					for (int k = 0; k < allMethodsInType.Length; ++k)
					{
						// If the current method does not have the ExecuteFromViewModelConstructor attribute attached, skip to the next one.
						MethodInfo method = allMethodsInType[k];
						object[] isAttributeOnMethod = method.GetCustomAttributes(typeof(ExecuteFromViewModelConstructor), false);
						if (isAttributeOnMethod.Length == 0)
							continue;

						// Invoke the current method.
						method.Invoke(null, null);
					}
				}
			}

			// On first run, set program's language to the OS's UI language.
			setLanguage(CultureInfo.CurrentUICulture.ThreeLetterISOLanguageName);
		}


		// ==== Protected methods ====
		private void OnPropertyChanged(string PropertyName) =>
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));

		private bool setField<T>(ref T Field, T Value, string PropertyName)
		{
			if (EqualityComparer<T>.Default.Equals(Field, Value)) return false;
			Field = Value;
			OnPropertyChanged(PropertyName);
			return true;
		}


		// ==== Private methods ====
		private void setLanguage(string LanguageThreeLetterIso)
		{
			switch (LanguageThreeLetterIso)
			{
				case "eng":
					// English
					setLanguageEnglish();
					break;

				default:
					// Unknown language. Default to English.
					setLanguageEnglish();
					break;
			}
		}

		private void setLanguageEnglish()
		{
			WindowTitle = "Zeus & Poseidon Resolution Customiser";
			ResolutionHeightText = "Resolution Height:";
			ResolutionWidthText = "Resolution Width:";
			ApplyAnimationFixCheckboxText = "Apply Animation Fixes";
			ApplyWindowModeFixesCheckboxText = "Apply Windowed Mode Fixes";
			PatchEngTextCheckboxText = "Patch Zeus_Text.eng";
			ResizeImagesCheckboxText = "Resize Images";
			StretchImagesCheckboxText = "Stretch menu images to fit window";
			SelectExeButtonText = "Select Zeus.exe";
			GenerateExeButtonText = "Generate EXE";
		}
	}
}
