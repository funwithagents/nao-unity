using System;
using System.Collections.Generic;
using UnityEngine;

namespace NaoUnity
{
    public class JointArticulationAssociations : MonoBehaviour
    {
        public List<JointArticulationAssociation> m_Associations;
    }

    [Serializable]
    public class JointArticulationAssociation
    {
        public string m_JointName;
        public CustomArticulation m_Articulation;
    }
}
