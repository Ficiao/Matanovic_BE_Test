using System;
using System.Runtime.Versioning;

namespace LiteNetLib.Tests.TestUtility
{
    internal static class TestPorts
    {
        private const int BaselineFrameworkMajor = 8;
        private const int FrameworkPortBlockSize = 1000;

        public static int ForFramework(int basePort)
        {
            TargetFrameworkAttribute frameworkAttribute = (TargetFrameworkAttribute)Attribute.GetCustomAttribute(
                typeof(TestPorts).Assembly,
                typeof(TargetFrameworkAttribute));

            if (frameworkAttribute == null)
                return basePort;

            const string versionMarker = "Version=v";
            int versionStart = frameworkAttribute.FrameworkName.IndexOf(versionMarker, StringComparison.Ordinal);
            if (versionStart < 0)
                return basePort;

            string versionText = frameworkAttribute.FrameworkName.Substring(versionStart + versionMarker.Length);
            if (!Version.TryParse(versionText, out Version version))
                return basePort;

            return basePort + Math.Max(0, version.Major - BaselineFrameworkMajor) * FrameworkPortBlockSize;
        }
    }
}
