using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class ControllerUI : MonoBehaviour
{
    [SerializeField] private GameObject documentoPrefab;
    [SerializeField] private Transform panelTransform;
    private int verticalSpacing = 135; // Espaciado vertical entre los TextMesh
    private int initialPosY = 200;
    void Start()
    {
        string[] filesNames = GetAllPDFs();

        for (int i = 0; i < filesNames.Length; i++)
        {
            string fileName = Path.GetFileName(filesNames[i]);
            int posY = CalculatePositionY(i);
            CreatePrefab(fileName, new Vector3(0, posY, 0), i);
        }
    }

    string[] GetAllPDFs()
    {
        string[] pdfFiles = Directory.GetFiles(Application.persistentDataPath, "*.pdf");
        return pdfFiles;
    }

    int CalculatePositionY(int index)
    {
        return initialPosY - (index * verticalSpacing);
    }

    void CreatePrefab(string text, Vector3 position, int id)
    {
        // Instanciar el prefab de TextMesh
        GameObject cardDocument = Instantiate(documentoPrefab, panelTransform);
        cardDocument.transform.localPosition = position;

        string fileName = text.Split(".")[0];

        DateTime dateTime = DateTime.ParseExact(fileName, "yyyyMMddHHmmss", null);

        TMP_Text fechaTMP = cardDocument.transform.Find("FechaTxt").GetComponent<TMP_Text>();
        if (fechaTMP != null)
        {
            fechaTMP.text = dateTime.ToString("dd/MM/yyyy");
        }

        TMP_Text horaTMP = cardDocument.transform.Find("HoraTxt").GetComponent<TMP_Text>();
        if (horaTMP != null)
        {
            horaTMP.text = dateTime.ToString("HH:mm:ss");
        }
    }
}
