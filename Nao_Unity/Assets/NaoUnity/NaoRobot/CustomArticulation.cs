using System;
using UnityEngine;

namespace NaoUnity
{
    public class CustomArticulation : MonoBehaviour
    {
        public Vector3 anchorPosition;
        public Quaternion anchorRotation;
        public ArticulationJointType jointType;

        public ArticulationDofLock xLockType;
        public ArticulationLimits xDriveLimits;
        public ArticulationDofLock yLockType;
        public ArticulationLimits yDriveLimits;
        public ArticulationDofLock zLockType;
        public ArticulationLimits zDriveLimits;
    }

    [Serializable]
    public class ArticulationLimits
    {
        public float lowerLimit;
        public float upperLimit;

        public ArticulationLimits(float lowerLimit_p, float upperLimit_p)
        {
            lowerLimit = lowerLimit_p;
            upperLimit = upperLimit_p;
        }
    }
}
