using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Xps.Serialization;

namespace SudokuSolver
{
    internal class SudokuBoard
    {
        private int[] _board;
        private int[] _given;
        private List<int>[] _pencilMarks;

        public SudokuBoard()
        {
            _board = new int[81];
            _given = new int[81];
            _pencilMarks = new List<int>[81];
            for (int i = 0; i < 81; i++)
            {
                _pencilMarks[i] = new List<int>();
            }
        }

        private void makeGiven()
        {
            for (int i = 0; i < 81; i++)
            {
                _given[i] = _board[i];
            }
        }

        // Loads board from file into given and board
        public void LoadBoard(string fileName)
        {
            Console.WriteLine("Loading Board");
            try
            {
                StreamReader sr = new StreamReader(fileName);
                var line = sr.ReadLine();
                int i = 0;
                while (line != null && i < 81)
                {
                    Console.WriteLine(line);
                    for (int j = 0; j < line.Length; j++)
                    {
                        _board[i] = line[j] - '0';
                        i++;
                    }
                    line = sr.ReadLine();
                }
                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            makeGiven();
            Console.WriteLine("Loaded Board");
        }

        // Get 1D array index from row and colm
        private int getindex(int x, int y)
        {
            return x + (y * 9);
        }

        // Get cords based on box num and box local row colm cord
        public (int, int) GetCordFromBoxAndLocalCord(int x, int y, int box)
        {
            int row = ((box / 3) * 3) + x;
            int col = ((box % 3) * 3) + y;

            return (col, row);
        }

        // get box number from cords
        public int GetBoxNumFromCord(int x, int y)
        {
            return (x / 3) + ((y/3) * 3);
        }

        //Check if cell is a given value
        public bool CheckIsCellGiven(int x, int y)
        {
            return _given[getindex(x, y)] != 0;
        }

        // Get the cell value from row and colm
        public int GetCellValue(int x, int y)
        {
            return _board[getindex(x, y)];
        }

        // Get list of pencil marks from row and colm
        public List<int> GetCellPencil(int x, int y)
        {
            return _pencilMarks[getindex(x, y)];
        }

        // Check if the cell contains a spcific pencil mark
        public bool CheckDoesCellContainPencilMark(int x, int y, int mark)
        {
            var pencilMarks = GetCellPencil(x, y);
            if (pencilMarks == null) { return false; }
            else { return pencilMarks.Contains(mark); }
        }

        // Check if row has spcific value
        public bool CheckDoesRowContainValue(int row, int value)
        {
            for (int i = 0; i < 9; i++)
            {
                if (GetCellValue(i, row) == value) { return true; }
            }
            return false;
        }

        // Check if colm contains a spcific value
        public bool CheckDoesColmContainValue(int colm, int value)
        {
            for (int i = 0; i < 9; i++)
            {
                if (GetCellValue(colm, i) == value) { return true;  }
            }
            return false;
        }

        // Check if box conatains a spcific value
        public bool CheckDoesBoxContainValue(int box, int value)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (GetCellValue(i + ((box % 3) * 3), j + ((box / 3) * 3)) == value) { return true; }
                }
            }
            return false;
        }

        // check if list is squential, helper function for the Check functions
        private bool isSequential(List<int> l)
        {
            for (int i = 1; i < l.Count; i++)
            {
                if (l[i-1]+1 != l[i]) { return false; }
            }
            return true;
        }

        // Check if row is valid
        public bool CheckIsRowValid(int row)
        {
            List<int> values = new List<int>();

            // get list of all values in row
            for (int i = 0; i < 9; i++)
            {
                values.Add(GetCellValue(i, row));
            }

            values.Sort();

            if (values.Count != 9) { return false; }
            if (values[0] != 1) { return false; }
            return isSequential(values);
        }

        // Check if colm is valid
        public bool CheckIsColmValid(int colm)
        {
            List<int> values = new List<int>();

            // get list of all values in row
            for (int i = 0; i < 9; i++)
            {
                values.Add(GetCellValue(colm, i));
            }

            values.Sort();

            if (values.Count != 9) { return false; }
            if (values[0] != 1) { return false; }
            return isSequential(values);
        }

        // Check if box is valid
        public bool CheckIsBoxValid(int box)
        {
            List<int> values = new List<int>();

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    values.Add(GetCellValue(i + ((box % 3) * 3), j + ((box / 3) * 3)));
                }
            }

            values.Sort();

            if (values.Count != 9) { return false; }
            if (values[0] != 1) { return false; }
            return isSequential(values);
        }

        // Check if board is valid
        public bool CheckIsBoardValid()
        {
            bool isValid = true;
            for (int i = 0; i < 9; i++)
            {
                if (!CheckIsRowValid(i)) { isValid = false; Console.WriteLine("Row {0} invalid", i); break; }
                if (!CheckIsColmValid(i)) { isValid = false; Console.WriteLine("Colm {0} invalid", i); break; }
                if (!CheckIsBoxValid(i)) { isValid = false; Console.WriteLine("Box {0} invalid", i); break; }
            }
            return isValid;
        }

        // Add pencil mark to list for cell
        public bool AddPencilMarkToCell(int x, int y, int mark)
        {
            if (!CheckDoesCellContainPencilMark(x, y, mark) && GetCellValue(x, y) == 0)
            {
                _pencilMarks[getindex(x,y)].Add(mark);
                return true;
            }
            else { return false; }
        }

        // Removes pencil mark from cell
        public bool RemovePencilMarkFromCell(int x, int y, int mark)
        {
            return _pencilMarks[getindex(x, y)].Remove(mark);
        }

        // Clear all pencil marks from cell
        public void ClearPencilMarkFromCell(int x, int y)
        {
            _pencilMarks[getindex(x,y)].Clear();
        }

        // Remove pencil mark from row
        public bool RemovePencilMarkFromRow(int row, int mark)
        {
            bool markExist = true;
            for (int i = 0; i < 9; i++)
            {
                if (RemovePencilMarkFromCell(i, row, mark) == false) { markExist = false; }
            }
            return markExist;
        }

        // Remove pencil mark from colm
        public bool RemovePencilMarkFromColm(int col, int mark)
        {
            bool markExist = true;
            for (int i = 0; i < 9; i++)
            {
                if (RemovePencilMarkFromCell(col, i, mark) == false) { markExist = false; }
            }
            return markExist;
        }

        //Remove pencil mark from box
        public bool RemovePencilMarkFromBox(int box, int mark)
        {
            bool markExist = true;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (RemovePencilMarkFromCell(i + ((box % 3) * 3), j + ((box / 3) * 3), mark) == false) { markExist = false; }
                }
            }
            return markExist;
        }

        // Fill each none filled cell with all pencil marks
        public void FillCellPencilMarks(int x, int y)
        {
            for (int i = 1; i <= 9; i++)
            {
                AddPencilMarkToCell(x, y, i);
            }
        }

        // Fill board with pencil marks
        public void FillBoardWithPencilMarks()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    FillCellPencilMarks(i, j);
                }
            }
        }

        // Clean the unneeded pencil marks from cell
        public void CleanPencilMarksFromCell(int x, int y)
        {
            for (int i = 1; i <= 9; i++)
            {
                if (CheckDoesRowContainValue(y, i)
                    || CheckDoesColmContainValue(x, i)
                    || CheckDoesBoxContainValue(GetBoxNumFromCord(x, y), i))
                {
                    RemovePencilMarkFromCell(x, y, i);
                }
            }
        }

        // Clean board of unneeded pencil marks
        public void CleanPencilMarksFromBoard()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    CleanPencilMarksFromCell(i, j);
                }
            }
        }

        // Set the value of the cell
        public void SetCellValue(int x, int y, int value)
        {
            _board[getindex(x, y)] = value;
            ClearPencilMarkFromCell(x, y);
            RemovePencilMarkFromBox(GetBoxNumFromCord(x, y), value);
            RemovePencilMarkFromColm(x, value);
            RemovePencilMarkFromRow(y, value);
        }

        // Print the current Board state to console
        public void ConsolePrintBoard()
        {
            Console.WriteLine("Print Board");
            Console.WriteLine("-------------");
            for (int i = 0; i < 9; i++)
            {
                Console.Write('|');

                for (int j = 0; j < 9; j++)
                {
                    var cellValue = GetCellValue(j, i);
                    if (cellValue == 0)
                    {
                        Console.Write(' ');
                    }
                    else
                    {
                        Console.Write(cellValue.ToString());
                    }

                    if ((j % 3) - 2 == 0)
                    {
                        Console.Write('|');
                    }

                }

                Console.Write('\n');

                if ((i % 3) - 2 == 0)
                {
                    Console.WriteLine("-------------");
                }
            }
            Console.WriteLine("Printed Board");
        }

        // Print the current pencil marks of the board
        public void ConsolePrintPencilBoard()
        {
            Console.WriteLine("Print Pencil Board");
            Console.WriteLine("###########################################");
            Console.WriteLine("#-------------#-------------#-------------#");
            for (int boxj = 0; boxj < 3; boxj++)
            {
                for (int cellj = 0; cellj < 3; cellj++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        for (int box = 0; box < 3; box++)
                        {
                            Console.Write('#');
                            Console.Write('|');
                            for (int cell = 0; cell < 3; cell++)
                            {
                                for (int i = 1; i <= 3; i++)
                                {
                                    if (CheckDoesCellContainPencilMark(cell + box*3, cellj + boxj*3, (i + j * 3)))
                                    {
                                        Console.Write(i+j*3);
                                    }
                                    else { Console.Write(' '); }
                                }
                                Console.Write('|');
                            }
                        }
                        Console.Write("#\n");
                    }
                    Console.WriteLine("#-------------#-------------#-------------#");
                }
                Console.WriteLine("###########################################");
            }
            
            
            

            Console.WriteLine("");
            Console.WriteLine("Printed Board");
        }
    }
}
