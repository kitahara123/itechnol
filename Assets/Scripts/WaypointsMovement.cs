using System.Collections.Generic;
using ITechnol;
using UnityEngine;

public class WaypointsMovement : MonoBehaviour
{
    [SerializeField] private float speed = 2000;

    [Tooltip("less number = more speed")] [SerializeField]
    private float rotationSpeedModifier = 200f;

    [SerializeField] private float maxSpeed = 4000;
    [SerializeField] private float minSpeed = 1000;
    [SerializeField] private float touchOffset = 1f;
    [SerializeField] private float almostThereOffset = 5f;
    [SerializeField] private int strafePointsSkip = 15;
    [SerializeField] private int speedDelta = 5;

    private List<Curve> track => TrackManager.Curves;
    private int currentPointIndx = 0;
    private int currentTrackIndx = 0;

    private Vector3 finishPoint => PointByIndex(track[currentTrackIndx].xyzPoints.Length - 1);
    private Vector3 currentPoint => PointByIndex(currentPointIndx);
    private bool isNextPointSkipPoint => indxToSkip >= currentPointIndx;

    private Quaternion initialRotation;
    private Rigidbody rb;
    private int indxToSkip;
    private bool inRace = false;

    private void Start()
    {
        initialRotation = transform.rotation;
        rb = GetComponent<Rigidbody>();
    }

    public GameObject Param(int startTrackIndx, int startPointIndx)
    {
        currentTrackIndx = startTrackIndx;
        currentPointIndx = startPointIndx;
        return this.gameObject;
    }

    private void Update()
    {
        if (!GameManager.RaceIsActive) return;
        var inputVert = Input.GetAxis("Vertical");

        if (!inRace)
        {
            inRace = inputVert > 0;
            return;
        }

        if (currentPoint == finishPoint)
        {
            inRace = false;
            rb.velocity = Vector3.zero;
            transform.position = finishPoint;

            var player = GetComponent<MyPlayer>();
            if (player != null)
                Messenger<MyPlayer>.Broadcast(GameEvent.PLAYER_FINISHED, player);
            return;
        }

        if (inputVert != 0 && Time.frameCount % 25 == 0)
        {
            if (inputVert > 0)
                speed = speed + speedDelta >= maxSpeed ? maxSpeed : speed + speedDelta;
            else
                speed = speed - speedDelta <= minSpeed ? minSpeed : speed - speedDelta;
        }

        var distanceToNextPoint = Vector3.Distance(transform.position, NextPoint());

        if (Input.GetKeyUp("left") &&
            (!isNextPointSkipPoint ||
             (isNextPointSkipPoint && distanceToNextPoint > almostThereOffset))) StrafeLeft();

        if (Input.GetKeyUp("right") &&
            (!isNextPointSkipPoint ||
             (isNextPointSkipPoint && distanceToNextPoint > almostThereOffset))) StrafeRight();

        if (distanceToNextPoint > touchOffset)
        {
            rb.velocity = transform.up * -speed * Time.deltaTime;
        }
        else
        {
            ShiftCurrentPoint(1);
        }

        var direction = Quaternion.LookRotation(NextPoint() - transform.position) * initialRotation;
        transform.rotation =
            Quaternion.Lerp(transform.rotation, direction, speed / rotationSpeedModifier * Time.deltaTime);
    }

    private void StrafeLeft()
    {
        if (currentTrackIndx <= 0) return;
        currentTrackIndx -= 1;

        if (indxToSkip < currentPointIndx)
        {
            ShiftCurrentPoint(strafePointsSkip);
            indxToSkip = currentPointIndx;
        }
    }

    private void StrafeRight()
    {
        if (currentTrackIndx >= track.Count - 1) return;
        currentTrackIndx += 1;

        if (indxToSkip < currentPointIndx)
        {
            ShiftCurrentPoint(strafePointsSkip);
            indxToSkip = currentPointIndx;
        }
    }

    private void ShiftCurrentPoint(int shiftBy)
    {
        if (currentPointIndx + shiftBy >= track[currentTrackIndx].xyzPoints.Length) return;
        currentPointIndx += shiftBy;
    }

    private Vector3 NextPoint()
    {
        return track[currentTrackIndx].xyzPoints.Length - 1 > currentPointIndx
            ? PointByIndex(currentPointIndx + 1)
            : PointByIndex(currentPointIndx);
    }

    private Vector3 PointByIndex(int indx) => track[currentTrackIndx].xyzPoints[indx];

    private void OnCollisionEnter(Collision other)
    {
        var direction = other.transform.position - transform.position;
        GetComponent<Rigidbody>().AddForce(-direction * other.gameObject.GetComponent<Rigidbody>().mass);
    }
}