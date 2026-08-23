using LiteNetLib;
using System.Collections.Generic;
using System;
using BETest.Enum;
using Newtonsoft.Json;
using System.Linq;

namespace BETest.Networking
{
    public class CustomLogger
    {
        public static NetworkLogLevel MinimumLevel = NetworkLogLevel.Info;

        public static void SetLevelFromEnv()
        {
            NetworkLogLevel logLevel = System.Enum.Parse<NetworkLogLevel>(Environment.GetEnvironmentVariable("KIL_LOG_LEVEL"));
            MinimumLevel = logLevel;
        }

        public static void Debug(string msg, Dictionary<string, object> ctx = null) => Write(NetworkLogLevel.Debug, msg, null, ctx);
        public static void Info(string msg, Dictionary<string, object> ctx = null) => Write(NetworkLogLevel.Info, msg, null, ctx);
        public static void Warning(string msg, Dictionary<string, object> ctx = null) => Write(NetworkLogLevel.Warn, msg, null, ctx);
        public static void Error(string msg, Exception ex = null, Dictionary<string, object> ctx = null) => Write(NetworkLogLevel.Error, msg, ex, ctx);

        static void Write(NetworkLogLevel level, string msg, Exception ex, Dictionary<string, object> ctx)
        {
            if (level < MinimumLevel) return;

            if (ctx == null) ctx = new();
            Dictionary<string, object> record = new Dictionary<string, object>()
            {
                ["msg"] = msg,
                ["ts"] = DateTime.UtcNow.ToString("o"),
                ["logLevel"] = level.ToString().ToLowerInvariant(),
            };
            foreach (KeyValuePair<string, object> element in ctx) record.Add(element.Key, element.Value);
            if (ex != null) record["exception"] = ex.ToString();

            string json = JsonConvert.SerializeObject(record);

            // stdout za kontejnere; u Editoru i dalje vidiš poruke u Console
#if UNITY_EDITOR
            switch (level)
            {
                case NetworkLogLevel.Error: UnityEngine.Debug.LogError(json); break;
                case NetworkLogLevel.Warn: UnityEngine.Debug.LogWarning(json); break;
                default: UnityEngine.Debug.Log(json); break;
            }
#else
            Console.WriteLine(json);
#endif
        }
    }

    public class LiteNetLibToCustomLogger : INetLogger
    {
        private Dictionary<string, object> _ctx = new()
        {
            ["source"] = "litenetlib"
        };

        public void WriteNet(NetLogLevel level, string str, params object[] args)
        {
            string msg = args != null && args.Length > 0 ? string.Format(str, args) : str;

            switch (level)
            {
                case NetLogLevel.Error:
                    CustomLogger.Error(msg, null, _ctx);
                    break;
                case NetLogLevel.Warning:
                    CustomLogger.Warning(msg, _ctx);
                    break;
                case NetLogLevel.Info:
                    CustomLogger.Info(msg, _ctx);
                    break;
                default:
                    CustomLogger.Debug(msg, _ctx);
                    break;
            }
        }
    }
}
