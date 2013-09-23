
# A Cell Shaded Adventure
##COMP30019 Graphics and Interaction - Game Proposal

###Overview

Our proposal is for a 3D exploration based game, with continuous randomly generated terrain and enemies with a unique
method of interaction for fighting. We will employ a cell-shaded art direction to achieve a cartoon look, not 
disimilar to The Legend Of Zelda: Windwaker. Our game will include achievements and online high scores, and aim
to comply with all Windows 8 'Modern UI' guidelines with the intention to put the game in the marketplace and 
make a profit.

###Type of Application

The application will comprise of both a first person shooter and a top down "table-top" view, with the change between the two initiated
by changing the orientation of the device on which it is played. We will employ the accelerometers to provide a virtual reality
like control scheme for the first person view, with the ability to slice and fight. Tilting the tablet such that its screen is facing
up then transforms the viewport to a top-down perspective, allowing for manipulation of objects on the screen through direct
touch interaction.

###Hardware Inputs

We intend to make full use of all available hardware interfaces on modern Windows 8 tablet devices. We intend to use the accelerometer for 
the view transformation, as well as looking around the world in the first person perspective. The touchscreen will be uitlised for 
more advance combat styling when in the top-down view, and for input for basic attacks when in the first person view.

###Visualisation

As explained earlier, we are aiming for a fully realised 3D world with lighting and appropriate models for the terrain.
We will be employing a cell-shaded rendering method to give a cartoonish look, rather than using Phong illumination or any of its ilk.
The camera will switch between a first person and top-down view. The angle of view will change based on the camera, being wider in 
first person and a tighter in the top-down perspective.

The number of enemies (and thus polygons) will not be consistent, as it will vary with the difficulty level. We intend to create
our 3D models for characters, weapons and terrain features in 3D modelling software, such as Blender, before importing them as the 
predefined Mesh type in SharpDX.

###Milestones

Our milestones will be as follows:
	
1. Creation of the models in Blender;
2. Generating a random terrain at a much larger scale than project 1;
3. Implementing the camera controls, for both view styles;
4. Importing the character and terrain models, placing them appropriately in the scene;
5. Creating the main menu and Windows 8 UI system in which we will show our 3D view;
6. Building a collision system for all objects in the world;
7. Implementing character controls and AI for the enemy units;
8. Implementing the combat system for the game;
9. Implementing achievements and highscores; and
10. Submission to COMP30019 and Windows Marketplace.
