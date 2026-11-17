using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagementApi.DTOs;
using StudentManagementApi.Services.Interfaces;

namespace StudentManagementApi.Controllers
{
    [Authorize] // Requires authentication for all endpoints
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _service;

        public StudentsController(IStudentService service) => _service = service;

        [HttpPost]
        public async Task<ActionResult<StudentResponseDto>> CreateStudent([FromBody] CreateStudentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var created = await _service.CreateStudentAsync(dto);
            return CreatedAtAction(nameof(GetStudentById), new { id = created.Id }, created);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentResponseDto>>> GetStudents([FromQuery] string? search)
        {
            var students = await _service.GetAllStudentsAsync(search);
            return Ok(students);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StudentResponseDto>> GetStudentById(int id)
        {
            var student = await _service.GetStudentByIdAsync(id);
            if (student == null) return NotFound();

            return Ok(student);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] UpdateStudentDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _service.UpdateStudentAsync(id, dto);
            if (!result.Succeeded)
            {
                if (result.ErrorType == "NotFound") return NotFound(new { message = result.Message });
                if (result.ErrorType == "Conflict") return Conflict(new { message = result.Message });
                return BadRequest(new { message = result.Message });
            }

            return Ok(new { message = "Student updated successfully." });
        }

        [Authorize(Policy = "AdminOnly")] // Restricted strictly to Admin users
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var deleted = await _service.DeleteStudentAsync(id);
            if (!deleted) return NotFound();

            return NoContent();
        }
    }
}