# Kompas
Kompas is a card game, played on a board. Players choose one card, their Avatar, to represent themselves, then make a deck of 49 other cards to play with. The first person whose Avatar dies, loses.

# Running this
The short of it is that, for now, you can't. When I set up this repo, I had very little idea what I was doing, and so it doesn't include all that's necessary to run the game (packages and such). What's on here is the important stuff I want to keep saved - the code, jsons, and other such - but it needs to be opened in the Unity editor to do anything.
That's not to say the game doesn't run at all; it technically does. It's just that, while you can move cards around and change numbers manually, nothing happens for you at the moment, so you can't really play a game. That should change soon.

# Installation
I mean, if you really want to.
Create a new Unity project. Clone the repo to that project's Assets folder.
In that same project, make sure you have the 2D Sprite, Burst, Mathematics, Test Framework, and TextMes Pro packages.
Also, download the Unity beta transport API from https://github.com/Unity-Technologies/multiplayer/tree/master/com.unity.transport and add that package manually to the Packages folder. Update the Unity project's manifest.json to include the com.unity.transport package.
From there, you should be able to build the Unity project. Make one build with the Server Scene checked, and one with the Client scene checked. (Sigh, because I haven't yet made a home screen that can open client or server.)
And voila! You should be able to host a game and join that game on localhost, because I haven't yet made sure the client takes in an IP because I'm not at that point in the project yet. Sorry.
