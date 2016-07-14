# StratusFramework
A gameplay scripting framework for the Unity Engine, written in C#.

In this repository you will find the source code to the Stratus Framework, an Unity-focused framework for facilitating the composition of gameplay code.
Its major tenet is to provide useful features with an easy to use interface while removing of as much boilerplate code from your scripts as posssible.

I consider this framework very much a work in progress. Even so I find its core features, the Events and Actions systems to be rather robust, and can be used as is. The interface for them won't be changing in the future, as any
future work on them will be adding features and fixing any discovered issues.

---
##Features
- **Events**: A custom event system using delegates that greatly simplifies the use of callbacks in code, for the implementation of code using the Observer pattern.
- **Spaces**: Working in tandem with the event system, a Space is an object containing all the GameObjects in a given scene. In practice the Space becomes a proxy which provides a common point for "scene-wide" events to be sent to.
- **Actions**: An Action list library with a very simplified interface for quickly construction action sets for interpolating properties, delayed function invocations, etc.
- **Trace**: A small library that decorates logging calls appropiately for quickly logging methods, member variables, etc.
- **Extensions**: A few extension methods which I found missing from the main classes we regularly interface with in Unity, such as GameObject and Monobehaviour.

---
##Examples:

I have provided sample scripts to display the functionality in the samples folder. To test them just add them to a component in Unity and read the console's output to inspect the sequence of calls.
Some snippets of the code within:

####Events

```C#

  public void OnSampleEvent(SampleEvent eventObj)
  {
    Trace.Script("Event received!", this);
    Trace.Script("Number = " + eventObj.Number, this);
  }
  
  public void DispatchEvent() 
  {
    // Connect a member function to the given event
    this.gameObject.Connect<SampleEvent>(OnSampleEvent);
    // Construct the event object
    SampleEvent eventObj = new SampleEvent();
    eventObj.Number = 5;
    // Dispatch the event
    Trace.Script("Event dispatched", this);
    this.gameObject.Dispatch<SampleEvent>(eventObj);
  }    
```

####Actions

```C#

    public float SampleFloat = 5;

    void ConstructActionSequence() 
    {
      // Construct the action sequence, adding it onto this GameObject's list of active actions
      var seq = Actions.Sequence(this);
      // First action we will create is a delay, which is a *blocking* action
      Actions.Delay(seq, 2.0f);      
      // Second, we will interpolate the value of the field 'SampleFloat' from its initial value (5) 
      // to a specified value (25) over 2 seconds using a specified easing (curve) algorithm
      Actions.Property(seq, ()=>this.SampleFloat, 25, 2.0f, Ease.Linear);
      // Third and last, we will invoke a specified function!
      Actions.Call(seq, this.Boop);
    }
    
    void Boop() 
    {
      Trace.Script("Boop!");
    }
    

```

---
##Download

I have provided the Unity package for importing the framework into your Unity project [here](StratusFramework.unitypackage)

---
##Contact

If you discover any bugs or have any and all constructive feedback on how to improve the framework, feel free to contact me.