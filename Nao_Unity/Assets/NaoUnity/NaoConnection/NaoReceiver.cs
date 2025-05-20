using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NaoUnity
{
    public class NaoReceiver : MonoBehaviourSingleton<NaoReceiver>
    {
        #region MonoBehaviour lifecycle functions
        // Start is called before the first frame update
        void Start()
        {
            NaoConnection.Instance.SubscribeToMessage(JointsMessage.ID, OnJointsMessageReceived);
            NaoConnection.Instance.SubscribeToMessage(TouchMessage.ID, OnTouchMessageReceived);
            NaoConnection.Instance.SubscribeToMessage(AudioMessage.ID, OnAudioMessageReceived);
        }
        #endregion

        #region Helpers

        #region Joints
        private class JointsMessage : NaoConnection.NaoConnectionMessage
        {
            public const string ID = "Joints";

            [JsonProperty("jointsNames")]
            public List<string> m_JointsNames;
            [JsonProperty("jointsAngles")]
            public List<float> m_JointsAngles;

            public override string GetMessageId() { return ID; }
        }

        private void OnJointsMessageReceived(string message)
        {
            JointsMessage jointsMessage = null;
            try
            {
                jointsMessage = JsonConvert.DeserializeObject<JointsMessage>(message);
            }
            catch (Exception e)
            {
                Debug.LogError($"NaoReceiver::OnJointsMessageReceived: failed to parse message received with error {e}");
                return;
            }

            Dictionary<string, float> joints = GetJointsFromMessage(jointsMessage);
            NaoWorld.Instance.JointAngles = joints;
        }

        private Dictionary<string, float> GetJointsFromMessage(JointsMessage message)
        {
            if (message == null || message.m_JointsNames == null || message.m_JointsAngles == null)
            {
                Debug.LogError("GetNaoJointsFromMessage: empty message or lists");
                return null;
            }

            int jointNamesCount = message.m_JointsNames.Count;
            int jointAnglesCount = message.m_JointsAngles.Count;

            if (jointNamesCount != jointAnglesCount)
            {
                Debug.LogError("GetNaoJointsFromMessage: lists do not have the same size");
                return null;
            }

            Dictionary<string, float> joints = new Dictionary<string, float>();
            for (int i = 0; i < jointNamesCount; ++i)
            {
                joints.Add(message.m_JointsNames[i], message.m_JointsAngles[i]);
            }
            return joints;
        }
        #endregion

        #region HeadTouch
        private class TouchMessage : NaoConnection.NaoConnectionMessage
        {
            public const string ID = "Touch";

            [JsonProperty("part")]
            public string m_Part;
            [JsonProperty("touched")]
            public bool m_Touched;

            public override string GetMessageId() { return ID; }
        }


        private void OnTouchMessageReceived(string message)
        {
            TouchMessage touchMessage = null;
            try
            {
                touchMessage = JsonConvert.DeserializeObject<TouchMessage>(message);
            }
            catch (Exception e)
            {
                Debug.LogError($"NaoReceiver::OnTouchMessageReceived: failed to parse message received with error {e}");
                return;
            }
            if (touchMessage.m_Part == "FrontTactilTouched")
                NaoWorld.Instance.FrontHeadTouched = touchMessage.m_Touched;
            else if (touchMessage.m_Part == "MiddleTactilTouched")
                NaoWorld.Instance.MiddleHeadTouched = touchMessage.m_Touched;
            else if (touchMessage.m_Part == "RearTactilTouched")
                NaoWorld.Instance.RearHeadTouched = touchMessage.m_Touched;
            else
                Debug.LogError("NaoReceiver::OnTouchMessageReceived: unknown part = " + touchMessage.m_Part);
        }

        #endregion

        #region Audio
        private class AudioMessage : NaoConnection.NaoConnectionMessage
        {
            public const string ID = "Audio";

            [JsonProperty("rate")]
            public int m_Rate;
            [JsonProperty("channels")]
            public int m_Channels;
            [JsonProperty("nbSamplesPerChannel")]
            public int m_SamplesPerChannel;
            [JsonProperty("data")]
            public string m_Data;

            public override string GetMessageId() { return ID; }
        }

        private void OnAudioMessageReceived(string message)
        {
            AudioMessage audioMessage = null;
            try
            {
                audioMessage = JsonConvert.DeserializeObject<AudioMessage>(message);
            }
            catch (Exception e)
            {
                Debug.LogError($"NaoReceiver::OnAudioMessageReceived: failed to parse message received with error {e}");
                return;
            }

            float audioClipDuration = 5f; // seconds

            if (NaoWorld.Instance.MicrophoneAudioClip == null)
                NaoWorld.Instance.MicrophoneAudioClip = AudioClip.Create("NaoMicrophone",
                                                                         (int)(audioClipDuration * audioMessage.m_Rate),
                                                                         audioMessage.m_Channels,
                                                                         audioMessage.m_Rate,
                                                                         false);

            byte[] pcmBytes = Convert.FromBase64String(audioMessage.m_Data);
            int totalSamples = pcmBytes.Length / 2;
            float[] floatData = new float[totalSamples];
            for (int i = 0; i < totalSamples; i++)
            {
                short s = BitConverter.ToInt16(pcmBytes, i * 2);
                floatData[i] = s / 32768f;
            }
            SetData(floatData);
        }

        private bool SetData(float[] data)
        {
            AudioClip audioClip = NaoWorld.Instance.MicrophoneAudioClip;
            int writeIndex = NaoWorld.Instance.MicrophoneIndex;

            bool success = audioClip.SetData(data, writeIndex);
            if (!success) return false;
            NaoWorld.Instance.MicrophoneIndex = (writeIndex + data.Length) % audioClip.samples;
            return true;
        }
        #endregion

        #endregion
    }
}
