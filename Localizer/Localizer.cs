using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Localizer
{
    public interface ILocalizer<TEnum> where TEnum : notnull, Enum
    {
        string this[TEnum key] { get; }
        string Get(TEnum key, CultureInfo cultureInfo = null);
    }

    public class Localizer<TEnum> : ILocalizer<TEnum> where TEnum : notnull, Enum
    {
        private readonly bool _throwWhenNotFound = false;
        private readonly IResourcesProvider _provider;

        // to reduce overhead of boxing of enum value when calling Enum.GetName(Type, object) every time
        private static readonly Dictionary<TEnum, string> _enumStringNames =
            Enum.GetValues(typeof(TEnum)).Cast<TEnum>()
                .ToDictionary(key => key, key => GetEnumStringRepresentation(typeof(TEnum), key));

        public Localizer(IResourcesProvider provider, bool throwWhenNotFound = false)
        {
            _provider = provider;
            _throwWhenNotFound = throwWhenNotFound;
        }

        public string this[TEnum index]
        {
            get { return Get(index, CultureInfo.CurrentUICulture); }
        }

        public string this[TEnum index, params object[] args]
        {
            get { return Get(index, CultureInfo.CurrentUICulture, args); }
        }

        #region Overloads with params

        public string Get(TEnum key, CultureInfo cultureInfo, object arg1)
        {
            return string.Format(Get(key, cultureInfo), arg1);
        }

        public string Get(TEnum key, CultureInfo cultureInfo, object arg1, object arg2)
        {
            return string.Format(Get(key, cultureInfo), arg1, arg2);
        }

        public string Get(TEnum key, CultureInfo cultureInfo, object arg1, object arg2, object arg3)
        {
            return string.Format(Get(key, cultureInfo), arg1, arg3);
        }

        // Add more if needed

        public string Get(TEnum key, CultureInfo cultureInfo, params object[] args)
        {
            return string.Format(Get(key, cultureInfo), args);
        }

        #endregion

        public string Get(TEnum key, CultureInfo cultureInfo)
        {
            if (cultureInfo == null) throw new ArgumentNullException(nameof(cultureInfo));

            var found = _provider.Get(_enumStringNames[key], cultureInfo);
            if (found == null && _throwWhenNotFound)
                throw new KeyNotFoundException($"Localization for key {key}, culture {cultureInfo} was not found");

            return found;
        }

        private static string GetEnumStringRepresentation(Type enumType, object value)
        {
            var descriptionAttribute = value.GetAttributeOfType<DescriptionAttribute>(enumType);
            if (descriptionAttribute == null)
            {
                return enumType.GetEnumName(value);
            }

            return descriptionAttribute.Description;
        }
    }

    public static class EnumHelper
    {
        public static T GetAttributeOfType<T>(this object enumVal, Type enumType) where T : Attribute
        {
            var memInfo = enumType.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            return (attributes.Length > 0) ? (T) attributes[0] : null;
        }
    }
}