using UnityEngine;

namespace ITechnol
{
    public class IsMine : Photon.Pun.MonoBehaviourPun
    {
        private void Start()
        {
            if (!photonView.IsMine) return;
            GetComponentInChildren<WaypointsMovement>().enabled = true;
            GetComponentInChildren<Camera>().enabled = true;
        }
    }
}