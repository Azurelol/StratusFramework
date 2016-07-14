/******************************************************************************/
/*!
@file   ActionsRegistration.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;

namespace Stratus
{
  /**************************************************************************/
  /*!
  @class ActionsRegistration 
  */
  /**************************************************************************/
  public class ActionsRegistration : MonoBehaviour
  {
    /**************************************************************************/
    /*!
    @class When this GameObject dies, it gets deregistered from the Stratus
           Actions system.
    */
    /**************************************************************************/
    void OnDestroy()
    {
      //Trace.Script("Unsubscribed!", this);
      if (this.enabled)
        ActionSpace.Unsubscribe(this.gameObject);
    }

  }

}