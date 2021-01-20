#nullable enable
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;

namespace Localizer
{
    public readonly struct LocalizationCacheKey
    {
        public string Key { get; }
        public string Culture { get; }

        public LocalizationCacheKey(string key, string culture)
        {
            Key = key;
            Culture = culture;
        }
    }
    
    public abstract class ResourcesProviderBase : IResourcesProvider
    {
        private static readonly HashSet<string> LoadedCultures = new HashSet<string>();
        private static readonly HashSet<string> MissingCultures = new HashSet<string>();
        private static readonly object LoadCultureLock = new object();
        private static readonly object MissingCulturesLock = new object();

        protected static readonly ConcurrentDictionary<LocalizationCacheKey, string> CachedValues =
            new ConcurrentDictionary<LocalizationCacheKey, string>();

        protected readonly string ResourceName;

        protected ResourcesProviderBase(string resourceName)
        {
            ResourceName = resourceName ?? throw new ArgumentNullException(nameof(resourceName));
        }
        
        /// <returns>True if culture exists, oterwise false</returns>
        protected abstract bool LoadResourcesForCultureInternal(CultureInfo cultureInfo);

        public string? Get(string key, CultureInfo cultureInfo)
        {
            LoadResourcesForCulture(cultureInfo);
            if (CachedValues.TryGetValue(new LocalizationCacheKey(key, cultureInfo.Name), out string result))
            {
                return result;
            }

            return null;
        }

        public bool LoadResourcesForCulture(CultureInfo cultureInfo)
        {
            var cultureKey = GetCultureKey(cultureInfo);
            
            // ReSharper disable once InconsistentlySynchronizedField
            if (!LoadedCultures.Contains(cultureKey))
            {
                lock (LoadCultureLock)
                {
                    if (!LoadedCultures.Contains(cultureKey))
                    {
                        var cultureExists = LoadResourcesForCultureInternal(cultureInfo);
                        if (cultureExists)
                        {
                            LoadedCultures.Add(cultureKey);
                        }
                        else
                        {
                            AddToMissingCultures(cultureKey);
                        }

                        return cultureExists;
                    }
                }
            }
            
            return true;
        }

        private void AddToMissingCultures(string cultureKey)
        {
            if (!MissingCultures.Contains(cultureKey))
            {
                lock (MissingCulturesLock)
                {
                    if (!MissingCultures.Contains(cultureKey))
                    {
                        MissingCultures.Add(cultureKey);
                    }                    
                }
            }
        }

        private string GetCultureKey(CultureInfo cultureInfo) => cultureInfo.Name;
    }
}