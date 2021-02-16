# HorrorShooter
A dark (literally) FPS.
The game makes use of the new Animation Rigging of Unity. I learned some new ways to implement a state machine in order to handle the enemy AI more cleanly. 
I always thought games that make good use of the dark have a lasting impressivion such as Dragon's Dogma and Kingdom Come: Deliverance. The night/darkness had a weight to them and I attempted to recreate this feeling.

The player plays in small arena with endless supply of ghouls, walking around in the dark. You have a flashlight and flares to light your way through the darkness.

<img src="/ReadmeContent/Gameplay_Gif.gif" width="600" height="300"/>

The monster will track the player within a set range and angle. A deliberate turning on spotting the player and a snappy movement when they lost interest.

<img src="/ReadmeContent/LookAtPlayer_Gif.gif" width="600" height="300"/>

If they've had the player in their vision for x amount of time, they'll give chase and attack.
