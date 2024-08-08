using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using System.Text;

public class UIController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public Button btnTranscripcion;
    [SerializeField] public GameObject modal;
    private AudioSource audioSource;
    [SerializeField] public TextMeshProUGUI transcriptionText;
    [SerializeField] public TextMeshProUGUI transcriptButtonText;
    private bool isRecording = false;
    private string filePath;
    [SerializeField] public SignQueue signQueue;
    private const string apiUrl = "http://192.168.16.99:8001/api/transcribe";
    private const string hardcodedValue = "a";

    public void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        btnTranscripcion.onClick.AddListener(ToggleRecording);
        modal.SetActive(false);
        transcriptButtonText.text = "Comenzar transcripción";
        signQueue = FindObjectOfType<SignQueue>(); // Automatically find SignQueue component in the scene
        if (signQueue == null)
        {
            Debug.LogWarning("SignQueue not found!");
        }
    }

    public void OpenModal()
    {
        modal.SetActive(true);
    }

    public void CloseModal()
    {
        modal.SetActive(false);
    }

    public void ToggleRecording()
    {
        //PlayAnimationHardcode();
        if (!isRecording)
        {
            transcriptButtonText.text = "Detener transcripción";
            StartRecording();
        }
        else
        {
            transcriptButtonText.text = "PROCESANDO TRANSCRIPCIÓN";
            StopRecording();
        }
    }

    public void StartRecording()
    {
        if (transcriptionText != null)
        {
            transcriptionText.text = "";
        }
        else
        {
            Debug.LogError("TranscriptionText reference is not set.");
        }

        if (Microphone.devices.Length > 0)
        {
            audioSource.clip = Microphone.Start(null, false, 10, 48000);
            isRecording = true;
            Debug.Log("Recording started");
        }
        else
        {
            isRecording = true;
            Debug.LogWarning("No microphone detected");
        }
    }

    public void StopRecording()
    {
        // TODO: remove hardcoded for testing
        //signQueue.StartAnimationQueue(new string[] { "A", "Hola", "Idle", "Hola", "Hola", "A" });
        if (isRecording)
        {
            Microphone.End(null);
            isRecording = false;
            Debug.Log("Recording stopped");
            SaveRecording(audioSource.clip);
        }
    }

    void SaveRecording(AudioClip clip)
    {
        filePath = Path.Combine(Application.persistentDataPath, "recordedAudio.wav");
        SaveWav(filePath, clip);
        Debug.Log("Audio saved at: " + filePath);
        StartCoroutine(SendAudio(filePath));
    }

    void SaveWav(string filePath, AudioClip clip)
    {
        var samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        byte[] wavFile = ConvertToWav(samples, clip.channels, clip.frequency);

        File.WriteAllBytes(filePath, wavFile);
    }

    byte[] ConvertToWav(float[] samples, int channels, int sampleRate)
    {
        MemoryStream stream = new MemoryStream();

        // Header del WAV
        stream.Write(System.Text.Encoding.UTF8.GetBytes("RIFF"), 0, 4);
        stream.Write(System.BitConverter.GetBytes(36 + samples.Length * 2), 0, 4);
        stream.Write(System.Text.Encoding.UTF8.GetBytes("WAVE"), 0, 4);

        // SubChunk1
        stream.Write(System.Text.Encoding.UTF8.GetBytes("fmt "), 0, 4);
        stream.Write(System.BitConverter.GetBytes(16), 0, 4);
        stream.Write(System.BitConverter.GetBytes((ushort)1), 0, 2);
        stream.Write(System.BitConverter.GetBytes((ushort)channels), 0, 2);
        stream.Write(System.BitConverter.GetBytes(sampleRate), 0, 4);
        stream.Write(System.BitConverter.GetBytes(sampleRate * channels * 2), 0, 4);
        stream.Write(System.BitConverter.GetBytes((ushort)(channels * 2)), 0, 2);
        stream.Write(System.BitConverter.GetBytes((ushort)16), 0, 2);

        // SubChunk2
        stream.Write(System.Text.Encoding.UTF8.GetBytes("data"), 0, 4);
        stream.Write(System.BitConverter.GetBytes(samples.Length * 2), 0, 4);

        // Datos del audio
        int maxValue = 32767;
        for (int i = 0; i < samples.Length; i++)
        {
            short value = (short)(samples[i] * maxValue);
            stream.Write(System.BitConverter.GetBytes(value), 0, 2);
        }

        return stream.ToArray();
    }

    IEnumerator SendAudio(string filePath)
    {
        byte[] fileData = File.ReadAllBytes(filePath);
        WWWForm form = new WWWForm();
        form.AddBinaryData("audio", fileData, "audio.mp3", "audio/mpeg");

        Debug.Log(apiUrl);
        using (UnityWebRequest www = UnityWebRequest.Post(apiUrl, form))
        {
            Debug.Log("PROBANDO");
            www.timeout = 10;

            yield return www.SendWebRequest();

            Debug.Log("PASA POR ACA");

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                transcriptButtonText.text = "Comenzar transcripción";
                Debug.Log("Error: " + www.error);
            }
            else
            {
                transcriptButtonText.text = "Comenzar transcripción";
                PlayRecordedAudio();

                Debug.Log("Response: " + www.downloadHandler.text);
                TranscriptionResponse response = JsonUtility.FromJson<TranscriptionResponse>(www.downloadHandler.text);
                Debug.Log("Transcription: " + response.transcription);
                if (transcriptionText != null)
                {
                    transcriptionText.text = response.transcription;
                }
                else
                {
                    Debug.LogError("TranscriptionText reference is not set.");
                }
                List<string> animationNames = new List<string>();
                foreach (Sign sign in response.signs)
                {
                    // TODO: we should split the `sign.value` into UPPER CASE characters
                    // and remove tildes and add those into the animationNames array.
                    // Example: Tomás should end up as `T O M A S`.
                    if (!string.IsNullOrEmpty(sign.value))
                    {
                        string value = sign.value.ToUpper();
                        value = Regex.Replace(value.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", "");
                        foreach(char c in value) {
                            Debug.Log("Adding animation: " + c);
                            animationNames.Add(c.ToString());
                        }
                    }
                    else
                    {
                        Debug.Log("Adding animation: " + sign.sign);
                        animationNames.Add(sign.sign);
                    }
                }
                if (signQueue != null)
                {
                    signQueue.StartAnimationQueue(animationNames.ToArray());
                }
                else
                {
                    Debug.LogError("SignQueue reference is not set.");
                }
                // Here we should send the text to be used in the
                // SignSystem script.
            }
        }
    }

    public void PlayAnimationHardcode() {
        List<string> animationNames = new List<string>();
        Sign[] signs = new Sign[1];
        signs[0] = new Sign();
        signs[0].sign = "test";
        signs[0].value = hardcodedValue;
        foreach (Sign sign in signs)
            {                
                if (!string.IsNullOrEmpty(sign.value))
                {
                    string value = sign.value.ToUpper();
                    value = Regex.Replace(value.Normalize(NormalizationForm.FormD), @"[^a-zA-z0-9 ]+", "");
                    foreach(char c in value) {
                        Debug.Log("Adding animation: " + c);
                        animationNames.Add(c.ToString());
                    }
                }
                else
                {
                    Debug.Log("Adding animation: " + sign.sign);
                    animationNames.Add(sign.sign);
                }
            }
        if (signQueue != null)
        {
            signQueue.StartAnimationQueue(animationNames.ToArray());
        }
        else
        {
            Debug.LogError("SignQueue reference is not set.");
        }
}

    public void PlayRecordedAudio()
    {
        audioSource.Play();
        Debug.Log("Playing recorded audio");
    }
}
