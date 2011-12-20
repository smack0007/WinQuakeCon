WinQuakeCon
===========

WinQuakeCon is a console / termninal / command shell manager program. A manager because the program itself is not a console. You configure WinQuakeCon
to use the console program of your choice (ex. cmd.exe, PowerShell, Console, Cygwin) and WinQuakeCon manages the program for you. Managing refers to
ensuring there is an instance started whenever you say you need one, and controling the position of the main window of the console.

By default, WinQuakeCon will slide the console from above downward onto your screen like the console in the original Quake game (hence the name). This
animation can be disabled or changed, for instance the console can slide in from the left side if you desire. All this can be done by changing the settings
in the Config.xml file.

Configuration
-------------

* __HotKeyCode__ - The keycode which will toggle the console. 192 by default which is the tilda key on US layout keyboards. (DE: 220, GB: 222)
* __HotKeyAlt__ - If set to "true", the ALT key must also be pressed to toggle the console.
* __HotKeyCtrl__ - If set to "true", the CTRL key must also be pressed to toggle the console.
* __HotKeyShift__ - If set to "true", the SHIFT key must also be pressed to toggle the console.
* __HotKeyWin__ - If set to "true", the WIN key must also be pressed to toggle the console.
* __Console__ - The program to use as your console.
* __ConsoleRemoveBorder__ - If set to "true", the border around console program will be removed. This feature does not work well with cmd.exe and PowerShell.
* __ConsoleWidth__ - The desired width of your console. Cmd.exe and PowerShell may not resize and you may need to set the width manually.
* __ConsoleHeight__ - The desired height of your console. Cmd.exe and PowerShell may not resize and you may need to set the height manually.
* __ConsoleScreen__ - The index of the screen to display the console on.
* __ConsoleHiddenX__ - The X position of your console while it is hidden. You can change this value in order to change animation when toggling.
* __ConsoleHiddenY__ - The Y position of your console while it is hidden. You can change this value in order to change animation when toggling.
* __ConsoleVisibleX__ - The X position of your console while it is visible.
* __ConsoleVisibleY__ - The Y position of your console while it is visible.
* __WorkingDirectory__ - The working directory to set for your console when it is initialized. This setting does not work in Console2.
* __Animate__ - If set to "true", the console will be animated when toggling. If "false", the console will simply be shown / hidden.
* __AnimateSpeedX__ - The speed of the animation in the X direction.
* __AnimateSpeedY__ - The speed of the animation in the Y direction.

Known Problems
--------------

* cmd.exe and powershell.exe do not work well with "ConsoleRemoveBorder" activated.
* If your console program has any kind of "snap to edge" feature, it may cause problems with animation.
* Having your taskbar at the top of the screen may cause problems with animation. You can try adjusting the AnimationSpeed values to fix it.
