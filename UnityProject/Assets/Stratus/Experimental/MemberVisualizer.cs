using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Stratus.Dependencies.Ludiq.Reflection;
using System;
using UnityEngine.UI;
using TMPro;
using Stratus.Utilities;
using System.Text;

namespace Stratus
{
  public class MemberVisualizer : StratusBehaviour
  {
    [Serializable]
    public class MemberVisualizationField
    {
      public enum VisualizationMode
      {
        Scene,
        Game,
        SceneGUI,
        GameGUI
      }

      public enum PrefixScheme
      {
        Full,
        Member
      }

      //------------------------------------------------------------------------/
      // Fields
      //------------------------------------------------------------------------/
      [Tooltip("The member that will be inspected")]
      [Filter(Methods = false, Properties = true, NonPublic = true, ReadOnly = true, Static = true, Inherited = true, Fields = true)]
      public UnityMember member;
      [Tooltip("How this member is visualized")]
      public VisualizationMode visualizationMode = VisualizationMode.Scene;
      [Tooltip("What color to use")]
      public Color color = Color.white;
      [Tooltip("How to describe this member")]
      public PrefixScheme prefix = PrefixScheme.Full;

      //------------------------------------------------------------------------/
      // Properties
      //------------------------------------------------------------------------/
      public object latestValue { get; set; }
      public string description { get; set; }
      public GameObject gameObject { get; set; }
      public Transform transform { get; set; }
      public string hexColor { get; set; }
      public bool hasValue => latestValue != null;
      private string prefixString
      {
        get
        {
          switch (prefix)
          {
            case PrefixScheme.Full:
              return $"{member.component}.{member.name}";
            case PrefixScheme.Member:
              return member.name;
          }
          throw new Exception();
        }
      }

      //------------------------------------------------------------------------/
      // Methods
      //------------------------------------------------------------------------/
      public bool Initialize()
      {
        gameObject = member.target as GameObject;
        if (!gameObject)
          return false;

        transform = gameObject.transform;
        hexColor = $"#{color.ToHex()}";
        return true;
      }

      public void Update()
      {
        if (!member.isAssigned)
          return;

        latestValue = member.Get();
        if (hasValue)
        {
          description = $"{prefixString} = {latestValue.ToString()}";

        }
        else
        {
          description = $"{prefixString} = NULL";
        }
      }
    }

    [Serializable]
    public class MemberVisualizationRenderSettings
    {
      [HideInInspector]
      public GameObject gameObject;
      [Tooltip("Offset to use for drawing from the GameObject")]
      public Vector3 offset = Vector3.zero;
      [Tooltip("The size of font to use")]
      public float fontSize = 14f;
    }

    public class DrawList : Dictionary<GameObject, List<MemberVisualizationField>> { }

    //------------------------------------------------------------------------/
    // Fields
    //------------------------------------------------------------------------/
    [Tooltip("All the properties being inspected")]
    public List<MemberVisualizationField> getters = new List<MemberVisualizationField>();
    [Header("Settings")]
    [Tooltip("How often the members are polled for their current value")]
    [Range(0f, 3f)]
    public float pollFrequency = 1f;
    [Tooltip("Color used for the background in the scene view")]
    public Color backgroundColor = Color.white;
    [Tooltip("Color used for the outline in the scene view")]
    public Color outlineColor = Color.black;
    //public int fontSize = 14;
    [Tooltip("What prefab to use for drawing the information")]
    [SerializeField]
    public TextMeshPro textPrefab;
    [SerializeField]
    [HideInInspector]
    public List<MemberVisualizationRenderSettings> renderSettingsList;

