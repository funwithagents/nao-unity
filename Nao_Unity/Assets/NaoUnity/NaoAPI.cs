using System;
using System.Collections.Generic;

namespace NaoUnity
{
    public class NaoAPI
    {
        public static void GenericNao(string text, Action<NaoCommandResult> onResult = null)
        {
            NaoSender.Instance.ApplyCommandOnNao(
                new NaoMessageGenericNao(text),
                onResult);
        }

        public static void SetTTSLanguage(NaoTTSLanguage language, Action<NaoCommandResult> onResult = null)
        {
            NaoSender.Instance.ApplyCommandOnNao(
                new NaoMessageSetTTSLanguage(language),
                (r) =>
                {
                    NaoWorld.Instance.CurrentTTSLanguage = language;
                    onResult?.Invoke(r);
                });
        }
        public static void Say(string textToSay, Action<NaoCommandResult> onResult = null)
        {
            NaoWorld.Instance.IsTalking = true;
            NaoSender.Instance.ApplyCommandOnNao(
                new NaoMessageSay(textToSay),
                (r) =>
                {
                    if (r.m_Type != NaoCommandResult.ResultType.Cancel)
                        NaoWorld.Instance.IsTalking = false;
                    onResult?.Invoke(r);
                });
        }
        public static void StopSay(Action<NaoCommandResult> onResult = null)
        {
            NaoSender.Instance.ApplyCommandOnNao(
                new NaoMessageStopSay(),
                (r) =>
                {
                    NaoWorld.Instance.IsTalking = false;
                    onResult?.Invoke(r);
                });
        }

        public static void WakeUp(Action<NaoCommandResult> onResult = null)
        {
            NaoSender.Instance.ApplyCommandOnNao(
                new NaoMessageWakeUp(),
                onResult);
        }
        public static void Rest(Action<NaoCommandResult> onResult = null)
        {
            NaoSender.Instance.ApplyCommandOnNao(
                new NaoMessageRest(),
                onResult);
        }

        public static void StandUp(Action<NaoCommandResult> onResult = null)
        {
            NaoWorld.Instance.CurrentPosture = PostureType.Transition;
            NaoSender.Instance.ApplyCommandOnNao(
                new NaoMessageStandUp(),
                (r) =>
                {
                    NaoWorld.Instance.CurrentPosture = PostureType.Standing;
                    onResult?.Invoke(r);
                });
        }
        public static void SitDown(Action<NaoCommandResult> onResult = null)
        {
            NaoWorld.Instance.CurrentPosture = PostureType.Transition;
            NaoSender.Instance.ApplyCommandOnNao(
                new NaoMessageSitDown(),
                (r) =>
                {
                    NaoWorld.Instance.CurrentPosture = PostureType.Sitting;
                    onResult?.Invoke(r);
                });
        }

        public static void ChangeEyesColor(NaoLedColor color, Action<NaoCommandResult> onResult = null)
        {
            NaoSender.Instance.ApplyCommandOnNao(
                new NaoMessageChangeEyesColor(color),
                onResult);
        }

        public static void SetBasicAwarenessState(bool enabled,
                                                  BasicAwareness_EngagementMode engagementMode,
                                                  BasicAwareness_TrackingMode trackingMode,
                                                  Action<NaoCommandResult> onResult = null)
        {
            NaoSender.Instance.ApplyCommandOnNao(
                new NaoMessageSetBasicAwarenessState(enabled, engagementMode, trackingMode),
                onResult);
        }

        #region Dance
        public static List<BehaviorInfos> AvailableDances { get; private set; } = new List<BehaviorInfos>();
        public static void GetDanceBehaviors(Action<NaoCommandResult<List<BehaviorInfos>>> onResult = null)
        {
            NaoSender.Instance.ApplyCommandOnNao(
                new NaoMessageGetDanceBehaviors(),
                (r) =>
                {
                    NaoCommandResult<List<BehaviorInfos>> result = new NaoCommandResult<List<BehaviorInfos>>(r);
                    if (result.m_Type == NaoCommandResult.ResultType.Success && result.m_Data != null)
                        AvailableDances = result.m_Data;
                    onResult?.Invoke(result);
                });
        }
        public static void Dance(string danceId, Action<NaoCommandResult> onResult = null)
        {
            NaoSender.Instance.ApplyCommandOnNao(
                new NaoMessageDance(danceId),
                onResult);
        }
        public static void StopDance(string danceId, Action<NaoCommandResult> onResult = null)
        {
            NaoSender.Instance.ApplyCommandOnNao(
                new NaoMessageStopDance(danceId),
                onResult);
        }
        #endregion

