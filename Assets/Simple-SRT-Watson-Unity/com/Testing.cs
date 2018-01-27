using UnityEngine;
using System.Collections;

namespace SRTWatsonUnity.com
{
    public class Testing : MonoBehaviour
    {
        private void Start()
        {
            SimpleSRTWatsonUnity srtWatson = FindObjectOfType<SimpleSRTWatsonUnity>();

            srtWatson.SetOnRecognizeFinalWords(OnRecognizeFinalWords);
            srtWatson.SetOnStartRecognize(OnStartRecognize);
        }

        private void OnStartRecognize()
        {
            Debug.Log("Loading...");
        }

        private void OnRecognizeFinalWords(string obj)
        {
            Debug.Log(obj);
        }
    }

}