    //------------------------------------------------------------------------/
    // Properties
    //------------------------------------------------------------------------/
    /// <summary>
    /// Active instances of the MemberVisualizer
    /// </summary>
    public static List<MemberVisualizer> instances { get; set; } = new List<MemberVisualizer>();
    /// <summary>
    /// All properties to be drawn in the scene view
    /// </summary>
    public DrawList sceneDrawList { get; private set; } = new DrawList();
    /// <summary>
    /// All properties to be drawn in the game view
    /// </summary>
    public DrawList gameDrawList { get; private set; } = new DrawList();
    /// <summary>
    /// All properties to be drawn in the scene view gui
    /// </summary>
    public DrawList sceneGUIDrawList { get; private set; } = new DrawList();
    /// <summary>
    /// All properties to be drawn in the scene view gui
    /// </summary>
    public DrawList gameGUIDrawList { get; private set; } = new DrawList();
    /// <summary>
    /// The draw list for all properties in the scene gui
    /// </summary>
    public static Dictionary<MemberVisualizer, DrawList> sceneGUIDrawLists { get; set; } = new Dictionary<MemberVisualizer, DrawList>();
    /// <summary>
    /// How many properties to draw in the scene gui
    /// </summary>
    public static int sceneGUIDrawCount { get; private set; }
    /// <summary>
    /// The draw list for all properties in the game gui
    /// </summary>
    public static Dictionary<MemberVisualizer, DrawList> gameGUIDrawLists { get; set; } = new Dictionary<MemberVisualizer, DrawList>();
    /// <summary>
    /// How many properties to draw in the game gui
    /// </summary>
    public static int gameGUIDrawCount { get; private set; }

    //------------------------------------------------------------------------/
    // Fields Private
    //------------------------------------------------------------------------/
    private Stopwatch pollTimer;
    private GUIStyle textStyle;
    private Dictionary<GameObject, TextMeshPro> textInstanceMap = new Dictionary<GameObject, TextMeshPro>();
    private Dictionary<GameObject, MemberVisualizationRenderSettings> renderSettingsMap;

    //------------------------------------------------------------------------/
    // Messages
    //------------------------------------------------------------------------/
    private void OnValidate()
    {
      UpdateRenderSettingsList();

      if (Application.isPlaying)
      {
        foreach (var member in getters)
          member.Initialize();
      }
    }

    private void Awake()
    {
      // Create the custom text style used by this visualizer
      textStyle = new GUIStyle();
      textStyle.richText = true;
      // Set the poll timer
      pollTimer = new Stopwatch(pollFrequency);
      pollTimer.SetCallback(OnPoll);
      // Create separate draw lists for different visualization modes
      CreateDrawLists();
      // Build the map used for custom rendering settings per object
      renderSettingsMap = new Dictionary<GameObject, MemberVisualizationRenderSettings>();
      foreach (var rs in renderSettingsList)
        renderSettingsMap.Add(rs.gameObject, rs);
      // For all members to be visualized in the game view...
      ConstructGameVisualizations();
      // Poll the member values initially
      OnPoll();
    }

