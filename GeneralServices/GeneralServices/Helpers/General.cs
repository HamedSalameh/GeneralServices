using System;
using System.Configuration;
using System.Reflection;
using System.Web.Configuration;

namespace GeneralServices.Helpers
{
    public static class General
    {
        private const uint _basePrime = 2166136261;
        private const int _Prime = 16777619;
        public static int? calculateSingleFieldHash(object field)
        {
            int? hash = null;

            // Calculating based on Jon Skeet's post
            unchecked // Overflow is fine, just wrap
            {
                hash = (int)_basePrime;
                // Suitable nullity checks etc, of course :)
                hash = (hash * _Prime) ^ field.GetHashCode();
            }

            return hash;
        }

        public static int? calculateClassHash(Type Object)
        {
            int? hash = null;
            
            PropertyInfo[] props = Object.UnderlyingSystemType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            unchecked
            {
                hash = (int)_basePrime;
                foreach (PropertyInfo prop in props)
                {
                    hash = (hash * _Prime) ^ prop.Name.GetHashCode();
                }

                // append the class name
                hash = (hash * _Prime) ^ Object.FullName.GetHashCode();
            }

            return hash;
        }

    }

    internal static class GeneralServicesHelpers
    {
        internal static string LoadConfigurationSettings(string settingName)
        {
            string config = string.Empty;

            Configuration configuration = WebConfigurationManager.OpenWebConfiguration(@"\web.config");
            if(configuration.AppSettings.Settings.Count > 0)
            {
                KeyValueConfigurationElement setting = configuration.AppSettings.Settings[settingName];
                if (setting != null)
                {
                    config = setting.Value;
                }
            }

            return config;
        }

        internal static string LoadConfigurationFile(string FileName)
        {
            string configuration = string.Empty;

            if (string.IsNullOrEmpty(FileName) == false)
            {
                // load the config file
            }

            return configuration;
        }
    }
}
