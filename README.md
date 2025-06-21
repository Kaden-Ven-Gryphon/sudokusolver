# SudokuSolver

This is a project to get me familiar with C# and WPF (mostly the WPF part).

It is my attempt to make a sudoku solver.  It applies increasingly more complex stratagies until it gets a solution, or it is unable to make any changes (get stumped).  Right now it just has the trivial stratagies avalible, along with using double pairs to elimitate pencil marks in other cell.  Just these are enough for it to solve many easy puzzles.  I plan to keep feeding it harder and harder puzzles, and each time it gets stuck add a new stratagie that handles the situation it got stuck at.

For the app itself I plan to have basic functionality to load puzzles, add values and pencil marks, run the full solver or run individual stratagies.  Right now the UI side of it is just the basic layout, it does not load any values in from the board (though it does load the list of puzzle found at the given path).

![image](./Resources/UITest1.png)