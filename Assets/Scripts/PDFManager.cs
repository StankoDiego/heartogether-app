using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using UnityEngine;

public class PDFManager : MonoBehaviour
{
    public static void CreateAndSavePdf(string data, GameObject modal, TMPro.TextMeshProUGUI transcriptionText)
    {
        string fileName = System.DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf";
        string path = Path.Combine(Application.persistentDataPath, fileName);

        Document document = new Document();
        PdfWriter.GetInstance(document, new FileStream(path, FileMode.Create));
        document.Open();
        document.Add(new Paragraph(data));
        document.Close();
        Debug.Log("PDF creado y guardado en: " + path);
        modal.SetActive(false);
        transcriptionText.text = "";
    }

    public static void OpenPdf(string fileName)
    {
        string filePath = Path.Combine(Application.temporaryCachePath, fileName);

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
