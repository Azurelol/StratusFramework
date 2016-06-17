---------------------------------------------------------------------------------------------------------------------------

Inspector Navigator - © 2014, 2015 Wasabimole http://wasabimole.com

---------------------------------------------------------------------------------------------------------------------------

HOW TO - INSTALLING INSPECTOR NAVIGATOR:

- Import package into project
- Then click on:
	Window -> Inspector Navigator -> Open Inspector Navigator Window

The window will appear grouped with the Inspector by default, in a different tab. We recommend then to drag the 
Inspector window just below the Inspector Navigator, and then adjust the Navigator's window height to a minimum.

---------------------------------------------------------------------------------------------------------------------------

[NOTE] The source is intended to be used as is. Do NOT compile it into a DLL. We can't give support to modified versions.

---------------------------------------------------------------------------------------------------------------------------

HOW TO - USING INSPECTOR NAVIGATOR:

- Use the back '<' and forward '>' buttons to navigate to recent inspectors
- Hotkeys [PC]:  Ctrl + Left/Right 
- Hotkeys [Mac]: Alt + Cmd + Left/Right
- Click on the breadcrumb bar to jump to any object
- Click on the padlock icon to lock the tool to current objects
- Drag and drop objects from the breadcrumb bar
- To delete a breadcrumb, drag it to the Remove box
- Access "Help and Options" from the menu to edit tool preferences

[NOTE] Hotkeys can be changed by editing Wasabimole/Inspector Navigator/Editor/KeyBindings.cs

---------------------------------------------------------------------------------------------------------------------------

INSPECTOR NAVIGATOR - VERSION HISTORY

1.23 Alignment update
- New option to set breadcrumb bar horizontal alignment preference
- New Asset Store Inspector filter
- Fixed breadcrumbs objects leaking when breadcrumbs were not serialized
- Keep assets in breadcrumbs when switching scenes and breadcrumbs are not serialized
- Use shared Wasabimole.Core.UpdateNotifications library
- Other small bug fixes

1.22 Disable serialization update
- Serialization of breadcrumbs on scenes can now be disabled
- Dialog for clearing project scenes when checking serialization off
- Optimized tool start-up times when opening up a scene
- Fixed problem where breadcrumbs disappeared when going into play mode
- Unselecting an object no longer marks the scene as changed
- Changed default object queue length to 64
- Other small bug fixes

1.20 Strip breadcrumbs update
- InspectorBreadcrumbs no longer included in build on Unity 5 (using new HideFlags.DontSaveInBuild)
- New menu option to delete inspector breadcrumbs from all project scenes (so they can be removed before performing a build on Unity 4.X)
- InspectorBreadcrumbs are no longer created again right after being deleted (only after a new object/asset selection)
- Do not create InspectorBreadcrumbs when selecting any filtered object
- Upon load scene, do not automatically center camera on last selected object
- New Scripts & TextAssets filter type, disabled by default
- New ProjectSettings filter type, disabled by default
- Removed all filtered objects from breadcrumbs on load scene
- Modifying object filters has now immediate effect
- InspectorBreadcrumbs object is now filtered, and no longer appears in the breadcrumbs bar
- Warning before enabling project settings tracking on Unity 4.X
- New menu option to check for new user notifications
- Other small bug fixes

1.18 Keys & colors update
- New button to define the hot-keys in the options
- Moved hot-key definitions to KeyBindings.cs
- Option to set the text color for different object types
- Fixed breadcrumbs not being properly removed
- Fixed changing InspectorBreadcrumbs visibility
- Fixed visual glitches on play mode
- Other small bug fixes

1.16 Remove breadcrumbs update
- Allow removing breadcrumbs by dragging them into the "Remove" box
- Option to remove and not track unnamed objs
- Fixed issue with lost notification messages
- Remove any duplicate inspector breadcrumbs scene objects
- Remove inspector breadcrumbs from scene when closing the tool
- Allow deleting by hand InspectorBreadcrumbs object
- Other small bug fixes

1.15 Unity 5 Hotfix [Must delete previous version first]
- Fixed error GameObject (named 'BreadCrumbs') references runtime script in scene file. Fixing!​
- Restructured project folders, now under Wasabimole/Inspector Navigator (must delete old Editor/InspectorNavigator.cs and Editor/NotificationCenter.cs files!)
- Added option to show breadcrumbs object in the scene
- Other small bugfixes

1.14 Drag and drop update
- Drag and drop breadcrumbs to any other Unity window or field
- Set minimum window size to match the width of the 2 arrows
- Selecting a filtered object now properly unselects the breadcrumb
- Added Wasabimle logo to help and options window
- Used base64 for resource images
- Other small bug fixes

1.11 Bug fixes update
- Several small bug fixes
- Removed compilation warnings
- Option to check for plugin updates
- Option to show other notifications

1.10 Big update
- Restore previous scene camera when navigating back to an object
- New improved breadcrumbs system and serialization method
- Object breadcrumbs are now local to every scene
- Optimized code for faster OnGUI calls
- Option to filter which inspectors to track (scene objects, assets, folders, scenes)
- Option to remove all duplicated objects
- Option to set maximum number of enqueued objects
- Option to mark the scene as changed or not on new selections
- Option to review the plugin on the Asset Store
- Other small bug fixes

1.07 Breadcrumb++ update
- Improved breadcrumb bar behaviour
- New Help and Options window
- New tool padlock to lock to current objects
- Option to set max label width
- Option to clear or insert when selecting new objects
- Option to remove duplicated objects when locked
- Option to choose scene camera behaviour
- Fixed default hotkeys on Mac to Alt + Cmd + Left/Right
- Other small bug fixes

1.03 Hotkeys update
- Added Inspector Navigator submenu + hotkeys Ctrl/Cmd + Left/Right
- Limited queue size
- Handle Undo/Redo operations better
- Handle inspector lock states

1.02 First public release
- Small bug fixes

1.00 Initial Release
– Back and Forward buttons navigate to recent object inspectors
– Breadcrumb bar shows recent objects and allows click to jump to them
– Inspector history is serialized when closing and opening Unity

---------------------------------------------------------------------------------------------------------------------------

Thank you for choosing this extension, we sincerely hope you like it!

Please send your feedback and suggestions to mailto://contact@wasabimole.com

---------------------------------------------------------------------------------------------------------------------------

--- [ You can delete this readme file from your project! ] ---
