![](https://github.com/PsychoSanchez/HoloDoom/raw/master/img/teamkill.gif)
# HoloDoom
Hololens multiplayer game. Set totem, fight against waves of monsters, take pickups, don't forget to shoot your buddy in a face :3

## About
Game demo created with Unity 5.6.2f1, and HoloToolkit for same version.

To play multiplayer, you'll need a SharingService. 
Creating rooms for players, sending data to players, synchronising spaces, fixing player anchor, etc. related to Microsoft + NASA service - SharingService.

All game logic and other multiplayer stuff located at player that first places world anchor (or totem). So technically this game is p2p, except syncing game spaces.

## Build with
* Unity 5.6.2f1
* UWP 10.0.15063
* [HoloToolkit (MixedRealityToolkit)](https://github.com/Microsoft/MixedRealityToolkit-Unity)
* [MRDL Unity Tools](https://github.com/Microsoft/MRDesignLabs_Unity_tools)
* [Sharing Service](https://github.com/Microsoft/MixedRealityToolkit/blob/master/Sharing/Src/Source/Docs/ExtendedDocs/GettingStarted.md)
* [Update Manager](https://www.assetstore.unity3d.com/en/#!/content/53581)

## Install
* Install UWP
* Install Unity
* (Optional) Install Hololens Emulator
* Start Sharing Service locally or install it as a service (via cmd: `SharingService.exe -local` or `SharingService.exe -install`)
* Build Unity project with settings:  
    - SDK: Universal 10
    - Target Device: Hololens
    - UWP Build Type: D3D
    - UWP SDK: 10.0.15063
    - Debugging Unity C# Projects: Checked
* Deploy solution to your device
* Enjoy

## Demo
![](https://github.com/PsychoSanchez/HoloDoom/raw/master/img/spawn.gif)
![](https://github.com/PsychoSanchez/HoloDoom/raw/master/img/coop.gif)
