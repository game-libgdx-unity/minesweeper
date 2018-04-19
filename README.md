# minesweeper

- This is an unity project (2017.3 or even older versions should work)
- I used UniRx and Zenject plugins.
- If you want to run test, in Unity editor, open "Windows/Test runner".
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
