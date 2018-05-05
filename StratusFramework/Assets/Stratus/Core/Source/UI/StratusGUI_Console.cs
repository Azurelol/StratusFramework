/******************************************************************************/
/*!
@file   Overlay_Console.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System.Collections.Generic;
using System;

namespace Stratus
{
  public partial class StratusGUI
  {
    /// <summary>
    /// A text console that receives lines of text
    /// </summary>
    public class Console : AbstractWindow
    {
      //------------------------------------------------------------------------/
      // Properties
      //------------------------------------------------------------------------/
      /// <summary>
      /// The size of the text buffer
      /// </summary>
      public int BufferSize = 5;

      //------------------------------------------------------------------------/
      // Fields
      //------------------------------------------------------------------------/
      /// <summary>
      /// The buffer of strings to print to the console
      /// </summary>
      private Queue<string> Buffer = new Queue<string>();

      /// <summary>
      /// This window is active as long as there are any messages on it
      /// </summary>
      public override bool active { get { return Buffer.Count != 0; } }

      //------------------------------------------------------------------------/
      // Methods
      //------------------------------------------------------------------------/
      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="name"></param>
      /// <param name="bufferSize"></param>
      public Console(string name, Vector2 relativeDimensions, Color color, Anchor position = Anchor.TopLeft)
        : base(name, relativeDimensions, color, position)
      {
      }

      /// <summary>
      /// Draws the console
      /// </summary>
      protected override void OnDrawWindow()
      {
        GUILayout.BeginVertical();        
        foreach (var message in Buffer)
          Print(message);
        GUILayout.EndVertical();
      }

      /// <summary>
      /// Prints a single message
      /// </summary>
      void Print(string message)
      {
        GUILayout.Label(message, GUI.skin.label);
      }

      /// <summary>
      /// Adds a message to the console
      /// </summary>
      /// <param name="message"></param>
      public void Log(object message)
      {
        if (Buffer.Count == BufferSize)
          Buffer.Dequeue();

        Buffer.Enqueue(message.ToString());
      }

    }
  }
}