// Copyright © Apptelic Ltd., 2014
// This source is subject to the Apptelic License described in "Licence.txt".
// All other rights reserved.

using System;
using System.Reflection;


namespace TownFish.App.Helpers
{
	/// <summary>
	/// Provides conversion helper extension methods.
	/// </summary>
	public static class ConvertAny
	{
		#region Methods

		/// <summary>
		/// Converts any input to a value of the given type.
		/// </summary>
		/// <typeparam name="T">Type of result</typeparam>
		/// <param name="value">Value to convert</param>
		/// <param name="def">Optional default value to return if value is null or DBNull,
		/// or conversion fails and throwOnError is false</param>
		/// <param name="throwOnError">Optionally set true to throw if conversion fails</param>
		/// <param name="provider">Optional <see cref="IFormatProvider"/>
		/// to help with conversion</param>
		/// <returns>Input value converted to type T</returns>
		public static T ConvertTo<T> (this object value, T def = default (T),
			bool throwOnError = false, IFormatProvider provider = null)
		{
			// if we're given nothing, return default (cah't check DBNull in a PCL)
			if (value == null || object.Equals (value, default (T)))// || value == DBNull.Value)
				return def;

			var t = typeof (T);
			var ti = t.GetTypeInfo();

			// if it's already correct type, just unbox and return
			if (value.GetType().Equals (t))
				return (T) value;

			try
			{
				// special-case generics, enums & TimeSpan, then do everything else
				if (ti.IsGenericType && ti.GetGenericTypeDefinition().Equals (sNullableT))
					return (T) Convert.ChangeType (value, Nullable.GetUnderlyingType (t), provider);
				else if (ti.BaseType.Equals (sEnum))
					return (T) (value is string ? Enum.Parse (t, value as string, false) :
							Convert.ChangeType (value, Enum.GetUnderlyingType (t), provider));
				else if (t.Equals (sTimeSpan))// TimeSpan isn't IConvertible
					return (T) (object) TimeSpan.Parse (value.ToString());
				else
					return (T) Convert.ChangeType (value, t, provider);
			}
			catch (Exception)
			{
				if (throwOnError)
					throw;

				return def;
			}
		}

		#endregion Methods

		#region Fields

		static readonly Type sEnum = typeof (Enum);
		static readonly Type sNullableT = typeof (Nullable<>);
		static readonly Type sTimeSpan = typeof (TimeSpan);

		#endregion Fields
	}
}
