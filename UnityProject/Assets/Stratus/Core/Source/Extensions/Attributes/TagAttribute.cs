/******************************************************************************/
/*!
@file   TagAttribute.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@note   Original by DYLAN ENGELMAN http://jupiterlighthousestudio.com/custom-inspectors-unity/
        Altered by Brecht Lecluyse http://www.brechtos.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;

namespace Stratus
{
  /// <summary>
  /// Allows tags to be set using a drop-down popup
  /// </summary>
  /// <remarks>Reference: http://www.brechtos.com/tagselectorattribute/</remarks>
  public class TagAttribute : PropertyAttribute
  {
    public bool UseDefaultTagFieldDrawer = false;
  }

}