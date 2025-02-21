using System;

namespace Raft.Scripts
{
    [Serializable]
    public struct ResourcesCostConfig
    {
        public string resourceName;
        public int amount;
    }
}