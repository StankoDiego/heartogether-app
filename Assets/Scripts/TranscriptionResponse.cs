using System;
using System.Collections.Generic;

[Serializable]
public class TranscriptionResponse
{
  public string transcription;
  public List<Sign> signs;
}