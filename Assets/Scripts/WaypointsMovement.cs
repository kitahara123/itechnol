using System.Collections.Generic;
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
    [SerializeField] private int startPointIndx = 0;
    [SerializeField] private int startTrackIndx = 0;

    private List<Curve> track => TrackManager.Curves;
    private int currentPointIndx = 0;
    private int currentTrackIndx = 0;
    private Vector3 startPoint => PointByIndex(startPointIndx);
    private Vector3 finishPoint => PointByIndex(track[currentTrackIndx].xyzPoints.Length - 1);
    private Vector3 currentPoint => PointByIndex(currentPointIndx);
    private bool isNextPointSkipPoint => indxToSkip >= currentPointIndx;

    private Quaternion initialRotation;
    private Rigidbody rb;
    private int indxToSkip;
    private bool finished = false;

    private void Start()
    {
        transform.position = startPoint;
        currentPointIndx = startPointIndx;
        currentTrackIndx = startTrackIndx;
        initialRotation = transform.rotation;
        rb = GetComponent<Rigidbody>();
    }


    private void Update()
    {
        if (finished) return;
        if (currentPoint == finishPoint)
        {
            finished = true;
            rb.velocity = Vector3.zero;
            transform.position = finishPoint;

            var player = GetComponent<Player>();
            if (player != null)
                Messenger<Player>.Broadcast(GameEvent.PLAYER_FINISHED, player);
            return;
        }

        var inputVert = Input.GetAxis("Vertical");

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
}