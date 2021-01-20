using System;
using System.ComponentModel;
using System.Globalization;
using Localizer;

namespace LocalizerApp
{
    public enum MsgDef
    {
        [Description("key1")] // has upper "K", but in json this letter is in lowercase
        Key1,
        // ReSharper disable once InconsistentNaming
        key2 // no description, so key string will be enum value string representation
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            var provider = new JsonResourcesProvider("Resources/res");
            var localizer = new Localizer<MsgDef>(provider, false);

            var value1 = localizer[MsgDef.Key1];
            Console.WriteLine(value1);
            
            var value2 = localizer[MsgDef.key2];
            Console.WriteLine(value2);
        }
    }
}