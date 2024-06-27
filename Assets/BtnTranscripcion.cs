using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class BtnTranscripcion : MonoBehaviour
{
    // Start is called before the first frame update
    public Button btnTranscripcion;
    private AudioSource audioSource;
    private bool isRecording = false;
    private string filePath;

    private const string apiUrl = "http://url:8001/api/transcribe";

    public void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        btnTranscripcion.onClick.AddListener(ToggleRecording);
    }

    public void ToggleRecording()
    {
        if (!isRecording)
        {
            StartRecording();
        }
        else
        {
            StopRecording();
        }
    }

    public void StartRecording()
    {
        if (Microphone.devices.Length > 0)
        {
            audioSource.clip = Microphone.Start(null, false, 10, 48000);
            isRecording = true;
            Debug.Log("Recording started");
        }
        else
        {
            Debug.LogWarning("No microphone detected");
        }
    }

    public void StopRecording()
    {
        if (isRecording)
        {
            Microphone.End(null);
            isRecording = false;
            Debug.Log("Recording stopped");
            SaveRecording(audioSource.clip);
            PlayRecordedAudio();
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
                Debug.Log("Error: " + www.error);
            }
            else
            {
                Debug.Log("Response: " + www.downloadHandler.text);
                GameObject textFieldObject = GameObject.Find("TranscriptionText");
                textFieldObject.GetComponent<Text>().text = www.downloadHandler.text;
            }
        }
    }

    public void PlayRecordedAudio()
    {
        audioSource.Play();
        Debug.Log("Playing recorded audio");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
