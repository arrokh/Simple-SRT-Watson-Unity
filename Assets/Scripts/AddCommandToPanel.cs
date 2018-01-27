using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddCommandToPanel : MonoBehaviour
{
    [SerializeField]
    private Transform m_targetPanel;

    [SerializeField]
    private GameObject m_prefabButton;

    private PlayerControllerHandler m_playerControllerHandler;

    void Start()
    {
        m_playerControllerHandler = GetComponent<PlayerControllerHandler>();

        foreach (var item in m_playerControllerHandler.GetAllWords())
            AddPrefabButton(item);
    }

    private void AddPrefabButton(string text)
    {
        GameObject target = Instantiate(m_prefabButton);
        target.transform.SetParent(m_targetPanel);
        target.transform.position = Vector3.zero;
        target.transform.localScale = Vector3.one;
        target.GetComponentInChildren<Text>().text = text;
    }
}
