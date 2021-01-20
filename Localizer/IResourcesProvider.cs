#nullable enable
using System.Globalization;

namespace Localizer
{
    public interface IResourcesProvider
    {
        string? Get(string key, CultureInfo cultureInfo);
    }
}