using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoAPI.Models;
using Microsoft.AspNetCore.Cors;

namespace ToDoAPI.Controllers
{
    [EnableCors]
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoesController : ControllerBase
    {
        private readonly ToDoContext _context;

        public ToDoesController(ToDoContext context)
        {
            _context = context;
        }

        // GET: api/ToDoes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ToDo>>> GetToDos()
        
        {
            if (_context.ToDos == null)
            {
                return NotFound();
            }

            var todo = await _context.ToDos.Include("Category").Select(x => new ToDo()
            {
                ToDoId = x.ToDoId,
                Name = x.Name,
                Done = x.Done,
                CategoryId = x.CategoryId,
                Category = x.Category != null ? new Category()
                {
                    CategoryId = x.Category.CategoryId,
                    CatName = x.Category.CatName,
                    CatDesc = x.Category.CatDesc
                } : null
            }).ToListAsync();

            return Ok(todo);
        }

        // GET: api/ToDo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ToDo>> GetTodo(int id)
        {

            if (_context.ToDos == null)
            {
                return NotFound();
            }

            var todo = await _context.ToDos.Where(x => x.ToDoId == id).Select(x => new ToDo()
            {
                ToDoId = x.ToDoId,
                Name = x.Name,
                Done = x.Done,
                CategoryId = x.CategoryId,
                Category = x.Category != null ? new Category()
                {
                    CategoryId = x.Category.CategoryId,
                    CatName = x.Category.CatName,
                    CatDesc = x.Category.CatDesc
                } : null
            }).FirstOrDefaultAsync();

            if (todo == null)
            {
                return NotFound();
            }

            return todo;
        }

        // PUT: api/ToDoes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutToDo(int id, ToDo todo)
        {
            if (id != todo.ToDoId)
            {
                return BadRequest();
            }

            _context.Entry(todo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ToDoExists(id))
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

        // POST: api/ToDos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ToDo>> PostToDo(ToDo todo)
        {
            if (_context.ToDos == null)
            {
                return Problem("Entity set 'ToDosContext.ToDo' is null.");
            }
            ToDo newToDo = new ToDo()
            {
                Name = todo.Name,
                Done = todo.Done,
                CategoryId = todo.CategoryId
            };

            _context.ToDos.Add(newToDo);
            await _context.SaveChangesAsync();

            return Ok(newToDo);
        }

        // DELETE: api/ToDoes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteToDo(int id)
        {
            var toDo = await _context.ToDos.FindAsync(id);
            if (toDo == null)
            {
                return NotFound();
            }

            _context.ToDos.Remove(toDo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ToDoExists(int id)
        {
            return _context.ToDos.Any(e => e.ToDoId == id);
        }
    }
}
