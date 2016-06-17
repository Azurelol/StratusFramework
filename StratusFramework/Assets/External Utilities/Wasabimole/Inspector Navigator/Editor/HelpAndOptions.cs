#define FULL_VERSION

using UnityEditor;
using UnityEngine;
using System.Collections;

// ---------------------------------------------------------------------------------------------------------------------------
// Help And Options - © 2014, 2015 Wasabimole http://wasabimole.com
// ---------------------------------------------------------------------------------------------------------------------------
// This source file is part of Inspector Navigator
// ---------------------------------------------------------------------------------------------------------------------------
// Please send your feedback and suggestions to mailto://contact@wasabimole.com
// ---------------------------------------------------------------------------------------------------------------------------

namespace Wasabimole.InspectorNavigator.Editor
{
    public class HelpAndOptions : EditorWindow
    {
        public const int Width = 300;
#if FULL_VERSION
        public const int Height = 124 + 96 + 18 * 20;
#else
        public const int Height = 114 + 82 + 18 * 26;
#endif
        InspectorNavigator inspector;

        const string dataLogo = "iVBORw0KGgoAAAANSUhEUgAAAJAAAAA/CAMAAAA40rGxAAABgFBMVEUAAAB0iIV0iIV0iIV0iIV0iIVzhH5rfnx0iIV0iIV0iIV3h3x0iIV0iIV1hXt0iIV5lHl0iIWZVzp0iIV0iIV0iIV0iIWNuSwjPztIT1JUVltJW2J0iIVBVVZ0iIV0iIUfPTkgPjoySElr1kRfj1V0iIVdU12GzDLOSCXKQiVmUF/HfD/GPBksXIUYOTSf2CDSfkPaVTNIamuMnTiO1iug1x/dsEHguj9uhYNxin50iIXVLhJrUF8XODNlVWBuSFwzTUqc1yKl2BtfV1+u2hVZV12K1i6T1ih01D1q1EQqR0NpGkBvQVh81DdSVlpHZGPVOh9KaWnTcULTZD1IbXCC2DNDXl0+WlhvOVQ5VVJKVFXURyluMU7UWTe43A4hQDth0kvRf0VsJkhAUFDTUDHC4wZU1FPTj0Q8WXffIg+WUEfkdjxWCEHiqETsDgcWNzJeN1qY6iJY7kw98F+QijWFwTZswUXLPhrCaUSraTyZxSdyokjY8QCDREq7UB9TfmXV9yXSAAAAgHRSTlMA2HDM8pwtGuaPOA77gAe9I1taUGZDrV/LQcSf/nZzd4KnT6D5emE+eabkgNL984/i5ur+6tPLepzQ/////////////////////////////////////////////////////////////////////////////////////////////0XcvXMAAAkmSURBVGje7ZrLc9vWFcZ/eBAECZEgKVGybEeObNlOrJS1ZJuqppPMdNOZLptF/sxON910ssiiqUuroTLMKI6tjOVXo0iUxIdIkAAIoIsL8GEpFhXRbRa5KwL3XOC733fOwb3nUuLtbT4udUCWjBf26QbadUkBvGD/6HSD+KzaARKB3W5ydpPe2vt+eqWKxQ3o8IX+7KTBB6pJoVKgQqFC/Xn7hMF16ybAahng1fOLAlr8BAGoQws6370a7f4wR6HSv7qnBX89rL8xIXP48vXO2YCUt/YmboQE9XDoMNN1vEFnbjUBe/3LwlTDub7cOhwYGNOL+ur87uDGd95FAbWaeVxydMChh59f6Lb6ai2M2l6VNQfer9f6at0vZLPfDuHZbozhQ/KYioY43Ju3Irk+eMPUlQGHj29EeD6gCtevA6viTmwMPGcwRO3YHChGgM+U0wLI3cMwrCHLe9MHDg4e145rAPM33BiWZZDN1gRLjR/cizOEDtARv31cIBsHcn8yDMgPW/4t5TgAfDwHaLfBsqA6YKnZngBDJGbI0aMVEYSfsI9JLy700FwGHBWmsqHD77XnD7pc+ijVAneIpcZzbxKAWtI9oVgPH3wIyOqHiytqTCCKIF2VtZaAA3Rq19clUqOQtg/GwXOmZEh9xcAFH2iTwCYBBpFsha/kAPb2RBZY1Wx8H+bnGQjXGwvPmQxR27s5rFgAijfziY2nxmI9LZLt6p3u0V7oIwHxzu8CiSCQSKVSLcHSP35gMgz1Ifv9O3/8AxbYhCTlAawwRQZBAHwaEATg+wiaLEtExyQYonuLFvQIIsXkH6s3tJob87y+Iy1NVZ8LcgBwrM3lDiBYIpVq4T72JsWQ8wVDbgQKrDRBkJQAw4CYl4vIgVgsxqdH2AyztG0zKUDhtP3hW9O7QYQoAQaJrcchGmIxALecw+5D8iHOxCSjdXh5RDFoT81pxxKuG/NUYj1mEtmqQKMoIpumVzot8DxVBGrgVybIEFNvKJaCpiDOwrZJsPv3SCpA13XYcoSP2zYQBDxsMkFAL/qfjaj92NydDxFhw5P641AqdJ1Go9Gg9828LA8g7TNJQPa/T0T+zCUxcyywE3tqMCAnXGP8GehD+tKeKCCREocUm+KgaZnC3y2Lr46ehGgajUYDQJK+7X0+N4DkM1lAu1snAk2b00JEWB3Y1nUaETmSFFEUQnq1Oz4gdSyr" +
            "KUIXEi4NJJtg0iBAYh8OifCESzrnay2leOGcq+PjGSfsga6SEt8xGeJMoeL3PM1Dt4FnO9C1k0Pk4HjAdx91ZTkA+NqbsGS0G+4bqZFkQgNMQu5UQIrgiIXaZwCKAo9sJg0oAhO6NMBO19EA0+Q5wO5RH42AI/mvP58RkM6j2LiAdr8fdTwNkjkNgCdtgKY7TI4kSRAP3fqfzjsAxBQnQreBpgHb/ezZJ0dwtbM5NzO+m54X0NODgWIiMndetDQ0NsP5138cIkeE+2LvS+DZ/jsBxMgmT2MPFhOA1v+idIfICRNi8Htgz343gE4m250XjkapXz940hqQI55qVz9/wrmS0HkA7T4bKKaFpYgcuIPNljNMDly+zNxn8NA5HyB1bMup4Yu9aPB/hhYF0/35XY6sPv2LzDtiiKdHwy4NsFN2NofKL/XDkJzLl6NlwlyPfd4VQxzn+4KFRZhFf6S81A2BDC1XS0f2OQFJ45vGjVOWSiP79dv1Ewb18wL6tf3a/r8tqZjazxyqmUr+gmF/2up/jW+GMnFckdrjDr10lc2LJcYxWu+ueuFnTBJQWmLsDSqv/weAUH8hz4haU96Sgl+SZPgN/2KaawZqXPFsgHQXvSlCRdwgLVuA2nMAIxkg2aGHGGLjo9UBuoYLks3w0HQvSdCyIaPUTFvtO5aWkpCsNwMxGW+YbQfItPGTqnT3kdEGlPs8BjDse2UAUlZRDHhcI5ldACrTh0DcXBL3X1IHHgCwJdUBZ70EkK8VAarbsTsAjzSRGZKd+wBsjBT0k/5dgJJqI6n2fFvucakHkIY0QO8SPQMwf1OMiCQjLwAU2mnISCEeFoYy0HIrA+k4PqDFxcj81BwAxdkkQPbuurB9IA2tT+KOOAhZ8wxsY+a4pYJYM7y6gjgNqy8AKMtQklGDVV6h3aGU42gtvU+mt0pFsSAI8ZalHtmjYrGcbjZFpdVboJyw3QcUYNNL14sLB0D2Qyi/V0daYr2c7quoL/MwYydbq8UNuq7maz/h1Nk1eO25bseDHFWIV6vq0za0C+A3XNeNwinWcd19DVajb0h+DeSa5VaBzY5zIMOqRvwYSt3tanV/u8JqPxa1ZUjUrAMZ5gzf02a9nwDUgs2Xw6GYBPugDT14Yp0wt74ZxGsCvreAKlR8wNmAFHKRkig5VONgZvruwJYFWFssaPhWzZJPL4kkh9aSR2hwKyEeoFA+7ZBJg+gosQY1gE6FripKoEAqdFHgsMRCJJkBGBktY7RAAbv5lsSoDYiIldZYqXhyGw/s7s/JdodLlf5EkqCECevVFZbHSoxxsAaZvOmVoLBiZy6QM0O6AC5B9hQT722fDhuSDvTPcLxtZ5liibb60zvZt8KRKUalU57nOQ5/XoVSrje80ZShEHVdBaLLiKFoKtXGv2AtM8Lh6OZl6qxPVFQFiQeQHPRMV2uiiQwuq2FvNywXJEFtEg99MH7U9z1/A2KorGsA06sj73OjCbxl37syHdJVpHTIe6I0B00DQLsWbrJkD1wNbi7B0jVRzfDAhG4eNKkIMPPba2D40CYOrgHZ29GbEulMRptdPuvMufY93L4JZJQHkIMmOBkOy6w6QNK/EjqDLEHBN6U8wBXJ9AtUJHj5iEJHUbRVwKTLFcm01yhbqLBum9KH/TctfXTnzv0l2LD6cR220RBoP4S8aiq9NdioQvCStTsSCXigm9LdYrS2U52v71IE2OAB6wC+A7ZWKRSAsr2OQgfRk+py+PQWReAh6yPv2zyZneoj/zRop7eWxYt47AJ1ewHi3drmijjWL2v9Pa42q+u6NAM3dV3XZ7VoV6DrumkkJSUDKV3X9TC9Tku6rseYUWOQl0zTNE0lJgYl1BRoiiScJaaLknZeUsKRiq7r0mzEXNaUEkBe0XVdSkVo/gvSq4CHYjpkoQAAAABJRU5ErkJggg==";

