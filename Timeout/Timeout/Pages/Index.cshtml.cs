using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Timeout.Models;
using Newtonsoft.Json;

namespace Timeout.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationContext _context;
        public List<Counter> Counter { get; set; } = new List<Counter>();
        public Counter soloCounter { get; set; } = new Counter() { counter = 0 };
        [BindProperty]
        public Counter CounterEntity { get; set; } = new Counter();
        [BindProperty]
        public Timer timer { get; set; } = new Timer();
        public int tempCounter { get; set; } = 0;
        public int timeout { get; set; } = 0;
        public static bool inProcess { get; set; }

        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger, ApplicationContext db)
        {
            _logger = logger;
            _context = db;
        }

        public async Task<IActionResult> OnPostHandleStart()
        {
            if(inProcess)
            {
                return Page();
            }
            inProcess = true;
            Random rnd = new Random();
            int value;
            if (ModelState.IsValid)
            {
                while(inProcess)
                {
                    await Task.Delay(timer.timer);
                    value = rnd.Next(1, 15);
                    this.tempCounter += value;
                }
                if (!_context.Counters.Any())
                {
                    _context.Counters.Add(CounterEntity);
                    await _context.SaveChangesAsync();
                }
                var counter = _context.Counters.FirstOrDefault();
                counter.counter += tempCounter;
                await _context.SaveChangesAsync();

                return new RedirectToPageResult("Index");
            }
            return Page();
        }
        public IActionResult OnPostHandleStop()
        {
            inProcess = false;
            return new RedirectToPageResult("Index");
        }

        public IActionResult OnPostHandleUpdate()
        {
            return new RedirectToPageResult("Index");
        }

        public async Task OnGet()
        {
            Counter = await _context.Counters?.ToListAsync();
            soloCounter = await _context.Counters?.FirstOrDefaultAsync();
        }
    }
}
