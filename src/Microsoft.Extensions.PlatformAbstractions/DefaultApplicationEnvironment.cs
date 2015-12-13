﻿using System;
using System.Reflection;
using System.Runtime.Versioning;

namespace Microsoft.Extensions.PlatformAbstractions
{
    public class DefaultApplicationEnvironment : IApplicationEnvironment
    {
        public string ApplicationBasePath
        {
            get
            {
#if NET451
                return (string)AppDomain.CurrentDomain.GetData("APP_CONTEXT_BASE_DIRECTORY") ?? AppDomain.CurrentDomain.BaseDirectory;
#else
                return AppContext.BaseDirectory;
#endif
            }
        }

        public string ApplicationName
        {
            get
            {
                return GetEntryAssembly()?.GetName().Name;
            }
        }

        public string ApplicationVersion
        {
            get
            {
                return GetEntryAssembly()?.GetName().Version.ToString();
            }
        }

        public FrameworkName RuntimeFramework
        {
            get
            {
                string frameworkName = null;
#if NET451
                // Try the setup information
                frameworkName = AppDomain.CurrentDomain.SetupInformation.TargetFrameworkName;
#endif
                // Try the target framework attribute
                frameworkName = frameworkName ?? GetEntryAssembly().GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName;

                // TODO: Use when implemented https://github.com/dotnet/corefx/issues/3049

                return string.IsNullOrEmpty(frameworkName) ? null : new FrameworkName(frameworkName);
            }
        }

        private static Assembly GetEntryAssembly()
        {
#if NET451
            return Assembly.GetEntryAssembly();
#else
            // TODO: Remove private reflection when we get this: https://github.com/dotnet/corefx/issues/4146
            return typeof(Assembly).GetMethod("GetEntryAssembly", BindingFlags.Static | BindingFlags.NonPublic).Invoke(obj: null, parameters: Array.Empty<object>()) as Assembly;
#endif
        }
    }
}