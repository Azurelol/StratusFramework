/******************************************************************************/
/*!
@file   CanvasMockup.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;

namespace Stratus
{
  namespace UI
  {
    /**************************************************************************/
    /*!
    @class CanvasMockup 
    */
    /**************************************************************************/
    public class CanvasMockup : MonoBehaviour
    {

      /**************************************************************************/
      /*!
      @brief  Initializes the CanvasMockup.
      */
      /**************************************************************************/
      void Awake()
      {
        this.gameObject.SetActive(false);
      }

    }

  } 
}