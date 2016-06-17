using System;
using UnityEngine;
using UnityEditor;
using System.Text;

// ---------------------------------------------------------------------------------------------------------------------------
// Update Notification - Info dialog for plugin updates - © 2015 Wasabimole http://wasabimole.com
// ---------------------------------------------------------------------------------------------------------------------------
// Please send your feedback and suggestions to mailto://contact@wasabimole.com
// ---------------------------------------------------------------------------------------------------------------------------

namespace Wasabimole.Core
{
    public class UpdateNotifications
    {
        // ---------------------------------------------------------------------------------------------------------------------------
        // Public properties

        public int RunCount;
        public bool Blink = true;
        public bool HasNotification;
        public bool HasPreviousNotification { get { return EditorPrefs.HasKey(keyPrev); } }

        // ---------------------------------------------------------------------------------------------------------------------------
        // Run-time properties are serialized in EditorPrefs

        string url;

        string CurrentNotification = string.Empty;

        bool showUpd, showInf;
        int packId, usages, currentVer, minLim;
        string tname, key, keyPrev, keyCurr, keyHas, keyUsg, keyRuns, keyLim;

        WWW www;
        Action repDlg;

        bool isDebugMode = false;

        // ---------------------------------------------------------------------------------------------------------------------------

        static Texture2D _grayLabel;
        static Texture2D _redLabel;
        public static Texture2D GrayLabel { get { if (_grayLabel == null) _grayLabel = GetIconContent("sv_icon_name0"); return _grayLabel; } }
        public static Texture2D RedLabel { get { if (_redLabel == null) _redLabel = GetIconContent("sv_icon_name6"); return _redLabel; } }

        // ---------------------------------------------------------------------------------------------------------------------------
        // Instantiate class, read editor prefs and check if a www call is required
        // ---------------------------------------------------------------------------------------------------------------------------

        public UpdateNotifications(int currentVer, string tname, int packId, Action repDlg, int usages, int minLim)
            : this(currentVer, tname, packId, repDlg, usages, true, true, true) { this.minLim = minLim; }

        public UpdateNotifications(int currentVer, string tname, int packId, Action repDlg, int usages, bool showUpd, bool showInf, bool mode)
            : this(currentVer, tname, packId, repDlg, usages, showUpd, showInf, mode, 3, "http://wasabimole.com/update/") { }

        public UpdateNotifications(int currentVer, string tname, int packId, Action repDlg, int usages, bool showUpd, bool showInf, bool mode, int period, string baseURL)
        {
            this.currentVer = currentVer;
            this.tname = tname;
            this.packId = packId;
            this.repDlg = repDlg;
            this.showUpd = showUpd;
            this.showInf = showInf;
            this.usages = usages;
            key = tname.Replace(" ", string.Empty);
            keyPrev = key + "Str";
            keyCurr = key + "Curr";
            keyHas = key + "Has";
            keyUsg = key + "Usages";
            keyRuns = key + "Runs";
            keyLim = key + "Lim";
            System.TimeSpan ts = System.DateTime.Now - new System.DateTime(2014, 11, 1);
            int days = (int)ts.TotalDays;
            int diff = days;
            if (EditorPrefs.HasKey(key)) diff -= EditorPrefs.GetInt(key);
            url = baseURL + key + (mode ? ".tinf" : ".info");
            if ((diff >= period || isDebugMode) && (showUpd || showInf))
            {
                EditorPrefs.SetInt(key, days);
                www = new WWW(url);
            }
            if (mode)
            {
                if (EditorPrefs.HasKey(keyLim)) this.usages = EditorPrefs.GetInt(keyLim);
                else EditorPrefs.SetInt(keyLim, usages);
            }
            else if (EditorPrefs.HasKey(keyLim)) EditorPrefs.DeleteKey(keyLim);
            if (EditorPrefs.HasKey(keyRuns))
                RunCount = EditorPrefs.GetInt(keyRuns);
            EditorPrefs.SetInt(keyRuns, RunCount + 1);
            if (EditorPrefs.HasKey(keyHas))
                HasNotification = EditorPrefs.GetBool(keyHas);
            if (EditorPrefs.HasKey(keyCurr))
                CurrentNotification = EditorPrefs.GetString(keyCurr);
        }

