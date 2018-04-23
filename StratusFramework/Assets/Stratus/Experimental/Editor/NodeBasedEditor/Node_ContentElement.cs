using UnityEngine;
using UnityEditor;

namespace Stratus
{
  namespace Editor
  {
    public abstract partial class Node
    {
      /// <summary>
      /// One of the items inside the node
      /// </summary>
      public class ContentElement
      {
        private static GUIStyle TitleStyle;
        private static GUIStyle DescriptionStyle;
        private static bool IsStaticInitialized { get; set; }

        //--------------------------------------------------------------------/
        // Properties
        //--------------------------------------------------------------------/
        public string Title { get; private set; }
        public string Description { get; private set; }
        public Texture2D Icon { get; private set; }
        public System.Action OnClick { get; private set; }

        public Rect Position { get; private set; }
        public Vector2 Size { get; private set; }
        public float Width { get { return Size.x; } }
        public float Height { get { return Size.y; } }

        const int IconSize = 32;
        const int TitleFontSize = 24;
        const int DescriptionFontSize = 12;

        //--------------------------------------------------------------------/
        // Methods
        //--------------------------------------------------------------------/
        public ContentElement(string title, string description, Texture2D icon, System.Action onClick)
        {
          this.Initialize(title, description, icon, onClick);
        }

        private void Initialize(string title, string description, Texture2D icon, System.Action onClick)
        {
          this.Title = title;
          this.Description = description;
          this.Icon = icon;
          //this.Icon.Resize(IconSize, IconSize);
          this.OnClick = onClick;

          if (!IsStaticInitialized)
            InitializeStatics();

          // Now calculate the size of this element
          this.CalculateAndSetSize();
        }

        private void InitializeStatics()
        {
          TitleStyle = new GUIStyle(EditorStyles.boldLabel);
          TitleStyle.fontSize = TitleFontSize;
          DescriptionStyle = new GUIStyle(EditorStyles.label);
          DescriptionStyle.fontSize = DescriptionFontSize;

          IsStaticInitialized = true;
        }

        private void CalculateAndSetSize()
        {
          var titleSize = TitleStyle.CalcSize(new GUIContent(Title));

          // The header size depends on the icon + title
          var headerSize = new Vector2(Icon != null ? Icon.width + titleSize.x : titleSize.x,
                                       Icon != null ? Icon.height : titleSize.y);
          headerSize.y += TitleStyle.margin.vertical;
          headerSize.x += TitleStyle.margin.horizontal;

          var descriptionSize = DescriptionStyle.CalcSize(new GUIContent(Description));
          descriptionSize.y += DescriptionStyle.margin.vertical;
          descriptionSize.x += DescriptionStyle.margin.horizontal;

          // Now calculate the width and height
          var width = headerSize.x > descriptionSize.x ? headerSize.x : descriptionSize.x;
          width *= 1.05f;
          var height = headerSize.y + (descriptionSize.y);

          Size = new Vector2(width, height);
        }

        /// <summary>
        /// Draws the content element within the node. It will draw a provided icon,
        /// a title and a description.
        /// </summary>
        /// <param name="position"></param>
        public void Draw(Rect position)
        {
          Position = position;
          GUI.Label(position, position.ToString());
          GUILayout.BeginArea(position, GUI.skin.button);
          {
            // Header
            GUILayout.BeginHorizontal();
            if (Icon != null) GUILayout.Box(Icon);
            GUILayout.Label(Title, TitleStyle);
            GUILayout.EndHorizontal();

            // Description
            GUILayout.Label(Description, DescriptionStyle);
          }
          GUILayout.EndArea();
        }     
        
        public void Select()
        {

        } 

        public void Deselect()
        {

        }

      }
    } 
  }

}