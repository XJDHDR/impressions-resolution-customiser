// This file is or was originally a part of the Impressions Resolution Customiser project, which can be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser
//
// The license for it may be found here:
// https://github.com/XJDHDR/impressions-resolution-customiser/blob/main/LICENSE
//

using JetBrains.Annotations;
using System;

namespace Emperor.ViewModels
{
	/// <summary>
	/// This creates an <see cref="Attribute"/> that is used by the ViewModel to invoke methods in other classes
	/// without creating a dependency in the ViewModel. Note that only methods that are designated as both
	/// non-Public and Static will be invoked. Any that don't meet both conditions will be ignored.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	[MeansImplicitUse]
	public class ExecuteFromViewModelConstructorAttribute : Attribute
	{}
}
