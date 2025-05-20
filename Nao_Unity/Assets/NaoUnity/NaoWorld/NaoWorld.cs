using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace NaoUnity
{
    public class NaoWorld : MonoBehaviourSingleton<NaoWorld>
    {
        #region Connection to Nao
        [SerializeField, ReadOnly]
        private bool m_ConnectedToNao = false;
        public event Action<bool> OnConnectedToNao;
        public bool ConnectedToNao
        {
            get
            {
                return m_ConnectedToNao;
            }
            set
            {
                m_ConnectedToNao = value;
                OnConnectedToNao?.Invoke(m_ConnectedToNao);
            }
        }
        [SerializeField, ReadOnly]
        private bool m_FakeRobot = false;
        public bool FakeRobot
        {
            get
            {
                return m_FakeRobot;
            }
            set
            {
                m_FakeRobot = value;
            }
        }
        #endregion

        #region JointsAngles
        [SerializeField, ReadOnly]
        private Dictionary<string, float> m_JointsAngles = new Dictionary<string, float>();
        public event Action<Dictionary<string, float>> onJointsUpdated;
        public Dictionary<string, float> JointAngles
        {
            get
            {
                return m_JointsAngles;
            }
            set
            {
                m_JointsAngles = value;
                onJointsUpdated?.Invoke(m_JointsAngles);
            }
        }
        #endregion

        #region Posture
        [SerializeField, ReadOnly]
        private PostureType m_CurrentPosture = PostureType.Standing;
        public event Action<PostureType> onCurrentPostureUpdated;
        public PostureType CurrentPosture
        {
            get
            {
                return m_CurrentPosture;
            }
            set
            {
                m_CurrentPosture = value;
                onCurrentPostureUpdated?.Invoke(m_CurrentPosture);
            }
        }
        #endregion

        #region HeadTouch

        [SerializeField, ReadOnly]
        private bool m_HeadTouched = false;
        public event Action<bool> onHeadTouchedUpdated;
        public bool HeadTouched
        {
            get
            {
                return m_HeadTouched;
            }
            set
            {
                m_HeadTouched = value;
                onHeadTouchedUpdated?.Invoke(m_HeadTouched);
            }
        }
        [SerializeField, ReadOnly]
        private TouchType m_HeadTouch = TouchType.NoTouch;
        public event Action<TouchType> onHeadTouchUpdated;
        public TouchType HeadTouch
        {
            get
            {
                return m_HeadTouch;
            }
            set
            {
                m_HeadTouch = value;
                onHeadTouchUpdated?.Invoke(m_HeadTouch);
            }
        }

        #region FrontHeadTouch
        [SerializeField, ReadOnly]
        private bool m_FrontHeadTouched = false;
        public event Action<bool> onFrontHeadTouchedUpdated;
        public bool FrontHeadTouched
        {
            get
            {
                return m_FrontHeadTouched;
            }
            set
            {
                m_FrontHeadTouched = value;
                onFrontHeadTouchedUpdated?.Invoke(m_FrontHeadTouched);
            }
        }
        [SerializeField, ReadOnly]
        private TouchType m_FrontHeadTouch = TouchType.NoTouch;
        public event Action<TouchType> onFrontHeadTouchUpdated;
        public TouchType FrontHeadTouch
        {
            get
            {
                return m_FrontHeadTouch;
            }
            set
            {
                m_FrontHeadTouch = value;
                onFrontHeadTouchUpdated?.Invoke(m_FrontHeadTouch);
            }
        }
        #endregion

        #region MiddleHeadTouch
        [SerializeField, ReadOnly]
        private bool m_MiddleHeadTouched = false;
        public event Action<bool> onMiddleHeadTouchedUpdated;
        public bool MiddleHeadTouched
        {
            get
            {
                return m_MiddleHeadTouched;
            }
            set
            {
                m_MiddleHeadTouched = value;
                onMiddleHeadTouchedUpdated?.Invoke(m_MiddleHeadTouched);
            }
        }
        [SerializeField, ReadOnly]
        private TouchType m_MiddleHeadTouch = TouchType.NoTouch;
        public event Action<TouchType> onMiddleHeadTouchUpdated;
        public TouchType MiddleHeadTouch
        {
            get
            {
                return m_MiddleHeadTouch;
            }
            set
            {
                m_MiddleHeadTouch = value;
                onMiddleHeadTouchUpdated?.Invoke(m_MiddleHeadTouch);
            }
        }
        #endregion

        #region RearHeadTouch
        [SerializeField, ReadOnly]
        private bool m_RearHeadTouched = false;
        public event Action<bool> onRearHeadTouchedUpdated;
        public bool RearHeadTouched
        {
            get
            {
                return m_RearHeadTouched;
            }
            set
            {
                m_RearHeadTouched = value;
                onRearHeadTouchedUpdated?.Invoke(m_RearHeadTouched);
            }
        }
        [SerializeField, ReadOnly]
        private TouchType m_RearHeadTouch = TouchType.NoTouch;
        public event Action<TouchType> onRearHeadTouchUpdated;
        public TouchType RearHeadTouch
        {
            get
            {
                return m_RearHeadTouch;
            }
            set
            {
                m_RearHeadTouch = value;
                onRearHeadTouchUpdated?.Invoke(m_RearHeadTouch);
            }
        }

        #endregion

        #endregion

        #region Current behavior
        [SerializeField, ReadOnly]
        private string m_CurrentBehavior = null;
        public string CurrentBehavior
        {
            get
            {
                return m_CurrentBehavior;
            }
            set
            {
                m_CurrentBehavior = value;
            }
        }
        #endregion

        #region TTSLanguage
        [SerializeField, ReadOnly]
        private NaoTTSLanguage m_CurrentTTSLanguage = NaoTTSLanguage.French;
        public event Action<NaoTTSLanguage> onCurrentTTSLanguageChanged;
        public NaoTTSLanguage CurrentTTSLanguage
        {
            get
            {
                return m_CurrentTTSLanguage;
            }
            set
            {
                m_CurrentTTSLanguage = value;
                onCurrentTTSLanguageChanged?.Invoke(m_CurrentTTSLanguage);
            }
        }
        #endregion

        #region IsTalking
        [SerializeField, ReadOnly]
        private bool m_IsTalking = false;
        public event Action<bool> onIsTalkingUpdated;
        public bool IsTalking
        {
            get
            {
                return m_IsTalking;
            }
            set
            {
                m_IsTalking = value;
                onIsTalkingUpdated?.Invoke(m_IsTalking);
            }
        }
        #endregion

        #region Microphones
        [SerializeField, ReadOnly]
        private AudioClip m_MicrophoneAudioClip = null;
        public AudioClip MicrophoneAudioClip
        {
            get
            {
                return m_MicrophoneAudioClip;
            }
            set
            {
                m_MicrophoneAudioClip = value;
            }
        }
        [SerializeField, ReadOnly]
        private int m_MicrophoneIndex = 0;
        public int MicrophoneIndex
        {
            get
            {
                return m_MicrophoneIndex;
            }
            set
            {
                m_MicrophoneIndex = value;
            }
        }
        #endregion


    }
}
