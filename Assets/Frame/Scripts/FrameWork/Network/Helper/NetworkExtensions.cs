using System;
using System.Text;
using BestHTTP.Extensions;

namespace Network.Helper
{
    public static class NetworkExtensions
    {
        public static UriBuilder Encrpt(this UriBuilder builder, string body)
        {
            builder.Query = "accesskey=" + body.MD5EncrptWithSand();
            return builder;
        }

        public static UriBuilder Query(this UriBuilder builder, string queryKey, string queryValue)
        {
            builder.Query = queryKey + "=" + queryValue;
            return builder;
        }

        public static string MD5EncrptWithSand(this string body)
        {
            return (body + "ATX".CalculateMD5Hash()).CalculateMD5Hash();
        }
        
        public static string CalculateMD5Hash(this string input)
        {
            return input.GetUtfBytes().CalculateMD5Hash();
        }
        
        public static byte[] GetUtfBytes(this string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }
    }
}