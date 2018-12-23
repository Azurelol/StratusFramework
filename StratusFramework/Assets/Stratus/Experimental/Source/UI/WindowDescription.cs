/******************************************************************************/
/*!
@file   WindowDescription.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using UnityEngine.UI;

namespace Stratus
{
  namespace UI
  {
    /**************************************************************************/
    /*!
    @class WindowDescription 
    */
    /**************************************************************************/
    public class WindowDescription : MonoBehaviour
    {
      public Text TitleText, SubtitleText, HelpText;

      /**************************************************************************/
      /*!
      @brief  Initializes the Script.
      */
      /**************************************************************************/
      void Start()
      {
        this.gameObject.Connect<Link.DescriptionEvent>(this.OnLinkDescriptionEvent);
      }

      void OnLinkDescriptionEvent(Link.DescriptionEvent e)
      {
        switch (e.Description.Type)
        {
          case Link.LinkDescription.DescriptionType.Title:
            UpdateTitle(e.Description.Message);
            break;
          case Link.LinkDescription.DescriptionType.Subtitle:
            UpdateSubtitle(e.Description.Message);
            break;
          case Link.LinkDescription.DescriptionType.Help:
            UpdateHelp(e.Description.Message);
            break;
        }
      }

      void OnWindowDescriptionEvent(Window.DescriptionEvent e)
      {
        UpdateTitle(e.Title);
        UpdateSubtitle(e.Description);
      }

      //void OnWindowUpdateHelpEvent(Window.UpdateHelpEvent e)
      //{
      //  UpdateHelp(e.Text);
      //}

      void UpdateTitle(string text)
      {
        if (!TitleText)
          return;

        TitleText.text = text;
      }

      void UpdateSubtitle(string text)
      {
        if (!SubtitleText)
          return;

        SubtitleText.text = text;
      }

      void UpdateHelp(string text)
      {
        if (!HelpText)
          return;

        HelpText.text = text;
      }



    }

  } 
}