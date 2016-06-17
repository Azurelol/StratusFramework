/******************************************************************************/
/*!
@file   CameraController.cs
@author Christian Sagel
@par    email: c.sagel\@digipen.edu
@par    DigiPen login: c.sagel
@date   5/25/2016
*/
/******************************************************************************/
using UnityEngine;
using Stratus;


/**************************************************************************/
/*!
@class CameraController 
*/
/**************************************************************************/
public class CameraController : MonoBehaviour {

  // The current state of the camera
  enum Mode { Idle, Follow, Fixed, Orbit };
  enum LookMode { Idle, Track, Look } 
  // Common camera settings
  public class Config
  {
    public GameObject Target;
    public Real Radius = 10.0f;
    public Real Height = 15.0f;
    public Real Angle = 90.0f;
    public Real Damping = 1.0f;
    public void Copy(Config rhs, bool toRadian = false)
    {
      Target = rhs.Target;
      Radius = rhs.Radius;
      Height = rhs.Height;
      Angle = rhs.Angle; if (toRadian) Angle *= Mathf.Deg2Rad; 
      Damping = rhs.Damping;
    }
  }

  public class Transition
  {
    public Real Duration = 1.0f;
    public Ease Easing = Ease.QuadOut;
    public Boolean Return = false;
    public Transition() {}
    public Transition(Real duration, Ease ease, Boolean returning)
    {
      Duration = duration; Easing = ease; Return = returning;
    }
  }

  //--------------------------------------------------------------------------/
  // Camera events
  //--------------------------------------------------------------------------/
  public class FollowEvent : Stratus.Event
  {
    public Config Configuration = new Config();
    public Transition Transition = new Transition();
  };  

  public class LookAtEvent : Stratus.Event
  {
    public GameObject Target;
    public Transition Transition = new Transition();
  }  

  public class RepositionEvent : Stratus.Event
  {
    public Config Configuration = new Config();
    public Transition Transition = new Transition();
  }
  //--------------------------------------------------------------------------/
  // Members
  //--------------------------------------------------------------------------/
  // State
  private Mode CurrentMode = Mode.Idle;
  private Mode PreviousMode = Mode.Idle;
  private Mode NextMode = Mode.Idle;
  private LookMode CurrentLookMode = LookMode.Idle;
  private LookMode PreviousLookMode = LookMode.Idle;
  private LookMode NextLookMode = LookMode.Idle;
  // Camera Properties
  Config Configuration = new Config();
  Config PreviousConfiguration = new Config();
  private Real CurrentAngle;
  private Vector3 UpVector = new Vector3(0.0f, 1.0f, 0.0f);

  /**************************************************************************/
  /*!
  @brief  Initializes the Script. Subscribes to Space-level events.
  */
  /**************************************************************************/
  void Awake () {
    // Subscribe to events  
    Stratus.Space.Connect<FollowEvent>(this.OnFollowEvent);
    Stratus.Space.Connect<LookAtEvent>(this.OnLookAtEvent);
    Stratus.Space.Connect<RepositionEvent>(this.OnRepositionEvent);
  }
    
  void OnFollowEvent(FollowEvent e)
  {
    // Save the previous configuration
    this.SaveConfiguration();
    // Update members
    this.Configuration = e.Configuration;
    this.CurrentAngle = e.Configuration.Angle;
    // Change the mode
    this.ChangeMode(Mode.Follow, LookMode.Track, e.Transition.Duration);
    // Reposition around the target
    this.Reposition(Configuration.Radius, Configuration.Height, Configuration.Angle, 
                    e.Transition.Duration, e.Transition.Easing);
    Trace.Object("Now following '" + Configuration.Target.name + "'", this);
}
  void OnLookAtEvent(LookAtEvent e)
  {
    // Save the previous configuration
    this.SaveConfiguration();
    // Update members
    this.Configuration.Target = e.Target;
    // Change the mode
    this.ChangeMode(Mode.Idle, LookMode.Look, e.Transition.Duration, e.Transition.Return);
    Trace.Object("Now looking at '" + e.Target.name + "'", this);
  }

  void OnRepositionEvent(RepositionEvent e)
  {
    // Save the previous configuration
    this.SaveConfiguration();
    // Update
    this.Configuration = e.Configuration;
    Trace.Object("Now repositioning the camera around '" + Configuration.Target.name + "'", this);
  }


