using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/todo")]
    [Produces("application/json")]
    public class TodoController : Controller
    {
        private const string GetTodoRouteName = "GetTodo";

        private readonly TodoContext _context;

        public TodoController(TodoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<TodoItem> GetAll()
        {
            return _context.TodoItems.ToList();
        }

        /// <response code="404">If the item does not exist</response>
        /// <response code="200">Returns the requested item</response>
        [HttpGet("{id}", Name = GetTodoRouteName)]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(TodoItem), 200)]
        public IActionResult GetById(long id)
        {
            var item = _context.TodoItems.SingleOrDefault(i => i.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        /// <response code="400">If the request body is malformed</response>
        /// <response code="201">Returns the created item</response>
        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(TodoItem), 201)]
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

        /// <response code="400">If the request body is malformed</response>
        /// <response code="404">If the item does not exist</response>
        /// <response code="204">If the item is updated</response>
        [HttpPut("{id}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(204)]
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

        /// <response code="404">If the item does not exist</response>
        /// <response code="204">If the item is deleted</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(404)]
        [ProducesResponseType(204)]
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
