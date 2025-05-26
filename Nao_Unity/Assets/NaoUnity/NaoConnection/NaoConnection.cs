using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using System.Net.WebSockets;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;

namespace NaoUnity
{
    public class NaoConnection : MonoBehaviourSingleton<NaoConnection>
    {
        #region Fields
        [SerializeField, ReadOnly]
        private ClientWebSocket m_ConnectedWebsocketClient = null;

        public string WEBSOCKET_SERVER_IP = "127.0.0.1";
        public int PORT_WEBSOCKET = 8002;

        private DateTime m_LastConnectTryTime = DateTime.MinValue;
        [SerializeField, ReadOnly]
        private bool m_IsConnected = false;

        private Dictionary<string, List<Action<string>>> m_MessageSubcribers = new Dictionary<string, List<Action<string>>>();
        #endregion

        #region MonoBehaviour lifecycle functions
        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("NaoConnection: call first connect");
            Connect();
        }

        // Update is called once per frame
        void Update()
        {
            float secondsSinceLastIPSent = (float)(DateTime.Now - m_LastConnectTryTime).TotalSeconds;
            if (secondsSinceLastIPSent > 5f && !m_IsConnected)
                Connect();
        }

        void OnApplicationQuit()
        {
            if (m_IsConnected)
                Disconnect();
        }
        #endregion

        #region Receive callback
        public abstract class NaoConnectionMessage
        {
            public abstract string GetMessageId();
        }

        private class NaoConnectionMessageWrapper
        {
            [JsonProperty("id")]
            public string m_Id;
            [JsonProperty("data")]
            public object m_Data;

            public NaoConnectionMessageWrapper(string id, object data)
            {
                m_Id = id;
                m_Data = data;
            }
        }

        private void OnMessageReceived(string message)
        {
            string messageId = "";
            string messageDataAsString = "";
            try
            {
                NaoConnectionMessageWrapper messageWrapper = JsonConvert.DeserializeObject<NaoConnectionMessageWrapper>(message);
                messageId = messageWrapper.m_Id;
                messageDataAsString = messageWrapper.m_Data.ToString();
            }
            catch (Exception e)
            {
                Debug.LogError($"NaoConnection::OnMessageReceived: failed to parse message received from websocket with error {e}");
                return;
            }

            if (messageId != "Joints" && messageId != "Audio")
                Debug.Log($"NaoConnection: received message => \n{message}");

            if (messageId == NaoStateMessage.ID)
                OnNaoStateMessageReceived(messageDataAsString);
            else if (messageId == LogMessage.ID)
                OnLogMessageReceived(messageDataAsString);
            else
            {
                if (m_MessageSubcribers.ContainsKey(messageId))
                {
                    foreach (Action<string> callback in m_MessageSubcribers[messageId])
                        callback?.Invoke(messageDataAsString);
                }
                else
                    Debug.Log($"NaoConnection: received websocket message with id '{messageId}', but no subscriber");
            }
        }

        private class NaoStateMessage : NaoConnectionMessage
        {
            public const string ID = "NaoState";

            [JsonProperty("connected")]
            public bool m_Connected;
            [JsonProperty("fakeRobot")]
            public bool m_FakeRobot;

            public override string GetMessageId() { return ID; }
        }

        private void OnNaoStateMessageReceived(string message)
        {
            NaoStateMessage naoStateMessage = null;
            try
            {
                naoStateMessage = JsonConvert.DeserializeObject<NaoStateMessage>(message);
            }
            catch (Exception e)
            {
                Debug.LogError($"NaoConnection::OnConnectionMessageReceived: failed to parse message received from websocket with error {e}");
                return;
            }

            Debug.Log($"NaoConnection: connection message received: connected = '{naoStateMessage.m_Connected}' fakeRobot = '{naoStateMessage.m_FakeRobot}' ");
            NaoWorld.Instance.FakeRobot = naoStateMessage.m_FakeRobot;
            NaoWorld.Instance.ConnectedToNao = naoStateMessage.m_Connected;
        }

