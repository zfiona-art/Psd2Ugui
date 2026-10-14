namespace Network.Base
{
    public enum Method
    {
        Post,
        Get
    }
    public abstract class RequestOperation
    {
        public abstract string Route { get; }
        public abstract Method Method { get; }
    }
}