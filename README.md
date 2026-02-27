# coloredcodecitycreator
Code metrics visualization tool

ColoredCodeCityCreator visualizes a code base to give a graphical overview of structure and problem areas.

This tool creates a 3D city from your code base. Streets are generated from namespaces. Each building represents a class.
Building size represents number of lines of code.
Height represents number of functions.
Color is a composite of dependencies / efferent coupling (R), dependants / afferent coupling (G), and complexity (B)

Currently supports Java, ActionScript, C#.

Instructions:
- Download assets and open MainScene in Unity and run from editor.

    or
  
- Download the Windows standalone and run ColoredCodeCityCreator and run it:
https://drive.google.com/file/d/1WYwZhaEuhdowlbWHdSO5r7pdJiezsWsQ/view?usp=sharing

Copy the path of your source code root directory.
Run ColoredCodeCityCreator
Paste the source path
Click on buildings to view class name, path, and metrics
