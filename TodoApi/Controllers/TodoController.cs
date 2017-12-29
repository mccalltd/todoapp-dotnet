using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/todo")]
    public class TodoController : Controller
    {
        private const string GetTodoRouteName = "GetTodo";

        private readonly TodoContext _context;

        public TodoController(TodoContext context)
        {
            _context = context;

            if (context.TodoItems.Count() == 0)
            {
                context.TodoItems.Add(new TodoItem { Name = "Stub Me!" });
                context.SaveChanges();
            }
        }

        [HttpGet]
        public IEnumerable<TodoItem> GetAll()
        {
            return _context.TodoItems.ToList();
        }

        [HttpGet("{id}", Name = GetTodoRouteName)]
        public IActionResult GetById(long id)
        {
            var item = _context.TodoItems.SingleOrDefault(i => i.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        [HttpPost]
        public IActionResult Create([FromBody] TodoItem item)
        {
            if (item == null)
            {
                return BadRequest();
            }

            _context.TodoItems.Add(item);
            _context.SaveChanges();

            return CreatedAtRoute(GetTodoRouteName, new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] TodoItem updatedItem)
        {
            if (updatedItem == null || updatedItem.Id != id)
            {
                return BadRequest();
            }

            var item = _context.TodoItems.SingleOrDefault(i => i.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            item.Name = updatedItem.Name;
            item.IsComplete = updatedItem.IsComplete;

            _context.TodoItems.Update(item);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            var item = _context.TodoItems.SingleOrDefault(i => i.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(item);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
