
/* ----------------------------------------------------------------------------------------------------------------------------------------
    Inspector Navigator Key Bindings (hot-key definitions)
-------------------------------------------------------------------------------------------------------------------------------------------

    To define a hot-key, use the following special characters: 

        % (Ctrl on Windows, Cmd on OS X)
        # (Shift)
        & (Alt)
        _ (no key modifiers). 

    For example, to define the hot-key Shift-Alt-g use "#&g". To define the hot-key g and no key modifiers pressed use "_g".

    Some special keys are supported, for example "#LEFT" would map to Shift-left. The keys supported like this are: 

        LEFT, RIGHT, UP, DOWN, F1 .. F12, HOME, END, PGUP, PGDN.

-------------------------------------------------------------------------------------------------------------------------------------------
[NOTE] If you change these keys, the next time you update the plugin, un-check ".../Editor/KeyBindings.cs" in the Importing package dialog. 
       Do this before clicking on Import, and your previous hot-key definitions will be preserved after the update. Thanks! 
---------------------------------------------------------------------------------------------------------------------------------------- */

namespace Wasabimole.InspectorNavigator.Editor
{
    public class KeyBindings
    {
        // PC hot-keys
        public const string BackPC = "%LEFT";
        public const string ForwardPC = "%RIGHT";

        // Mac hot-keys
        public const string BackMac = "&%LEFT";
        public const string ForwardMac = "&%RIGHT";
    }
}