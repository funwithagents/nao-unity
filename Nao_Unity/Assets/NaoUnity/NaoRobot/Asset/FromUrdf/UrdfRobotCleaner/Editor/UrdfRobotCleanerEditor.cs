using Unity.Robotics.UrdfImporter;
using UnityEditor;
using UnityEngine;

namespace NaoUnity
{
    [CustomEditor(typeof(UrdfRobotCleaner))]
    public class UrdfRobotCleanerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Clean URDF"))
            {
                DeleteComponentsInChildren<UrdfVisuals>();
                DeleteComponentsInChildren<UrdfVisual>();
                DeleteComponentsInChildren<UrdfInertial>();
                DeleteComponentsInChildren<UrdfJoint>();
                DeleteComponentsInChildren<UrdfPlugin>();
                DeleteComponentsInChildren<UrdfLink>();

                ArticulationBody[] articulationBodies = Selection.activeGameObject.GetComponentsInChildren<ArticulationBody>();
                foreach (ArticulationBody articulationBody in articulationBodies)
                {
                    CustomArticulation existingCustomArticulation = articulationBody.GetComponent<CustomArticulation>();
                    if (existingCustomArticulation != null)
                        DestroyImmediate(existingCustomArticulation);

                    CustomArticulation customArticulation = articulationBody.gameObject.AddComponent<CustomArticulation>();
                    customArticulation.anchorPosition = articulationBody.anchorPosition;
                    customArticulation.anchorRotation = articulationBody.anchorRotation;
                    customArticulation.jointType = articulationBody.jointType;

                    customArticulation.xLockType = articulationBody.linearLockX;
                    customArticulation.xDriveLimits = new ArticulationLimits(articulationBody.xDrive.lowerLimit,
                                                                             articulationBody.xDrive.upperLimit);
                    customArticulation.yLockType = articulationBody.linearLockY;
                    customArticulation.yDriveLimits = new ArticulationLimits(articulationBody.yDrive.lowerLimit,
                                                                             articulationBody.yDrive.upperLimit);
                    customArticulation.zLockType = articulationBody.linearLockZ;
                    customArticulation.zDriveLimits = new ArticulationLimits(articulationBody.zDrive.lowerLimit,
                                                                             articulationBody.zDrive.upperLimit);

                    DestroyImmediate(articulationBody);
                }
            }
        }

        private void DeleteComponentsInChildren<T>() where T : Component
        {
            T[] components = Selection.activeGameObject.GetComponentsInChildren<T>();
            foreach (T component in components)
            {
                DestroyImmediate(component);
            }
        }
    }
}
