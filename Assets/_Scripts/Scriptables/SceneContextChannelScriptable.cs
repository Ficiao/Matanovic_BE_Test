using BETest.Infra.DependacyHandling;
using System;
using UnityEngine;

namespace BETest.Scriptables
{
    [CreateAssetMenu(fileName = "SceneContextChannel", menuName = "Scriptables/Scene Context Channel")]
    public class SceneContextChannel : ScriptableObject
    {
        public event Action<SceneContext> ContextReady;
        public event Action<SceneContext> ContextReleased;

        public bool HasListener => ContextReady != null;

        public void Register(SceneContext context)
        {
            ContextReady?.Invoke(context);
        }

        public void Unregister(SceneContext context)
        {
            ContextReleased?.Invoke(context);
        }
    }
}