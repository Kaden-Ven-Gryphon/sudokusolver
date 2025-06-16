using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuSolver
{
    internal class Solver : SudokuBoard
    {
        public Solver() { }

        // Looks at every cell, and for each one if it has only a single pencil mark, it sets that cells
        // Value to that mark.
        public bool FillSinglePencilMarksAsValue()
        {
            bool changesMade = false;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    var pencilMarks = GetCellPencil(i, j);
                    if (pencilMarks != null && pencilMarks.Count == 1)
                    {
                        changesMade = true;
                        SetCellValue(i, j, pencilMarks[0]);
                    }
                }
            }
            return changesMade;
        }

        // Check doubles for a row, and removes marks accordingly.
        // Only resolves the first double pair found
        // Returns true is changes made
        public bool ResolveDoublesForRow(int row)
        {
            bool changesMade = false;
            
            // Look for any cells with two pencil marks
            for (int i = 0;i < 9; i++)
            {
                var pencilList = GetCellPencil(i,row);
                if (pencilList.Count != 2) { continue; }

                // Do any of the pairs match
                for (int j = i+1; j < 9; j++)
                {
                    var comparedList = GetCellPencil(j, row);
                    if (comparedList.Count == 2 && comparedList.All(pencilList.Contains))
                    {
                        // for each maching pair, get their cell number, and pencil values
                        for (int k = 0; k < 9; k++)
                        {
                            // make sure not to clear the pencil mark from the pair that causes the elemination
                            if (k != j && k != i)
                            {
                                if (RemovePencilMarkFromCell(k, row, pencilList[0])) { changesMade = true; }
                                if (RemovePencilMarkFromCell(k, row, pencilList[1])) { changesMade = true; }
                            }
                        }
                        if (changesMade) { return changesMade; }
                    }
                }
            }
            return changesMade;
        }

        // Check doubles for a row, and removes marks accordingly.
        // Only resolves the first double pair found
        // Returns true is changes made
        public bool ResolveDoublesForColm(int colm)
        {
            bool changesMade = false;

            // Look for any cells with two pencil marks
            for (int i = 0; i < 9; i++)
            {
                var pencilList = GetCellPencil(colm, i);
                if (pencilList.Count != 2) { continue; }

                // Do any of the pairs match
                for (int j = i + 1; j < 9; j++)
                {
                    var comparedList = GetCellPencil(colm, j);
                    if (comparedList.Count == 2 && comparedList.All(pencilList.Contains))
                    {
                        // for each maching pair, get their cell number, and pencil values
                        for (int k = 0; k < 9; k++)
                        {
                            // make sure not to clear the pencil mark from the pair that causes the elemination
                            if (k != j && k != i)
                            {
                                if (RemovePencilMarkFromCell(colm, k, pencilList[0])) { changesMade = true; }
                                if (RemovePencilMarkFromCell(colm, k, pencilList[1])) { changesMade = true; }
                            }
                        }
                        if (changesMade) { return changesMade; }
                    }
                }
            }
            return changesMade;
        }

        // Check for basic double pairs in boxes
        // returns true if changes where made to pencil marks
        public bool ResolveDoublesForBox(int box)
        {
            bool changesMade = false;

            // Looping through the first list to compare from
            for (int i = 0; i < 3; i++)
            {
                for (int j =0; j < 3; j++)
                {
                    // Get first comparitor
                    var (col, row) = GetCordFromBoxAndLocalCord(i, j, box);
                    var pencilList = GetCellPencil(col, row);
                    if (pencilList.Count != 2 ) { continue; }

                    for (int k = 0; k < 3; k++)
                    {
                        for (int l = 0; l < 3; l++)
                        {
                            // Do not compare the same cell aginst its self
                            if ( i == k && j == l ) { continue; }

                            var (colComp, rowComp) = GetCordFromBoxAndLocalCord(k, l, box);
                            var comparedList = GetCellPencil(colComp, rowComp);
                            if (comparedList.Count == 2 && comparedList.All(pencilList.Contains))
                            {
                                // Remove the paired pencil marks from other cells in box
                                for (int m = 0; m < 3; m++)
                                {
                                    for (int n = 0; n < 3; n++)
                                    {
                                        // makesure not to remove marks for original double
                                        if (!((m == i && n == j) || (m == k && n == l)))
                                        {
                                            var (colRemove, rowRemove) = GetCordFromBoxAndLocalCord(m, n, box);
                                            if (RemovePencilMarkFromCell(colRemove, rowRemove, pencilList[0])) { changesMade = true; }
                                            if (RemovePencilMarkFromCell(colRemove, rowRemove, pencilList[1])) { changesMade = true; }
                                        }
                                    }
                                }
                                if (changesMade) { return changesMade; }
                            }
                        }
                    }// loop for second comparator
                }
            }// loop for first comparator

            return changesMade;
        }

        // Check for basic double pairs in rows, colm, and boxes
        // returns true if changes where made to pencil marks
        public bool ResolveDoublesForBoard()
        {
            bool changesMade = false;
            for(int i = 0; i < 9; i++)
            {
                if (ResolveDoublesForRow(i)) { changesMade = true; }
                if (ResolveDoublesForColm(i)) { changesMade = true; }
                if (ResolveDoublesForBox(i)) { changesMade = true; }
            }

            return changesMade;
        }


        public void Solve()
        {
            Console.WriteLine("Starting solver.");

            Console.WriteLine("Fill Initial Pencil Marks");
            FillBoardWithPencilMarks();
            CleanPencilMarksFromBoard();

            Console.WriteLine("Fill single pencil mark cells");
            while (FillSinglePencilMarksAsValue()) { continue; }

            bool changesMade = false;
            do
            {
                changesMade = false;
                int changeCount = 0;

                Console.WriteLine("Handleing Simple Doubles");
                while (ResolveDoublesForBoard()) { changeCount++; }

                Console.WriteLine("Fill single pencil mark cells");
                while (FillSinglePencilMarksAsValue()) { changeCount++; }

                if (changeCount > 0) { changesMade = true; }

            } while (changesMade);
        }
    }
}
