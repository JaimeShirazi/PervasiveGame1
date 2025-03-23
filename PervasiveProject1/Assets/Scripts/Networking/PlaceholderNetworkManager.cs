using UnityEngine;

public class PlaceholderNetworkManager : MonoBehaviour, INetworkManager
{
    void Start()
    {
        GameStateManager.SetNetworkManager(this);
    }
    public string ReceiveQuestion(int index)
    {
        Debug.Log("Requesting question for index " + index);
        return "Placeholder question?";
    }
    public void SendQuestion(int index, string question)
    {
        Debug.Log("Sending question \"" + question + "\" with index " + index);
    }
}