        // ---------------------------------------------------------------------------------------------------------------------------
        // Poll for www response
        // ---------------------------------------------------------------------------------------------------------------------------

        public bool Update(bool updateBlink)
        {
            if (www != null)
            {
                try
                {
                    if (www.isDone)
                    {
                        if (string.IsNullOrEmpty(www.error)) CheckNotifications(www.text);
                        www = null;
                    }
                }
                catch { }
            }

            if (updateBlink && HasNotification)
            {
                bool tmp = ((int)EditorApplication.timeSinceStartup) % 2 == 0;
                if (tmp != Blink) { Blink = tmp; return true; }
            }
            return false;
        }

        // ---------------------------------------------------------------------------------------------------------------------------
        // Check if www response contains a new, relevant notification
        // ---------------------------------------------------------------------------------------------------------------------------

        bool isForceGet = false;

        public void ForceGetNotification()
        {
            if (www != null) return;
            www = new WWW(url);
            isForceGet = true;
        }

        void CheckNotifications(string values)
        {
            var newNotification = values.Replace("\\n", "\n").Replace("<br/>", "\n");
            string[] split = newNotification.Split('|');

            if (split.Length < 6) return;

            int oldHash = 0;
            if (HasPreviousNotification)
            {
                string tmp = EditorPrefs.GetString(keyPrev);
                int.TryParse(tmp.Substring(0, tmp.IndexOf('|')), out oldHash);
            }

            int newHash = 0;
            int.TryParse(split[0], out newHash);

            int newVersion = 0;
            int.TryParse(split[1], out newVersion);

            if (isDebugMode || isForceGet || (newVersion == 0 && oldHash != newHash && showInf) || (newVersion > currentVer && showUpd))
            {
                if (HasNotification && !string.IsNullOrEmpty(CurrentNotification))
                    EditorPrefs.SetString(keyPrev, CurrentNotification);
                else
                    EditorPrefs.SetBool(keyHas, HasNotification = true);
                EditorPrefs.SetString(keyCurr, CurrentNotification = newNotification);
                isForceGet = false;
            }
        }

        // ---------------------------------------------------------------------------------------------------------------------------
        // Attend current notification if user clicks on the button
        // ---------------------------------------------------------------------------------------------------------------------------

        public void AttendNotification()
        {
            EditorPrefs.SetBool(keyHas, HasNotification = false);
            if (!string.IsNullOrEmpty(CurrentNotification))
            {
                string[] split = CurrentNotification.Split('|');
                EditorWindow.GetWindowWithRect<NotificationWindow>(new Rect(100f, 100f, 570f, 256f), true, split[2]).Init(split);
                EditorPrefs.SetString(keyPrev, CurrentNotification);
                EditorPrefs.SetString(keyCurr, CurrentNotification = string.Empty);
            }
            else
            {
                repDlg();

                if (EditorPrefs.HasKey(keyLim))
                {
                    var option = EditorUtility.DisplayDialog(GetString("Ticmov&aos\"wv|oig!") + tname, GetString("Ig\"zkp&aiof#") + tname + GetString(" tqfbpj+ qnfevc'cnlpmacu rwstjtsioe#mqu'ddtfhjvjeov#f|&rpfpb`lh` um#pmc'ftno$scushmm*") + "\n\n" + GetString("Ticmo%hu "), GetString("Uqeqeac"), GetString("M`{aa%jftdp"));
                    if (option) NotificationWindow.OpenAssetStore("content/" + packId);
                    usages -= 0x10;
                    if (usages < minLim) usages = minLim;
                    EditorPrefs.SetInt(keyLim, usages);
                    EditorPrefs.SetInt(keyUsg, 0);
                }
                else
                {
                    var option = EditorUtility.DisplayDialogComplex("Thanks for using " + tname, "If you find " + tname + " useful, please consider taking a minute to help us, and giving us a good review on the Unity Asset Store.\n\n" + "                        * * * * * / * * * * *\n\n" + "Good reviews mean more users, and the more time we can afford on improving this tool.", "Ok, review now", "Don't ask again", "Maybe later");
                    if (option < 2)
                    {
                        if (option == 0) NotificationWindow.OpenAssetStore("content/" + packId);
                        EditorPrefs.SetInt(keyUsg, -1);
                    }
                }

                EditorPrefs.SetInt(keyUsg, 0);
            }
        }

