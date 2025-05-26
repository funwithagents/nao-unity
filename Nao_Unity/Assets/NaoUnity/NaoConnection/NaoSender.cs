using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NaoUnity
{
    public class NaoSender : MonoBehaviourSingleton<NaoSender>
    {
        #region Fields
        private Dictionary<string, Action<NaoCommandResult>> m_CommandCallbacks;
        #endregion

        #region MonoBehaviour lifecycle functions
        // Start is called before the first frame update
        void Start()
        {
            m_CommandCallbacks = new Dictionary<string, Action<NaoCommandResult>>();

            NaoConnection.Instance.SubscribeToMessage(CommandEndedMessage.ID, OnCommandEndedMessageReceived);
        }
        #endregion

        #region SendCommand
        private class CommandMessage : NaoConnection.NaoConnectionMessage
        {
            public const string ID = "Command";

            [JsonProperty("commandUuid")]
            public string m_CommandUuid;
            [JsonProperty("commandId")]
            public string m_CommandId;
            [JsonProperty("commandData")]
            public NaoMessage m_CommandData;

            public override string GetMessageId() { return ID; }
        }

        public void ApplyCommandOnNao(NaoMessage message, Action<NaoCommandResult> onResult)
        {
            Debug.Log(string.Format("NaoSender::ApplyCommandOnNao: id = '{0}', data = '{1}'",
                                    message.m_Id, JsonConvert.SerializeObject(message)));

            if (NaoWorld.Instance.ConnectedToNao)
            {
                string uuid = Guid.NewGuid().ToString();
                m_CommandCallbacks.Add(uuid, onResult);

                CommandMessage commandMessage = new CommandMessage()
                {
                    m_CommandUuid = uuid,
                    m_CommandId = message.m_Id,
                    m_CommandData = message
                };
                NaoConnection.Instance.SendMessageToNao(commandMessage);
            }
            else
            {
                Debug.LogError("NaoSender::ApplyCommandOnNao: not connected to Nao");
                onResult?.Invoke(new NaoCommandResult(NaoCommandResult.ResultType.Error, "Not connected to Nao"));
            }
        }


        private class CommandEndedMessage : NaoConnection.NaoConnectionMessage
        {
            public const string ID = "CommandEnded";

            [JsonProperty("commandUuid")]
            public string m_CommandUuid;
            [JsonProperty("resultType")]
            public string m_ResultType;
            [JsonProperty("message")]
            public string m_Message;
            [JsonProperty("data")]
            public object m_Data;

            public override string GetMessageId() { return ID; }
        }

        private void OnCommandEndedMessageReceived(string message)
        {
            CommandEndedMessage commandEndedMessage = null;
            try
            {
                commandEndedMessage = JsonConvert.DeserializeObject<CommandEndedMessage>(message);
            }
            catch (Exception e)
            {
                Debug.LogError($"NaoReceiver::OnTouchMessageReceived: failed to parse message received with error {e}");
                return;
            }

            // UUID
            string uuid = commandEndedMessage.m_CommandUuid;
            // ResultType
            string resultTypeAsString = commandEndedMessage.m_ResultType;
            NaoCommandResult.ResultType resultType = NaoCommandResult.ResultType.Error;
            if (!Enum.TryParse<NaoCommandResult.ResultType>(resultTypeAsString, out resultType))
                Debug.LogError(string.Format("OnCommandEndedMessageReceived: could not parse received string '{0}' as ResultType",
                                             resultTypeAsString));
            // Message
            string msg = commandEndedMessage.m_Message;
            // Data
            object data = commandEndedMessage.m_Data;

            if (m_CommandCallbacks.ContainsKey(uuid))
            {
                Action<NaoCommandResult> callback = m_CommandCallbacks[uuid];
                m_CommandCallbacks.Remove(uuid);
                callback?.Invoke(new NaoCommandResult(resultType, msg, data?.ToString()));
            }
            else
            {
                Debug.LogError(string.Format("OnCommandEndedMessageReceived: unknown uuid '{0}' received",
                                             uuid));
            }
        }

        #endregion
    }

    public class NaoCommandResult
    {
        public enum ResultType
        {
            Success,
            Error,
            Cancel
        }

        public ResultType m_Type;
        public string m_Message;
        public string m_DataAsString;

        public NaoCommandResult(ResultType type, string message, string dataAsString = null)
        {
            m_Type = type;
            m_Message = message;
            m_DataAsString = dataAsString;
        }
    }

    public class NaoCommandResult<T> : NaoCommandResult
    {
        public T m_Data;

        public NaoCommandResult(NaoCommandResult naoCommandResult)
            : base(naoCommandResult.m_Type, naoCommandResult.m_Message, naoCommandResult.m_DataAsString)
        {
            T data = default(T);
            try
            {
                data = JsonConvert.DeserializeObject<T>(naoCommandResult.m_DataAsString);
            }
            catch (Exception e)
            {
                Debug.LogError($"NaoReceiver::NaoCommandResult<T>: failed to parse data as '{typeof(T).Name}', with error {e}");
                return;
            }

            m_Data = data;
        }
    }
}
