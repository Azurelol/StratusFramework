#define FULL_VERSION

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Wasabimole.Core;

// ---------------------------------------------------------------------------------------------------------------------------
// Inspector Navigator - © 2014, 2015 Wasabimole http://wasabimole.com
// ---------------------------------------------------------------------------------------------------------------------------
// HOW TO - INSTALLING INSPECTOR NAVIGATOR:
//
// - Import package into project
// - Then click on:
//      Window -> Inspector Navigator -> Open Inspector Navigator Window
//
// The window will appear grouped with the Inspector by default, in a different tab. We recommend then to drag the 
// Inspector window just below the Inspector Navigator, and then adjust the Navigator's window height to a minimum.
// ---------------------------------------------------------------------------------------------------------------------------
// [NOTE] The source is intended to be used as is. Do NOT compile it into a DLL. We can't give support to modified versions.
// ---------------------------------------------------------------------------------------------------------------------------
// HOW TO - USING INSPECTOR NAVIGATOR:
//
// - Use the back '<' and forward '>' buttons to navigate to recent inspectors
// - Hotkeys [PC]:  Ctrl + Left/Right 
// - Hotkeys [Mac]: Alt + Cmd + Left/Right
// - Click on the breadcrumb bar to jump to any object
// - Click on the padlock icon to lock the tool to current objects
// - Drag and drop objects from the breadcrumb bar
// - To delete a breadcrumb, drag it to the Remove box
// - Access "Help and Options" from the menu to edit tool preferences
//
// [NOTE] Hotkeys can be changed by editing Wasabimole/Inspector Navigator/Editor/KeyBindings.cs
// ---------------------------------------------------------------------------------------------------------------------------
// INSPECTOR NAVIGATOR - VERSION HISTORY
//
// 1.23 Alignment update
// - New option to set breadcrumb bar horizontal alignment preference
// - New Asset Store Inspector filter
// - Fixed breadcrumbs objects leaking when breadcrumbs were not serialized
// - Keep assets in breadcrumbs when switching scenes and breadcrumbs are not serialized
// - Use shared Wasabimole.Core.UpdateNotifications library
// - Other small bug fixes
//
// 1.22 Disable serialization update
// - Serialization of breadcrumbs on scenes can now be disabled
// - Dialog for clearing project scenes when checking serialization off
// - Optimized tool start-up times when opening up a scene
// - Fixed problem where breadcrumbs disappeared when going into play mode
// - Unselecting an object no longer marks the scene as changed
// - Changed default object queue length to 64
// - Other small bug fixes
// 
// 1.20 Strip breadcrumbs update
// - InspectorBreadcrumbs no longer included in build on Unity 5 (using new HideFlags.DontSaveInBuild)
// - New menu option to delete inspector breadcrumbs from all project scenes (so they can be removed before performing a build on Unity 4.X)
// - InspectorBreadcrumbs are no longer created again right after being deleted (only after a new object/asset selection)
// - Do not create InspectorBreadcrumbs when selecting any filtered object
// - Upon load scene, do not automatically center camera on last selected object
// - New Scripts & TextAssets filter type, disabled by default
// - New ProjectSettings filter type, disabled by default
// - Removed all filtered objects from breadcrumbs on load scene
// - Modifying object filters has now immediate effect
// - InspectorBreadcrumbs object is now filtered, and no longer appears in the breadcrumbs bar
// - Warning before enabling project settings tracking on Unity 4.X
// - New menu option to check for new user notifications
// - Other small bug fixes
//
// 1.18 Keys & colors update
// - New button in options to define the hotkeys
// - Moved hot-key definitions to KeyBindings.cs
// - Option to set the text color for different object types
// - Fixed breadcrumbs not being properly removed
// - Fixed changing InspectorBreadcrumbs visibility
// - Fixed visual glitches on play mode
// - Other small bug fixes
//
// 1.16 Remove breadcrumbs update
// - Allow removing breadcrumbs by dragging them into the "Remove" box
// - Option to remove and not track unnamed objs
// - Fixed issue with lost notification messages
// - Remove any duplicate inspector breadcrumbs scene objects
// - Remove inspector breadcrumbs from scene when closing the tool
// - Allow deleting by hand InspectorBreadcrumbs object
// - Other small bug fixes
//
// 1.15 Unity 5 Hotfix [Must delete previous version first]
// - Fixed error GameObject (named 'BreadCrumbs') references runtime script in scene file. Fixing!​
// - Restructured project folders, now under Wasabimole/Inspector Navigator (must delete old Editor/InspectorNavigator.cs and Editor/NotificationCenter.cs files!)
// - Added option to show breadcrumbs object in the scene
// - Other small bug fixes
// 
// 1.14 Drag and drop update
// - Drag and drop breadcrumbs to any other Unity window or field
// - Set minimum window size to match the width of the 2 arrows
// - Selecting a filtered object now properly unselects breadcrumb
// - Added Wasabimle logo to help and options window
// - Used base64 for resource images
// - Other small bug fixes
//
// 1.11 Bug fixes update
// - Several small bug fixes
// - Removed compilation warnings
// - Option to check for plugin updates
// - Option to show other notifications
//
// 1.10 Big update
// - Restore previous scene camera when navigating back to an object
// - New improved breadcrumbs system and serialization method
// - Object breadcrumbs are now local to every scene
// - Optimized code for faster OnGUI calls
// - Option to filter which inspectors to track (scene objects, assets, folders, scenes)
// - Option to remove all duplicated objects
// - Option to set maximum number of enqueued objects
// - Option to mark the scene as changed or not on new selections
// - Option to review the plugin on the Asset Store
// - Other small bug fixes
//
// 1.07 Breadcrumb++ update
// - Improved breadcrumb bar behaviour
// - New Help and Options window
// - New tool padlock to lock to current objects
// - Option to set max label width
// - Option to clear or insert when selecting new objects
// - Option to remove duplicated objects when locked
// - Option to choose scene camera behaviour
// - Fixed default hotkeys on Mac to Alt + Cmd + Left/Right
// - Other small bug fixes
// 
// 1.03 Hotkeys update
// - Added Inspector Navigator submenu + hotkeys Ctrl/Cmd + Left/Right
// - Limited queue size
// - Handle Undo/Redo operations better
// - Handle inspector lock states
//
// 1.02 First public release
// - Small bug fixes
//
// 1.00 Initial Release
// – Back and Forward buttons navigate to recent object inspectors
// – Breadcrumb bar shows recent objects and allows click to jump to them
// – Inspector history is serialized when closing and opening Unity
// ---------------------------------------------------------------------------------------------------------------------------
// Thank you for choosing this extension, we sincerely hope you like it!
//
// Please send your feedback and suggestions to mailto://contact@wasabimole.com
// ---------------------------------------------------------------------------------------------------------------------------

namespace Wasabimole.InspectorNavigator.Editor
{
    public enum CameraBehavior
    {
        DoNotChange,
        AutoFrameSelected,
        RestorePreviousCamera
    }

