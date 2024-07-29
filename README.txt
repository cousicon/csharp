README - Connor Cousineau, Spencer Durrant.

For PS9

In this assignment the goal was to establish a server such that multiple clients can connect and play the game TANK WARS! 
It works with the given client and PS8.
Our server is currently able to handle 50 AI and not crash, but it does become quite slow. 
The more players that join causes a seeable lag. 
This may be caused by certain code not be optimal but for the most part it works. 
Powerup function as intended, though we did find out the hard way that you need to remove it from the world or there is a permanent kill line on the map. 
They also can spawn almost instantly with how we have it programed. It just has a counter that compares to a random value selected every frame.
Before we set up collisions we found out how much you can DDOS the server by just not removing the projectiles. 
We also had a couple issues with the random, but found that we break from the loop before if found a proper spot. 
When coming to the end of the assignment we found that our program was not quitting out. 
The solution to this is to add a flag that tells the thread to stop. 
With SQL neither of us knew enough about it to jump in, but with some googling and some thinking we were able to get it working to our knowledge. 
On our side we are able to play a game close the server, reopen the server and run the localhost page and see tables. 
We've chosen to do three tables. One for the player and game id, another for Id and time, and the last one for the player stats. 
All collisions detect if the point is within the given point values and if it is then it increments the proper values. 
The settings can be changed in the XML and will affect the world. They are read using an XML reader, which will throw if it's not in the proper format.
We also added a NAN check that checks in the number provided is not a number and returns true or false in the Vector2D class.



For PS8

This is a The Tank Wars assignment for CS 3500 at the University of Utah.
The Tank Wars game is a working game client that connects to a server. This game can
be played in multiplayer. The solution of this program follows MVC design following 
the software practice of seperation of concerns.

The following presents the version changes of the program.

Current bug: rarely fails to connect and will not repaint or recieve data. 

Version 1.0: November 14, 2019

Added all the JSON and added some basic controller functionallity. 
No way to test if anything works as there is no working view.

Version 1.1 November 15, 2019

Got a basic form and drawning panel working to help debug issues with basic code -> basically all of it.

Version 1.2 November 18, 2019

Worked more on form to get it to launch with the drawning panel and buttons added.
Connect button functions although not all the time. 

Version 1.3 November 20, 2019

All the offical coding for the Controller is "finished".
Notable drawing issues with just about everything. 
Transforming appears to be the major issue. 

Version 1.4 November 21, 2019

Talked with the TA and found out how to fix the drawning panel. 
Tank is centered and the movements of the tank can be drawn everytime a command is sent to the server. 
Powerups collected will cause the world to not draw the walls.
Walls sometimes will not be drawn at all.
Program does not appear to work at all.

Version 1.5 November 22, 2019

Fixed the world, Everything draws correctly to our knowledge.
Firing breaks the world is the rate of fire is to high... don't know how to fix this. 
Everything works the last thing to do is add some polish to the game before completion.

Version 1.6 November 23, 2019 - Overtime

Fixed issue with walls and now collecting powerups does not cause walls to disappear. 
When dying a tombstone is drawn in its place.
There is a functioning nameplate and health bar.
Tank will be assigned different color shots and tank color upon logging in due to the server not reseting its connected player count.
Server was being sent "null" at the start of the game which was causing some players to expierience sadness.
This was fixed.