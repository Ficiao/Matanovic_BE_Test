using LiteNetLib;
using System;
using UnityEngine;

namespace BETest.Networking
{
    public class NetworkBootstrap : MonoBehaviour
    {
        private void Awake()
        {
            InitLogger();
        }

        private static void InitLogger()
        {
            Environment.SetEnvironmentVariable("BETEST_LOG_LEVEL", "Debug");
            CustomLogger.SetLevelFromEnv();
            NetDebug.Logger = new LiteNetLibToCustomLogger();
            CustomLogger.Info("Logger initialized", new()
            {
                ["minimumLevel"] = CustomLogger.MinimumLevel.ToString()
            });
        }

        private void OnApplicationQuit()
        {
            NetDebug.Logger = null;
        }
    }
}