    public enum DuplicatesBehavior
    {
        LeaveDuplicates,
        RemoveWhenLocked,
        RemoveAllDuplicates
    }

    public enum BarAlignment
    {
        Right,
        Center,
        Left
    }

    public class InspectorNavigator : EditorWindow, IHasCustomMenu
    {
        public const int CurrentVersion = 123;

        static InspectorNavigator _instance;
        public static InspectorNavigator Instance { get { if (_instance == null) OpenWindow(); return _instance; } }

        static GUIStyle LabelStyle;
        static GUIStyle DeleteLabelStyle;
        static Color DeleteLabelStyleColor;
        static GUIStyle OpaqueBoxStyle;
        static Texture2D backBtn;
        static Texture2D forwardBtn;
        static GUIContent gc = new GUIContent();
        static Texture2D grayLabel;
        static Texture2D greenLabel;
        static Texture2D yellowLabel;
        static Texture2D redLabel;

        InspectorBreadcrumbs _breadcrumbs;
        public InspectorBreadcrumbs breadcrumbs { get { return GetInspectorBreadcrumbs(); } }

        InspectorBreadcrumbs GetInspectorBreadcrumbs()
        {
            if (_breadcrumbs == null)
            {
                foreach (var br in Transform.FindObjectsOfType<InspectorBreadcrumbs>())
                    if (_breadcrumbs != null)
                        DestroyImmediate(br.gameObject);
                    else
                        _breadcrumbs = br;
                if (_breadcrumbs == null)
                {
                    _breadcrumbs = new GameObject("InspectorBreadcrumbs").AddComponent<InspectorBreadcrumbs>();
                    _breadcrumbs.DataVersion = CurrentVersion;
                }
                SetBreadcrumbsProperties();
            }
            return _breadcrumbs;
        }

        bool SceneHasBreadcrumbs()
        {
            return Transform.FindObjectOfType<InspectorBreadcrumbs>() != null;
        }

        public void SetBreadcrumbsProperties()
        {
            if (_breadcrumbs == null) return;
#if !(UNITY_4_3||UNITY_4_5||UNITY_4_6)
            _breadcrumbs.gameObject.hideFlags &= ~HideFlags.DontSaveInBuild;
#endif
#if FULL_VERSION
            if (!SerializeBreadcrumbs)
            {
                if (_breadcrumbs.gameObject.hideFlags != HideFlags.HideAndDontSave)
                {
                    _breadcrumbs.gameObject.SetActive(false);
                    _breadcrumbs.gameObject.hideFlags = HideFlags.HideAndDontSave;
                    _breadcrumbs.gameObject.SetActive(true);
                }
            }
            else if (ShowBreadcrumbsObject)
            {
                if (_breadcrumbs.gameObject.hideFlags != HideFlags.None)
                {
                    _breadcrumbs.gameObject.SetActive(false);
                    _breadcrumbs.gameObject.hideFlags = HideFlags.None;
                    _breadcrumbs.gameObject.SetActive(true);
                }
            } else
#endif
            if (_breadcrumbs.gameObject.hideFlags != (HideFlags.HideInHierarchy | HideFlags.HideInInspector))
            {
                _breadcrumbs.gameObject.SetActive(false);
                _breadcrumbs.gameObject.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
                _breadcrumbs.gameObject.SetActive(true);
            }
#if !(UNITY_4_3||UNITY_4_5||UNITY_4_6)
            _breadcrumbs.gameObject.hideFlags |= HideFlags.DontSaveInBuild;
#endif
        }

        List<ObjectReference> Back { get { return breadcrumbs.Back; } set { breadcrumbs.Back = value; } }
        List<ObjectReference> Forward { get { return breadcrumbs.Forward; } set { breadcrumbs.Forward = value; } }
        ObjectReference CurrentSelection { get { return breadcrumbs.CurrentSelection; } set { breadcrumbs.CurrentSelection = value; } }

        [SerializeField]
        public int ConfigurationDataVersion = 100;
        [SerializeField]
        public bool Locked = false;

#if FULL_VERSION
        [SerializeField]
        public int MaxLabelWidth = 0;
        [SerializeField]
        public bool InsertNewObjects = false;
        [SerializeField]
        public bool TrackObjects = true;
        [SerializeField]
        public bool TrackAssets = true;
        [SerializeField]
        public bool TrackFolders = false;
        [SerializeField]
        public bool TrackScenes = false;
        [SerializeField]
        public bool TrackTextAssets = false;
        [SerializeField]
        public bool TrackProjectSettings = false;
        [SerializeField]
        public bool TrackAssetStoreInspector = false;
        [SerializeField]
        public bool ForceDirty = false;
        [SerializeField]
        public int MaxEnqueuedObjects = 64;
        [SerializeField]
        public CameraBehavior CameraBehavior = CameraBehavior.RestorePreviousCamera;
        [SerializeField]
        public DuplicatesBehavior DuplicatesBehavior = DuplicatesBehavior.RemoveWhenLocked;
        [SerializeField]
        public bool CheckForUpdates = true;
        [SerializeField]
        public bool OtherNotifications = true;
        [SerializeField]
        public bool ShowBreadcrumbsObject = false;
        [SerializeField]
        public bool RemoveUnnamedObjects = false;
        [SerializeField]
        public Color ColorInstance = Color.white;
        [SerializeField]
        public Color ColorAsset = Color.white;
        [SerializeField]
        public Color ColorFolder = Color.white;
        [SerializeField]
        public Color ColorScene = Color.white;
        [SerializeField]
        public Color ColorTextAssets = Color.white;
        [SerializeField]
        public Color ColorProjectSettings = Color.white;
        [SerializeField]
        public Color ColorAssetStoreInspector = Color.white;
        [SerializeField]
        public bool SerializeBreadcrumbs = true;
        [SerializeField]
        public BarAlignment BarAlignment = BarAlignment.Right;

        ObjectReference lastSelection = null;
#else
        int MaxEnqueuedObjects = 64;
#endif

        bool areAllInspectorsLocked;
        bool wasLocked = false;
        bool isDragging = false;

        [SerializeField]
        public UpdateNotifications UpdateNotifications;

        IEnumerable InspectorList;

        GUIStyle lockButtonStyle;

        Rect rectLabels, rectRightCut, rectLeftCut;

        float centerX, labelOffset;
        float maxWidth, currentWidthHalf;
        string lastScene = "";

