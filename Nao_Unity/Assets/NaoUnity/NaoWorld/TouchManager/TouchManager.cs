using UnityEngine;

namespace NaoUnity
{
    public class TouchManager : MonoBehaviourSingleton<TouchManager>
    {
        #region Fields
        #endregion

        #region MonoBehaviour lifecycle functions
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            UpdateUnitHeadTouch();
            UpdateHeadTouch();
        }
        #endregion

        private void UpdateUnitHeadTouch()
        {
            TouchType newFrontHeadTouch = GetNewTouchType(NaoWorld.Instance.FrontHeadTouched, NaoWorld.Instance.FrontHeadTouch);
            if (newFrontHeadTouch != NaoWorld.Instance.FrontHeadTouch)
                NaoWorld.Instance.FrontHeadTouch = newFrontHeadTouch;

            TouchType newMiddleHeadTouch = GetNewTouchType(NaoWorld.Instance.MiddleHeadTouched, NaoWorld.Instance.MiddleHeadTouch);
            if (newMiddleHeadTouch != NaoWorld.Instance.MiddleHeadTouch)
                NaoWorld.Instance.MiddleHeadTouch = newMiddleHeadTouch;

            TouchType newRearHeadTouch = GetNewTouchType(NaoWorld.Instance.RearHeadTouched, NaoWorld.Instance.RearHeadTouch);
            if (newRearHeadTouch != NaoWorld.Instance.RearHeadTouch)
                NaoWorld.Instance.RearHeadTouch = newRearHeadTouch;
        }
        private void UpdateHeadTouch()
        {
            bool touched = NaoWorld.Instance.FrontHeadTouched
                           && NaoWorld.Instance.MiddleHeadTouched
                           && NaoWorld.Instance.RearHeadTouched;
            if (touched != NaoWorld.Instance.HeadTouched)
                NaoWorld.Instance.HeadTouched = touched;

            TouchType newHeadTouch = GetNewTouchType(NaoWorld.Instance.HeadTouched, NaoWorld.Instance.HeadTouch);
            if (newHeadTouch != NaoWorld.Instance.HeadTouch)
                NaoWorld.Instance.HeadTouch = newHeadTouch;
        }


        private TouchType GetNewTouchType(bool touched, TouchType previousTouchType)
        {
            if (touched)
            {
                switch (previousTouchType)
                {
                    case TouchType.NoTouch:
                        return TouchType.StartTouch;
                    case TouchType.StartTouch:
                        return TouchType.Touch;
                    case TouchType.Touch:
                        return TouchType.Touch;
                    case TouchType.EndTouch:
                        return TouchType.StartTouch;
                    default:
                        Debug.LogError("TouchManager::GetNewTouchType: unknown touch type = " + previousTouchType.ToString());
                        return TouchType.NoTouch;
                }
            }
            else
            {
                switch (previousTouchType)
                {
                    case TouchType.NoTouch:
                        return TouchType.NoTouch;
                    case TouchType.StartTouch:
                        return TouchType.EndTouch;
                    case TouchType.Touch:
                        return TouchType.EndTouch;
                    case TouchType.EndTouch:
                        return TouchType.NoTouch;
                    default:
                        Debug.LogError("TouchManager::GetNewTouchType: unknown touch type = " + previousTouchType.ToString());
                        return TouchType.NoTouch;
                }
            }
        }
    }
}
