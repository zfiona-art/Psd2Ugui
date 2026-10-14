using System;
using System.Collections.Generic;
using BestHTTP;
using FrameWork.Log;
using GameConfig.Network;
using LitJson;

namespace Network.Manager
{
    public class NetworkManager
    {
        private static NetworkManager instance = new NetworkManager();

        public static NetworkManager Instance
        {
            get { return instance; }
        }

        private Dictionary<string, HTTPRequest> httpRequestDic = new Dictionary<string, HTTPRequest>();
        private long lastRequestId = 0;

        public NetworkManager()
        {
            HTTPManager.MaxConnectionPerServer = NetworkConfigLoader.Instance.MaxConnectionPerServer;
            HTTPManager.RequestTimeout = TimeSpan.FromSeconds(NetworkConfigLoader.Instance.RequestTimeout);
            HTTPManager.ConnectTimeout = TimeSpan.FromSeconds(NetworkConfigLoader.Instance.ConnectionTimeout);
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="body"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onFail"></param>
        /// <returns></returns>
        public string PostRequest(Uri uri, string body, Dictionary<string, string> header, OnSuccess onSuccess,
            OnFail onFail = null)
        {
            lastRequestId++;
            string tag = lastRequestId.ToString();
            HTTPRequest request = new(uri, HTTPMethods.Post, (request, response) =>
            {
                Debugger.TypeLog(DebuggerType.Network, "request.Uri :  " + request.Uri.ToString());
                if (request.State == HTTPRequestStates.Finished && response.IsSuccess)
                {
                    string responseText = response.DataAsText;
                    onSuccess?.Invoke(responseText);
                    Debugger.TypeLog(DebuggerType.Network, "request succeed with response : " + responseText);
                    onFail = null;
                }
                else
                {
                    Debugger.TypeLog(DebuggerType.Network, "request fail");
                    HandleFailRequest(request, response, onFail);
                    onSuccess = null;
                }

                RemoveRequest(tag);
            });

            if (header != null)
            {
                foreach (var kv in header)
                {
                    request.AddHeader(kv.Key, kv.Value);
                }
            }

            request.RawData = System.Text.Encoding.UTF8.GetBytes(body);
            SendRequest(request, tag);
            return tag;
        }

        /// <summary>
        /// Get类型请求
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onFail"></param>
        /// <returns></returns>
        public string GetRequest(Uri uri, Dictionary<string, string> header, OnSuccess onSuccess, OnFail onFail = null)
        {
            lastRequestId++;
            string tag = lastRequestId.ToString();
            HTTPRequest request = new HTTPRequest(uri, (req, resp) =>
            {
                HandleTextCallback(req, resp, (string result) =>
                {
                    if (onSuccess != null)
                    {
                        onSuccess(result);
                    }
                }, onFail);
                RemoveRequest(tag);
            });
            if (header != null)
            {
                foreach (var kv in header)
                {
                    request.AddHeader(kv.Key, kv.Value);
                }
            }

            SendRequest(request, tag);
            return tag;
        }


        public void AbortAll()
        {
            foreach (var request in httpRequestDic.Values)
            {
                request.Abort();
            }

            Debugger.TypeLog(DebuggerType.Network, "request abort all!!!");
            httpRequestDic.Clear();
        }

        void AbortRequest(string tag)
        {
            HTTPRequest request;
            httpRequestDic.TryGetValue(tag, out request);
            if (request != null)
            {
                Debugger.TypeLog(DebuggerType.Network, "request abort: " + tag);
                request.Abort();
                httpRequestDic.Remove(tag);
            }
        }

        void AddRequest(string tag, HTTPRequest request)
        {
            if (httpRequestDic.ContainsKey(tag))
            {
                AbortRequest(tag);
            }

            httpRequestDic.Add(tag, request);
        }

        void HandleTextCallback(HTTPRequest request, HTTPResponse response, OnSuccess successCallback,
            OnFail failCallback)
        {
            Debugger.TypeLog(DebuggerType.Network, "HandleTextCallback request.Uri :  " + request.Uri.ToString());

            // 判断请求是否完成且响应成功
            if (request.State == HTTPRequestStates.Finished && response != null && response.IsSuccess)
            {
                string responseText = response.DataAsText;
                Debugger.TypeLog(DebuggerType.Network, "HandleTextCallback succeed with response : " + responseText);

                // 执行成功回调
                successCallback?.Invoke(responseText);
            }
            else
            {
                // 请求失败处理
                Debugger.TypeLog(DebuggerType.Network, "HandleTextCallback fail");
                HandleFailRequest(request, response, failCallback);
            }
        }

        /// <summary>
        /// 对失败请求的回调
        /// </summary>
        /// <param name="request"></param>
        /// <param name="failCallback"></param>
        void HandleFailRequest(HTTPRequest request, HTTPResponse response, OnFail failCallback)
        {
            try
            {
                ErrorState state = ErrorState.Unknown;
                int errorCode = response != null ? response.StatusCode : 0;
                string errorMsg = response != null ? response.Message : string.Empty;
                switch (request.State)
                {
                    case HTTPRequestStates.Error:
                        state = ErrorState.Error;
                        errorMsg = "Request Finished with Error! " + request.Exception != null
                            ? request.Exception.Message
                            : "No Exception";
                        break;

                    // The request aborted, initiated by the user.
                    case HTTPRequestStates.Aborted:
                        state = ErrorState.Aborted;
                        errorMsg = "Request Aborted!";
                        break;

                    // Ceonnecting to the server timed out.
                    case HTTPRequestStates.ConnectionTimedOut:
                        state = ErrorState.ConnectionTimedOut;
                        errorMsg = "Connection Timed Out!";
                        break;
                    // The request didn't finished in the given time.
                    case HTTPRequestStates.TimedOut:
                        state = ErrorState.TimedOut;
                        errorMsg = "Processing the request Timed Out!";
                        break;
                    case HTTPRequestStates.Finished:
                        ErrorMessageEntity errorMessageEntity = new ErrorMessageEntity();
                        try
                        {
                            Debugger.TypeLog(DebuggerType.Network, "error state : " + response.DataAsText.ToString());
                            JsonData jsonData = JsonMapper.ToObject(response.DataAsText);
                            string messageStr = jsonData["message"].ToString();
                            string codeStr = jsonData["code"].ToString();
                        }
                        catch (Exception ex)
                        {
                            Debugger.TypeLog(DebuggerType.Network, "err: " + ex);
                        }

                        state = ErrorState.DefaultError;
                        errorMsg = errorMessageEntity.message;
                        break;
                    default:
                        state = ErrorState.Unknown;
                        errorMsg = response.Message;
                        break;
                }

                NetworkError error = new NetworkError();
                error.code = errorCode;
                error.message = errorMsg;
                error.state = state;
                Debugger.TypeLog(DebuggerType.Network, "HandleFailRequest with error state : " +
                                                       error.state.ToString() +
                                                       "with error msg : " + error.message);
                failCallback?.Invoke(error);
            }
            catch (System.Exception e)
            {
                Debugger.TypeLog(DebuggerType.Network, "HandleFailRequest exception  : " + e.Message);
            }
        }

        void RemoveRequest(string tag)
        {
            HTTPRequest request;
            httpRequestDic.TryGetValue(tag, out request);
            if (request != null)
            {
                httpRequestDic.Remove(tag);
            }
        }


        private void SendRequest(HTTPRequest request, string tag)
        {
            AddRequest(tag, request);
            request.Send();
            Debugger.TypeLog(DebuggerType.Network, "url: " + request.Uri.ToString());
            if (request.MethodType == HTTPMethods.Post)
            {
                Debugger.TypeLog(DebuggerType.Network,
                    "Request Post Body : " + System.Text.Encoding.UTF8.GetString(request.RawData));
            }
        }
    }
}