        private class LogMessage : NaoConnectionMessage
        {
            public const string ID = "Log";

            [JsonProperty("log")]
            public string m_Log;
            [JsonProperty("loglevel")]
            public string m_LogLevel;
            public override string GetMessageId() { return ID; }
        }

        private void OnLogMessageReceived(string message)
        {
            LogMessage logMessage = null;
            try
            {
                logMessage = JsonConvert.DeserializeObject<LogMessage>(message);
            }
            catch (Exception e)
            {
                Debug.LogError($"NaoConnection::OnLogMessageReceived: failed to parse message received from websocket with error {e}");
                return;
            }

            switch (logMessage.m_LogLevel)
            {
                case "DEBUG":
                case "INFO":
                    Debug.Log("FROM NAO: " + logMessage.m_Log);
                    break;
                case "WARNING":
                    Debug.LogWarning("FROM NAO: " + logMessage.m_Log);
                    break;
                case "ERROR":
                    Debug.LogError("FROM NAO: " + logMessage.m_Log);
                    break;
                default:
                    Debug.LogError("Unknown log level received: " + logMessage.m_LogLevel);
                    Debug.LogError("FROM NAO: " + logMessage.m_Log);
                    break;
            }
            
        }
        #endregion

        #region Connection methods
        private async void Connect()
        {
            //Debug.Log("NaoConnection: trying to connect to websocket");
            m_LastConnectTryTime = DateTime.Now;
            m_IsConnected = await Websocket_Connect();
            if (!m_IsConnected)
                return;
            _ = Websocket_WaitForMessages(OnMessageReceived);
        }

        private async void Disconnect()
        {
            //Debug.Log("NaoConnection: sending disconnection message");
            await WebsocketDisconnect();
            m_IsConnected = false;

            NaoWorld.Instance.FakeRobot = false;
            NaoWorld.Instance.ConnectedToNao = false;
        }
        #endregion

        #region Subcribe API
        public void SubscribeToMessage(string id, Action<string> onMessageReceivedCallback)
        {
            if (m_MessageSubcribers.ContainsKey(id))
            {
                if (m_MessageSubcribers[id] == null)
                    m_MessageSubcribers[id] = new List<Action<string>>();
                m_MessageSubcribers[id].Add(onMessageReceivedCallback);
            }
            else
            {
                m_MessageSubcribers.Add(id, new List<Action<string>>() { onMessageReceivedCallback });
            }
        }

        public void UnsubcribeToMessage(string address, Action<string> onMessageReceivedCallback)
        {
            if (m_MessageSubcribers.ContainsKey(address))
            {
                if (m_MessageSubcribers[address].Contains(onMessageReceivedCallback))
                    m_MessageSubcribers[address].Remove(onMessageReceivedCallback);
                else
                    Debug.LogError(string.Format("NaoConnection::UnsubcribeToMessage: trying to unsubscribe to address '{0}', but not cannot find subscription",
                                                 address));
            }
            else
            {
                Debug.LogError(string.Format("NaoConnection::UnsubcribeToMessage: trying to unsubscribe to address '{0}', but not subcribed",
                                             address));
            }
        }
        #endregion

        #region SendMessage API
        public async void SendMessageToNao(NaoConnectionMessage message)
        {
            NaoConnectionMessageWrapper wrapperMessage =
                new NaoConnectionMessageWrapper(message.GetMessageId(), message);
            string messageAsString = JsonConvert.SerializeObject(wrapperMessage);
            //Debug.Log($"NaoConnection: sending message '{messageAsString}'");
            await Websocket_SendMessage(messageAsString);
        }
        #endregion

        #region Websocket
        private async Task<bool> Websocket_Connect()
        {
            ClientWebSocket webSocket = new ClientWebSocket();
            Uri uri = new Uri($"ws://{WEBSOCKET_SERVER_IP}:{PORT_WEBSOCKET}");
            try
            {
                await webSocket.ConnectAsync(uri, CancellationToken.None);
                Debug.Log($"Websocket => Connected!");
                m_ConnectedWebsocketClient = webSocket;
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Websocket => failed to connect with error = {ex.Message} !");
                return false;
            }
        }

