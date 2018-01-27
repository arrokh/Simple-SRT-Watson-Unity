using System.Collections;
using UnityEngine;
using IBM.Watson.DeveloperCloud.Utilities;
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;
using IBM.Watson.DeveloperCloud.DataTypes;
using System;

namespace SRTWatsonUnity.com
{
    public class SimpleSRTWatsonUnity : MonoBehaviour
    {
        [SerializeField]
        private bool m_debugSpeech;

        [SerializeField]
        private SSRTConfiguration config;

        private int m_recordingRoutine = 0;
        private string m_microphoneID = null;
        private AudioClip m_recording = null;
        private int m_recordingBufferSize = 1;
        private int m_recordingHZ = 22050;

        private SpeechToText m_service;

        private Action<string> m_onRecognizeFinalWords;
        public void SetOnRecognizeFinalWords(Action<string> onRecognizeFinalWords)
        {
            m_onRecognizeFinalWords = onRecognizeFinalWords;
        }

        private Action m_onStartRecognize;
        public void SetOnStartRecognize(Action onStartRecognize)
        {
            m_onStartRecognize = onStartRecognize;
        }

        private Action<string> m_onError;
        public void SetOnError(Action<string> onError)
        {
            m_onError = onError;
        }

        public bool Active
        {
            get { return m_service.IsListening; }
            set
            {
                if (value && !m_service.IsListening)
                {
                    m_service.DetectSilence = true;
                    m_service.EnableWordConfidence = true;
                    m_service.EnableTimestamps = true;
                    m_service.SilenceThreshold = 0.01f;
                    m_service.MaxAlternatives = 0;
                    m_service.EnableInterimResults = true;
                    m_service.OnError = OnError;
                    m_service.InactivityTimeout = -1;
                    m_service.ProfanityFilter = false;
                    m_service.SmartFormatting = true;
                    m_service.SpeakerLabels = false;
                    m_service.WordAlternativesThreshold = null;
                    m_service.StartListening(OnRecognize, OnRecognizeSpeaker);
                }
                else if (!value && m_service.IsListening)
                {
                    m_service.StopListening();
                }
            }
        }

        void Start()
        {
            Credentials credentials = new Credentials(config.username, config.password, config.url);

            m_service = new SpeechToText(credentials);
            Active = true;

            StartRecording();
        }

        private void StartRecording()
        {
            if (m_recordingRoutine == 0)
            {
                UnityObjectUtil.StartDestroyQueue();
                m_recordingRoutine = Runnable.Run(RecordingHandler());
            }
        }

        private void StopRecording()
        {
            if (m_recordingRoutine != 0)
            {
                Microphone.End(m_microphoneID);
                Runnable.Stop(m_recordingRoutine);
                m_recordingRoutine = 0;
            }
        }

        private IEnumerator RecordingHandler()
        {
            //Log.Debug("ExampleStreaming.RecordingHandler()", "devices: {0}", Microphone.devices);
            Debug.Log("ExampleStreaming.RecordingHandler() | devices: " + Microphone.devices);

            m_recording = Microphone.Start(m_microphoneID, true, m_recordingBufferSize, m_recordingHZ);
            yield return null;

            if (m_recording == null)
            {
                StopRecording();
                yield break;
            }

            bool bFirstBlock = true;
            int midPoint = m_recording.samples / 2;
            float[] samples = null;

            while (m_recordingRoutine != 0 && m_recording != null)
            {
                int writePos = Microphone.GetPosition(m_microphoneID);
                if (writePos > m_recording.samples || !Microphone.IsRecording(m_microphoneID))
                {
                    //Log.Error("ExampleStreaming.RecordingHandler()", "Microphone disconnected.");
                    Debug.Log("ExampleStreaming.RecordingHandler() | Microphone disconnected.");

                    StopRecording();
                    yield break;
                }

                if ((bFirstBlock && writePos >= midPoint)
                  || (!bFirstBlock && writePos < midPoint))
                {
                    // front block is recorded, make a RecordClip and pass it onto our callback.
                    samples = new float[midPoint];
                    m_recording.GetData(samples, bFirstBlock ? 0 : midPoint);

                    AudioData record = new AudioData();
                    record.MaxLevel = Mathf.Max(Mathf.Abs(Mathf.Min(samples)), Mathf.Max(samples));
                    record.Clip = AudioClip.Create("Recording", midPoint, m_recording.channels, m_recordingHZ, false);
                    record.Clip.SetData(samples, 0);

                    m_service.OnListen(record);

                    bFirstBlock = !bFirstBlock;
                }
                else
                {
                    // calculate the number of samples remaining until we ready for a block of audio, 
                    // and wait that amount of time it will take to record.
                    int remaining = bFirstBlock ? (midPoint - writePos) : (m_recording.samples - writePos);
                    float timeRemaining = (float)remaining / (float)m_recordingHZ;

                    yield return new WaitForSeconds(timeRemaining);
                }

            }
            yield break;
        }

