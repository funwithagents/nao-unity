using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace NaoUnity
{
    public class RobotRepresentationManager : MonoBehaviour
    {
        [SerializeField]
        private JointArticulationAssociations m_JointArticulationAssociations;

        private NaoWorld m_World;

        private bool m_JointUpdated = false;
        private Dictionary<string, float> m_NewJointAngles = null;

        // Start is called before the first frame update
        void Start()
        {
            m_World = FindObjectOfType<NaoWorld>();
            if (m_JointArticulationAssociations == null || m_JointArticulationAssociations.m_Associations == null)
            {
                Debug.LogError("RobotRepresentationManager: null joint articulation associations");
                enabled = false;
                return;
            }

            m_World.onJointsUpdated += OnJointsUpdated;
        }

        // Update is called once per frame
        void Update()
        {
            if (m_JointUpdated)
            {
                m_JointUpdated = false;
                ApplyJointAngles(m_NewJointAngles);
                m_NewJointAngles = null;
            }
        }

        private void OnJointsUpdated(Dictionary<string, float> jointAngles)
        {
            m_JointUpdated = true;
            m_NewJointAngles = jointAngles;
        }

        private void ApplyJointAngles(Dictionary<string, float> jointAngles)
        {
            foreach (JointArticulationAssociation asso in m_JointArticulationAssociations.m_Associations)
            {
                if (!jointAngles.ContainsKey(asso.m_JointName))
                {
                    Debug.LogError("RobotRepresentationManager: did not receive update for joint " + asso.m_JointName);
                    continue;
                }

                ApplyJointAngle(asso, jointAngles[asso.m_JointName] * Mathf.Rad2Deg);
            }
        }

        private void ApplyJointAngle(JointArticulationAssociation asso, float angleInDegrees)
        {
            if (asso.m_Articulation.jointType != ArticulationJointType.RevoluteJoint)
            {
                Debug.Log("Not applying anything to " + asso.m_JointName + " ==> jointType = " + asso.m_Articulation.jointType);
                return;
            }

            Quaternion newRotation = asso.m_Articulation.anchorRotation * Quaternion.Euler(angleInDegrees, 0f, 0f) * Quaternion.Inverse(asso.m_Articulation.anchorRotation);
            asso.m_Articulation.transform.localRotation = newRotation;
        }

        private void LogList<T>(List<T> list)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                sb.Append(list[i]);
                sb.Append(" ");
            }
            Debug.Log(sb.ToString());
        }
    }
}
