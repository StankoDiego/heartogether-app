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
}
