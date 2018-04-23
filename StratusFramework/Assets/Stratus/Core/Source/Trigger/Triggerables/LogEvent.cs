/******************************************************************************/
/*!
@file   LogEvent.cs
@author Christian Sagel
@par    email: ckpsm@live.com
*/
/******************************************************************************/
using UnityEngine;
using Stratus;
using System;
using Stratus.Dependencies.Ludiq.Reflection;

namespace Stratus
{
  /// <summary>
  /// Simple event that logs a message to the console when triggered.
  /// </summary>
  public class LogEvent : Triggerable
  {
    public enum LogType
    {
      Description,
      Member
    }
    

    ///[Tooltip("What type og log")]
    ///public LogType type;
    ///[Tooltip("The member to log")]
    ///[DrawIf("type", LogType.Member, ComparisonType.Equals)]
    ///[Filter(Methods = false, Properties = true, NonPublic = true, ReadOnly = true, Static = true, Inherited = true, Fields = true)]
    ///public UnityMember member;

    protected override void OnAwake()
    {
      
    }

    protected override void OnReset()
    {
      descriptionMode = DescriptionMode.Manual;
    }

    protected override void OnTrigger()
    {
      Trace.Script(description, this);

      //string value = null;
      //
      //switch (type)
      //{
      //  case LogType.Description:
      //    value = description;
      //    break;
      //  case LogType.Member:
      //    value = member.Get().ToString();
      //    break;
      //  default:
      //    break;
      //}
      //
      //Trace.Script(value, this);      
    }
    
  }
}
