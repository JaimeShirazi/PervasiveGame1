using UnityEngine;
using Dan.Main;
using System.Collections.Generic;

public class PlaceholderNetworkManager : MonoBehaviour, INetworkManager
{
    private struct GameEntry
    {
        public GameEntry(int id, string question){
            Id = id;
            Question = question;
        }

        public int Id {get;}
        public string Question {get;}
    }
    private List<GameEntry> _privateEntries;
    void Start()
    {
        LoadEntries();
        GameStateManager.SetNetworkManager(this);
    }
    public string ReceiveQuestion(int index)
    {
        Debug.Log("Requesting question for index " + index);
        List<GameEntry> validList = new List<GameEntry>();
        foreach (GameEntry entry in _privateEntries){
            Debug.Log("Entry found " + entry.Question);
            if (entry.Id == index)
                validList.Add(entry);
        }
        if (validList.Count > 0){
            return validList[Random.Range(0,validList.Count)].Question;
        }
        return "Placeholder question?";
    }
    public void SendQuestion(int index, string question)
    {
        Debug.Log("Sending question \"" + question + "\" with index " + index);
        Leaderboards.PervasiveOneGame.UploadNewEntry(question, index, isSuccessful =>
        {
            if (isSuccessful)
                Debug.Log("Question \"" + question + "\" with index " + index + "sent to server.");
        });
    }

    private void LoadEntries()
    {
        _privateEntries = new List<GameEntry>();
        Leaderboards.PervasiveOneGame.GetEntries(entries =>
        {
            for (int i = 0; i < entries.Length; i++){
                Debug.Log("Starting entry found " + entries[i].Username);
                _privateEntries.Add(new GameEntry(entries[i].Score,entries[i].Username));
            }
        });
    }
}
