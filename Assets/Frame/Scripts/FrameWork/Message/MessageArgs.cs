using System;
namespace FrameWork.Message
{
    public class MessageArgs : EventArgs
    {
        public object[] ParamArray
        {
            get; set;
        }
}
}