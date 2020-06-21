GETTING STARTED
===============

1) You can instantiate a Sine Wave object from either the GameObject menu or the Create menu in the Unity Editor, under 2D Object.

2) Upon instantiation, you will be able to scale and move position the Sine Wave object as you see fit.

3) Press play to watch the sine wave move!

CONFIGURATION
=============

The Sine Wave script attached to the Sine Wave object has a number of adjustable sliders.

Amplitude
---
0 to 1 
This controls the amplitude of the sine wave.

Ordinary Frequency
---
-5 to +5
This controls the speed the sine waves appears to move, either left or right. A negative value will cause the sine wave to move to the right, while a positive value will cause it to move left.

Step
---
1 to 100 
This controls how many individual waves appear.

Polygons 
---
10 to 200
This controls how many individual squares the mesh is made up of, and will affect the quality of the mesh as well as how many waves appear.

Height 
---
-0.5 to 0.5
This is added to the Y value of each vertex of the mesh, affecting how high up in the frame the sine wave will appear.

Update Time
---
0.01 to 0.1
This is the value in seconds between every iteration of the sine wave, i.e. every time the sine wave mesh is updated. A value of 0.01 will appear very smooth, while a value of 0.1 will appear slightly jerky. It is advised that this be kept at 0.01.