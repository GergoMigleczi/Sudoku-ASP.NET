using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Sudoku.Model;
using Sudoku.Data;
using System.Text;

namespace Sudoku.Pages
{
    public class IndexModel : PageModel
    {
        private readonly StaticsContext _context;

        public IndexModel(StaticsContext context)
        {
            _context = context;
        }

        //PROPERTIES
        public int[,] SolvedSudoku { get; set; } = new int[9, 9];
        public int[,] Puzzle { get; set; } = new int[9, 9];
        public string Level { get; set; }
        [BindProperty(SupportsGet =true)]
        [Range(1, 9, ErrorMessage = "The number must be between 1 and 9") ]
        public string cell { get; set; }
        [BindProperty(SupportsGet = true)]
        public int row { get; set; }
        [BindProperty(SupportsGet = true)]
        public int col { get; set; }
        public int Mistakes { get; set; }
        public List<Statistics> Stats { get; set; } = new List<Statistics>();
        [BindProperty(SupportsGet = true)]
        [Required]
        public string PlayerName { get; set; }
        public Statistics Sudoku { get; set; }
        //METHODS
        public void OnGet()
        {
            //StreamReader puzzle = new StreamReader("Puzzle.txt");
            //StreamReader solved = new StreamReader("Solved.txt");
            Puzzle = new int[9, 9];
            SolvedSudoku = new int[9, 9];
            Stats = _context.Statistics.ToList();
            int id = 0;
            foreach (var item in Stats)
            {
                id = item.ID;
            }
            Sudoku = _context.Statistics.Find(id);
            if (_context.Statistics.Any())
            {
                PlayerName = Sudoku.Name;

                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        Puzzle[i, j] = int.Parse(Sudoku.PuzzleSudoku[i*9+j].ToString());
                        SolvedSudoku[i, j] = int.Parse(Sudoku.SolvedSudoku[i * 9 + j].ToString());
                    }
                }
            }
            
