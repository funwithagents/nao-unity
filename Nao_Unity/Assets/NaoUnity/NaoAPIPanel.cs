using System;
using UnityEngine;

namespace NaoUnity
{
    // Holds the parameters of NaoAPI calls.
    // Buttons are drawn by NaoAPIPanelEditor, public methods can also be wired to UnityEvents.
    public class NaoAPIPanel : MonoBehaviour
    {
        #region Fields
        [Header("Speech")]
        public string m_TextToSay = "Hello, I am Nao";
        public NaoTTSLanguage m_TTSLanguage = NaoTTSLanguage.English;

        [Header("Eyes")]
        public NaoLedColor m_EyesColor = NaoLedColor.white;

        [Header("Basic awareness")]
        public bool m_BasicAwarenessEnabled = true;
        public BasicAwareness_EngagementMode m_EngagementMode = BasicAwareness_EngagementMode.FullyEngaged;
        public BasicAwareness_TrackingMode m_TrackingMode = BasicAwareness_TrackingMode.Head;

        [Header("Breathing")]
        public bool m_BreathingEnabled = true;
        public Breathing_ChainName m_BreathingChain = Breathing_ChainName.Body;

        [Header("Catalogs (ids)")]
        public string m_DanceId;
        public string m_AppId;
        public string m_BodyActionId;
        public string m_ReactionType;

        [Header("Behaviors")]
        public string m_BehaviorName;
        public string m_GenericNaoText;
        #endregion

        #region Properties
        public int PendingCount { get; private set; }
        #endregion

        #region API calls
        public void Say() => Call(r => NaoAPI.Say(m_TextToSay, r));
        public void StopSay() => Call(r => NaoAPI.StopSay(r));
        public void SetTTSLanguage() => Call(r => NaoAPI.SetTTSLanguage(m_TTSLanguage, r));

        public void WakeUp() => Call(r => NaoAPI.WakeUp(r));
        public void Rest() => Call(r => NaoAPI.Rest(r));
        public void StandUp() => Call(r => NaoAPI.StandUp(r));
        public void SitDown() => Call(r => NaoAPI.SitDown(r));

        public void ChangeEyesColor() => Call(r => NaoAPI.ChangeEyesColor(m_EyesColor, r));
        public void SetBasicAwarenessState() =>
            Call(r => NaoAPI.SetBasicAwarenessState(m_BasicAwarenessEnabled, m_EngagementMode, m_TrackingMode, r));
        public void SetBreathingEnabled() =>
            Call(r => NaoAPI.SetBreathingEnabled(m_BreathingEnabled, m_BreathingChain, r));

        public void GetDanceBehaviors() => Call(r => NaoAPI.GetDanceBehaviors(r));
        public void Dance() => Call(r => NaoAPI.Dance(m_DanceId, r));
        public void StopDance() => Call(r => NaoAPI.StopDance(m_DanceId, r));

        public void GetAppBehaviors() => Call(r => NaoAPI.GetAppBehaviors(r));
        public void RunApp() => Call(r => NaoAPI.RunApp(m_AppId, r));
        public void StopApp() => Call(r => NaoAPI.StopApp(m_AppId, r));

        public void GetBodyActionBehaviors() => Call(r => NaoAPI.GetBodyActionBehaviors(r));
        public void BodyAction() => Call(r => NaoAPI.BodyAction(m_BodyActionId, r));
        public void StopBodyAction() => Call(r => NaoAPI.StopBodyAction(m_BodyActionId, r));

        public void GetExpressiveReactionTypes() => Call(r => NaoAPI.GetExpressiveReactionTypes(r));
        public void ExpressiveReaction() => Call(r => NaoAPI.ExpressiveReaction(m_ReactionType, r));
        public void StopExpressiveReaction() => Call(r => NaoAPI.StopExpressiveReaction(m_ReactionType, r));

        public void RunBehavior() => Call(r => NaoAPI.RunBehavior(m_BehaviorName, r));
        public void StopBehavior() => Call(r => NaoAPI.StopBehavior(m_BehaviorName, r));
        public void StopCurrentBehavior() => Call(r => NaoAPI.StopCurrentBehavior(r));

        public void GenericNao() => Call(r => NaoAPI.GenericNao(m_GenericNaoText, r));
        #endregion

        #region Call
        private void Call(Action<Action<NaoCommandResult>> apiCall)
        {
            // NaoAPI relies on scene singletons: outside play mode they would be created in the edited scene
            if (!Application.isPlaying)
            {
                Debug.LogWarning("NaoAPIPanel: NaoAPI can only be called in play mode");
                return;
            }

            PendingCount++;
            apiCall(r => PendingCount--);
        }
        #endregion
    }
}
