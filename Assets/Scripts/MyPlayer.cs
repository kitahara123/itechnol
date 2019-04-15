using System;
using UnityEngine;

namespace ITechnol
{
    public class MyPlayer : MonoBehaviour
    {
        public string Name { get; set; }
        public float FinishTime { get; set; }

        public MyPlayer(string name, float finishTime)
        {
            Name = name;
            FinishTime = finishTime;
        }
    }
}