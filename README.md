![](https://github.com/PsychoSanchez/HoloDoom/raw/master/img/teamkill.gif)
# HoloDoom
Hololens multiplayer game. Set totem, fight against waves of monsters, take pickups, don't forget to shoot your buddy in a face :3

## About
Game created with Unity 5.6.2f1, and HoloToolkit for same version. 
All multiplayer backend, such as: Creating rooms for players, sending data to players, synchronising spaces, fixing player anchor, used Microsoft + NASA service - SharingService.
All game logic and other multiplayer stuff located at player that first places world anchor (or totem). So technically this game is p2p, except syncing game spaces.

##Build with
* Unity 5.6.2f1
* UWP 10.0.15...
* Sharing Service
* Update Manager
* ImportExportManager - one of microsoft libs

## Install
* Install UWP
* Install Unity
* (Optional)Install Hololens Emulator
* Start Sharing Service locally or install it as a service
* Build Unity project
* Deploy solution to your device
* Enjoy

## Demo
![](https://github.com/PsychoSanchez/HoloDoom/raw/master/img/spawn.gif)
![](https://github.com/PsychoSanchez/HoloDoom/raw/master/img/coop.gif)