        [MenuItem("Window/Inspector Navigator/Open Inspector Navigator Window")]
        public static void OpenWindow()
        {
            var ins = GetWindow<InspectorNavigator>("Insp.Navigator", false, new System.Type[] { GetInspectorWindowType() });
            ins.minSize = new Vector2(84, 20);
            if (ins != null) _instance = ins;
        }

#if UNITY_EDITOR_OSX
        [MenuItem("Window/Inspector Navigator/Back " + KeyBindings.BackMac)]
#else
        [MenuItem("Window/Inspector Navigator/Back " + KeyBindings.BackPC)]
#endif
        public static void DoBackCommand()
        {
            Instance.UnlockFirstInspector();
            Instance.UpdateNotifications.AddUsage();
            if (Instance.CurrentSelection != null) Instance.UpdateCurrentSelection();
            if (Instance.DoBack())
            {
                if (Instance.CurrentSelection != null) Instance.RestorePreviousCamera();
                Instance.Repaint();
            }
        }

#if UNITY_EDITOR_OSX
        [MenuItem("Window/Inspector Navigator/Forward " + KeyBindings.ForwardMac)]
#else
        [MenuItem("Window/Inspector Navigator/Forward " + KeyBindings.ForwardPC)]
#endif
        public static void DoForwardCommand()
        {
            Instance.UnlockFirstInspector();
            Instance.UpdateNotifications.AddUsage();
            if (Instance.CurrentSelection != null) Instance.UpdateCurrentSelection();
            if (Instance.DoForward())
            {
                if (Instance.CurrentSelection != null) Instance.RestorePreviousCamera();
                Instance.Repaint();
            }
        }

        [MenuItem("Window/Inspector Navigator/Clear current scene breadcrumbs")]
        public static void ClearSceneBreadcrumbs()
        {
            if (Instance._breadcrumbs != null)
            {
                Instance.Back.Clear();
                Instance.Forward.Clear();
                Instance.Locked = false;
                if (Instance.CurrentSelection != null)
                {
                    Instance.UpdateCurrentSelection();
                    Instance.CurrentSelection = null;
                }
                Instance.Repaint();
            }
            foreach (var go in SceneRoots())
            {
                if (go.name != "InspectorBreadcrumbs") continue;
                if (go.transform.childCount > 0) continue;
                if (go == Instance._breadcrumbs) continue;
                if (go.GetComponents(typeof(Component)).Length >= 3) continue;
                DestroyImmediate(go);
            }
            if (Instance._breadcrumbs != null)
            {
                var tmp = Instance._breadcrumbs;
                Instance._breadcrumbs = null;
                DestroyImmediate(tmp.gameObject);
            }
        }

        [MenuItem("Window/Inspector Navigator/Clear all project scene breadcrumbs")]
        public static void ClearAllProjectBreadcrumbs()
        {
            ClearSceneBreadcrumbs();
            EditorApplication.SaveCurrentSceneIfUserWantsTo();

            var projectAssets = AssetDatabase.GetAllAssetPaths();
            foreach (var asset in projectAssets)
            {
                if (System.IO.Path.GetExtension(asset) != ".unity") continue;
                if (!EditorApplication.OpenScene(asset)) continue;
                Debug.Log("> Clearing breadcrumbs from " + asset);
                ClearSceneBreadcrumbs();
                EditorApplication.SaveScene();
            }
            Debug.Log("Cleared all inspector breadcrumbs from project scenes!");
        }

        [MenuItem("Window/Inspector Navigator/Check for new user notifications")]
        public static void CheckForNewUserNotifications()
        {
            Instance.UpdateNotifications.ForceGetNotification();
        }

        [MenuItem("Window/Inspector Navigator/Show last notification again")]
        public static void ShowLastNotification()
        {
            Instance.UpdateNotifications.ShowPreviousNotification();
        }

        [MenuItem("Window/Inspector Navigator/Show last notification again", true)]
        public static bool HasLastNotification()
        {
            return Instance.UpdateNotifications.HasPreviousNotification;
        }

        public static IEnumerable<GameObject> SceneRoots()
        {
            var prop = new HierarchyProperty(HierarchyType.GameObjects);
            var expanded = new int[0];
            while (prop.Next(expanded))
            {
                yield return prop.pptrValue as GameObject;
            }
        }

        [MenuItem("Window/Inspector Navigator/Help and Options ...")]
        public static void ShowHelpAndOptions()
        {
            GetWindowWithRect<HelpAndOptions>(new Rect(0, 0, HelpAndOptions.Width, HelpAndOptions.Height), true, "Help and Options", true);
        }

        static System.Type GetInspectorWindowType()
        {
            return typeof(EditorGUI).Assembly.GetType("UnityEditor.InspectorWindow");
        }

        void OnEnable()
        {
#if FULL_VERSION
            UpdateNotifications = new UpdateNotifications(CurrentVersion, "Inspector Navigator", 26181, Repaint, 0x2BC, CheckForUpdates, OtherNotifications, false);
#else
            NotificationCenter = new NotificationCenter(CurrentVersion, "Inspector Navigator", 26181, Repaint, 0x220, 0x80);
#endif
            minSize = maxSize = new Vector2(300, 22);
            GetInspectorList();
        }

        void OnDestroy()
        {
            foreach (var br in Transform.FindObjectsOfType<InspectorBreadcrumbs>())
                DestroyImmediate(br.gameObject);
        }

        void ShowButton(Rect position)
        {
            if (lockButtonStyle == null)
                lockButtonStyle = "IN LockButton";
            Locked = GUI.Toggle(position, Locked, GUIContent.none, lockButtonStyle);
        }

        float GetLabelWidthFromInstance(ObjectReference obj)
        {
            if (obj == null || obj.OReference == null) return 0;
            gc.text = obj.OReference.name;
            if (string.IsNullOrEmpty(gc.text)) gc.text = "...";
            var w = LabelStyle.CalcSize(gc).x;
#if FULL_VERSION
            if (MaxLabelWidth > 0f && w > MaxLabelWidth) w = MaxLabelWidth;
#endif
            return w;
        }

        bool DrawLabel(Event e, ObjectReference obj, float x, float w, Texture2D background)
        {
            if (obj == null || obj.OReference == null)
                return false;
            var text = obj.OReference.name;
            if (string.IsNullOrEmpty(text)) text = "...";
            LabelStyle.normal.background = background;
            gc.text = text;
            rectLabels.width = w;
            rectLabels.x = x;
#if FULL_VERSION
            switch(obj.OType)
            {
                case ObjectType.Asset: LabelStyle.normal.textColor = ColorAsset; break;
                case ObjectType.Instance: LabelStyle.normal.textColor = ColorInstance; break;
                case ObjectType.Folder: LabelStyle.normal.textColor = ColorFolder; break;
                case ObjectType.Scene: LabelStyle.normal.textColor = ColorScene; break;
                case ObjectType.TextAssets: LabelStyle.normal.textColor = ColorTextAssets; break;
                case ObjectType.ProjectSettings: LabelStyle.normal.textColor = ColorProjectSettings; break;
                case ObjectType.AssetStoreAssetInspector: LabelStyle.normal.textColor = ColorAssetStoreInspector; break;
                default: LabelStyle.normal.textColor = Color.white; break;
            }
            GUI.Label(rectLabels, gc, LabelStyle);
            LabelStyle.normal.textColor = Color.white;
#else
            GUI.Label(rectLabels, gc, LabelStyle);
#endif
            if (rectLabels.Contains(e.mousePosition) && e.type == EventType.MouseDrag && e.mousePosition.x < rectRightCut.x)
            {
                DragAndDrop.PrepareStartDrag();
                DragAndDrop.objectReferences = new Object[] { obj.OReference };
                string asset_path = AssetDatabase.GetAssetPath(obj.OReference);
                if (asset_path.Length > 0) DragAndDrop.paths = new[] { asset_path.Length > 0 ? asset_path : null };
                DragAndDrop.StartDrag("Breadcrumb");
                DragAndDrop.SetGenericData("ObjectReference", obj);
                isDragging = true;
                e.Use();
                return false;
            }
            return (e.type == EventType.mouseUp && rectLabels.Contains(e.mousePosition) && e.mousePosition.x < rectRightCut.x);
        }

