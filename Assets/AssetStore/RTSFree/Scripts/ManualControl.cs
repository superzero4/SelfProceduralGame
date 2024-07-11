using UnityEngine;

namespace RTSToolkitFree
{
    public class ManualControl : MonoBehaviour
    {
        public bool isSelected;
        public bool prepareMoving;
        public bool isMoving;

        [HideInInspector] public float prevDist;
        [HideInInspector] public int failedDist;
        public int critFailedDist = 10;

        public Vector3 manualDestination;
    }
}
