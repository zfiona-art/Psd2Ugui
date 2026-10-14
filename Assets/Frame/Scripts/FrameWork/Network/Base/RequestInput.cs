namespace Network.Base
{
    using LitJson;
    
    public class RequestInput
    {
        public virtual string ToJson ()
        {
            return JsonMapper.ToJson (this);
        }
    }
}