using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuClient
{
    internal class PuzzleListEntryWpf
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public PuzzleListEntryWpf(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
