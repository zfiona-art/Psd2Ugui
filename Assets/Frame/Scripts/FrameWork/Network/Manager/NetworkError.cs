
namespace Network.Manager
{
    using System;
    public struct NetworkError
    {
        public ErrorState state;
        public int code;
        public string message;

        public override string ToString()
        {
            return String.Format("Request state :{0}\n Return code:{1}\n  message:{2}", state.ToString(), code, message);
        }
    }

    public enum ErrorState
    {
        //未知错误
        Unknown,

        //请求错误
        Error,

        //请求取消
        Aborted,

        //服务端请求超时
        ConnectionTimedOut,

        //客户端请求超时
        TimedOut,

        DefaultError
    }

    public class ErrorMessageEntity
    {
        public string code;
        public string message;

    }
}