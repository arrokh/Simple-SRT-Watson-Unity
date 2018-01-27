using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SRTWatsonUnity.com
{
    public class SceneHandler : MonoBehaviour
    {

        public void Load(string scene)
        {
            SceneManager.LoadScene(scene);
        }

        public void Load(int scene)
        {
            SceneManager.LoadScene(scene);
        }
    }
}
