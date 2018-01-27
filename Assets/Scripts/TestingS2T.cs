using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestingS2T : MonoBehaviour
{
    [SerializeField]
    private Text m_text;

    private ArrokhWatsonS2T m_arrokhWatsonS2T;

    private void Start()
    {
        m_arrokhWatsonS2T = FindObjectOfType<ArrokhWatsonS2T>();

        m_arrokhWatsonS2T.SetOnRecognizeFinalWords(OnRecognizeFinalWords);
        m_arrokhWatsonS2T.SetOnStartRecognize(OnStartRecognize);
    }

    private void OnStartRecognize()
    {
        m_text.text = "Loading...";
    }

    private void OnRecognizeFinalWords(string obj)
    {
        m_text.text = obj;
    }
}