        #region ExpressiveReaction
        public static List<string> AvailableReactionTypes { get; private set; } = new List<string>();
        public static void GetExpressiveReactionTypes(Action<NaoCommandResult<List<string>>> onResult = null)
        {
            NaoSender.Instance.ApplyCommandOnNao(
                new NaoMessageGetExpressiveReactionTypes(),
                (r) =>
                {
                    NaoCommandResult<List<string>> result = new NaoCommandResult<List<string>>(r);
                    if (result.m_Type == NaoCommandResult.ResultType.Success && result.m_Data != null)
                        AvailableReactionTypes = result.m_Data;
                    onResult?.Invoke(result);
                });
        }
        public static void ExpressiveReaction(string reactionType, Action<NaoCommandResult> onResult = null)
        {
            NaoSender.Instance.ApplyCommandOnNao(
                new NaoMessageExpressiveReaction(reactionType),
                onResult);
        }
        public static void StopExpressiveReaction(string reactionType, Action<NaoCommandResult> onResult = null)
        {
            NaoSender.Instance.ApplyCommandOnNao(
                new NaoMessageStopExpressiveReaction(reactionType),
                onResult);
        }
        #endregion

        #region App
        public static List<BehaviorInfos> AvailableApps { get; private set; } = new List<BehaviorInfos>();
        public static void GetAppBehaviors(Action<NaoCommandResult<List<BehaviorInfos>>> onResult = null)
        {
            NaoSender.Instance.ApplyCommandOnNao(
                new NaoMessageGetAppBehaviors(),
                (r) =>
                {
                    NaoCommandResult<List<BehaviorInfos>> result = new NaoCommandResult<List<BehaviorInfos>>(r);
                    if (result.m_Type == NaoCommandResult.ResultType.Success && result.m_Data != null)
                        AvailableApps = result.m_Data;
                    onResult?.Invoke(result);
                });
        }
        public static void RunApp(string appId, Action<NaoCommandResult> onResult = null)
        {
            NaoSender.Instance.ApplyCommandOnNao(
                new NaoMessageRunApp(appId),
                onResult);
        }
        public static void StopApp(string appId, Action<NaoCommandResult> onResult = null)
        {
            NaoSender.Instance.ApplyCommandOnNao(
                new NaoMessageStopApp(appId),
                onResult);
        }
        #endregion

        #region BodyAction
        public static List<BehaviorInfos> AvailableBodyActions { get; private set; } = new List<BehaviorInfos>();
        public static void GetBodyActionBehaviors(Action<NaoCommandResult<List<BehaviorInfos>>> onResult = null)
        {
            NaoSender.Instance.ApplyCommandOnNao(
                new NaoMessageGetBodyActionBehaviors(),
                (r) =>
                {
                    NaoCommandResult<List<BehaviorInfos>> result = new NaoCommandResult<List<BehaviorInfos>>(r);
                    if (result.m_Type == NaoCommandResult.ResultType.Success && result.m_Data != null)
                        AvailableBodyActions = result.m_Data;
                    onResult?.Invoke(result);
                });
        }
        public static void BodyAction(string bodyActionId, Action<NaoCommandResult> onResult = null)
        {
            NaoSender.Instance.ApplyCommandOnNao(
                new NaoMessageBodyAction(bodyActionId),
                onResult);
        }
        public static void StopBodyAction(string bodyActionId, Action<NaoCommandResult> onResult = null)
        {
            NaoSender.Instance.ApplyCommandOnNao(
                new NaoMessageStopBodyAction(bodyActionId),
                onResult);
        }
        #endregion

        public static void SetBreathingEnabled(bool enabled,
                                               Breathing_ChainName chainName,
                                               Action<NaoCommandResult> onResult = null)
        {
            NaoSender.Instance.ApplyCommandOnNao(
                new NaoMessageSetBreathingEnabled(enabled, chainName),
                onResult);
        }

        public static void RunBehavior(string behaviorName, Action<NaoCommandResult> onResult = null)
        {
            if (!string.IsNullOrEmpty(behaviorName))
            {
                NaoWorld.Instance.CurrentBehavior = behaviorName;
                NaoSender.Instance.ApplyCommandOnNao(
                    new NaoMessageRunBehavior(behaviorName),
                    (r) =>
                    {
                        NaoWorld.Instance.CurrentBehavior = null;
                        onResult?.Invoke(r);
                    });
            }
            else
                onResult(new NaoCommandResult(NaoCommandResult.ResultType.Error, "Empty or null behavior name"));
        }
        public static void StopBehavior(string behaviorName, Action<NaoCommandResult> onResult = null)
        {
            NaoSender.Instance.ApplyCommandOnNao(
                new NaoMessageStopBehavior(behaviorName),
                (r) =>
                {
                    NaoWorld.Instance.CurrentBehavior = null;
                    onResult?.Invoke(r);
                });
        }

        public static void StopCurrentBehavior(Action<NaoCommandResult> onResult = null)
        {
            if (!string.IsNullOrEmpty(NaoWorld.Instance.CurrentBehavior))
                StopBehavior(NaoWorld.Instance.CurrentBehavior, onResult);
            else
                onResult(new NaoCommandResult(NaoCommandResult.ResultType.Success, ""));
        }
    }
}
