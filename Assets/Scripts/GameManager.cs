using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI congratsWinner;
    [SerializeField] private TextMeshProUGUI timerLabel;
    [SerializeField] private TextMeshProUGUI finisherPlaceLabelPref;
    [SerializeField] private Transform finishersList;

    private HashSet<Player> players;
    private Queue<Player> finishQueue;
    private float timer;
    private bool raceIsDone = false;

    private void Awake()
    {
        players = new HashSet<Player>();
        finishQueue = new Queue<Player>();
        Messenger<Player>.AddListener(GameEvent.PLAYER_FINISHED, OnPlayerFinished);
        Messenger<Player>.AddListener(GameEvent.PLAYER_JOINED, OnPlayerJoined);
    }

    private void OnPlayerJoined(Player player)
    {
        players.Add(player);
    }

    private void OnPlayerFinished(Player player)
    {
        player.FinishTime = timer;
        if (finishQueue.Count == 0)
        {
            congratsWinner.gameObject.SetActive(true);
            congratsWinner.text = $"WINNER WINNER CHICKEN DINNER {player.Name}";
        }

        if (finishQueue.Contains(player)) return;
        finishQueue.Enqueue(player);
    }

    private void Update()
    {
        if (raceIsDone) return;
        if (finishQueue.Count == players.Count)
        {
            ShowFinishersList();
            raceIsDone = true;
            return;
        }
        timer += Time.deltaTime;
        timerLabel.text = $"{(int) timer}";
    }

    private void ShowFinishersList()
    {
        int i = 1;
        foreach (var player in finishQueue)
        {
            var label = Instantiate(finisherPlaceLabelPref);
            label.text = $"{i}. Name: {player.Name} Time: {player.FinishTime}";
            label.transform.SetParent(finishersList);
            i++;
        }
        finishersList.gameObject.SetActive(true);
    }
}