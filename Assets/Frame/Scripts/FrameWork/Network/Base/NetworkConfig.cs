namespace GameConfig.Network.Model
{
    using UnityEngine;
    using System;
    [CreateAssetMenu(menuName = "Config/NetworkConfig")]
    [Serializable]
    public class NetworkConfig : ScriptableObject
    {
        public string DevServerDomain;
        public string ProServerDomain;
        public byte MaxConnectionPerServer;
        public int RequestTimeout;
        public int ConnectionTimeout;
        public string EnSecKey;
        public string DeSecKey;
        public string DevEnSecKey;
        public string DevDeSecKey;
    }
}