        private void OnError(string error)
        {
            Active = false;

            //Log.Debug("ExampleStreaming.OnError()", "Error! {0}", error);
            //Debug.Log("ExampleStreaming.OnError() | Error! " + error);
            if (m_onError != null)
                m_onError(error);
        }

        private void OnRecognize(SpeechRecognitionEvent result)
        {
            if (result != null && result.results.Length > 0)
            {
                if (m_onStartRecognize != null)
                    m_onStartRecognize();

                foreach (var res in result.results)
                {
                    foreach (var alt in res.alternatives)
                    {
                        //string text = string.Format("{0} ({1}, {2:0.00})\n", alt.transcript, res.final ? "Final" : "Interim", alt.confidence);
                        //ResultsField.text = text;

                        if (m_debugSpeech)
                        {
                            if (res.final)
                                Debug.Log("=>" + alt.transcript);
                            else
                                Debug.Log("Recognizing...");
                        }

                        if (res.final)
                        {
                            if (m_onRecognizeFinalWords != null)
                                m_onRecognizeFinalWords(alt.transcript.ToLower());
                        }
                    }

                }
            }

            /*
            if (res.keywords_result != null && res.keywords_result.keyword != null)
            {
                foreach (var keyword in res.keywords_result.keyword)
                {
                    Log.Debug("ExampleStreaming.OnRecognize()", "keyword: {0}, confidence: {1}, start time: {2}, end time: {3}", keyword.normalized_text, keyword.confidence, keyword.start_time, keyword.end_time);
                }
            }

            if (res.word_alternatives != null)
            {
                foreach (var wordAlternative in res.word_alternatives)
                {
                    Log.Debug("ExampleStreaming.OnRecognize()", "Word alternatives found. Start time: {0} | EndTime: {1}", wordAlternative.start_time, wordAlternative.end_time);
                    foreach (var alternative in wordAlternative.alternatives)
                        Log.Debug("ExampleStreaming.OnRecognize()", "\t word: {0} | confidence: {1}", alternative.word, alternative.confidence);
                }
            }
            */
        }

        private void OnRecognizeSpeaker(SpeakerRecognitionEvent result)
        {
            if (result != null)
            {
                foreach (SpeakerLabelsResult labelResult in result.speaker_labels)
                {
                    //Log.Debug("ExampleStreaming.OnRecognize()", string.Format("speaker result: {0} | confidence: {3} | from: {1} | to: {2}", labelResult.speaker, labelResult.from, labelResult.to, labelResult.confidence));
                    Debug.Log("ExampleStreaming.OnRecognize()" + string.Format("speaker result: {0} | confidence: {3} | from: {1} | to: {2}", labelResult.speaker, labelResult.from, labelResult.to, labelResult.confidence));
                }
            }
        }
    }

}