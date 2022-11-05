using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sudoku.Model
{
    public class Statistics
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        [Required]
        public string Name { get; set; }
        public int Mistakes { get; set; }
        public string Level { get; set; }
        public int UnsolvedCells { get; set; }
        public string SolvedSudoku { get; set; }
        public string PuzzleSudoku { get; set; }
    }
}
