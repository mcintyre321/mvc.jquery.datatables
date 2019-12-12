// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace Mvc.JQuery.DataTables
{
    internal static class ModelBindingHelper
    {
        /// <summary>
        /// Converts the provided <paramref name="value"/> to a value of <see cref="Type"/> <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> for conversion.</typeparam>
        /// <param name="value">The value to convert."/></param>
        /// <param name="culture">The <see cref="CultureInfo"/> for conversion.</param>
        /// <returns>
        /// The converted value or the default value of <typeparamref name="T"/> if the value could not be converted.
        /// </returns>
        public static T ConvertTo<T>(object value, CultureInfo culture)
        {
            var converted = ConvertTo(value, typeof(T), culture);
            return converted == null ? default(T) : (T)converted;
        }

        /// <summary>
        /// Converts the provided <paramref name="value"/> to a value of <see cref="Type"/> <paramref name="type"/>.
        /// </summary>
        /// <param name="value">The value to convert."/></param>
        /// <param name="type">The <see cref="Type"/> for conversion.</param>
        /// <param name="culture">The <see cref="CultureInfo"/> for conversion.</param>
        /// <returns>
        /// The converted value or <c>null</c> if the value could not be converted.
        /// </returns>
        public static object ConvertTo(object value, Type type, CultureInfo culture)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (value == null)
            {
                // For value types, treat null values as though they were the default value for the type.
                return type.GetTypeInfo().IsValueType ? Activator.CreateInstance(type) : null;
            }

            if (type.IsAssignableFrom(value.GetType()))
            {
                return value;
            }

            var cultureToUse = culture ?? CultureInfo.InvariantCulture;
            return UnwrapPossibleArrayType(value, type, cultureToUse);
        }

        private static object UnwrapPossibleArrayType(object value, Type destinationType, CultureInfo culture)
        {
            // array conversion results in four cases, as below
            var valueAsArray = value as Array;
            if (destinationType.IsArray)
            {
                var destinationElementType = destinationType.GetElementType();
                if (valueAsArray != null)
                {
                    // case 1: both destination + source type are arrays, so convert each element
                    var converted = (IList)Array.CreateInstance(destinationElementType, valueAsArray.Length);
                    for (var i = 0; i < valueAsArray.Length; i++)
                    {
                        converted[i] = ConvertSimpleType(valueAsArray.GetValue(i), destinationElementType, culture);
                    }
                    return converted;
                }
                else
                {
                    // case 2: destination type is array but source is single element, so wrap element in
                    // array + convert
                    var element = ConvertSimpleType(value, destinationElementType, culture);
                    var converted = (IList)Array.CreateInstance(destinationElementType, 1);
                    converted[0] = element;
                    return converted;
                }
            }
            else if (valueAsArray != null)
            {
                // case 3: destination type is single element but source is array, so extract first element + convert
                if (valueAsArray.Length > 0)
                {
                    value = valueAsArray.GetValue(0);
                    return ConvertSimpleType(value, destinationType, culture);
                }
                else
                {
                    // case 3(a): source is empty array, so can't perform conversion
                    return null;
                }
            }

            // case 4: both destination + source type are single elements, so convert
            return ConvertSimpleType(value, destinationType, culture);
        }

        private static object ConvertSimpleType(object value, Type destinationType, CultureInfo culture)
        {
            if (value == null || destinationType.IsAssignableFrom(value.GetType()))
            {
                return value;
            }

            // In case of a Nullable object, we try again with its underlying type.
            destinationType = UnwrapNullableType(destinationType);

            // if this is a user-input value but the user didn't type anything, return no value
            if (value is string valueAsString && string.IsNullOrWhiteSpace(valueAsString))
            {
                return null;
            }

            var converter = TypeDescriptor.GetConverter(destinationType);
            var canConvertFrom = converter.CanConvertFrom(value.GetType());
            if (!canConvertFrom)
            {
                converter = TypeDescriptor.GetConverter(value.GetType());
            }
            if (!(canConvertFrom || converter.CanConvertTo(destinationType)))
            {
                // EnumConverter cannot convert integer, so we verify manually
                if (destinationType.GetTypeInfo().IsEnum &&
                    (value is int ||
                    value is uint ||
                    value is long ||
                    value is ulong ||
                    value is short ||
                    value is ushort ||
                    value is byte ||
                    value is sbyte))
                {
                    return Enum.ToObject(destinationType, value);
                }

                throw new InvalidOperationException($"NoConverterExists: {value.GetType()} -> {destinationType}");
            }

            try
            {
                return canConvertFrom
                    ? converter.ConvertFrom(null, culture, value)
                    : converter.ConvertTo(null, culture, value, destinationType);
            }
            catch (FormatException)
            {
                throw;
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    throw;
                }
                else
                {
                    // TypeConverter throws System.Exception wrapping the FormatException,
                    // so we throw the inner exception.
                    ExceptionDispatchInfo.Capture(ex.InnerException).Throw();

                    // This code is never reached because the previous line will always throw.
                    throw;
                }
            }
        }

        private static Type UnwrapNullableType(Type destinationType)
        {
            return Nullable.GetUnderlyingType(destinationType) ?? destinationType;
        }
    }
}