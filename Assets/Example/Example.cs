using SRTWatsonUnity.com;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Example : MonoBehaviour
{
    [SerializeField]
    private string m_detectedWord;

    [SerializeField]
    private string[] m_statesName;

    private Animator m_animator;
    private SimpleSRTWatsonUnity m_srtWatson;

    private void Start()
    {
        m_animator = GetComponent<Animator>();

        m_srtWatson = FindObjectOfType<SimpleSRTWatsonUnity>();

        m_srtWatson.SetOnRecognizeFinalWords(OnRecognizeFinalWords);
        m_srtWatson.SetOnStartRecognize(OnStartRecognize);
    }

    private void OnStartRecognize()
    {
        Debug.Log("Loading...");
    }

    private void OnRecognizeFinalWords(string obj)
    {
        m_detectedWord = obj.TrimStart().TrimEnd();

        if (!m_detectedWord.Contains(" "))
            PlayState();
        else
        {
            Debug.Log("Detected more one word");
            m_detectedWord = "";
        }
    }

    public void PlayState()
    {
        if (m_detectedWord.Length == 0)
            return;

            ChangeState(m_detectedWord[0].ToString());
        m_detectedWord = m_detectedWord.Remove(0,1);
    }

    private void ChangeState(string state)
    {
        m_animator.SetBool(state.ToUpper(), true);
        Invoke("UnCheckAllState", 0.3f);
    }

    private void UnCheckAllState()
    {
        foreach (var stateName in m_statesName)
            m_animator.SetBool(stateName.ToUpper(), false);
    }
}
