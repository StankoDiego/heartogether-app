using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnTranscripcion : MonoBehaviour
{
    // Start is called before the first frame update
    public Button btnTranscripcion;
    private AudioSource audioSource;
    private bool isRecording = false;

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
            audioSource.clip = Microphone.Start(null, false, 10, 44100);
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
            PlayRecordedAudio();
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
