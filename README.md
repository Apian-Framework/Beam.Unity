# **Beam.Unity**


**An Apian demo app**


<p >
  <img src="./docs/beam-title.jpg" width="600" title="hover text">
</p>


`Beam` is a free-running sim-style game intended to function as an Apian testbed. The majority of the code is platform-independent and lives in [a separate repository](https://github.com/Apian-Framework/BeamGameCode). This repo contains a Unity 3D project that incorporates that code and implements a 3D frontend and can be deployed under Windows, MacOs, Android, iOS, or in a web browser.

In addition to this Unity version there is also [a headless CLI implementation](https://github.com/Apian-Framework/Beam.Cli) that is fully interoperable with this one and can be used as a standalone "validator" peer or to supply peach-can AI bikes.

---

## **The Game**

In Beam a player pilots a futuristic space-bike on a grid, trying to avoid hitting pylons left by other bikes, all the while dropping pylons on the grid and trying to get other bikes to hit them. It's basically 3D multiplayer snake in space.

When a pylon is hit the player hitting it loses a bunch of points and the player who created it is awarded a bunch of them. Also, every bike on the grid that is on the same 'team' as the pylon creator (is the same color) is awarded a smaller amount of points. If the collision reduces the hitting player's score to zero then the bike explodes and many more points are awarded.

Team membership is random and when a player respawns her new team is probably not the same as it was. Awarded points and damages are fewer if the bike and the pylon are the same color, so if you are on the Yellow team and in a position where you can't avoid hitting a pylon, try to hit a yellow one. Likewise, a certain amount of impromptu cooperation with other Yellow bikes in trying to snare a member of another team is probably a good idea. There is no concept of a "team score", though. You really are on your own.

Oh, also: don't drive off the edge of the grid.

---
## **Installing / Building**

This is a Unity 3D project and so you will have to have Unity 3D installed. The easiest way to get all of the sources necessary is to clone the [Beam-Releases repo](https://github.com/Apian-Framework/Beam-Releases) and follow the instructions there.

---

## **Gameplay**

When you first art the game you will probably want to practice. So click on "Practice" and you will be put in a local-only arena where you can hone your skills.

### **Driving**

<p >
  <img src="./docs/play-hilit-btns.jpg" width="600" title="hover text">
</p>


 - To turn your bike use the keyboard arrow keys or click on the lower-left and lower-right quadrants of the screen.

 - To cycle through the camera views, press the spacebar or click on the upper-middle of the screen.

 - In "target" view cycle through other bikes using the "Z" and "X" keys or clicking on the upper-left and upper-right of the screen.

 - In "follow" view look left and right using the "Z" and "X" keys or clicking on the upper-left and upper-right of the screen.

- Note that you cannot control the speed of your bike.

### **Other controls**

- Clicking on the score display a the lower left or pressing the "S" key will toggle its full display.

- Clicking on the menu icon at the lower right allows you to exit practice or adjust the game volume.

---
## **Multiplayer**

The real reason for Beam (and Apian) is of course to play with other people.

