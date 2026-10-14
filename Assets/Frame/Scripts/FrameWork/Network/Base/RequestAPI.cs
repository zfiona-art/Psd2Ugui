namespace Network.Base
{
    public class RequestAPI<Operation, Input, Output>
        where Operation : RequestOperation, new ()
    where Input : RequestInput, new ()
    where Output : RequestOutput, new ()
    {
        public Operation operation;
        public Input input;
        public Output output;

        public RequestAPI ()
        {
            operation = new Operation ();
            input = new Input ();
            output = new Output ();
        }

    }
}