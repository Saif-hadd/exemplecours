using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WebApplication4.Repositories;
using Models;

namespace WebApplication4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;

        // Dependency injection of the IEmployeeRepository service
        public EmployeesController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        // Endpoint to retrieve all employees
        [HttpGet]
        public async Task<ActionResult> GetEmployees()
        {
            try
            {
                var employees = await _employeeRepository.GetEmployees();
                return Ok(employees);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}"); // Log the exception message for debugging
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }

        // Endpoint to retrieve a specific employee by ID
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            try
            {
                var result = await _employeeRepository.GetEmployee(id);

                if (result == null)
                    return NotFound();

                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}"); // Log the exception message for debugging
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }

        // Endpoint to create a new employee
        [HttpPost]
        public async Task<ActionResult<Employee>> CreateEmployee([FromBody] Employee employee)
        {
            try
            {
                if (employee == null)
                    return BadRequest();

                // Check if an employee with the same email already exists
                var existingEmployee = await _employeeRepository.GetEmployeeByEmail(employee.Email);
                if (existingEmployee != null)
                {
                    ModelState.AddModelError("email", "Employee email already in use");
                    return BadRequest(ModelState);
                }

                var createdEmployee = await _employeeRepository.AddEmployee(employee);
                return CreatedAtAction(nameof(GetEmployee), new { id = createdEmployee.EmployeeId }, createdEmployee);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}"); // Log the exception message for debugging
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating new employee record");
            }
        }

        // Endpoint to update an existing employee
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Employee>> UpdateEmployee(int id, [FromBody] Employee employee)
        {
            try
            {
                // Check if the provided ID matches the employee ID
                if (id != employee.EmployeeId)
                    return BadRequest("Employee ID mismatch");

                // Retrieve the employee to update
                var employeeToUpdate = await _employeeRepository.GetEmployee(id);
                if (employeeToUpdate == null)
                    return NotFound($"Employee with Id {id} not found");

                // Update employee details
                var updatedEmployee = await _employeeRepository.UpdateEmployee(employee);
                return Ok(updatedEmployee);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}"); // Log the exception message for debugging
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating data");
            }
        }

        // Endpoint to delete an employee
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Employee>> DeleteEmployee(int id)
        {
            try
            {
                var result = await _employeeRepository.DeleteEmployee(id);
                if (result == null)
                    return NotFound($"Employee with Id {id} not found");

                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}"); // Log the exception message for debugging
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting data");
            }
        }
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Employee>>> Search([FromQuery] string name, [FromQuery] Gender? gender)
        {
            try
            {
                var result = await _employeeRepository.Search(name, gender);
                if (result.Any())
                {
                    return Ok(result);
                }
                return NotFound("No employees found matching the search criteria.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}"); // Log the exception message for debugging
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving data from the database");
            }
        }
    }
}