            /*
            for (int i = 0; i < 9; i++)
            {
                string[] p = puzzle.ReadLine().Split();
                string[] s = solved.ReadLine().Split();
                for (int j = 0; j < 9; j++)
                {
                    Puzzle[i, j] = int.Parse(p[j]);
                    SolvedSudoku[i, j] = int.Parse(s[j]);

                }
            }
            */
            foreach (var item in Stats)
            {
                Mistakes = item.Mistakes;
            }
            if (!string.IsNullOrEmpty(cell))
            {
                
                if (SolvedSudoku[row, col] != int.Parse(cell))
                {
                    Mistakes++;
                    Sudoku.Mistakes = Mistakes;
                    Sudoku.ID = id;
                    _context.Attach(Sudoku).State = EntityState.Modified;
                    _context.SaveChanges();
                }
                else
                {
                    Sudoku.UnsolvedCells--;
                    char[] ch = Sudoku.PuzzleSudoku.ToCharArray();
                    ch[(row * 9) + col] = Convert.ToChar(cell);
                    Sudoku.PuzzleSudoku = new string(ch);
                    Sudoku.ID = id;
                    _context.Attach(Sudoku).State = EntityState.Modified;
                    _context.SaveChanges();
                    
                    Puzzle[row, col] = int.Parse(cell);
                    /*
                    StreamWriter p = new StreamWriter("Puzzle.txt");
                    for (int i = 0; i < 9; i++)
                    {
                        for (int j = 0; j < 9; j++)
                        {
                            if (j == 8)
                            {
                                p.Write(Puzzle[i, j]);
                            }
                            else
                            {
                                p.Write(Puzzle[i, j] + " ");
                            }
                        }
                        p.WriteLine();
                    }
                    p.Flush();
                    p.Close();
                    */
                }
            }
            //puzzle.Close();
            //solved.Close();

        }
        [BindProperty]
        public Statistics stat { get; set; }
        public void OnPost(string Level)
        {
             stat = new Statistics();
            
            SolvedSudoku = new int[9, 9];
            Puzzle = new int[9, 9];

            SolvedSudoku = CreateRandomSudoku();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    sb.Append(SolvedSudoku[i, j].ToString());
                }
            }
            stat.SolvedSudoku = sb.ToString();
            /*
            StreamWriter solved = new StreamWriter("Solved.txt");

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (j == 8)
                    {
                        solved.Write(SolvedSudoku[i, j]);
                    }
                    else
                    {
                        solved.Write(SolvedSudoku[i, j]+" ");
                    }
                    sb.Append(SolvedSudoku[i, j].ToString());
                }
                solved.WriteLine();
            }
            stat.SolvedSudoku = sb.ToString();
            solved.Flush();
            solved.Close();*/

            int unsolvedCells = 0;
            Puzzle = MakePuzzle(SolvedSudoku, Level);
            sb = new StringBuilder();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (Puzzle[i, j] == 0)
                        unsolvedCells++;
                    sb.Append(Puzzle[i, j].ToString());
                }
            }
            stat.PuzzleSudoku = sb.ToString();
            /*
            StreamWriter puzzle = new StreamWriter("Puzzle.txt");
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if(j == 8)
                    {
                        puzzle.Write(Puzzle[i, j]);
                        if (Puzzle[i, j] == 0)
                            unsolvedCells++;
                    }
                    else
                    {
                        puzzle.Write(Puzzle[i, j] + " ");
                        if (Puzzle[i, j] == 0)
                            unsolvedCells++;
                    }
                }
                puzzle.WriteLine();
            }
            puzzle.Flush();
            puzzle.Close();*/
            stat.Date = DateTime.Today;
            stat.Name = PlayerName;
            stat.Level = Level;
            stat.Mistakes = 0;
            stat.UnsolvedCells = unsolvedCells;
            _context.Statistics.Add(stat);
            _context.SaveChanges();

        }
        public bool IsItInRow(int[,] board, int number, int row)
        {
            for (int i = 0; i < 9; i++)
            {
                if (board[row, i] == number)
                    return true;
                
            }
            return false;
        }

        public bool IsItInColumn(int[,] board, int number, int column)
        {
            for (int i = 0; i < 9; i++)
            {
                if (board[i, column] == number)
                    return true;

            }
            return false;
        }

        public bool IsItInBox(int[,] board, int number, int row, int column)
        {
            int BoxRow = row - row % 3;
            int BoxColumn = column - column % 3;

            for (int i = BoxRow; i < BoxRow+3; i++)
            {
                for (int j = BoxColumn; j < BoxColumn+3; j++)
                {
                    if (board[i, j] == number)
                        return true;
                }
            }
            return false;
        }

        public bool IsValidPlacement(int[,] board, int number, int row, int column)
        {
            return !IsItInRow(board, number, row) && !IsItInColumn(board, number, column) && !IsItInBox(board, number, row, column);
        }

        public bool SolveSudoku(int[,] board)
        {
            for (int row = 0; row < 9; row++)
            {
                for (int column = 0; column < 9; column++)
                {
                    if(board[row,column] == 0)
                    {
                        for (int number = 1; number < 10; number++)
                        {
                            if(IsValidPlacement(board, number, row, column))
                            {
                                board[row, column] = number;

                                if (SolveSudoku(board))
                                {
                                    return true;
                                }
                                else
                                {
                                    board[row, column] = 0;
                                }
                            }
                        }
                        return false;
                    }
                }
            }
            return true;
        }

        public static int[,] CreateRandomSudoku() 
        {
            int[,] board = new int[9, 9];
            List<int>[,] pencil = new List<int>[9, 9];
            int[] numbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            Random rd = new Random();
            numbers = numbers.OrderBy(x => rd.Next()).ToArray();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    board[i, j] = 0;
                    pencil[i, j] = new List<int>();
                    pencil[i, j].Add(0);
                }
            }

            board[4, 4] = numbers[0];

            for (int i = 0; i < 9; i++)
            {
                pencil[i, 4].Add(numbers[0]);
                pencil[4, i].Add(numbers[0]);
            }
            int startColBox = 5 - 5 % 3;
            int startRowBox = 5 - 5 % 3;
            for (int row = startRowBox; row < startRowBox + 3; row++)
            {
                for (int col = startColBox; col < startColBox + 3; col++)
                {
                    if (!pencil[row, col].Contains(numbers[0]))
                    {
                        pencil[row, col].Add(numbers[0]);
                    }

                }
            }
            pencil[4, 4].Clear();


            bool IsNotReady = true;
            int count = 0;
            do
            {
                IsNotReady = false;
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        count = 0;
                        for (int pencilRow = 0; pencilRow < 9; pencilRow++)
                        {
                            for (int pencilCol = 0; pencilCol < 9; pencilCol++)
                            {
                                if (pencil[pencilRow, pencilCol].Count > count)
                                {
                                    count = pencil[pencilRow, pencilCol].Count;
                                }
                            }
                        }

                        for (int num = 0; num < 9; num++)
                        {
                            if (!pencil[i, j].Contains(numbers[num]) && pencil[i, j].Count == count && board[i, j] == 0)
                            {
                                board[i, j] = numbers[num];

                                for (int a = 0; a < 9; a++)
                                {
                                    if (!pencil[a, j].Contains(numbers[num]) && board[a, j] == 0)
                                        pencil[a, j].Add(numbers[num]);
                                    if (!pencil[i, a].Contains(numbers[num]) && board[i, a] == 0)
                                        pencil[i, a].Add(numbers[num]);
                                }
                                startColBox = j - j % 3;
                                startRowBox = i - i % 3;
                                for (int row = startRowBox; row < startRowBox + 3; row++)
                                {
                                    for (int col = startColBox; col < startColBox + 3; col++)
                                    {
                                        if (!pencil[row, col].Contains(numbers[num]) && board[row, col] == 0)
                                            pencil[row, col].Add(numbers[num]);
                                    }
                                }

                                pencil[i, j].Clear();
                            }
                        }
                    }
                }

                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        if (board[i, j] == 0)
                        {
                            IsNotReady = true;
                        }
                    }
                }
                /*
                Console.WriteLine(IsNotReady);
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        Console.Write(board[i, j] + " ");
                    }
                    Console.WriteLine();
                }*/

            } while (IsNotReady);

            /* for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Console.Write(board[i, j] + " ");
                }
                Console.WriteLine();
            }*/

            /*Console.WriteLine();
            Console.WriteLine();
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {

                        Console.Write(pencil[i, j][pencil[i, j].Count]);

                    Console.Write(" ");
                }
                Console.WriteLine();
            }*/
            return board;
        }

        public bool IsSudokuValid(int[,] board)
        {
            for (int row = 0; row < 9; row++)
            {
                for (int c = 0; c < 9; c++)
                {
                    if (IsItInRow(board, board[row, c], row))
                        return false;
                    if (IsItInColumn(board, board[row, c], c))
                        return false;
                    if (IsItInBox(board, board[row, c], row, c))
                        return false;
                }
            }
            return true;
        }

        public static int[,] MakePuzzle(int[,] board, string level)
        {
            int[,] puzzle = new int[9, 9];
            puzzle = board;
            Random rd = new Random();
            int removed;
            int row;
            int col;
            switch (level.ToLower())
            {
                case "easy": //42
                    removed = 0;
                    while (removed < 42)
                    {
                        row = rd.Next(0, 9);
                        col = rd.Next(0, 9);
                        if (puzzle[row, col] != 0)
                        {
                            puzzle[row, col] = 0;
                            removed++;
                        }
                    }
                    break;
                case "medium": //46
                    removed = 0;
                    while (removed < 46)
                    {
                        row = rd.Next(0, 9);
                        col = rd.Next(0, 9);
                        if (puzzle[row, col] != 0)
                        {
                            puzzle[row, col] = 0;
                            removed++;
                        }
                    }
                    break;
                case "hard": //50
                    removed = 0;
                    while (removed < 50)
                    {
                        row = rd.Next(0, 9);
                        col = rd.Next(0, 9);
                        if (puzzle[row, col] != 0)
                        {
                            puzzle[row, col] = 0;
                            removed++;
                        }
                    }
                    break;
                case "insane":
                    removed = 0;
                    while (removed < 58)
                    {
                        row = rd.Next(0, 9);
                        col = rd.Next(0, 9);
                        if (puzzle[row, col] != 0)
                        {
                            puzzle[row, col] = 0;
                            removed++;
                        }
                    }
                    break;

            }
            return puzzle;
        }
    }

    

}
