/******************************************************************************/
/*!
@file   Search.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
@brief  Adds a custom search bar to Unity with its common commands.
@note   References:
        http://forum.unity3d.com/threads/search-bar.144093/
        http://answers.unity3d.com/questions/26736/focus-on-text-field-on-load.html
*/
/******************************************************************************/
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System;

namespace Stratus
{
  namespace Editor
  {

    /**************************************************************************/
    /*!
    @class Search Provides a quick search bar of all known Unity Editor commands.
    */
    /**************************************************************************/
    public class Search : UnityEditor.EditorWindow
    {
      static Search Instance;
      static private bool Enabled;

      static List<Command> AvailableCommands;
      static List<string >AvailableCommandNames = new List<string>();
      static Command CurrentCommand;

      static private string SearchText = "";
      static private Vector2 ScrollPos;

      /**************************************************************************/
      /*!
      @brief Initializes the Search window.
      */
      /**************************************************************************/
      static void Init()
      {
        Instance = (Search)UnityEditor.EditorWindow.GetWindow(typeof(Search), true, "Search");
      }

      /**************************************************************************/
      /*!
      @brief Toggles the Search window on and off.
      */
      /**************************************************************************/
      //[UnityEditor.MenuItem("Window/Search #_a")]
      static public void Toggle()
      {
        Enabled = !Enabled;
        SearchText = "";
        //Trace.Script("Enabled = '" + Enabled + "'");
        if (Enabled) Init();
        else if (Instance != null) Instance.Close();
      }        

      static void AddDelegateCommands()
      {
        foreach (var method in typeof(Commands).GetMethods())
        {
          if (method.IsPublic)
          {
            var callback = Delegate.CreateDelegate(typeof(DelegateCommand.Callback), method) as DelegateCommand.Callback;
            AvailableCommands.Add(new DelegateCommand(method.Name, callback));
            AvailableCommandNames.Add(method.Name);
          }
        }
      }

      /**************************************************************************/
      /*!
      @brief Adds the specified commands to the list of available commands.
      @param The path of the menu item.
      */
      /**************************************************************************/
      static void AddMenuitemCommand(string path)
      {
        //var name = Path.GetFileNameWithoutExtension(path);
        var name = path;
        AvailableCommands.Add(new MenuItemCommand(name, path));
        AvailableCommandNames.Add(name);
      }

      /**************************************************************************/
      /*!
      @brief Adds all known commands by hand.
      */
      /**************************************************************************/
      static void AddCommandsManually()
      {
        var commands = MenuItemsList.Generate();
        foreach (var command in commands)
          AddMenuitemCommand(command);        

      }      

      /**************************************************************************/
      /*!
      @brief Adds the commands to the list.
      */
      /**************************************************************************/
      static public void AddCommands()
      {
        Trace.Script("Adding all available commands...");
        AvailableCommands = new List<Command>();

        //AddMenuItems();
        //AddDelegateCommands();
        AddCommandsManually();
      }


      /**************************************************************************/
      /*!
      @brief Implements the Editor layout functions.
      */
      /**************************************************************************/
      void OnGUI()
      {
        if (UnityEngine.Event.current.type == EventType.Repaint)
        {
        }

        DisplayCommandList();
        CheckInput(UnityEngine.Event.current);
      }

      /**************************************************************************/
      /*!
      @brief Checks the current event for valid input.
      @param e A reference to the current unity event.
      */
      /**************************************************************************/
      void DisplayCommandList()
      {
        if (AvailableCommands == null) AddCommands();
        GUI.SetNextControlName("SearchBar");
        SearchText = GUILayout.TextField(SearchText);
        GUI.FocusControl("SearchBar");

        // Declare the GUI style
        GUIStyle customGUIStyle = GUI.skin.GetStyle("Button");
        customGUIStyle.alignment = TextAnchor.MiddleLeft;
        customGUIStyle.margin.top = 0; customGUIStyle.margin.bottom = 0;

        // Begin scroll
        ScrollPos = UnityEditor.EditorGUILayout.BeginScrollView(ScrollPos);

        // Filter the possible commands
        List<Command> filteredCommands = new List<Command>();                        
        var index = 0;
        foreach (var command in AvailableCommands)
        {
          if (string.IsNullOrEmpty(SearchText) || AvailableCommandNames[index++].Contains(SearchText))
          {
            filteredCommands.Add(command);            
            if (GUILayout.Button(command.Name, customGUIStyle))
            {
              command.Execute();
              Toggle();
            }
          }
        }

        UnityEditor.EditorGUILayout.EndScrollView();

        // Save the top command
        if (filteredCommands.Count > 0)
          CurrentCommand = filteredCommands[0];
        else
          CurrentCommand = null;        

        return;
      }

      /**************************************************************************/
      /*!
      @brief Checks the current event for valid input.
      @param e A reference to the current unity event.
      */
      /**************************************************************************/
      void CheckInput(UnityEngine.Event e)
      {
        if (e != null && e.isKey)
        {
          // If the user presses enter, execute the top command
          if (e.keyCode == KeyCode.Return && CurrentCommand != null)
          {
            Trace.Script("Executing '" + CurrentCommand.Name + "'");
            CurrentCommand.Execute();
            Toggle();
          }

          if (e.keyCode == KeyCode.Escape)
          {
            Toggle();
          }

        }
      }
      
    }
  }
}
