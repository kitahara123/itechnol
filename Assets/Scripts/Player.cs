using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private string name;

    public string Name => name;
    public float FinishTime { get; set; }

    private void Start()
    {
        Messenger<Player>.Broadcast(GameEvent.PLAYER_JOINED, this);
    }
}