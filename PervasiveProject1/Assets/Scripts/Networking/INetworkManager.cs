public interface INetworkManager
{
    public string ReceiveQuestion(int index);
    public void SendQuestion(int index, string question);
}