        private async Task<bool> Websocket_SendMessage(string message)
        {
            if (m_ConnectedWebsocketClient == null)
                return false;

            byte[] messageByte = Encoding.UTF8.GetBytes(message);
            try
            {
                await m_ConnectedWebsocketClient.SendAsync(new ArraySegment<byte>(messageByte), WebSocketMessageType.Text, true, CancellationToken.None);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Websocket => failed to send with error = {ex.Message} !");
                return false;
            }
        }

        private CancellationTokenSource cts = null;
        private async Task Websocket_WaitForMessages(Action<string> onMessageCallback)
        {
            while (m_ConnectedWebsocketClient != null
                   && m_ConnectedWebsocketClient.State == WebSocketState.Open)
            {
                try
                {
                    cts = new CancellationTokenSource();
                    (WebSocketMessageType? messageType, object result) = await ReceiveFullMessageAsync(m_ConnectedWebsocketClient, cts.Token);
                    if (messageType == null)
                    {
                        Debug.LogError($"Websocket => Null message type, should not happen");
                    }
                    else if (messageType == WebSocketMessageType.Close)
                    {
                        Debug.Log($"Websocket => closing from server");
                        await m_ConnectedWebsocketClient.CloseAsync(WebSocketCloseStatus.NormalClosure, "OK", CancellationToken.None);
                        m_ConnectedWebsocketClient.Dispose();
                        m_ConnectedWebsocketClient = null;
                        m_IsConnected = false;
                    }
                    else if (messageType == WebSocketMessageType.Binary)
                    {
                        Debug.LogError($"Websocket => binary message type, should not happen");
                    }
                    else if (messageType == WebSocketMessageType.Text)
                    {
                        onMessageCallback?.Invoke(result as string);
                    }
                    else
                    {
                        Debug.Log($"Unknown result type = '{messageType}'");
                    }
                }
                catch (OperationCanceledException)
                {
                    // Gracefully exited due to cancellation
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Websocket => failed to receive with error = {ex.Message} !");
                    continue;
                }
            }
        }

        private async Task<bool> WebsocketDisconnect()
        {
            if (m_ConnectedWebsocketClient == null)
                return false;

            if (m_ConnectedWebsocketClient.State == WebSocketState.Open)
            {
                try
                {
                    Debug.Log($"Websocket => Closing!");
                    await m_ConnectedWebsocketClient.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "OK", CancellationToken.None);
                    Debug.Log($"Websocket => Closed!");
                    cts.Cancel(); // Cancel receive loop
                    return true;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Websocket => failed to close with error = {ex.Message} !");
                    return false;
                }
            }
            else
            {
                Debug.Log($"Websocket was already closed");
                m_ConnectedWebsocketClient.Dispose();
                m_ConnectedWebsocketClient = null;
                return true;
            }
        }

        private static async Task<(WebSocketMessageType?, object)> ReceiveFullMessageAsync(WebSocket webSocket, CancellationToken cancellationToken)
        {
            var buffer = new ArraySegment<byte>(new byte[8192]);
            using (var ms = new MemoryStream())
            {
                WebSocketReceiveResult result;

                do
                {
                    result = await webSocket.ReceiveAsync(buffer, cancellationToken);
                    ms.Write(buffer.Array, buffer.Offset, result.Count);
                }
                while (!result.EndOfMessage);

                ms.Seek(0, SeekOrigin.Begin);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    using (var reader = new StreamReader(ms, Encoding.UTF8))
                    {
                        string text = await reader.ReadToEndAsync();
                        return (WebSocketMessageType.Text, text);
                    }
                }
                else if (result.MessageType == WebSocketMessageType.Binary)
                {
                    byte[] binaryData = ms.ToArray();
                    return (WebSocketMessageType.Binary, binaryData);

                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    return (WebSocketMessageType.Close, result);
                }

                return (null, null);
            }
        }
        #endregion
    }
}
