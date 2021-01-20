using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Localizer
{
    public class JsonResourcesProvider : ResourcesProviderBase
    {
        /// <param name="resourceName">
        /// example: if path is Resources/res.xx-XX.json resource name is Resources/res
        /// path without ext and culture code
        /// </param>
        public JsonResourcesProvider(string resourceName) : base(resourceName)
        {
        }

        protected override bool LoadResourcesForCultureInternal(CultureInfo cultureInfo)
        {
            var fileName = GetJsonFileName(ResourceName, cultureInfo);
            try
            {
                var text = File.ReadAllText(fileName); // TODO: buffered read?
                ReadOnlySpan<byte> span = Encoding.UTF8.GetBytes(text);
            
                var reader = new Utf8JsonReader(span);
                ReadJsonAndSaveToCache(ref reader, cultureInfo.Name);
            }
            catch (FileNotFoundException e) // This is not an error if no resources for specific culture
            {
                return false;
            }

            return true;
        }
        
        private void ReadJsonAndSaveToCache(ref Utf8JsonReader reader, string culture)
        {
            reader.Read();
            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException("Expected PropertyName");
                }
                string key = reader.GetString() ?? throw new JsonException("PropertyName cannot be null");
                string? value = ParseValue(ref reader);
                if (!CachedValues.TryAdd(new LocalizationCacheKey(key, culture), value))
                {
                    throw new InvalidOperationException();
                }
            }

        }

        private static string? ParseValue(ref Utf8JsonReader reader)
        {
            reader.Read();
            if (reader.TokenType != JsonTokenType.String) throw new JsonException("Could not parse json");

            return reader.GetString();

        }

        private static string GetJsonFileName(string resourceName, CultureInfo cultureInfo) =>
            $"{resourceName}.{cultureInfo.Name}.json";
    }
}