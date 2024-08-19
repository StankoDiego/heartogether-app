using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrefabController : MonoBehaviour
{
    private string filePath;

    public void SetData(string filePath)
    {
        this.filePath = filePath;
    }

    public void OnClick()
    {
        // Aquí puedes mostrar los datos como desees, por ejemplo, en una UI de detalles.
        if (!File.Exists(filePath))
        {
            Debug.LogError("PDF file not found: " + filePath);
            return;
        }

        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");

        AndroidJavaObject uri = new AndroidJavaClass("android.net.Uri").CallStatic<AndroidJavaObject>("parse", "file://" + filePath);
        AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", "android.intent.action.VIEW");
        intent.Call<AndroidJavaObject>("setDataAndType", uri, "application/pdf");
        intent.Call<AndroidJavaObject>("addFlags", 268435456); // FLAG_ACTIVITY_NEW_TASK

        currentActivity.Call("startActivity", intent);
    }
}
