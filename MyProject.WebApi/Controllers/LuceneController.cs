using Microsoft.AspNetCore.Mvc;
using MyProject.Domain.LuceneEntities;
using MyProject.LuceneDb.Repositories;

namespace MyProject.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LuceneController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;
        public LuceneController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        [HttpPost]
        public IActionResult AddEmployee(Employee employee)
        {
            return Ok(_employeeRepository.AddEmployeeToIndex(employee));
        }

        [HttpGet]
        public IActionResult Search([FromQuery]string employeeName, [FromQuery] bool fuzzy)
        {
            return Ok(_employeeRepository.Search(employeeName, fuzzy));
        }
    }
}
