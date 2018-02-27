using UnityEngine;
using Stratus;

namespace Stratus
{
  public partial class StratusGUI
  {
    /// <summary>
    /// A simple in-game FPS counter. Courtesy of Catlike coding tutorials
    /// </summary>
    public class FPSCounter
    {
      //----------------------------------------------------------------------/
      // Properties
      //----------------------------------------------------------------------/
      /// <summary>
      /// The target frame rate
      /// </summary>
      public int frameRange { get; private set; } = 60;

      public int averageFPS { get; private set; }
      public int highestFPS { get; private set; }
      public int lowestFPS { get; private set; }

      public string averageFPSLabel => stringsFrom00To99[Mathf.Clamp(averageFPS, 0, 99)];
      public string highestFPSLabel => stringsFrom00To99[Mathf.Clamp(highestFPS, 0, 99)];
      public string lowestFPSLabel => stringsFrom00To99[Mathf.Clamp(lowestFPS, 0, 99)];

      //----------------------------------------------------------------------/
      // Fields
      //----------------------------------------------------------------------/
      int[] fpsBuffer;
      int fpsBufferIndex;
      private static string[] stringsFrom00To99 = {
        "00", "01", "02", "03", "04", "05", "06", "07", "08", "09",
        "10", "11", "12", "13", "14", "15", "16", "17", "18", "19",
        "20", "21", "22", "23", "24", "25", "26", "27", "28", "29",
        "30", "31", "32", "33", "34", "35", "36", "37", "38", "39",
        "40", "41", "42", "43", "44", "45", "46", "47", "48", "49",
        "50", "51", "52", "53", "54", "55", "56", "57", "58", "59",
        "60", "61", "62", "63", "64", "65", "66", "67", "68", "69",
        "70", "71", "72", "73", "74", "75", "76", "77", "78", "79",
        "80", "81", "82", "83", "84", "85", "86", "87", "88", "89",
        "90", "91", "92", "93", "94", "95", "96", "97", "98", "99"
      };

      //----------------------------------------------------------------------/
      // Methods
      //----------------------------------------------------------------------/
      public void Update()
      {
        if (fpsBuffer == null || fpsBuffer.Length != frameRange)
          InitializeBuffer();

        UpdateBuffer();
        CalculateFPS();
      }

      public void Display()
      {

      }

      void InitializeBuffer()
      {
        if (frameRange <= 0)
          frameRange = 1;

        fpsBuffer = new int[frameRange];
        fpsBufferIndex = 0;
      }

      void UpdateBuffer()
      {
        fpsBuffer[fpsBufferIndex++] = (int)(1f / Time.unscaledDeltaTime);
        if (fpsBufferIndex >= frameRange)
          fpsBufferIndex = 0;
      }

      void CalculateFPS()
      {
        int sum = 0;
        int highest = 0;
        int lowest = int.MaxValue;
        for (int i = 0; i < frameRange; i++)
        {
          int fps = fpsBuffer[i];
          sum += fps;
          if (fps > highest)
            highest = fps;
          if (fps < lowest)
            lowest = fps;

          averageFPS = (int)(float)sum / frameRange;
        }
      }





    }




  }

}
