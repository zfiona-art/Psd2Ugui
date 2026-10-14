using System;
using LitJson;

namespace Network.Base
{
    public class RequestOutput
    {
        public string output { get; set; }

        public Response BaseReponse
        {
            get
            {
                return Response<Response>();
            }
        }

        public T Response<T>()
        {
            if (!string.IsNullOrEmpty(output))
                try
                {
                    return JsonMapper.ToObject<T>(output);
                }
                catch (Exception)
                {
                }
            return default;
        }
    }

    public class Response
    {
        public int code;
        public string message;
    }

}