/******************************************************************************/
/*!
@file   LookAt.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using Stratus;

namespace Overworld
{
  /**************************************************************************/
  /*!
  @class LookAt 
  */
  /**************************************************************************/
  public class LookAt : MonoBehaviour
  {
    public float Duration = 1.5f;
    private Boolean Looking = false;

    /**************************************************************************/
    /*!
    @brief  Initializes the Script.
    */
    /**************************************************************************/
    void Start()
    {
      this.gameObject.Connect<PlayerInteract.InteractEvent>(this.OnInteractEvent);
      this.gameObject.Connect<PlayerInteract.ScanEvent>(this.OnScanEvent);
    }

    void OnScanEvent(PlayerInteract.ScanEvent e)
    {
      e.Context = "Look";
    }

    void OnInteractEvent(PlayerInteract.InteractEvent e)
    {
      if (!Looking)
      {
        DispatchCameraEvent.LookAt(this.gameObject, new CameraController.Transition(Duration, Ease.Linear, true));
        var seq = Actions.Sequence(this.gameObject.Actions());
        Actions.Property(seq, Looking, true, 0.0f, Ease.Linear);
        Actions.Property(seq, Looking, false, Duration, Ease.Linear);
      }
    }

  }

}