        public void ClearDuplicates()
        {
            if (CurrentSelection == null || Back == null || Forward == null) return;
            var hash = new HashSet<Object>();
            var list = new List<ObjectReference>();
            hash.Add(null);
            if (CurrentSelection.OReference != null) hash.Add(CurrentSelection.OReference);
            foreach (var i in Back)
                if (!hash.Contains(i.OReference)) { hash.Add(i.OReference); list.Add(i); }
            var tmp = Back;
            Back = list;
            list = tmp;
            list.Clear();
            foreach (var i in Forward)
                if (!hash.Contains(i.OReference)) { hash.Add(i.OReference); list.Add(i); }
            Forward = list;
        }

        Object[] emptyObjectArray = new Object[0];

        public void OnGUI()
        {
            Preloads();
            CheckIfAllInspectorsAreLocked();

            if (_breadcrumbs == null)
            {
                EditorGUILayout.BeginHorizontal();
                GetControlRect(Screen.width - 90);
                GUI.enabled = false;
                GUILayout.Button(backBtn);
                GUILayout.Button(forwardBtn);
                GUI.enabled = true;
                if (UpdateNotifications.HasNotification) DrawNotificationIcon(Event.current);
                EditorGUILayout.EndHorizontal();
                return;
            }

            var eventCurrent = Event.current;
            var eventType = eventCurrent.type;

#if FULL_VERSION
            if (Locked && !wasLocked && DuplicatesBehavior == DuplicatesBehavior.RemoveWhenLocked) ClearDuplicates();
            if (RemoveUnnamedObjects && CurrentSelection != null && CurrentSelection.OReference != null && string.IsNullOrEmpty(CurrentSelection.OReference.name))
            {
                CurrentSelection = null;
                Repaint();
            }
#else
            if (Locked && !wasLocked) ClearDuplicates();
#endif

            wasLocked = Locked;

            if (eventType == EventType.MouseUp || eventType == EventType.MouseDown)
            {
                DragAndDrop.PrepareStartDrag();
                DragAndDrop.paths = null;
                DragAndDrop.objectReferences = emptyObjectArray;
            }
            else if (eventType == EventType.DragUpdated)
            {
                var data = DragAndDrop.GetGenericData("ObjectReference");
                if (data != null && data is ObjectReference && (ObjectReference)data != null) isDragging = true;
            }
            else if (eventType == EventType.DragExited)
            {
                isDragging = false;
            }

            EditorGUILayout.BeginHorizontal();

            if (eventType == EventType.Repaint)
            {
                rectLabels = GetControlRect(Screen.width - 90);
                rectLabels.y += 3f;
                rectRightCut = rectLabels;
                rectRightCut.x += rectLabels.width;
                rectRightCut.width = 90f;
                rectRightCut.height--;
                rectLeftCut = rectLabels;
                rectLeftCut.x -= 4f;
                rectLeftCut.width = 4f;
                maxWidth = rectLabels.width * 0.5f;
                currentWidthHalf = CurrentSelection == null ? 0f : GetLabelWidthFromInstance(CurrentSelection) * 0.5f;
                ComputeLabelPositions(maxWidth, currentWidthHalf, out centerX, out labelOffset);
            }
            else GetControlRect(Screen.width - 90);

            if (eventType == EventType.Repaint || eventType == EventType.MouseUp || eventType == EventType.MouseDrag)
            {
                int iBack = 0, iForward = 0;
                DrawLabels(eventCurrent, ref iBack, ref iForward, maxWidth, currentWidthHalf, centerX, labelOffset);

                if (iForward != 0)
                {
                    UpdateNotifications.AddUsage();
                    if (CurrentSelection != null) UpdateCurrentSelection();
                    for (int i = 0; i < iForward; ++i)
                        DoForward();
                    if (CurrentSelection != null) RestorePreviousCamera();
                    Repaint();
                }
                else if (iBack != 0)
                {
                    UpdateNotifications.AddUsage();
                    if (CurrentSelection != null) UpdateCurrentSelection();
                    for (int i = 0; i < iBack; ++i)
                        DoBack();
                    if (CurrentSelection != null) RestorePreviousCamera();
                    Repaint();
                }
            }


            GUI.color = GetSkinBackgroundColor();
            GUI.DrawTexture(rectRightCut, EditorGUIUtility.whiteTexture);
            GUI.DrawTexture(rectLeftCut, EditorGUIUtility.whiteTexture);
            GUI.color = Color.white;
            
            EditorGUILayout.BeginHorizontal(GUILayout.Width(80));

            if (isDragging)
            {
                GUILayout.Label("Remove", DeleteLabelStyle);
                var rect = GUILayoutUtility.GetLastRect();
                if (rect.Contains(eventCurrent.mousePosition))
                {
                    DeleteLabelStyle.normal.textColor = Color.green;
                    if (eventType == EventType.DragUpdated)
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Move;
                        eventCurrent.Use();
                    }
                    else if (eventType == EventType.DragPerform) 
                    {
                        var data = DragAndDrop.GetGenericData("ObjectReference");
                        if (data != null && data is ObjectReference && (ObjectReference)data != null)
                        {
                            var or = (ObjectReference)data;
                            if (CurrentSelection == or)
                            {
                                Instance.UpdateCurrentSelection();
                                Instance.CurrentSelection = null;
                            }
                            else
                            {
                                for (int i = Back.Count - 1; i >= 0; i--)
                                {
                                    var obj = Back[i];
                                    if (obj != or) continue;
                                    Back.RemoveAt(i);
                                }
                                for (int i = Forward.Count - 1; i >= 0; i--)
                                {
                                    var obj = Forward[i];
                                    if (obj != or) continue;
                                    Forward.RemoveAt(i);
                                }
                            }
                        }

                        isDragging = false;
                        Repaint();
                    }
                }
                else
                {
                    DeleteLabelStyle.normal.textColor = DeleteLabelStyleColor;
                    if (eventType == EventType.DragUpdated)
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                        eventCurrent.Use();
                    }
                }
                GUI.Label(rect, "Remove", DeleteLabelStyle);
                GUILayout.Label(string.Empty,GUILayout.Width(0));
            }
            else
            {
                if (eventType == EventType.DragUpdated)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Rejected; 
                    eventCurrent.Use();
                }

