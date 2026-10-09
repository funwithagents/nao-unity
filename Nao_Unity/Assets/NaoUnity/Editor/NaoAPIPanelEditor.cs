using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NaoUnity
{
    [CustomEditor(typeof(NaoAPIPanel))]
    public class NaoAPIPanelEditor : Editor
    {
        #region Fields
        private static bool s_StatusFoldout = true;
        private static bool s_SpeechFoldout = true;
        private static bool s_PostureFoldout = true;
        private static bool s_EyesFoldout = false;
        private static bool s_AwarenessFoldout = false;
        private static bool s_BreathingFoldout = false;
        private static bool s_DancesFoldout = false;
        private static bool s_AppsFoldout = false;
        private static bool s_BodyActionsFoldout = false;
        private static bool s_ReactionsFoldout = false;
        private static bool s_BehaviorsFoldout = false;

        private NaoAPIPanel m_Panel;
        private bool m_CanCall;
        #endregion

        // Live status and results are updated from websocket messages
        public override bool RequiresConstantRepaint() => Application.isPlaying;

        public override void OnInspectorGUI()
        {
            m_Panel = (NaoAPIPanel)target;
            serializedObject.Update();

            // Do not touch NaoWorld.Instance outside play mode: it would create the singleton in the edited scene
            NaoWorld world = Application.isPlaying && !NaoWorld.Quitting ? NaoWorld.Instance : null;
            m_CanCall = world != null && world.ConnectedToNao;

            if (!Application.isPlaying)
                EditorGUILayout.HelpBox("Enter play mode to call NaoAPI.", MessageType.Info);
            else if (!m_CanCall)
                EditorGUILayout.HelpBox("Not connected to Nao.", MessageType.Warning);

            DrawStatus(world);

            if (Foldout(ref s_SpeechFoldout, "Speech"))
            {
                Property("m_TextToSay");
                ButtonRow(("Say", m_Panel.Say), ("Stop Say", m_Panel.StopSay));
                Property("m_TTSLanguage");
                ButtonRow(("Set TTS Language", m_Panel.SetTTSLanguage));
                EndFoldout();
            }

            if (Foldout(ref s_PostureFoldout, "Posture"))
            {
                ButtonRow(("Wake Up", m_Panel.WakeUp), ("Rest", m_Panel.Rest));
                ButtonRow(("Stand Up", m_Panel.StandUp), ("Sit Down", m_Panel.SitDown));
                EndFoldout();
            }

            if (Foldout(ref s_EyesFoldout, "Eyes"))
            {
                Property("m_EyesColor");
                ButtonRow(("Change Eyes Color", m_Panel.ChangeEyesColor));
                EndFoldout();
            }

            if (Foldout(ref s_AwarenessFoldout, "Basic Awareness"))
            {
                Property("m_BasicAwarenessEnabled", "Enabled");
                Property("m_EngagementMode");
                Property("m_TrackingMode");
                ButtonRow(("Set Basic Awareness State", m_Panel.SetBasicAwarenessState));
                EndFoldout();
            }

            if (Foldout(ref s_BreathingFoldout, "Breathing"))
            {
                Property("m_BreathingEnabled", "Enabled");
                Property("m_BreathingChain", "Chain");
                ButtonRow(("Set Breathing Enabled", m_Panel.SetBreathingEnabled));
                EndFoldout();
            }

            DrawCatalog(ref s_DancesFoldout, "Dances", "m_DanceId", BehaviorIds(NaoAPI.AvailableDances), BehaviorLabels(NaoAPI.AvailableDances),
                        m_Panel.GetDanceBehaviors, m_Panel.Dance, m_Panel.StopDance);
            DrawCatalog(ref s_AppsFoldout, "Apps", "m_AppId", BehaviorIds(NaoAPI.AvailableApps), BehaviorLabels(NaoAPI.AvailableApps),
                        m_Panel.GetAppBehaviors, m_Panel.RunApp, m_Panel.StopApp);
            DrawCatalog(ref s_BodyActionsFoldout, "Body Actions", "m_BodyActionId", BehaviorIds(NaoAPI.AvailableBodyActions), BehaviorLabels(NaoAPI.AvailableBodyActions),
                        m_Panel.GetBodyActionBehaviors, m_Panel.BodyAction, m_Panel.StopBodyAction);
            DrawCatalog(ref s_ReactionsFoldout, "Expressive Reactions", "m_ReactionType", NaoAPI.AvailableReactionTypes, NaoAPI.AvailableReactionTypes,
                        m_Panel.GetExpressiveReactionTypes, m_Panel.ExpressiveReaction, m_Panel.StopExpressiveReaction);

            if (Foldout(ref s_BehaviorsFoldout, "Behaviors"))
            {
                Property("m_BehaviorName");
                ButtonRow(("Run Behavior", m_Panel.RunBehavior), ("Stop Behavior", m_Panel.StopBehavior));
                ButtonRow(("Stop Current Behavior", m_Panel.StopCurrentBehavior));
                Property("m_GenericNaoText");
                ButtonRow(("Generic Nao", m_Panel.GenericNao));
                EndFoldout();
            }

            serializedObject.ApplyModifiedProperties();
        }

        #region Sections
        private void DrawStatus(NaoWorld world)
        {
            if (world == null || !Foldout(ref s_StatusFoldout, "Status"))
                return;

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.Toggle("Connected", world.ConnectedToNao);
                EditorGUILayout.Toggle("Fake Robot", world.FakeRobot);
                EditorGUILayout.EnumPopup("Posture", world.CurrentPosture);
                EditorGUILayout.Toggle("Is Talking", world.IsTalking);
                EditorGUILayout.EnumPopup("TTS Language", world.CurrentTTSLanguage);
                EditorGUILayout.TextField("Current Behavior", world.CurrentBehavior ?? "");
                EditorGUILayout.IntField("Pending Commands", m_Panel.PendingCount);
            }
            EndFoldout();
        }

        private void DrawCatalog(ref bool foldout, string title, string propertyName,
                                 List<string> ids, List<string> labels,
                                 Action fetch, Action run, Action stop)
        {
            if (!Foldout(ref foldout, title))
                return;

            ButtonRow(("Fetch List", fetch));

            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (ids.Count == 0)
            {
                // No list fetched yet: the id can still be typed manually
                EditorGUILayout.PropertyField(property, new GUIContent("Id"));
            }
            else
            {
                int index = Mathf.Max(0, ids.IndexOf(property.stringValue));
                index = EditorGUILayout.Popup("Id", index, labels.ToArray());
                property.stringValue = ids[index];
            }

            ButtonRow(("Run", run), ("Stop", stop));
            EndFoldout();
        }
        #endregion

        #region Helpers
        private static List<string> BehaviorIds(List<BehaviorInfos> behaviors)
        {
            return behaviors.Select(b => b.m_Id).ToList();
        }

        private static List<string> BehaviorLabels(List<BehaviorInfos> behaviors)
        {
            return behaviors.Select(b => string.IsNullOrEmpty(b.m_LocalizedName?.en_US) ? b.m_Id : b.m_LocalizedName.en_US)
                            .ToList();
        }

        private static bool Foldout(ref bool foldout, string title)
        {
            foldout = EditorGUILayout.BeginFoldoutHeaderGroup(foldout, title);
            if (!foldout)
            {
                EditorGUILayout.EndFoldoutHeaderGroup();
                return false;
            }
            EditorGUI.indentLevel++;
            return true;
        }

        private static void EndFoldout()
        {
            EditorGUI.indentLevel--;
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Space();
        }

        private void Property(string propertyName, string label = null)
        {
            SerializedProperty property = serializedObject.FindProperty(propertyName);
            if (label == null)
                EditorGUILayout.PropertyField(property);
            else
                EditorGUILayout.PropertyField(property, new GUIContent(label));
        }

        private void ButtonRow(params (string label, Action action)[] buttons)
        {
            using (new EditorGUI.DisabledScope(!m_CanCall))
            using (new EditorGUILayout.HorizontalScope())
            {
                foreach ((string label, Action action) in buttons)
                {
                    if (GUILayout.Button(label))
                    {
                        // Make sure the panel reads the values currently displayed
                        serializedObject.ApplyModifiedProperties();
                        action();
                    }
                }
            }
        }
        #endregion
    }
}
