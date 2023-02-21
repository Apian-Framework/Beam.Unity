# Beam.Unity


**An Apian demo app**


<p >
  <img src="./docs/beam-title.jpg" width="600" title="hover text">
</p>


`Beam` is a free-running sim-style game intended to function as an Apian testbed. The majority of the code is platform-independent and lives in [a separate repository](https://github.com/Apian-Framework/BeamGameCode). This repo contains a Unity 3D project that incorporates that code and implements a 3D frontend and can be deployed under Windows, MacOs, Android, iOS, or in a web browser.

In addition to this Unity version there is also [a headless CLI implementation](https://github.com/Apian-Framework/Beam.Cli) that is fully interoperable with this one and can be used as a standalone "validator" peer or to supply peach-can AI bikes.

## The Game

In Beam a player pilots a futuristic space-bike on a grid, trying to avoid hitting pylons left by other bikes, all the while dropping pylons on the grid and trying to get other bikes to hit them. It's basically 3D multiplayer snake in space.

When a pylon is hit the player hitting it loses a bunch of points and the player who created it is awarded a bunch of them. Also, every bike on the grid that is on the same 'team' as the creator (is the same color) is awarded a smaller amount of points. If the collision reduces the hitting player's score to zero then the bike explodes and many more points are awarded.

Team membership is random and when a player respawns her new team is probably not the same as it was. Awarded points and damages are less if the bike and the pylon are the same color. So if you are on the Yellow team and in a position where you can't avoid hitting a pylon, try to hit a yellow one. Likewise, a certain amount of impromptu cooperation with other Yellow bikes in trying to snare one of another team is probably a good idea. There is no concept of a "team score", though. You really are on your own.

## Installing / Building

This is a Unity 3D project and so you will have to have Unity 3D installed. The easiest way to get all of the sources necessary is to clone the [Beam-Releases repo](https://github.com/Apian-Framework/Beam-Releases) and follow the instructions there.

## Gameplay






<br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br /><br />



**A collection of versioned git submodules of all of the repos needed for working releases of Beam**

Apian has an awful lot of moving parts, and almost all of them are currently in development. For this reason Beam and other apps are currently built using project (source) references to the various Apian pieces, rather than Nuget package references. While using a single "monorepo" might be a good idea, I'm not sure my git-fu is strong enough to manage something like that. So I'm going with separate package repositories.

Working on the code with things this way works fine for me or anyone else who has already cloned all of the necessary Apian repos locally, but it's really a pain for anyone who just wants to "have a look" and maybe build and the Beam CLI and Unity demo applications.

This repository is my attempt at a solution. Cloning just it is all that is necessary to examine and build all of the different releases of Beam. I find that development within git submodules is a fragile and hairy affair, so I don't recommend doing it in this repo - but looking and building and running are easy.

As an extra added bonus, if someone is interested in creating an Apian app (which is kinda the point to all of this) but has no interest in working on the Apian codebase itself, a clone of this repo can be used to satisfy all of their project's build references.

---

## Installation

### .NET
The Apian Framework runs under the open-source cross-platform .NET 6.0 (or newer) environment, so you will first have to install it. There are many tutorials on the web describing how to do this for almost any platform, but I usually find it easiest to simply go straight to the source:

[Get .NET from MicroSoft](https://dotnet.microsoft.com/download)

### Beam-Releases
Once you have .NET installed, clone this repository and its submodules (make sure to use `--recurse submodules` )

`git clone --recurse-submodules https://github.com/Apian-Framework/Beam-Releases.git`

---

## Selecting a Release

Immediately after you clone it, the repository will have the `main` branch checked out, which generally corresponds to he most recent release. To see a list of the releases available, use the command

`git branch -a`

to list all of the branches. Initially the list will contain `main` and all of the available remote branches. I Setting the release version is just a matter of switching to a particular branch. So, to switch to the branch `remotes/origin/REL_230112b` you would execute:

`git switch REL_230112b`

and git would create a local tracking branch corresponding to  the remote one and make it current.

*Note:* Because these "releases" are really just collections of particular versions of the various repos and don;t correspond to any overall feature or bug-fix changes, the version names do not follow the semver format. Most of them simply represent that date. `REL_230112b`, for instance, is the 2nd release for January 11, 2023.

---

## Building Beam.Cli

To build the console version of Beam you will first need to open a terminal. Unfortunately, .NET doesn't (yet?) have the concept of a default project for a solution, so you either have to specify a project file in your commands, or actually be in the directory containing the project file. The latter is usually easier, so:

`cd Beam.CLI/src/BeamCli`

To make sure everything is in place, you can restore all of the dependencies:

`dotnet restore`

or actually just try running the app:

`dotnet run -- --help`

will list the options. To learn more about them check out the [Beam.Cli readme](https://github.com/Apian-Framework/Beam.Cli#readme)

---

## Building Beam.Unity

To build Beam under Unity 3D you will have to have a licensed copy of Unity on you development machine.

The project references for  Unity are all in `Beam.Unity/packages/manifest.json` and by default expect all of the dependencies to be in folders "beside" `Beam.Unity`. Since that's how they are organized in this repository all you should need to do is to start Unity Hub and open the Beam.Unity folder as a project.

---

## Using this repository to satisfy project references in your own builds

TBD (Hint: uses `Directory.Build.props`)