  /**************************************************************************/
  /*!
  @brief Interpolates to the next camera mode.  
  @param nextMode The mode to set.
  @param nextLookMode The next look mode.
  @param delay How much time to wait.
  */
  /**************************************************************************/
  void ChangeMode(Mode nextMode, LookMode nextLookMode, Real transition, bool returnToPrev = false)
  {
    // Save the previous mode
    this.PreviousMode = this.CurrentMode;
    this.PreviousLookMode = this.CurrentLookMode;
    // Change to the next mode
    this.NextMode = nextMode;
    this.NextLookMode = nextLookMode;
    var seq = Actions.Sequence(this.gameObject.Actions());
    Actions.Delay(seq, transition);
    Actions.Call(seq, this.SetNextMode);    
    if (returnToPrev)
    {
      Actions.Delay(seq, transition);
      Actions.Call(seq, this.ReturnToPreviousMode);    
    }
  }
  
  /**************************************************************************/
  /*!
  @brief Sets the next mode and look mode.
  */
  /**************************************************************************/
  void SetNextMode() {
    Trace.Object("Move = " + this.NextMode + ", Look = " + this.NextLookMode, this);
    this.CurrentMode = this.NextMode;
    this.CurrentLookMode = this.NextLookMode;
  }

  /**************************************************************************/
  /*!
  @brief Sets the next mode and look mode.
  */
  /**************************************************************************/
  void ReturnToPreviousMode()
  {
    Trace.Object("Move = " + this.PreviousMode + ", Look = " + this.PreviousLookMode, this);
    this.CurrentMode = this.PreviousMode;
    this.CurrentLookMode = this.PreviousLookMode;
    // Revert to the previous configuration
    this.Configuration.Copy(this.PreviousConfiguration);
  }

  /**************************************************************************/
  /*!
  @brief Save the previous configuration
  */
  /**************************************************************************/
  void SaveConfiguration()
  {
    this.PreviousConfiguration.Copy(this.Configuration);
  }

  /**************************************************************************/
  /*!
  @brief Updates the camera controller. This is where its operation branches
         depending on what the current modes are set to.
  */
  /**************************************************************************/
  void Update()
  {
    if (!Configuration.Target)
      return;

    // Move
    if (this.CurrentMode == Mode.Follow) this.Follow();
    // Look
    if (this.CurrentLookMode == LookMode.Track) this.Track();
  }


  /**************************************************************************/
  /*!
  @brief The camera follows the current target while looking at it.
  */
  /**************************************************************************/
  void Follow()
  {
    // Update the current angle
    this.CurrentAngle = Mathf.Lerp(this.CurrentAngle, 
                                   this.Configuration.Angle, 
                                   Time.deltaTime * this.Configuration.Damping);        

    // Calculate the new position of the camera
    var targetPos = this.Configuration.Target.transform.position;
    var newPos = this.CalculatePosition(this.Configuration.Radius, this.Configuration.Height, 
                                        this.CurrentAngle, targetPos);


    // Apply the new translation
    this.transform.position = Vector3.Lerp(this.transform.position, newPos, 
                                           Time.deltaTime * this.Configuration.Damping);
  }

  /**************************************************************************/
  /*!
  @brief Tracks the current target.
  */
  /**************************************************************************/
  void Track()
  {
    if (!this.Configuration.Target)
    {
      this.CurrentLookMode = LookMode.Idle;
      return;
    }

    // Calculate the new rotation
    this.transform.LookAt(this.Configuration.Target.transform.position, this.UpVector);
  }

  /**************************************************************************/
  /*!
  @brief Repositions the camera.
  */
  /**************************************************************************/
  void Reposition(Real radius, Real height, Real angle, Real duration, Ease ease)
  {
    var targetPos = this.Configuration.Target.transform.position + UpVector;
    var newCameraPos = this.CalculatePosition(radius, height, angle, targetPos);
    // Start the sequence
    var seq = Actions.Sequence(this.gameObject.Actions());
    Actions.Property(seq, this.gameObject.transform.position, newCameraPos, duration, ease);
  }

  /**************************************************************************/
  /*!
  @brief Calculates the next position of the camera.
  */
  /**************************************************************************/
  Vector3 CalculatePosition(Real radius, Real height, Real angle, Real3 targetPos)
  {
    var newPos = new Vector3(Mathf.Cos(angle) * radius,
                              height,
                              Mathf.Sin(angle) * radius)
                              + targetPos;
    return newPos;
  }

  
  
}
