# minesweeper

- This is an unity project (2017.3 or even older versions should work)
- I used UniRx and Zenject plugins.
- This C# code is using reactive programming, dependency injection and testable. in Unity editor, open "Windows/Test runner".
- Game Scene is located at App/Scene/Game.unity

This repository implements a function that will generate a field for the minesweeper game. The function name should be GenerateMineField and it should take field width and height, and a number of mines on the field.

The produced minefield should use the following enumeration to indicate minefield cell state
  ... GenerateMineField(uint width, uint height, uint count, ...);
        public enum Cell { EMPTY,
  M1,
  M2,
  M3,
  M4,
  M5,
  M6,
  M7,
  M8,
  M9,
  MINE,
};

Also this repository also mplements minesweeper game solver (algorithm that opens minesweeper game field).
Please Note, it is possible that solver may fail to open the whole field.

Updates:
* Added an EventSystem object to GameScene 
* More optimized for system using reactive programming (UniRx)"

Watch those videos on Youtube for explaining how this works.

Introduce the game (open project, run the game, etc)
https://youtu.be/Arve8BjagZQ

part 2: more about game logics, inversion of controls and reactive programming.
https://youtu.be/NZW1xmH5-6g

The game is separated by data layer, businesses and the UI layer, 

[data]
https://github.com/game-libgdx-unity/Universal-Resolver/blob/source-only/Assets/DependencyResolver/SampleGame/Scripts/Cells/CellData.cs
the CellData.cs only contains data, no business included, also I use reactive property from UniRx then linked them to UI layer, so when the business level doing its job, the data will be modified, then the UI should be auto-updated by the data changes.

[UI / View]
This is the view of the CellData, View should be a mono-behaviour to represent the cell status, then It updates the UI visually (Text, color, background, etc) to the Users. Also no business included in UI layer.
UI layer can also fire events which will make business objects react.
In case high optimization needed, i would prefer low-level apis to render the object without using GameObject-Components of unity. 

[Business]
https://github.com/game-libgdx-unity/Universal-Resolver/blob/source-only/Assets/DependencyResolver/SampleGame/Scripts/Boards/GameBoard.cs
[GameBoard] is response create the field of cells with method Build(), modify the cell state with OpenCell() and then avoid hitting bombs at the firstMove, the FirstMove() is used to open any first cell with no bombs.

https://github.com/game-libgdx-unity/Universal-Resolver/blob/source-only/Assets/DependencyResolver/SampleGame/Scripts/Boards/GameSolver.cs
Also I had [GameSolver] which will run algorithms to marks a cell has bombs base all the current status of opened cells. If it can't give a decision then a random cell will be opened and the gameSolver may fail to resolve the game.