    private void Update()
    {
      pollTimer.AutomaticUpdate(Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
      DrawSceneVisualization();
    }

    private void OnEnable()
    {
      instances.Add(this);
      sceneGUIDrawLists.Add(this, sceneGUIDrawList);
      sceneGUIDrawCount += sceneGUIDrawList.Count;
      gameGUIDrawLists.Add(this, gameGUIDrawList);
      gameGUIDrawCount += gameGUIDrawList.Count;

      if (Application.isPlaying && gameGUIDrawList.Count > 0 && !MemberVisualizerGameGUI.instantiated)
      {
        MemberVisualizerGameGUI.Instantiate();
        //MemberVisualizerGameGUI.get.Add(this);
      }

    }

    private void OnDisable()
    {      
      instances.Remove(this);
      sceneGUIDrawLists.Remove(this);
      sceneGUIDrawCount -= sceneGUIDrawList.Count;
      gameGUIDrawCount -= gameGUIDrawList.Count;
      gameGUIDrawLists.Remove(this);
      //MemberVisualizerGameGUI.get.Remove(this);
    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    private void DrawSceneVisualization()
    {
#if UNITY_EDITOR
      // For every GameObject, get all the properties for it
      foreach (var gameObject in sceneDrawList)
      {
        MemberVisualizationRenderSettings rs = renderSettingsMap[gameObject.Key];
        Vector3 pos = gameObject.Key.transform.position;
        pos += rs.offset;
        UnityEditor.Handles.BeginGUI();
        {
          string description = GetDescription(gameObject.Value);
          Rect rect = UnityEditor.HandleUtility.WorldPointToSizedRect(pos, new GUIContent(description), textStyle);
          UnityEditor.Handles.DrawSolidRectangleWithOutline(rect, backgroundColor, outlineColor);
          UnityEditor.Handles.Label(pos, description, textStyle);
        }
        UnityEditor.Handles.EndGUI();
      }
#endif
    }

    private void CreateDrawLists()
    {
      foreach (var memberField in getters)
      {
        bool valid = memberField.Initialize();
        if (!valid)
        {
          Trace.Error($"No member has been selected on the target {memberField.member.target.name}", this);
          continue;
        }

        switch (memberField.visualizationMode)
        {
          case MemberVisualizationField.VisualizationMode.Scene:
            AddToDrawList(memberField, sceneDrawList);
            break;
          case MemberVisualizationField.VisualizationMode.Game:
            AddToDrawList(memberField, gameDrawList);
            break;
          case MemberVisualizationField.VisualizationMode.SceneGUI:
            AddToDrawList(memberField, sceneGUIDrawList);
            break;
          case MemberVisualizationField.VisualizationMode.GameGUI:
            AddToDrawList(memberField, gameGUIDrawList);
            break;
          default:
            break;
        }
      }
    }

    private void AddToDrawList(MemberVisualizationField memberField, DrawList drawList)
    {
      if (!drawList.ContainsKey(memberField.gameObject))
        drawList.Add(memberField.gameObject, new List<MemberVisualizationField>());
      drawList[memberField.gameObject].Add(memberField);
    }

    private void ConstructGameVisualizations()
    {
      foreach (var gameObject in gameDrawList)
      {
        var text = GameObject.Instantiate(textPrefab);
        text.gameObject.name = gameObject.Key.name + " Member Visualizer Text";
        //text.transform.localPosition = Vector3.zero;
        MemberVisualizationRenderSettings rs = renderSettingsMap[gameObject.Key];
        text.fontSize = rs.fontSize;
        //text.fontSize = fontSize;
        textInstanceMap.Add(gameObject.Key, text);
      }
    }

    private void ClearDrawLists()
    {
      sceneDrawList.Clear();
      gameDrawList.Clear();
      sceneGUIDrawList.Clear();
      foreach (var gameObject in textInstanceMap)
      {
        TextMeshPro text = gameObject.Value;
        Destroy(text);
      }
      textInstanceMap.Clear();
    }

    private string GetDescription(List<MemberVisualizationField> list)
    {
      string description = string.Empty;
      for (int i = 0; i < list.Count; ++i)
      {
        MemberVisualizationField memberField = list[i];

        //if (!memberField.hasValue)
        //  continue;

        if (i > 0)
          description += "\n";
        description += ($"<color={memberField.hexColor}>{memberField.description}</color>");
      }
      return description;
    }

    private void UpdateRenderSettingsList()
    {
      // 1. Get the list of unique GameObjects
      var selectedGameObjects = new List<GameObject>();
      foreach (var memberField in getters)
      {
        var go = memberField.member.target as GameObject;
        if (go == null)
          continue;
        bool isPresent = selectedGameObjects.Contains(go);
        if (!isPresent)
          selectedGameObjects.Add(go);
      }

      // No objects are being inspected
      if (selectedGameObjects.Empty())
        return;

      // 2. Remove render settings for missing GameObjects
      renderSettingsList.RemoveAll(x => selectedGameObjects.Contains(x.gameObject) == false);

      // 3. Add render settings for new GameObjects
      foreach (var go in selectedGameObjects)
      {
        MemberVisualizationRenderSettings rs = renderSettingsList.Find(x => x.gameObject == go);
        bool isPresent = rs == null;
        if (isPresent)
          renderSettingsList.Add(new MemberVisualizationRenderSettings() { gameObject = go });
      }
    }

    //------------------------------------------------------------------------/
    // Methods
    //------------------------------------------------------------------------/
    private void OnPoll()
    {
      foreach (var memberField in getters)
        memberField.Update();

      Vector3 cameraPosition = Camera.main.transform.position;
      Vector3 cameraNormal = Camera.main.transform.forward;

      foreach (var kp in gameDrawList)
      {
        GameObject go = kp.Key;
        string description = GetDescription(kp.Value);
        TextMeshPro textInstance = textInstanceMap[go];
        MemberVisualizationRenderSettings rs = renderSettingsMap[go];
        textInstance.transform.position = go.transform.position + rs.offset;
        textInstance.fontSize = rs.fontSize;
        textInstance.text = description;
        // Apply billboarding
        textInstance.transform.LookAt(cameraNormal + textInstance.transform.position);
        textInstance.transform.Rotate(0, 180f, 0);
      }

    }


  }

}