                GUI.enabled = Back.Count > 0;
                if (EditorApplication.isPlayingOrWillChangePlaymode) GUI.color = colorDarken;
                if (GUILayout.Button(backBtn))
                {
                    UnlockFirstInspector();
                    UpdateNotifications.AddUsage();
                    if (CurrentSelection != null) UpdateCurrentSelection();
                    if (DoBack())
                        if (CurrentSelection != null) RestorePreviousCamera();
                }
                GUI.enabled = Forward.Count > 0;
                if (GUILayout.Button(forwardBtn))
                {
                    UnlockFirstInspector();
                    UpdateNotifications.AddUsage();
                    if (CurrentSelection != null) UpdateCurrentSelection();
                    if (DoForward())
                        if (CurrentSelection != null) RestorePreviousCamera();
                }
                GUI.color = Color.white;
            }
            GUI.enabled = true;

            if (UpdateNotifications.HasNotification) DrawNotificationIcon(eventCurrent);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndHorizontal();

#if FULL_VERSION
            if (CameraBehavior == CameraBehavior.AutoFrameSelected)
            {
                if (CurrentSelection != null && CurrentSelection != lastSelection)
                    if (SceneView.lastActiveSceneView != null)
                        SceneView.lastActiveSceneView.FrameSelected();
                lastSelection = CurrentSelection;
            }
