using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.IMGUI.Controls;
using System;

namespace Stratus
{

  [Serializable]
  public class StratusTreeView<T> where T : UnityEngine.Object
  {
    //------------------------------------------------------------------------\
    // Fields
    //------------------------------------------------------------------------\
    [SerializeField] TreeViewState treeViewState;
    [SerializeField] MultiColumnHeaderState multiColumnHeaderState;
    SearchField searchField;
    //MultiColumnTreeView<T>

    //------------------------------------------------------------------------\
    // Properties
    //------------------------------------------------------------------------\
    public bool initialized { get; private set; }


    //------------------------------------------------------------------------\
    // Methods
    //------------------------------------------------------------------------\
    public void BuildRoot()
    {
      
    }
  }

}