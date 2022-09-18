using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgressTracker.Contexts;
using ProgressTracker.Models;
using ProgressTracker.Tools;

namespace ProgressTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TasksController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Tasks
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<TaskModel>>> GetTaskModels()
        {
            if (_context.TaskModels == null)
            {
                return NotFound();
            }
            return await _context.TaskModels
                .Include(x => x.Dependencies)
                .Include(x => x.Creator)
                .Include(x => x.Target)
                .Include(x => x.Finisher)
                .ToListAsync();
        }

        // GET: api/Tasks/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<TaskModel>> GetTaskModel(int id)
        {
            if (_context.TaskModels == null)
            {
                return NotFound();
            }
            var taskModel = await _context.TaskModels
                .Include(x => x.Dependencies)
                .Include(x => x.Creator)
                .Include(x => x.Target)
                .Include(x => x.Finisher)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (taskModel == null)
            {
                return NotFound();
            }

            return taskModel;
        }

        // PUT: api/Tasks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutTaskModel(int id, TaskModel taskModel)
        {
            if (id != taskModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(taskModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskModelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Tasks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<TaskModel>> PostTaskModel(TaskModel taskModel)
        {
            if (_context.TaskModels == null)
            {
                return Problem("Entity set 'AppDbContext.TaskModels'  is null.");
            }

            var reference = new UserReference() {
                UserId = User.GetLoggedInUserId<string>()
            };
            
            var entry = _context.UserReferences.Add(reference).Entity;
            _context.SaveChanges();

            if (taskModel.Creator == null)
            {
                taskModel.Creator = entry;
            }

            if (taskModel.Target == null)
            {
                taskModel.Target = entry;
            }

            _context.TaskModels.Add(taskModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTaskModel), new { id = taskModel.Id }, taskModel);
        }

        // DELETE: api/Tasks/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTaskModel(int id)
        {
            if (_context.TaskModels == null)
            {
                return NotFound();
            }
            var taskModel = await _context.TaskModels.FindAsync(id);
            if (taskModel == null)
            {
                return NotFound();
            }

            _context.TaskModels.Remove(taskModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TaskModelExists(int id)
        {
            return (_context.TaskModels?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
