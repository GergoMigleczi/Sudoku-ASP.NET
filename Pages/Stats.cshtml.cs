using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sudoku.Data;
using Sudoku.Model;

namespace Sudoku.Pages
{
    public class StatsModel : PageModel
    {
        private readonly StaticsContext _context;

        public StatsModel(StaticsContext context)
        {
            _context = context;
        }

        //PROPERTIES
        public List<Statistics> Stats { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SelectedName { get; set; }
        public SelectList Names { get; set; }
        //METHODS
        public void OnGet()
        {
            /*
            IQueryable<string> statisticsQuery = from c in _context.Statistics
                                                    orderby c.Name
                                                    select c.Name;
            Names = new SelectList(statisticsQuery.ToList());
            */
            var s = _context.Statistics.ToList();
            List<string> N = new List<string>();
            foreach (var item in s)
            {
                if (!N.Contains(item.Name))
                {
                    N.Add(item.Name);
                }
            }
            Names = new SelectList(N);
            
            var stats = from c in _context.Statistics
                            select c;
            if (!string.IsNullOrEmpty(SelectedName))
            {
                stats = stats.Where(n => n.Name == SelectedName);
            }
            Stats = stats.ToList();

        }

        public async Task<IActionResult> OnPostAsync(int id)
        {

            Statistics Stats = await _context.Statistics.FindAsync(id);
            _context.Statistics.Remove(Stats);

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}
