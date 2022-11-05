using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sudoku.Model;

namespace Sudoku.Data
{
    public class StaticsContext : DbContext
    {
        public StaticsContext (DbContextOptions<StaticsContext> options)
            : base(options)
        {
        }

        public DbSet<Sudoku.Model.Statistics> Statistics { get; set; }
    }
}
