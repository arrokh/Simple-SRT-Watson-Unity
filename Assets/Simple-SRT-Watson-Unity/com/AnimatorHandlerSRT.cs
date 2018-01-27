using UnityEngine;
using System.Collections;

namespace SRTWatsonUnity.com
{
    public class AnimatorHandlerSRT : MonoBehaviour
    {
        [SerializeField]
        private string[] m_statesName;

        private Animator m_animator;
        private SimpleSRTWatsonUnity m_srtWatson;

        private void Start()
        {
            m_animator = GetComponent<Animator>();
            m_srtWatson = FindObjectOfType<SimpleSRTWatsonUnity>();
        }

        private void ChangeState(string state)
        {
            m_animator.SetBool(state, true);
            Invoke("UnCheckAllState", 0.3f);
        }

        private void UnCheckAllState()
        {
            foreach (var stateName in m_statesName)
                m_animator.SetBool(stateName, false);
        }
    }

}