#endif
        }

        public void RemoveFilteredObjects()
        {
            if (_breadcrumbs == null && !SceneHasBreadcrumbs()) return;

            bool removedAny = false;

            if (CurrentSelection != null && IsObjectTypeFiltered(CurrentSelection.OReference))
            {
                CurrentSelection = null;
                removedAny = true;
            }
            for (int i = Back.Count - 1; i >= 0; i--)
                if (IsObjectTypeFiltered(Back[i].OReference))
                {
                    Back.RemoveAt(i);
                    removedAny = true;
                }
            for (int i = Forward.Count - 1; i >= 0; i--)
                if (IsObjectTypeFiltered(Forward[i].OReference))
                {
                    Forward.RemoveAt(i);
                    removedAny = true;
                }

            if (removedAny) Repaint();
        }

        public void ReassignObjectTypes()
        {
            if (_breadcrumbs == null && !SceneHasBreadcrumbs()) return;
            if (CurrentSelection != null)
                CurrentSelection.OType = GetObjectType(CurrentSelection.OReference);
            for (int i = Back.Count - 1; i >= 0; i--)
                Back[i].OType = GetObjectType(Back[i].OReference);
            for (int i = Forward.Count - 1; i >= 0; i--)
                Forward[i].OType = GetObjectType(Forward[i].OReference);
        }
        
        Color skinBackgroundColor;
        Color colorDarken;
        int skinState = -1;

        Color GetSkinBackgroundColor()
        {
            var state = Application.isPlaying ? 1 : 0;
            state += EditorGUIUtility.isProSkin ? 2 : 0;
            state += EditorApplication.isPlayingOrWillChangePlaymode ? 4 : 0;
            if (state == skinState)
                return skinBackgroundColor;
            else try
            {
                var HostViewType = typeof(EditorGUI).Assembly.GetType("UnityEditor.HostView");
                var kViewColor = (Color)HostViewType.GetField("kViewColor", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).GetValue(null);
                var kPlayModeDarken = HostViewType.GetField("kPlayModeDarken", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).GetValue(null);
                var PrefColorType = typeof(EditorGUI).Assembly.GetType("UnityEditor.PrefColor");
                colorDarken = (Color)PrefColorType.GetField("m_color", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(kPlayModeDarken);
                var kDarkViewBackground = (Color)typeof(EditorGUIUtility).GetField("kDarkViewBackground", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).GetValue(null);
                var color = (!EditorGUIUtility.isProSkin) ? kViewColor : kDarkViewBackground;
                skinBackgroundColor = (!EditorApplication.isPlayingOrWillChangePlaymode) ? color : (color * colorDarken);
                skinBackgroundColor.a = 255;
            }
            catch
            {
                colorDarken = Color.white;
                skinBackgroundColor = EditorGUIUtility.isProSkin ? new Color32(56, 56, 56, 255) : new Color32(194, 194, 194, 255);
            }
            skinState = state;
            return skinBackgroundColor;
        }

        private void DrawLabels(Event e, ref int iBack, ref int iForward, float maxWidth, float currentWidthHalf, float centerX, float labelOffset)
        {
            if (CurrentSelection != null)
                if (DrawLabel(e, CurrentSelection, centerX + labelOffset - currentWidthHalf, currentWidthHalf * 2f, Locked ? yellowLabel : greenLabel))
                    RestorePreviousCamera();

            var xl = currentWidthHalf;
            for (int i = Back.Count - 1; i >= 0; i--)
            {
                try
                {
                    var obj = Back[i];
                    if (obj == null || obj.OReference == null) throw new System.Exception();
#if FULL_VERSION
                    if (RemoveUnnamedObjects && obj.OReference.name.Length == 0) throw new System.Exception();
#endif
                    if (xl >= maxWidth + labelOffset) break;
                    var width = GetLabelWidthFromInstance(obj);
                    xl += width;
                    if (DrawLabel(e, Back[i], centerX + labelOffset - xl, width, grayLabel))
                    {
                        UnlockFirstInspector();
                        iBack = Back.Count - i;
                    }
                }
                catch
                {
                    Back.RemoveAt(i);
                }
            }
            var xr = currentWidthHalf;
            for (int i = Forward.Count - 1; i >= 0; i--)
            {
                try
                {
                    var obj = Forward[i];
                    if (obj == null || obj.OReference == null) throw new System.Exception();
#if FULL_VERSION
                    if (RemoveUnnamedObjects && obj.OReference.name.Length == 0) throw new System.Exception();
#endif
                    if (xr >= maxWidth - labelOffset) break;
                    var width = GetLabelWidthFromInstance(obj);
                    if (DrawLabel(e, Forward[i], centerX + labelOffset + xr, width, grayLabel))
                    {
                        UnlockFirstInspector();
                        iForward = Forward.Count - i;
                    }
                    xr += width;
                }
                catch
                {
                    Forward.RemoveAt(i);
                }
            }
        }

        private void ComputeLabelPositions(float maxWidth, float currentWidthHalf, out float centerX, out float labelOffset)
        {
            centerX = rectLabels.x + maxWidth;
            labelOffset = 0f;
            var xl = currentWidthHalf;
            var xr = currentWidthHalf;
            int il = 1, ir = 1;
            do
            {
                if (Back.Count >= il && (xl <= xr || Forward.Count < ir))
                {
                    var obj = Back[Back.Count - il++];
                    if (obj == null || obj.OReference == null) { Back.RemoveAt(Back.Count - --il); continue; }
                    xl += GetLabelWidthFromInstance(obj);
                    if (xl > maxWidth && Forward.Count < ir && xr < maxWidth)
                    {
                        var freeSpace = maxWidth - xr;
                        var reqSpace = xl - maxWidth;
                        if (reqSpace < freeSpace)
                        {
                            labelOffset += reqSpace;
                            xl -= reqSpace;
                            xr += reqSpace;
                        }
                        else
                        {
                            labelOffset += freeSpace;
                            xl -= freeSpace;
                            xr += freeSpace;
                            break;
                        }
                    }
                }
                else if (Forward.Count >= ir)
                {
                    var obj = Forward[Forward.Count - ir++];
                    if (obj == null || obj.OReference == null) { Forward.RemoveAt(Forward.Count - --ir); continue; }
                    xr += GetLabelWidthFromInstance(obj);
                    if (xr > maxWidth && Back.Count < il && xl < maxWidth)
                    {
                        var freeSpace = maxWidth - xl;
                        var reqSpace = xr - maxWidth;
                        if (reqSpace < freeSpace)
                        {
                            labelOffset -= reqSpace;
                            xl += reqSpace;
                            xr -= reqSpace;
                        }
                        else
                        {
                            labelOffset -= freeSpace;
                            xl += freeSpace;
                            xr -= freeSpace;
                            break;
                        }
                    }
                }
                else break;
            } while (true);

#if FULL_VERSION
            switch (BarAlignment)
            {
                case BarAlignment.Right:
                    if (xl <= maxWidth && xr <= maxWidth) labelOffset += maxWidth - xr;
                    break;
                case BarAlignment.Left:
                    if (xl <= maxWidth && xr <= maxWidth) labelOffset -= maxWidth - xl;
                break;
            }
#else
            if (xl <= maxWidth && xr <= maxWidth) labelOffset += maxWidth - xr;
#endif
        }

        void AddCurrentToBack()
        {
            if (CurrentSelection == null) return;
            if (Back.Count == 0 || Back[Back.Count - 1] != CurrentSelection)
            {
                Back.Add(CurrentSelection);
                if (Back.Count > MaxEnqueuedObjects)
                    Back.RemoveAt(0);
            }
        }

        void AddCurrentToForward()
        {
            if (CurrentSelection == null) return;
            if (Forward.Count == 0 || Forward[Forward.Count - 1] != CurrentSelection)
            {
                Forward.Add(CurrentSelection);
                if (Back.Count > MaxEnqueuedObjects)
                    Forward.RemoveAt(0);
            }
        }

        bool DoForward()
        {
            if (areAllInspectorsLocked) return false;
            if (Forward.Count > 0)
            {
                AddCurrentToBack();
                CurrentSelection = Forward[Forward.Count - 1];
                Forward.RemoveAt(Forward.Count - 1);
                return true;
            }
            return false;
        }

        bool DoBack()
        {
            if (areAllInspectorsLocked) return false;
            if (Back.Count > 0)
            {
                AddCurrentToForward();
                CurrentSelection = Back[Back.Count - 1];
                Back.RemoveAt(Back.Count - 1);
                return true;
            }
            return false;
        }

        void DrawNotificationIcon(Event e)
        {
            LabelStyle.normal.background = UpdateNotifications.Blink ? redLabel : grayLabel;
            LabelStyle.fontStyle = FontStyle.Bold;
            LabelStyle.margin.top = 4;
            gc.text = "\x21";
            GUILayout.Label(gc, LabelStyle, GUILayout.Width(16));
            LabelStyle.fontStyle = FontStyle.Normal;
            if (e.type == EventType.mouseDown && GUILayoutUtility.GetLastRect().Contains(e.mousePosition))
                UpdateNotifications.AttendNotification();
        }

        public void LimitQueueSizes()
        {
            if (_breadcrumbs == null) return;
            if (Back.Count > MaxEnqueuedObjects) Back.RemoveRange(0, Back.Count - MaxEnqueuedObjects);
            if (Forward.Count > MaxEnqueuedObjects) Forward.RemoveRange(0, Forward.Count - MaxEnqueuedObjects);
        }

        void OnLoadScene()
        {
#if FULL_VERSION
            if (!SerializeBreadcrumbs)
            {
                if (Instance._breadcrumbs != null)
                {
                    LimitQueueSizes();
                    UnselectCurrent();

                    var list = new List<ObjectReference>();
                    foreach (var i in Back)
                        if (GetObjectType(i.OReference) != ObjectType.Instance) list.Add(i);
                    var tmp = Back;
                    Back = list;
                    list = tmp;
                    list.Clear();
                    foreach (var i in Forward)
                        if (GetObjectType(i.OReference) != ObjectType.Instance) list.Add(i);
                    Forward = list;
                }
            }
            else
            {
                LimitQueueSizes();
                UnselectCurrent();
            }

            if (ConfigurationDataVersion < 110 && CameraBehavior == CameraBehavior.DoNotChange)
                CameraBehavior = CameraBehavior.RestorePreviousCamera;
            if (DuplicatesBehavior == DuplicatesBehavior.RemoveAllDuplicates)
                ClearDuplicates();
#else
            LimitQueueSizes();
            UnselectCurrent();
#endif
            if (_breadcrumbs != null || SceneHasBreadcrumbs())
            {
                if (breadcrumbs.DataVersion < CurrentVersion)
                {
                    if (breadcrumbs.DataVersion < 120)
                    {
                        ReassignObjectTypes();
                        RemoveFilteredObjects();
                    }
                    breadcrumbs.DataVersion = CurrentVersion;
                    EditorUtility.SetDirty(_breadcrumbs);
                }
            }
            ConfigurationDataVersion = CurrentVersion;
        }

        void OnHierarchyChange()
        {
            if (_breadcrumbs == null && SceneHasBreadcrumbs())
                GetInspectorBreadcrumbs();

            if (EditorApplication.currentScene != lastScene)
            {
                lastScene = EditorApplication.currentScene;
                OnLoadScene();
            }
            Repaint();
        }

        void OnProjectChange()
        {
            Repaint();
        }

        void OnSelectionChange()
        {
            if (EditorApplication.currentScene != lastScene) return;

            var activeObject = Selection.activeObject;

            if (_breadcrumbs == null && (Locked || activeObject == null)) return;
            
            if (CheckIfAllInspectorsAreLocked()) return;

            var type = activeObject == null? ObjectType.None : GetObjectType(activeObject);

            if (IsObjectTypeFiltered(activeObject, type)) activeObject = null;

            if (activeObject == null) // Unselect / Select a filtered type
            {
                UnselectCurrent();
                return;
            }
            
            if (CurrentSelection == null || CurrentSelection.OReference != activeObject) // New object selected
            {
                if (CurrentSelection != null) UpdateCurrentSelection();

                if (Locked)
                {
                    AddCurrentToBack();
                    int i = 1, oi = 0;
                    do
                    {
                        if (Back.Count >= i && Back[Back.Count - i].OReference == activeObject) { oi = -i; break; }
                        if (Forward.Count >= i && Forward[Forward.Count - i].OReference == activeObject) { oi = i; break; }
                        i++;
                    } while (Back.Count >= i || Forward.Count >= i);
                    if (oi < 0)
                    {
                        while (oi++ < 0) DoBack();
                        CurrentSelection = CreateObjectReference(activeObject, type);
                    }
                    else if (oi > 0)
                    {
                        while (oi-- > 0) DoForward();
                        CurrentSelection = CreateObjectReference(activeObject, type);
                    }
                    else CurrentSelection = null;
                }
                else if (Back.Count > 0 && Back[Back.Count - 1].OReference == activeObject) // Undo/Back
                {
                    DoBack();
                    CurrentSelection = CreateObjectReference(activeObject, type);
                }
                else if (Forward.Count > 0 && Forward[Forward.Count - 1].OReference == activeObject) // Redo/Forward
                {
                    DoForward();
                    CurrentSelection = CreateObjectReference(activeObject, type);
                }
                else
                {
#if FULL_VERSION
                    ObjectReference or = null;
                    if (DuplicatesBehavior == DuplicatesBehavior.RemoveAllDuplicates)
                    {
                        for (var n = Back.Count - 1; n >= 0; n--)
                            if (Back[n].OReference == activeObject)
                            {
                                if (or == null) or = Back[n];
                                Back.RemoveAt(n);
                            }
                        for (var n = Forward.Count - 1; n >= 0; n--)
                            if (Forward[n].OReference == activeObject)
                            {
                                if (or == null) or = Forward[n];
                                if (InsertNewObjects) Forward.RemoveAt(n);
                            }
                    }
                    AddCurrentToBack();
                    if (!InsertNewObjects) Forward.Clear();
                    CurrentSelection = or ?? CreateObjectReference(activeObject, type);
#else
					AddCurrentToBack();
                    Forward.Clear();
                    CurrentSelection= CreateObjectReference(activeObject, type);
#endif

                }
#if FULL_VERSION
                if (ForceDirty) EditorUtility.SetDirty(_breadcrumbs);
#endif
                Repaint();
            }
        }

        private bool IsObjectTypeFiltered(Object obj)
        {
            return IsObjectTypeFiltered(obj, GetObjectType(obj));
        }

        private bool IsObjectTypeFiltered(Object obj, ObjectType type)
        {
            if (obj == null) return false;

#if FULL_VERSION
            if (RemoveUnnamedObjects && string.IsNullOrEmpty(obj.name)) return true;
            switch (type)
            {
                case ObjectType.InspectorBreadcrumbs: return true; 
                case ObjectType.Folder: return !TrackFolders;
                case ObjectType.Instance: return !TrackObjects;
                case ObjectType.Asset: return !TrackAssets;
                case ObjectType.Scene: return !TrackScenes;
                case ObjectType.TextAssets: return !TrackTextAssets;
                case ObjectType.ProjectSettings: return !TrackProjectSettings;
                case ObjectType.AssetStoreAssetInspector: return !TrackAssetStoreInspector;
            }
#else
			if (type == ObjectType.Folder || type == ObjectType.Scene || type == ObjectType.ProjectSettings || type == ObjectType.InspectorBreadcrumbs || type == ObjectType.TextAssets || type == ObjectType.AssetStoreAssets) return true;
#endif

            return false;
        }

        private void UnselectCurrent()
        {
            if (_breadcrumbs != null || SceneHasBreadcrumbs())
            {
                if (CurrentSelection != null)
                {
                    AddCurrentToBack();
                    UpdateCurrentSelection();
                    CurrentSelection = null;
                    Repaint();
                }
            }
        }

        private static ObjectType GetObjectType(Object activeObject)
        {
            if (activeObject == null) return ObjectType.None;

            if (activeObject is GameObject)
            {
				if (Instance._breadcrumbs != null)
					if (activeObject == Instance._breadcrumbs.gameObject) return ObjectType.InspectorBreadcrumbs;

                PrefabType pt = PrefabUtility.GetPrefabType((GameObject)activeObject);
                if (pt == PrefabType.None || pt == PrefabType.DisconnectedModelPrefabInstance || pt == PrefabType.DisconnectedPrefabInstance ||
                    pt == PrefabType.MissingPrefabInstance || pt == PrefabType.ModelPrefabInstance || pt == PrefabType.PrefabInstance) 
                    return ObjectType.Instance;
            }

            if (activeObject as UnityEngine.TextAsset != null)
                return ObjectType.TextAssets;
            
            if (activeObject.ToString().Contains("UnityEngine.SceneAsset"))
                return ObjectType.Scene;

            var asset_path = AssetDatabase.GetAssetPath(activeObject);
            if (string.IsNullOrEmpty(asset_path))
            {
                if (activeObject.GetType().ToString().Contains("AssetStoreAssetInspector"))
                    return ObjectType.AssetStoreAssetInspector;
                else
                    return ObjectType.Asset;
            }
            if (asset_path.StartsWith("ProjectSettings/")) return ObjectType.ProjectSettings;
                
            try
            {
                System.IO.FileAttributes file_attr = System.IO.File.GetAttributes(Application.dataPath + "/" + asset_path.Replace("Assets/", ""));
                if ((file_attr & System.IO.FileAttributes.Directory) == System.IO.FileAttributes.Directory)
                    return ObjectType.Folder;
            }
            catch { }

            return ObjectType.Asset;
        }

        void Update()
        {
            if (UpdateNotifications.Update(true)) Repaint();
        }

        void Preloads()
        {
            if (LabelStyle == null)
            {
                minSize = maxSize = new Vector2(300, 22);
                LabelStyle = new GUIStyle()
                {
                    alignment = TextAnchor.UpperLeft,
                    border = new RectOffset(7, 7, 0, 0),
                    clipping = TextClipping.Clip,
                    fontSize = 0,
                    fixedHeight = 15,
                    margin = new RectOffset(3, 3, 0, 0),
                    padding = new RectOffset(6, 6, 0, 0),
                };
                LabelStyle.normal.textColor = Color.white;

                DeleteLabelStyle = new GUIStyle(EditorStyles.miniButtonMid) { margin = new RectOffset(0, 0, 4, 5) };
                DeleteLabelStyleColor = DeleteLabelStyle.normal.textColor;
            }

            if (OpaqueBoxStyle == null)
            {
                OpaqueBoxStyle = new GUIStyle();
                OpaqueBoxStyle.normal.background = EditorGUIUtility.whiteTexture;
            }

            if (grayLabel == null) grayLabel = IconContent("sv_icon_name0");
            if (greenLabel == null) greenLabel = IconContent("sv_icon_name3");
            if (yellowLabel == null) yellowLabel = IconContent("sv_icon_name4");
            if (redLabel == null) redLabel = IconContent("sv_icon_name6");

            if (forwardBtn == null || backBtn == null)
            {
                forwardBtn = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                if (!EditorGUIUtility.isProSkin)
                    forwardBtn.LoadImage(System.Convert.FromBase64String(dataLight));
                else
                    forwardBtn.LoadImage(System.Convert.FromBase64String(dataDark));
                forwardBtn.Apply();
                forwardBtn.hideFlags = HideFlags.HideAndDontSave;

                backBtn = FlipTexture(forwardBtn);
                backBtn.hideFlags = HideFlags.HideAndDontSave;
            }
        }

        Texture2D FlipTexture(Texture2D original)
        {
            var flipped = new Texture2D(original.width, original.height, original.format, false) { filterMode = original.filterMode };
            var w = original.width;
            var h = original.height;
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    flipped.SetPixel(w - x - 1, y, original.GetPixel(x, y));
            flipped.Apply();
            return flipped;
        }

        public void AddItemsToMenu(GenericMenu menu)
        {
            menu.AddItem(new GUIContent("Lock"), Locked, () => { Locked = !Locked; });
            menu.AddItem(new GUIContent("Clear current scene breadcrumbs"), false, new GenericMenu.MenuFunction(ClearSceneBreadcrumbs));
            menu.AddItem(new GUIContent("Clear all project scene breadcrumbs"), false, new GenericMenu.MenuFunction(ClearAllProjectBreadcrumbs));
            menu.AddItem(new GUIContent("Check for new user notifications"), false, new GenericMenu.MenuFunction(() => { UpdateNotifications.ForceGetNotification(); }));
            if (UpdateNotifications.HasPreviousNotification)
                menu.AddItem(new GUIContent("Show last notification again"), false, new GenericMenu.MenuFunction(() => { UpdateNotifications.ShowPreviousNotification(); }));
            menu.AddItem(new GUIContent("Help and Options ..."), false, new GenericMenu.MenuFunction(() => GetWindowWithRect<HelpAndOptions>(new Rect(0, 0, HelpAndOptions.Width, HelpAndOptions.Height), true, "Help and Options", true)));
        }

        const string dataDark = "iVBORw0KGgoAAAANSUhEUgAAAAwAAAAMCAQAAAD8fJRsAAAAuUlEQVQY02P8z4AdMDHgAv8ZVMTm+VvwI4tdZvjPwMTA8F8kxGtTxTI3B0E0oxiZ/jNwCPkEr2+65sDAiCTxn4GR4T8DAwMTm3Tky/pJNhBJJgYGRgYGqBQjA4ekS9DkQDcOBgYWiAMYoWZMeVB9mOEWw28GBhaYUR/+rHky/+yVawyvGX5d/g/V8eb31hfzLpy/zPCS4QeSPxikGZwYFBjYUf3B+J9hFTMDM8Of//8YGWD2MTCEMQAAsmU4qA1aNcUAAAAASUVORK5CYII=";
        const string dataLight = "iVBORw0KGgoAAAANSUhEUgAAAAwAAAAMCAQAAAD8fJRsAAAAr0lEQVQY02P8z4AdMDHglngoluB/kh9Z0AAiwSiyzSukItLtgCCajv9MDAw/hPYERzTpOjAwotnByMDI8JftZaRMfa4NRBLN8l+SB4NyAvdyMDCwwIT+MzAyMDAkPeg4zHCL4TeSBP8fvydpZzWuMbxm+GXw/wJEQuC364u0C/qXGV4y/GBAGPFf+r/Tf4X/7P8ZYFCf4T8DCwND+AuG1wx//v9jZGBggASQOgMDAwDAYjipmcIucgAAAABJRU5ErkJggg==";

        Texture2D IconContent(string name)
        {
            System.Reflection.MethodInfo mi = typeof(EditorGUIUtility).GetMethod("IconContent", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public, null, new System.Type[] { typeof(string) }, null);
            if (mi == null) mi = typeof(EditorGUIUtility).GetMethod("IconContent", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic, null, new System.Type[] { typeof(string) }, null);
            return (Texture2D)((GUIContent)mi.Invoke(null, new object[] { name })).image;
        }

        Rect GetControlRect(float width)
        {
            System.Reflection.MethodInfo mi = typeof(EditorGUILayout).GetMethod("GetControlRect", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public, null, new System.Type[] { typeof(GUILayoutOption[]) }, null);
            if (mi == null) mi = typeof(EditorGUILayout).GetMethod("GetControlRect", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic, null, new System.Type[] { typeof(GUILayoutOption[]) }, null);
            return (Rect)mi.Invoke(null, new object[] { new GUILayoutOption[] { GUILayout.Width(width) } });
        }

        void GetInspectorList()
        {
            try
            {
                InspectorList = (IEnumerable)GetInspectorWindowType().GetMethod("GetInspectors", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).Invoke(null, null);
            }
            catch { };
        }

        bool CheckIfAllInspectorsAreLocked()
        {
            try
            {
                var enumerator = InspectorList.GetEnumerator();
                while (enumerator.MoveNext())
                    if (!(bool)GetInspectorWindowType().GetProperty("isLocked", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public).GetValue(enumerator.Current, null))
                        return areAllInspectorsLocked = false;
                return areAllInspectorsLocked = true;
            }
            catch { };
            return areAllInspectorsLocked = false;
        }

        void UnlockFirstInspector()
        {
            if (!areAllInspectorsLocked) return;
            try
            {
                var enumerator = InspectorList.GetEnumerator();
                enumerator.MoveNext();
                var firstInspectorWindow = enumerator.Current;
                GetInspectorWindowType().GetProperty("isLocked", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public).SetValue(firstInspectorWindow, false, null);
                areAllInspectorsLocked = false;
            }
            catch { };
        }

        ObjectReference CreateObjectReference(Object _object, ObjectType type)
        {
            var or = new ObjectReference();
            or.OReference = _object;
            or.OType = type;
            if (SceneView.lastActiveSceneView != null)
            {
                or.CPosition = SceneView.lastActiveSceneView.pivot;
                or.CRotation = SceneView.lastActiveSceneView.rotation;
                or.CSize = SceneView.lastActiveSceneView.size;
                or.COrtho = SceneView.lastActiveSceneView.orthographic;
            }
            return or;
        }

        void RestorePreviousCamera()
        {
            Selection.activeObject = CurrentSelection.OReference;
#if FULL_VERSION
            if (CameraBehavior != CameraBehavior.RestorePreviousCamera) return;
#endif
            if (SceneView.lastActiveSceneView != null)
                SceneView.lastActiveSceneView.LookAt(CurrentSelection.CPosition, CurrentSelection.CRotation, CurrentSelection.CSize, CurrentSelection.COrtho, true);
        }

        void UpdateCurrentSelection()
        {
            if (SceneView.lastActiveSceneView != null)
            {
                CurrentSelection.CPosition = SceneView.lastActiveSceneView.pivot;
                CurrentSelection.CRotation = SceneView.lastActiveSceneView.rotation;
                CurrentSelection.CSize = SceneView.lastActiveSceneView.size;
                CurrentSelection.COrtho = SceneView.lastActiveSceneView.orthographic;
            }
        }
    }
}