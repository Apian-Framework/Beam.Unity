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

The real reason for Beam (and Apian) is of course to play with other people. First you will have to take care of some settings. So click on `Settings`

### **Settings**

There are a number of values which must be selected or entered in the Settings panel before being able to connect and play a network game. Because Beam is a test article it is useful to allow the user to select as many different options as are possible, but on the other hand a graphical UI that allowed everything would be completely impractical to use - especially in a demonstration environment. Because of the Beam stores user settings [in a text file](#SettingsFileLocations) to allow users and developers to enter whatever they like, but the Beam.Unity interface only allows users to choose entries that are already in the settings file more some of the values.

**Please:** At _least_ change the Screen Name to something that will uniquely identify you.

<p >
  <img src="./docs/beam-settings.jpg" width="600" title="hover text">
</p>

- P2P Connection. Beam and Apian use the [P2pNet](https://github.com/Apian-Framework/P2pNet) library to manage Apian peer-to-per communications. P2pNet supports several different communications protocols which often make use of public or private message brokers. This dropdown provides access to a number of broker/protocol options (or relay/protocol in the case of libp2p.) In most cases you will only be able to connect to other peers who have made the same selection. _An exception here is that games being played from within a browser who select "<SOME_BROKER> MQTT WS" to select MQTT over WebSockets can communicate with native build peers that select "<SOME_BROKER> MQTT"._

- Blockchain. Beam can connect to any EVM-based chain. Selections for Gnosis and Ethereum mainnets and testnets are provided by default.

- In-Game Acct. Beam creates and manages a blockchain account for use by the application. This account acts on its own, without necessarily informing the player, and is intended to serve as a proxy to the more traditionally-managed "permanent account" listed below. Currently the in-game account is just used among the peers as a unique identifier and to sign data originating from the local peer, though in the future it will submit transactions to the chain. Only one peer with this account address is able to join the p2p network at a time.

- Permanent Acct. This value represents a "traditional" (MetaMask or Ledger managed) account owned by the user and will never actually perform any transactions in the game. It is not currently used, but in the future will be entered by providing an externally-generated and signed attestation saying that the in-game account is authorized to act as a proxy to this one. This attestation will be passed to other peers to they can validate that the player is who they say they are. In addition the proxy account, on submitting a transaction to the blockchain, might also provide the attestation. _Though it is more likely that whatever external tool created the attestation in the first place will also submit it to the relevant contract at that point in time - so the contract will already know about the proxy relationship._

- Apian Network. This is simply a text name that allows for games or groups of games to use the same network infrastructure (broker or mesh) without being able to see one another.

- Screen name. How you will be seen on the network.

- Enable LogLevel Edit. This developer option places a small button at the bottom-right of the screen which, when pressed, presents a panel that allows the user at runtime to [specify logging verbosity options](#UsingLogFiles) for the different modules in the app.

### **Connecting**

Click on `Connect`. It will take a few seconds, while the network is joined and the app listens for peers and existing game sessions, before the `Create/Join` button become active.

<p >
  <img src="./docs/beam-connect.jpg" width="600" title="hover text">
</p>

Assuming everything goes well and you see a game you would like to join, or if you'd like to create a new game session, then press `Create/Join Game`

### **Create/Join**

<p >
  <img src="./docs/beam-join.jpg" width="600" title="hover text">
</p>

To join a pre-existing game select it in the dropdown list and then press `Join`.

To create a new game session, enter a name for it, select an [Apian group type](#ApianGroupTypes), and press `create`.

In either case if you wish to participate in the session as a validator only, and not be able to play, select the `Join as Validator Only` checkbox. _This is really not very interesting at the moment, but in the future there will be an interactive "Validator Cam" that will allow you move around the game  grid and focus on particular vehicles._

> ### Apian Group Types
>Selecting a game session's Apian group type determines which consensus algorithm used by its peers. Because Beam is a free-running simulator-style application, and is therefore very sensitive to message propagation time, there are only 2 available options, both of which involve a single peer selecting and ordering the requests to change the game state. Leader-based consensus algorithms like these are common, and typically manage to be "decentralized" by having different peers take on the role over the course of time. The 2 types available for Beam are:
> - `CreatorSez` - The peer that creates the game session is the leader. Always. This is the absolute simplest algorithm possible, and can sort-of be thought of as `client/server`, but that's not really the case. In a true client/server system a particular client has no visibility into the internals of the server, nor does it know anything about the communications between the server and the other clients. In `CreatorSez` neither of these are true: each client sees all of the requests made by other clients, and is running the exact same AppCore that the leader is running. If the leader were to distribute an illegal command or even one that wasn't requested by one of the clients _every_ client would know it and complain. `CreatorSez` is useful if you have are using a headless permanent, validator-only peer to create a game session that won't end even if all of the players leave.
> - `LeaderSez` - This is also leader-based, but each leader only reigns for a single fixed-duration `epoch` before giving way to the next leader. The leader selection and failover mechanisms for `LeaderSez` are pretty much the simplest-thing-that-can-possibly work and certainly not production-ready. In the works is an adaptation of the popular RAFT algorithm, which is both elegant and anti-fragile, though a fully-faithful implementation might not be performant enough for a game like Beam.

---
## Settings File Locations





---
## Using Log Files




