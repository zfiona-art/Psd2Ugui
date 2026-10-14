namespace Network.Base
{
    using System;
    using Network.Manager;
    using System.Collections.Generic;
    using GameConfig.Network;
    using FrameWork.Log;

    public delegate void RequestSuccess<RequestOutput>(RequestOutput output);
    public delegate void RequestFail(NetworkError error);
    public class ClientBaseRequest<Operation, Input, Output> where Operation : RequestOperation, new() where Input : RequestInput, new() where Output : RequestOutput, new()
    {
        public RequestAPI<Operation, Input, Output> RequestAPI;
        private Dictionary<string, string> header;
        public ClientBaseRequest()
        {
            RequestAPI = new RequestAPI<Operation, Input, Output>();
        }

        public string RequestWithEnc(RequestSuccess<Output> onSuccess = null, RequestFail onFail = null)
        {
            return Request(onSuccess, onFail, true);
        }

        public string Request(RequestSuccess<Output> onSuccess = null, RequestFail onFail = null, bool enc = false)
        {
            if (RequestAPI == null)
            {
                throw new NullReferenceException();
            }
            if (RequestAPI.operation.Method == Method.Post)
                return PostRequst(onSuccess, onFail, enc);
            else
                return GetRequst(onSuccess, onFail, enc);
        }

        private string PostRequst(RequestSuccess<Output> onSuccess = null, RequestFail onFail = null, bool enc = false)
        {

            RequestInput input = RequestAPI.input;
            string body = input.ToJson();
            // Debugger.TypeLog("request with body :" + body, DebuggerType.Network);
            if (enc)
            {
                byte[] rgbKey = NetworkConfigLoader.Instance.GetEnBytes();
                body = AESUtils.AESEncrtyptToStr(body, rgbKey, rgbKey);
            }
            // Debugger.TypeLog("request with body :" + body, DebuggerType.Network);

            Uri uri = new Uri(Path());
            void successHandler(string result)
            {
                Output output = new();
                if (enc)
                {
                    byte[] rgbKey = NetworkConfigLoader.Instance.GetDeBytes();
                    result = AESUtils.AESDecryptToStr(result, rgbKey, rgbKey);
                }
                output.output = result;
                Debugger.TypeLog(DebuggerType.Network, "post resp: " + result);
                onSuccess?.Invoke(output);
                onFail = null;
            }

            void failHandler(NetworkError failError)
            {
                onFail?.Invoke(failError);
                onSuccess = null;
            }
            return NetworkManager.Instance.PostRequest(uri, body, header, successHandler, failHandler);
        }

        private string GetRequst(RequestSuccess<Output> onSuccess = null, RequestFail onFail = null, bool enc = false)
        {
            RequestInput input = RequestAPI.input;
            string body = input.ToJson();
            // Debugger.TypeLog((object)("request with body : " + body), DebuggerType.Network);

            Uri uri = new Uri(Path());

            void successHandler(string result)
            {
                Output output = new();
                if (enc)
                {
                    byte[] rgbKey = NetworkConfigLoader.Instance.GetDeBytes();
                    result = AESUtils.AESDecryptToStr(result, rgbKey, rgbKey);
                }
                output.output = result;
                Debugger.TypeLog(DebuggerType.Network, "requst resp: " + result);

                onSuccess?.Invoke(output);
                onFail = null;
            }

            void failHandler(NetworkError failError)
            {
                onFail?.Invoke(failError);
                onSuccess = null;
            }

            return NetworkManager.Instance.GetRequest(uri, header, successHandler, failHandler);
        }

        public void ConfigHttpHeader(string token)
        {
            if (header == null)
                header = new();
            header["token"] = token;
            header["Content-Type"] = "application/json";
        }

        private string Path()
        {
            return ApiDomain() + RequestAPI.operation.Route;
        }

        private string ApiDomain()
        {
            return NetworkConfigLoader.Instance.GetServerDomain();
        }
    }

}