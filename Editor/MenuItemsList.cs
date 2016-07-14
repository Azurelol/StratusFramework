/******************************************************************************/
/*!
@file   MenuItemsList.cs
@author Christian Sagel
@par    email: ckpsm@live.com
@date   5/25/2016
@brief  Generates a list of strings of all known menu items path.
*/
/******************************************************************************/
using UnityEngine;
using System.Collections.Generic;

namespace Stratus {
  namespace Editor
  {
    /**************************************************************************/
    /*!
    @class MenuItemsList 
    */
    /**************************************************************************/
    public class MenuItemsList
    {
      static List<string> GenerateByHand()
      {
        List<string> menuItems = new List<string>();

        // File // These don't work 
        //menuItems.Add("File/Build & Run");
        //menuItems.Add("File/Build Settings...");
        //menuItems.Add("File/Exit");
        //menuItems.Add("File/New Project...");
        //menuItems.Add("File/New Scene");
        //menuItems.Add("File/Open Project");
        //menuItems.Add("File/Open Scene ");
        //menuItems.Add("File/Save Project");
        //menuItems.Add("File/Save Scene");
        //menuItems.Add("File/Save Scene as...");
        // Edit
        string[] editItems = {
          "Graphics Emulation/No Emulation",
          "Graphics Emulation/Shader Model 2",
          "Graphics Emulation/Shader Model 3",
          "Network Emulation/Broadband",
          "Network Emulation/DSL",
          "Network Emulation/Dial-Up",
          "Network Emulation/ISDN",
          "Network Emulation/None",
          "Pause",
          "Play",
          "Project Settings/Audio",
          "Project Settings/Editor",
          "Project Settings/Graphics",
          "Project Settings/Input",
          "Project Settings/Network",
          "Project Settings/Physics",
          "Project Settings/Physics 2D",
          "Project Settings/Player",
          "Project Settings/Quality",
          "Project Settings/Script Execution Order",
          "Project Settings/Tags and Layers",
          "Project Settings/Time",
          "Selection/Load Selection 0",
          "Selection/Load Selection 1",
          "Selection/Load Selection 2",
          "Selection/Load Selection 3",
          "Selection/Load Selection 4",
          "Selection/Load Selection 5",
          "Selection/Load Selection 6",
          "Selection/Load Selection 7",
          "Selection/Load Selection 8",
          "Selection/Load Selection 9",
          "Selection/Save Selection 0",
          "Selection/Save Selection 1",
          "Selection/Save Selection 2",
          "Selection/Save Selection 3",
          "Selection/Save Selection 4",
          "Selection/Save Selection 5",
          "Selection/Save Selection 6",
          "Selection/Save Selection 7",
          "Selection/Save Selection 8",
          "Selection/Save Selection 9",
          "Sign in...",
          "Sign out",
          "Snap Settings...",
          "Step"
        }; foreach (var item in editItems) menuItems.Add("Edit/" + item);
        // Assets
        string[] assetItems =
        {
        "Create/Animation",
        "Create/Animator Controller",
        "Create/Animator Override Controller",
        "Create/Audio Mixer",
        "Create/Avatar Mask",
        "Create/C# Script",
        "Create/Custom Font",
        "Create/Editor Test C# Script",
        "Create/Folder",
        "Create/GUI Skin",
        "Create/Javascript",
        "Create/Legacy/Cubemap",
        "Create/Lens Flare",
        "Create/Lightmap Parameters",
        "Create/Material",
        "Create/Physic Material",
        "Create/Physics2D Material",
        "Create/Prefab",
        "Create/Render Texture",
        "Create/Scene",
        "Create/Shader/Compute Shader",
        "Create/Shader/Image Effect Shader",
        "Create/Shader/Standard Surface Shader",
        "Create/Shader/Unlit Shader",
        "Create/Shader Variant Collection",
        "Create/Sprite/Circle",
        "Create/Sprite/Diamond",
        "Create/Sprite/Hexagon",
        "Create/Sprite/Polygon",
        "Create/Sprite/Square",
        "Create/Sprite/Triangle",
        "Delete",
        "Export Package...",
        "Find References In Scene",
        "Import New Asset...",
        "Import Package/2D",
        "Import Package/Cameras",
        "Import Package/Characters",
        "Import Package/CrossPlatformInput",
        "Import Package/Custom Package...",
        "Import Package/Effects",
        "Import Package/Environment",
        "Import Package/ParticleSystems",
        "Import Package/Prototyping",
        "Import Package/Utility",
        "Import Package/Vehicles",
        "Import Package/Visual Studio 2015 Tools",
        "Open",
        "Open C# Project",
        "Open Scene Additive",
        "Refresh",
        "Refresh",
        "Reimport",
        "Reimport All",
        "Run API Updater...",
        "Select Dependencies",
        "Show in Explorer",
        }; foreach (var item in assetItems) menuItems.Add("Assets/" + item);
        // GameObject
        string[] gameObjectItems =
        {
          "2D Object/Sprite",          
          "3D Object/3D Text",
          "3D Object/Capsule",
          "3D Object/Cube",
          "3D Object/Cylinder",
          "3D Object/Plane",
          "3D Object/Quad",
          "3D Object/Ragdoll...",
          "3D Object/Sphere",
          "3D Object/Terrain",
          "3D Object/Tree",
          "3D Object/Wind Zone",
          "Align View to Selected",
          "Align With View",
          "Apply Changes To Prefab",
          "Audio/Audio Reverb Zone",
          "Audio/Audio Source",
          "Break Prefab Instance",
          "Camera",
          "Center On Children",
          "Clear Parent",
          "Create Empty",
          "Create Empty Child",
          "Light/Area Light",
          "Light/Directional Light",
          "Light/Light Probe Group",
          "Light/Point Light",
          "Light/Reflection Probe",
          "Light/Spotlight",
          "Make Parent",
          "Move To View",
          "Particle System",
          "Set as first sibling",
          "Set as last sibling",
          "Toggle Active State",    
          "UI/Button",
          "UI/Canvas",
          "UI/Dropdown",
          "UI/Event System",
          "UI/Image",
          "UI/Input Field",
          "UI/Panel",
          "UI/Raw Image",
          "UI/Scroll View",
          "UI/Scrollbar",
          "UI/Slider",
          "UI/Text",
          "UI/Toggle",
        }; foreach (var item in gameObjectItems) menuItems.Add("GameObject/" + item);
        // Component 
        string[] componentItems =
        {
          "Add...",
          "Audio/Audio Chorus Filter",
          "Audio/Audio Distortion Filter",
          "Audio/Audio Echo Filter",
          "Audio/Audio High Pass Filter",
          "Audio/Audio Listener",
          "Audio/Audio Low Pass Filter",
          "Audio/Audio Reverb Filter",
          "Audio/Audio Reverb Zone",
          "Audio/Audio Source",    
          "Effects/Halo",
          "Effects/Legacy Particles/Ellipsoid Particle Emitter",
          "Effects/Legacy Particles/Mesh Particle Emitter",
          "Effects/Legacy Particles/Particle Animator",
          "Effects/Legacy Particles/Particle Renderer",
          "Effects/Legacy Particles/World Particle Collider",
          "Effects/Lens Flare",
          "Effects/Line Renderer",
          "Effects/Particle System",
          "Effects/Projector",
          "Effects/Trail Renderer",    
          "Event/Event System",
          "Event/Event Trigger",
          "Event/Graphic Raycaster",
          "Event/Physics 2D Raycaster",
          "Event/Physics Raycaster",
          "Event/Standalone Input Module",
          "Event/Touch Input Module",
          "Layout/Aspect Ratio Fitter",
          "Layout/Canvas",
          "Layout/Canvas Group",
          "Layout/Canvas Scaler",
          "Layout/Content Size Fitter",
          "Layout/Grid Layout Group",
          "Layout/Horizontal Layout Group",
          "Layout/Layout Element",
          "Layout/Rect Transform",
          "Layout/Vertical Layout Group",
          "Mesh/Mesh Filter",
          "Mesh/Mesh Renderer",
          "Mesh/Skinned Mesh Renderer",
          "Mesh/Text Mesh",
          "Miscellaneous/Animation",
          "Miscellaneous/Animator",
          "Miscellaneous/Billboard Renderer",
          "Miscellaneous/Network View",
          "Miscellaneous/Terrain",
          "Miscellaneous/Wind Zone",
          "Navigation/Nav Mesh Agent",
          "Navigation/Nav Mesh Obstacle",
          "Navigation/Off Mesh Link",    
          "Network/NetworkAnimator",
          "Network/NetworkDiscovery",
          "Network/NetworkIdentity",
          "Network/NetworkLobbyManager",
          "Network/NetworkLobbyPlayer",
          "Network/NetworkManager",
          "Network/NetworkManagerHUD",
          "Network/NetworkMigrationManager",
          "Network/NetworkProximityChecker",
          "Network/NetworkStartPosition",
          "Network/NetworkTransform",
          "Network/NetworkTransformChild",
          "Network/NetworkTransformVisualizer",
          "Physics/Box Collider",
          "Physics/Capsule Collider",
          "Physics/Character Controller",
          "Physics/Character Joint",
          "Physics/Cloth",
          "Physics/Configurable Joint",
          "Physics/Constant Force",
          "Physics/Fixed Joint",
          "Physics/Hinge Joint",
          "Physics/Mesh Collider",
          "Physics/Rigidbody",
          "Physics/Sphere Collider",
          "Physics/Spring Joint",
          "Physics/Terrain Collider",
          "Physics/Wheel Collider",
          "Physics 2D/Area Effector 2D",
          "Physics 2D/Box Collider 2D",
          "Physics 2D/Buoyancy Effector 2D",
          "Physics 2D/Circle Collider 2D",
          "Physics 2D/Constant Force 2D",
          "Physics 2D/Distance Joint 2D",
          "Physics 2D/Edge Collider 2D",
          "Physics 2D/Fixed Joint 2D",
          "Physics 2D/Friction Joint 2D",
          "Physics 2D/Hinge Joint 2D",
          "Physics 2D/Platform Effector 2D",
          "Physics 2D/Point Effector 2D",
          "Physics 2D/Polygon Collider 2D",
          "Physics 2D/Relative Joint 2D",
          "Physics 2D/Rigidbody 2D",
          "Physics 2D/Slider Joint 2D",
          "Physics 2D/Spring Joint 2D",
          "Physics 2D/Surface Effector 2D",
          "Physics 2D/Target Joint 2D",
          "Physics 2D/Wheel Joint 2D",
          "Rendering/Camera",
          "Rendering/Canvas Renderer",
          "Rendering/Flare Layer",
          "Rendering/GUI Layer",
          "Rendering/GUI Text",
          "Rendering/GUI Texture",
          "Rendering/LOD Group",
          "Rendering/Light",
          "Rendering/Light Probe Group",
          "Rendering/Occlusion Area",
          "Rendering/Occlusion Portal",
          "Rendering/Reflection Probe",
          "Rendering/Skybox",
          "Rendering/Sprite Renderer",    
          "UI/2D Rect Mask",
          "UI/Button",
          "UI/Dropdown",
          "UI/Effects/Outline",
          "UI/Effects/Position As UV1",
          "UI/Effects/Shadow",
          "UI/Image",
          "UI/Input Field",
          "UI/Mask",
          "UI/Raw Image",
          "UI/Scroll Rect",
          "UI/Scrollbar",
          "UI/Selectable",
          "UI/Slider",
          "UI/Text",
          "UI/Toggle",
          "UI/Toggle Group",
        }; foreach (var item in componentItems) menuItems.Add("Component/" + item);
        // Mobile Input
        menuItems.Add("Mobile Input/Disable");
        menuItems.Add("Mobile Input/Enable");
        // Window
        string[] windowItems = 
        {
          "Animation",
          "Animator",
          "Animator Parameter",
          "Audio Mixer",
          "Console",
          "Editor Tests Runner",
          "Frame Debugger",
          "Game",
          "Hierarchy",
          "Inspector",
          "Lighting",
          "Navigation",
          "Next Window" ,
          "Occlusion Culling",
          "Previous Window",
          "Project",
          "Scene",
          "Services",
          "Sprite Packer"
        }; foreach (var item in windowItems) menuItems.Add("Window/" + item);
        // Help
        menuItems.Add("Help/About Unity");
        menuItems.Add("Help/Manage License...");
        menuItems.Add("Help/Check for Updates");
        menuItems.Add("Help/Download Beta...");
        menuItems.Add("Help/Release Notes");
        menuItems.Add("Help/Report a Bug...");
        menuItems.Add("Help/Scripting Reference");
        menuItems.Add("Help/Unity Answers");
        menuItems.Add("Help/Unity Forum");
        menuItems.Add("Help/Unity Manual");
        menuItems.Add("Help/Unity Services");

        return menuItems;
      }

      static public List<string> Generate()
      {
        return GenerateByHand();
      }

    }


  }
}