        // ---------------------------------------------------------------------------------------------------------------------------
        // Show window with previous notification
        // ---------------------------------------------------------------------------------------------------------------------------

        public void ShowPreviousNotification()
        {
            string[] split = EditorPrefs.GetString(keyPrev).Split('|');
            EditorWindow.GetWindowWithRect<NotificationWindow>(new Rect(100f, 100f, 570f, 256f), true, split[2]).Init(split);
        }

        // ---------------------------------------------------------------------------------------------------------------------------
        // Register use count
        // ---------------------------------------------------------------------------------------------------------------------------

        public void AddUsage() { AddUsage(1); }

        public void AddUsage(int count)
        {
            int counter = 0;
            if (EditorPrefs.HasKey(keyUsg))
                counter = EditorPrefs.GetInt(keyUsg);
            if (isDebugMode && counter == -1)
                EditorPrefs.SetInt(keyUsg, counter = 0);
            if (counter == -1) return;
            EditorPrefs.SetInt(keyUsg, counter += count);
            if (counter >= usages && !HasNotification)
                EditorPrefs.SetBool(keyHas, HasNotification = true);
        }

        // ---------------------------------------------------------------------------------------------------------------------------
        // Interpret coded string
        // ---------------------------------------------------------------------------------------------------------------------------

        string GetString(string s)
        {
            var sb = new StringBuilder();
            for (var n = 0; n < s.Length; n++)
            {
                var c = (char)((n & 7) ^ s[n]);
                if (c == '"' || c == '\\') sb.Append('\\');
                sb.Append(c);
            }
            s = sb.ToString();
            return s;
        }

        // ---------------------------------------------------------------------------------------------------------------------------
        // Get Unity interal icon content
        // ---------------------------------------------------------------------------------------------------------------------------

        public static Texture2D GetIconContent(string name)
        {
            System.Reflection.MethodInfo mi = typeof(EditorGUIUtility).GetMethod("IconContent", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public, null, new System.Type[] { typeof(string) }, null);
            if (mi == null) mi = typeof(EditorGUIUtility).GetMethod("IconContent", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic, null, new System.Type[] { typeof(string) }, null);
            return (Texture2D)((GUIContent)mi.Invoke(null, new object[] { name })).image;
        }

        // ---------------------------------------------------------------------------------------------------------------------------
        // Notification label GUI style
        // ---------------------------------------------------------------------------------------------------------------------------

        GUIStyle iconGUIStyle;
        GUIContent iconGUIContent;

        void PreloadIconStyle()
        {
            if (iconGUIStyle == null)
            {
                iconGUIStyle = new GUIStyle() // Create a labelStyle for the notification icon
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 0,
                    fixedHeight = 16,
                    margin = new RectOffset(3, 3, 5, 0),
                    padding = new RectOffset(0, 1, -2, 0),
                    fontStyle = FontStyle.Bold,
                };
                iconGUIStyle.normal.background = GetIconContent("sv_icon_name6");
                iconGUIStyle.normal.textColor = Color.white;
                iconGUIContent = new GUIContent("\x21", "Click to read new notification!"); // GUIContent for the notification icon
            }
        }

        // ---------------------------------------------------------------------------------------------------------------------------
        // Draw '!' icon if there's a notification
        // ---------------------------------------------------------------------------------------------------------------------------
        
