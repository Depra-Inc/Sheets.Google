using System;
using System.Linq;
using System.Reflection;

namespace Depra.Sheets.Google.Editor.Extensions
{
	internal static class FieldInfoExtensions
	{
		public static TAttribute GetAttribute<TAttribute>(this FieldInfo fieldInfo) where TAttribute : Attribute =>
			fieldInfo.GetCustomAttributes(typeof(TAttribute), false).FirstOrDefault() as TAttribute;
	}
}