using System;
using System.IO;
using System.Runtime;
using System.Runtime.Serialization.Formatters.Binary;
using Mercury.Globalization;

namespace Mercury.Globalization
{
	internal static class Resources
	{
		private static LocaleCollection locales;

		/// <summary>
		/// Initializes the locales resources.
		/// </summary>
		static Resources()
		{
			// Create the binary formatter.
			BinaryFormatter formatter = new BinaryFormatter();

			using (MemoryStream stream = new MemoryStream(Mercury.Locales.Collection))
			{
				Resources.locales = formatter.Deserialize(stream) as LocaleCollection;
			}
		}

		// Public properties.

		/// <summary>
		/// Gets the locales collection.
		/// </summary>
		internal static LocaleCollection Locales { get { return Resources.locales; } }
	}
}
