using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControllerHandler : MonoBehaviour
{
    [SerializeField]
    private Text m_textTarget;

    [SerializeField]
    private List<string> m_myWords;

    [SerializeField]
    private List<string> m_idleWords = new List<string>();

    [SerializeField]
    private List<string> m_walkWords = new List<string>();

    [SerializeField]
    private List<string> m_runWords = new List<string>();

    [SerializeField]
    private List<string> m_jumpWords = new List<string>();

    [SerializeField]
    private List<string> m_damagedWords = new List<string>();

    private Animator m_animator;
    private ArrokhWatsonS2T m_arrokhWatsonS2T;

    private void Start()
    {
        m_animator = GetComponent<Animator>();
        m_arrokhWatsonS2T = FindObjectOfType<ArrokhWatsonS2T>();

        m_arrokhWatsonS2T.SetOnRecognizeFinalWords(UpdateStateBySpeech);
        m_arrokhWatsonS2T.SetOnStartRecognize(() => { m_textTarget.text = "Loading... :D"; });
        m_arrokhWatsonS2T.SetOnError((string res) =>
        {
            m_textTarget.text = "Faile recognizing :(";
            Invoke("DisableS2T", 2.0f);
        });

        m_myWords = GetAllWords();
    }

    private void DisableS2T()
    {
        CancelInvoke();
        m_arrokhWatsonS2T.enabled = false;
    }

    private void EnableS2T()
    {
        m_arrokhWatsonS2T.enabled = true;
    }

    private void UpdateStateBySpeech(string val)
    {
        m_textTarget.text = val;

        if (CheckIsContainWords(val, m_idleWords))
            ChangeState("idle");

        if (CheckIsContainWords(val, m_walkWords))
            ChangeState("walk");

        if (CheckIsContainWords(val, m_jumpWords))
            ChangeState("jump");

        if (CheckIsContainWords(val, m_runWords))
            ChangeState("run");

        if (CheckIsContainWords(val, m_damagedWords))
            ChangeState("damaged");
    }

    private bool CheckIsContainWords(string checkWords, List<string> listWords)
    {

        checkWords = checkWords.Trim();

        List<string> words = (from i in listWords
                              where i == checkWords
                              select i).ToList();

        return (words.Count > 0) ? true : false;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
            ChangeState("idle");

        if (Input.GetKeyUp(KeyCode.Alpha2))
            ChangeState("walk");

        if (Input.GetKeyUp(KeyCode.Alpha3))
            ChangeState("run");

        if (Input.GetKeyUp(KeyCode.Alpha4))
            ChangeState("jump");

        if (Input.GetKeyUp(KeyCode.Alpha5))
            ChangeState("damaged");
    }

    private void ChangeState(string state)
    {
        m_animator.SetBool(state, true);
        Invoke("UnCheckAllState", 0.3f);
    }

    private void UnCheckAllState()
    {
        m_animator.SetBool("idle", false);
        m_animator.SetBool("walk", false);
        m_animator.SetBool("run", false);
        m_animator.SetBool("jump", false);
        m_animator.SetBool("damaged", false);
    }

    public List<string> GetAllWords()
    {
        return GetWords("i", m_idleWords)
            .Concat(GetWords("w", m_walkWords))
            .Concat(GetWords("r", m_runWords))
            .Concat(GetWords("j", m_jumpWords))
            .Concat(GetWords("d", m_damagedWords))
            .ToList();
    }

    private List<string> GetWords(string keyword, List<string> listWords)
    {
        List<string> getWords = new List<string>();

        foreach (var item in listWords)
            getWords.Add(keyword + "|" + item);

        return getWords;
    }
}
