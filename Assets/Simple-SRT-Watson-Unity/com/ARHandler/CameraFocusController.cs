﻿using UnityEngine;
using System.Collections;
using Vuforia;

namespace SRTWatsonUnity.com.ARHandler
{
    public class CameraFocusController : MonoBehaviour
    {

        [SerializeField]
        private bool m_debugMode = false;

        private bool mVuforiaStarted = false;

        void Start()
        {
            VuforiaARController vuforia = VuforiaARController.Instance;

            if (vuforia != null)
                vuforia.RegisterVuforiaStartedCallback(StartAfterVuforia);
        }

        private void StartAfterVuforia()
        {
            mVuforiaStarted = true;
            SetAutofocus();
        }

        void OnApplicationPause(bool pause)
        {
            if (!pause)
            {
                // App resumed
                if (mVuforiaStarted)
                {
                    // App resumed and vuforia already started
                    // but lets start it again...
                    SetAutofocus(); // This is done because some android devices lose the auto focus after resume
                                    // this was a bug in vuforia 4 and 5. I haven't checked 6, but the code is harmless anyway
                }
            }
        }

        private void SetAutofocus()
        {
            if (CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO))
            {
                if (m_debugMode)
                    Debug.Log("Autofocus set");
            }
            else
            {
                // never actually seen a device that doesn't support this, but just in case
                if (m_debugMode)
                    Debug.Log("this device doesn't support auto focus");
            }
        }
    }

}

