using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using ITechnol;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum PhotonEventCodes
{
    PlayerFinished = 0,
    StartRace
}

public class GameManager : MonoBehaviourPunCallbacks, IOnEventCallback
{
    [SerializeField] private TextMeshProUGUI congratsWinner;
    [SerializeField] private TextMeshProUGUI timerLabel;
    [SerializeField] private float timerFontSize;
    [SerializeField] private TextMeshProUGUI finisherPlaceLabelPref;
    [SerializeField] private Transform finishersList;
    [SerializeField] private int startPointIndx = 0;
    [SerializeField] private float countdown = 5;

    private Queue<MyPlayer> finishQueue;
    private float timer;
    private static bool raceIsActive = false;
    public static bool RaceIsActive => raceIsActive;
    private MyPlayer player;

    private void Awake()
    {
        Messenger<MyPlayer>.AddListener(GameEvent.PLAYER_FINISHED, OnPlayerFinished);
    }

    private void Start()
    {
        var startTrackIndx = PhotonNetwork.CurrentRoom.PlayerCount - 1;
        var playerGo = PhotonNetwork.Instantiate("PlayerPref",
            TrackManager.PointByIndx(startTrackIndx, startPointIndx), Quaternion.identity);
        playerGo.GetComponentInChildren<WaypointsMovement>().Param(startTrackIndx, startPointIndx);
        player = playerGo.GetComponentInChildren<MyPlayer>();
        player.Name = PhotonNetwork.NickName;

        finishQueue = new Queue<MyPlayer>();
        timerFontSize = timerLabel.fontSize;

        if (PhotonNetwork.CurrentRoom.MaxPlayers == 1) StartCoroutine(Countdown());
    }

    private void OnPlayerFinished(MyPlayer player)
    {
        player.FinishTime = timer;
        if (finishQueue.Count == 0)
        {
            congratsWinner.gameObject.SetActive(true);
            congratsWinner.text = $"WINNER WINNER CHICKEN DINNER {player.Name}";
        }

        if (finishQueue.Contains(player)) return;
        finishQueue.Enqueue(player);

        var options = new RaiseEventOptions()
        {
            CachingOption = EventCaching.DoNotCache,
            Receivers = ReceiverGroup.Others
        };
        PhotonNetwork.RaiseEvent((byte) PhotonEventCodes.PlayerFinished,
            new object[] {player.Name, player.FinishTime},
            options, SendOptions.SendReliable);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Q)) PhotonNetwork.LeaveRoom();

        if (!raceIsActive) return;
        if (finishQueue.Count > 0 && finishQueue.Count == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            ShowFinishersList();
            raceIsActive = false;
            return;
        }

        timer += Time.deltaTime;
        timerLabel.text = $"{(int) timer}";
    }

    public override void OnPlayerEnteredRoom(Player newPLayer)
    {
        if (countdown > 0 && PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            PhotonNetwork.RaiseEvent((byte) PhotonEventCodes.StartRace, null, new RaiseEventOptions()
            {
                CachingOption = EventCaching.DoNotCache,
                Receivers = ReceiverGroup.All
            }, SendOptions.SendReliable);
        }
    }

    public override void OnLeftRoom() => SceneManager.LoadScene(0);

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

    private IEnumerator Countdown()
    {
        while (countdown > 0)
        {
            timerLabel.text = $"{(int) countdown}";
            countdown--;

            if (countdown < 3)
                timerLabel.fontSize = timerFontSize * Math.Abs(4 - (int) countdown) + 1;
            yield return new WaitForSeconds(1);
        }

        timerLabel.fontSize = timerFontSize;
        raceIsActive = true;
    }

    public void OnEvent(EventData photonEvent)
    {
        switch ((PhotonEventCodes) photonEvent.Code)
        {
            case PhotonEventCodes.PlayerFinished:
            {
                var data = photonEvent.CustomData as object[];

                finishQueue.Enqueue(new MyPlayer((string) data[0], (float) data[1]));
                break;
            }
            case PhotonEventCodes.StartRace:
                StartCoroutine(Countdown());
                break;
        }
    }
}