        public void DrawNotificationIcon(Event e)
        {
            if (!HasNotification) return;
            PreloadIconStyle();
            iconGUIStyle.normal.background = Blink ? RedLabel : GrayLabel;
            GUILayout.Label(iconGUIContent, iconGUIStyle, GUILayout.Width(25));
            if (e.type == EventType.mouseDown && GUILayoutUtility.GetLastRect().Contains(e.mousePosition))
                AttendNotification();
        }
    }

    // ---------------------------------------------------------------------------------------------------------------------------
    // Actual editor window that displays the notification
    // ---------------------------------------------------------------------------------------------------------------------------

    public class NotificationWindow : EditorWindow
    {
        public string[] Values;

        WWW www;
        Texture2D ImageTexture;
        GUIStyle HeaderStyle = new GUIStyle(EditorStyles.wordWrappedLabel);
        GUIStyle BodyStyle = new GUIStyle(EditorStyles.wordWrappedLabel);
        GUIContent gc = new GUIContent();

        // ---------------------------------------------------------------------------------------------------------------------------
        // Initialise window data
        // ---------------------------------------------------------------------------------------------------------------------------

        public void Init(string[] values)
        {
            Values = values;
            www = new WWW(Values[3]);
            HeaderStyle.richText = true;
            BodyStyle.richText = true;
        }

        // ---------------------------------------------------------------------------------------------------------------------------
        // Poll for image download
        // ---------------------------------------------------------------------------------------------------------------------------

        public void Update()
        {
            if (www != null)
            {
                if (www.isDone)
                {
                    ImageTexture = www.texture;
                    ImageTexture.hideFlags = HideFlags.HideAndDontSave;
                    Repaint();
                    www = null;
                }
            }
        }

        // ---------------------------------------------------------------------------------------------------------------------------
        // Draw notification message
        // ---------------------------------------------------------------------------------------------------------------------------

        public void OnGUI()
        {
            if (Values == null) return;

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(ImageTexture, GUILayout.Width(128));
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(Values[4], HeaderStyle);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(Values[5], BodyStyle);
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            float width = 0;
            if (Values.Length > 6 && !string.IsNullOrEmpty(Values[6]))
            {
                gc.text = Values[6];
                width = EditorStyles.whiteLabel.CalcSize(gc).x + 10; ;
                if (GUILayout.Button(gc, GUILayout.Width(width)))
                {
                    if (Values.Length > 7 && !string.IsNullOrEmpty(Values[7]))
                        if (Values[7].Substring(0, 8) == "content/")
                            OpenAssetStore(Values[7]);
                        else
                            Application.OpenURL(Values[7]);
                }
            }

            EditorGUILayout.Space();
            if (Values.Length > 8 && !string.IsNullOrEmpty(Values[8]))
            {
                gc.text = Values[8];
                width = EditorStyles.whiteLabel.CalcSize(gc).x + 10; ;
                if (GUILayout.Button(gc, GUILayout.Width(width)))
                {
                    if (Values.Length > 9 && !string.IsNullOrEmpty(Values[9]))
                    {
                        if (Values[9].Substring(0, 8) == "content/")
                            OpenAssetStore(Values[9]);
                        else
                            Application.OpenURL(Values[9]);
                    }
                }
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            if (Event.current.type == EventType.repaint)
            {
                Rect rect = GUILayoutUtility.GetLastRect();
                float size = rect.height + 28;
                if (minSize.y != size) minSize = maxSize = new Vector2(570f, size);
            }
        }

        // ---------------------------------------------------------------------------------------------------------------------------
        // Open asset store window
        // ---------------------------------------------------------------------------------------------------------------------------

        public static void OpenAssetStore(string content)
        {
            typeof(EditorGUI).Assembly.GetType("UnityEditor.AssetStoreWindow").GetMethod("OpenURL", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public).
                Invoke(null, new object[] { content });
        }
    }
}