        static Texture2D logo;

        void OnGUI()
        {
            if (Application.unityVersion[0] >= '5') EditorGUILayout.Space();
            inspector = InspectorNavigator.Instance;
            if (inspector == null)
            {
                InspectorNavigator.OpenWindow();
                inspector = InspectorNavigator.Instance;
            }
#if FULL_VERSION
            EditorGUILayout.LabelField("Inspector Navigator Version " + InspectorNavigator.CurrentVersion / 100 + "." + (InspectorNavigator.CurrentVersion % 100).ToString("00") + " [Full]", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();
            if (GUILayout.Button("Edit Navigator Key Bindings")) EditKeyBindings(inspector);
            EditorGUILayout.Space();
            inspector.MaxEnqueuedObjects = EditorGUILayout.IntField(new GUIContent("Max. enqueued objects", "Max. number of objects on Back/Forward queues"), inspector.MaxEnqueuedObjects);
            if (inspector.MaxEnqueuedObjects < 8) inspector.MaxEnqueuedObjects = 8; else if (inspector.MaxEnqueuedObjects > 512) inspector.MaxEnqueuedObjects = 512;
            inspector.MaxLabelWidth = EditorGUILayout.IntField(new GUIContent("Max. label width", "Maximum object label width in pixels (0: full)"), inspector.MaxLabelWidth);
            if (inspector.MaxLabelWidth < 0) inspector.MaxLabelWidth = 0;
            else if (inspector.MaxLabelWidth > 0 && inspector.MaxLabelWidth < 20) inspector.MaxLabelWidth = 20;
            if (EditorGUI.EndChangeCheck()) inspector.Repaint();
            EditorGUI.BeginChangeCheck();
            inspector.BarAlignment = (BarAlignment)EditorGUILayout.EnumPopup(new GUIContent("Bar alignment", "Set horizontal breadcrumb alignment preference"), inspector.BarAlignment);
            if (EditorGUI.EndChangeCheck()) inspector.Repaint();
            inspector.InsertNewObjects = EditorGUILayout.Toggle(new GUIContent("Insert new objects", "Insert objects, instead of clearing forward list"), inspector.InsertNewObjects);
            EditorGUI.BeginChangeCheck();
            inspector.RemoveUnnamedObjects = EditorGUILayout.Toggle(new GUIContent("Remove unnamed objs", "Remove breadcrumbs with no name"), inspector.RemoveUnnamedObjects);
            if (EditorGUI.EndChangeCheck() && inspector.RemoveUnnamedObjects) inspector.Repaint();
            var prevDupBehaviour = inspector.DuplicatesBehavior;
            inspector.DuplicatesBehavior = (DuplicatesBehavior)EditorGUILayout.EnumPopup(new GUIContent("Duplicated Objects", "What to do with duplicated objects"), inspector.DuplicatesBehavior);
            if (inspector.DuplicatesBehavior != prevDupBehaviour && inspector.DuplicatesBehavior == DuplicatesBehavior.RemoveAllDuplicates)
            {
                inspector.ClearDuplicates();
                inspector.Repaint();
            }
            inspector.CameraBehavior = (CameraBehavior)EditorGUILayout.EnumPopup(new GUIContent("Scene camera behavior", "What to do with the scene camera"), inspector.CameraBehavior);
            inspector.CheckForUpdates = EditorGUILayout.Toggle(new GUIContent("Check for updates", "Check if there's a new version"), inspector.CheckForUpdates);
            inspector.OtherNotifications = EditorGUILayout.Toggle(new GUIContent("Other notifications", "Show other wasabimole notifications"), inspector.OtherNotifications);
            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();
            GUI.enabled = inspector.SerializeBreadcrumbs = EditorGUILayout.ToggleLeft(new GUIContent("Save InspectorBreadcrumbs as a scene object", "Serialize InspectorBreadcrumbs in every scene"), inspector.SerializeBreadcrumbs);
            if (EditorGUI.EndChangeCheck())
            {
                inspector.SetBreadcrumbsProperties();
                if (!inspector.SerializeBreadcrumbs && Event.current.type == EventType.Used)
                    if (EditorUtility.DisplayDialog("Remove breadcrumbs from all of your project scenes?", "Would you like to remove the Inspector Navigator's breadcrumbs object from all your existing project scenes? (might take a while if you have many big scenes)", "Yes, clean all scenes", "No, cancel"))
                        InspectorNavigator.ClearAllProjectBreadcrumbs();
            }
            var showBC = inspector.ShowBreadcrumbsObject;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(18);
            inspector.ShowBreadcrumbsObject = EditorGUILayout.Toggle(new GUIContent("Show breadcrumbs obj", "Show breadcrumbs object in scene"), inspector.ShowBreadcrumbsObject);
            EditorGUILayout.EndHorizontal();
            if (showBC != inspector.ShowBreadcrumbsObject) inspector.SetBreadcrumbsProperties();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(18);
            inspector.ForceDirty = EditorGUILayout.Toggle(new GUIContent("Mark scene as changed", "New selections mark scene as changed"), inspector.ForceDirty);
            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Object filters (inspectors to track):", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            inspector.ColorInstance = EditorGUILayout.ColorField(inspector.ColorInstance, GUILayout.Width(32));
            EditorGUILayout.Space();
            inspector.TrackObjects = EditorGUILayout.ToggleLeft("Track scene GameObjects", inspector.TrackObjects);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            inspector.ColorAsset = EditorGUILayout.ColorField(inspector.ColorAsset, GUILayout.Width(32));
            EditorGUILayout.Space();
            inspector.TrackAssets = EditorGUILayout.ToggleLeft("Track project Assets", inspector.TrackAssets);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            inspector.ColorTextAssets = EditorGUILayout.ColorField(inspector.ColorTextAssets, GUILayout.Width(32));
            EditorGUILayout.Space();
            inspector.TrackTextAssets = EditorGUILayout.ToggleLeft("Track Scripts & TextAssets", inspector.TrackTextAssets);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            inspector.ColorFolder = EditorGUILayout.ColorField(inspector.ColorFolder, GUILayout.Width(32));
            EditorGUILayout.Space();
            inspector.TrackFolders = EditorGUILayout.ToggleLeft("Track project Folders", inspector.TrackFolders);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            inspector.ColorScene = EditorGUILayout.ColorField(inspector.ColorScene, GUILayout.Width(32));
            EditorGUILayout.Space();
            inspector.TrackScenes = EditorGUILayout.ToggleLeft("Track project Scenes", inspector.TrackScenes);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            inspector.ColorProjectSettings = EditorGUILayout.ColorField(inspector.ColorProjectSettings, GUILayout.Width(32));
            EditorGUILayout.Space();
#if (UNITY_4_3||UNITY_4_5||UNITY_4_6)
            EditorGUI.BeginChangeCheck();
#endif
            inspector.TrackProjectSettings = EditorGUILayout.ToggleLeft("Track project Settings", inspector.TrackProjectSettings);
#if (UNITY_4_3||UNITY_4_5||UNITY_4_6)
            if (EditorGUI.EndChangeCheck() && inspector.TrackProjectSettings && Event.current.type == EventType.Used)
                if (!EditorUtility.DisplayDialog("Enable tracking Project Settings inspectors?", "A few users reported problems on builds after having the Inspector Navigator's breadcrumbs pointing to Project Settings on Unity 4.X, are you sure you want to track settings objects?\n\nNote: You can also use the navigator's menu option \"Clear all project breadcrumbs\" to delete all the breadcrumbs from your project scenes before performing a build.", "Yes, track settings", "No, cancel"))
                    inspector.TrackProjectSettings = false;
#endif
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            inspector.ColorAssetStoreInspector = EditorGUILayout.ColorField(inspector.ColorAssetStoreInspector, GUILayout.Width(32));
            EditorGUILayout.Space();
            inspector.TrackAssetStoreInspector = EditorGUILayout.ToggleLeft("Track Asset Store Inspector", inspector.TrackAssetStoreInspector);
            EditorGUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck()) inspector.RemoveFilteredObjects();
#else
            EditorGUILayout.LabelField("Inspector Navigator Version " + InspectorNavigator.CurrentVersion / 100 + "." + (InspectorNavigator.CurrentVersion % 100).ToString("00") + " [Trial]", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Default options can not be changed on trial version", MessageType.Warning);
            EditorGUILayout.Space();
            if (GUILayout.Button("Upgrade to full version")) NotificationWindow.OpenAssetStore("content/26181");
            EditorGUILayout.Space();
            GUI.enabled = false;
            GUILayout.Button("Edit Navigator Key Bindings");
            EditorGUILayout.Space();
            EditorGUILayout.IntField(new GUIContent("Max. enqueued objects", "Max. number of objects on Back/Forward queues"), 64);
            EditorGUILayout.IntField(new GUIContent("Max. label width", "Maximum object label width in pixels (0: full)"), 0);
            EditorGUILayout.EnumPopup(new GUIContent("Bar alignment", "Set horizontal breadcrumb alignment preference"), BarAlignment.Right);
            EditorGUILayout.Toggle(new GUIContent("Insert new objects", "Insert objects, instead of clearing forward list"), false);
            EditorGUILayout.Toggle(new GUIContent("Remove unnamed objs", "Remove breadcrumbs with no name"), false);
            EditorGUILayout.EnumPopup(new GUIContent("Duplicated Objects", "What to do with duplicated objects"), DuplicatesBehavior.RemoveWhenLocked);
            EditorGUILayout.EnumPopup(new GUIContent("Scene camera behavior", "What to do with the scene camera"), CameraBehavior.RestorePreviousCamera);
            EditorGUILayout.Toggle(new GUIContent("Check for updates", "Check if there's a new version"), true);
            EditorGUILayout.Toggle(new GUIContent("Other notifications", "Show other wasabimole notifications"), true);
            EditorGUILayout.Space();
            EditorGUILayout.ToggleLeft(new GUIContent("Save InspectorBreadcrumbs as a scene object", "Serialize InspectorBreadcrumbs in every scene"), true);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(18);
            EditorGUILayout.Toggle(new GUIContent("Show breadcrumbs obj", "Show breadcrumbs object in scene"), false);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(18);
            EditorGUILayout.Toggle(new GUIContent("Mark scene as changed", "New selections mark scene as changed"), false);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Object filters (inspectors to track):", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.ColorField(Color.white, GUILayout.Width(32));
            EditorGUILayout.Space();
            EditorGUILayout.ToggleLeft("Track scene GameObjects", true);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.ColorField(Color.white, GUILayout.Width(32));
            EditorGUILayout.Space();
            EditorGUILayout.ToggleLeft("Track project Assets", true);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.ColorField(Color.white, GUILayout.Width(32));
            EditorGUILayout.Space();
            EditorGUILayout.ToggleLeft("Track Scripts & TextAssets", false);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.ColorField(Color.white, GUILayout.Width(32));
            EditorGUILayout.Space();
            EditorGUILayout.ToggleLeft("Track project Folders", false);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.ColorField(Color.white, GUILayout.Width(32));
            EditorGUILayout.Space();
            EditorGUILayout.ToggleLeft("Track project Scenes", false);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.ColorField(Color.white, GUILayout.Width(32));
            EditorGUILayout.Space();
            EditorGUILayout.ToggleLeft("Track project Settings", false);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.ColorField(Color.white, GUILayout.Width(32));
            EditorGUILayout.Space();
            EditorGUILayout.ToggleLeft("Track AssetStore assets", false);
            EditorGUILayout.EndHorizontal();
            GUI.enabled = true;
#endif
            EditorGUILayout.Space();
            if (GUILayout.Button("Help / F.A.Q.")) Application.OpenURL(" http://www.wasabimole.com/inspector-navigator");
            if (GUILayout.Button("Visit website")) Application.OpenURL("http://wasabimole.com");
            EditorGUILayout.Space();
            if (logo == null)
            {
                logo = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                logo.LoadImage(System.Convert.FromBase64String(dataLogo));
                logo.Apply();
                logo.hideFlags = HideFlags.HideAndDontSave;
            }
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(logo, "Label")) Application.OpenURL("http://wasabimole.com");
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField(" Inspector Navigator - © 2014, 2015 Wasabimole");
        }

#if FULL_VERSION
        void EditKeyBindings(InspectorNavigator inspector)
        {
            var script = MonoScript.FromMonoBehaviour(inspector.breadcrumbs);
            var path = AssetDatabase.GetAssetPath(script);
            path = path.Substring(0, path.LastIndexOfAny(new[] { '/', '\\' }));
            path += "/Editor/KeyBindings.cs";
            script = AssetDatabase.LoadAssetAtPath(path, typeof(MonoScript)) as MonoScript;
            if (script != null)
            {
                Close();
                AssetDatabase.OpenAsset(script);
            }
        }
#endif

        void OnDisable()
        {
            if (inspector != null) inspector.LimitQueueSizes();
        }
    }
}