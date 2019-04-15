using System;
using UnityEngine;

namespace ITechnol
{
    public class MyPlayer : MonoBehaviour
    {
        [SerializeField] private string name;


        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public float FinishTime { get; set; }

        public MyPlayer(string name, float finishTime)
        {
            this.name = name;
            FinishTime = finishTime;
        }
    }
}