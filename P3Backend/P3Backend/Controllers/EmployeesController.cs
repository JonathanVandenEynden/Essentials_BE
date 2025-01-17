﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using P3Backend.Model;
using P3Backend.Model.DTO_s;
using P3Backend.Model.RepoInterfaces;
using P3Backend.Model.Users;
using System;
using System.Collections.Generic;
using System.Linq;

namespace P3Backend.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EmployeesController : ControllerBase {
        private readonly IEmployeeRepository _employeeRepo;
        private readonly IOrganizationRepository _organizationRepo;

        public EmployeesController(IEmployeeRepository employeeRepository,
            IOrganizationRepository organizationRepo) {
            _employeeRepo = employeeRepository;
            _organizationRepo = organizationRepo;
        }

        /// <summary>
        /// Get all employees of an organization
        /// </summary>
        /// <returns></returns>
        [Route("[action]")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Policy = "EmployeeAccess")]
        public ActionResult<IEnumerable<Employee>> GetAllEmployeesFromOrganization() {
            try {
                Employee loggedInEmployee = _employeeRepo.GetByEmail(User.Identity.Name);

                Organization o = _organizationRepo.GetAll().FirstOrDefault(o => o.ChangeManagers.Any(cm => cm.Id == loggedInEmployee.Id) || o.Employees.Any(e => e.Id == loggedInEmployee.Id));

                return o.Employees;
            } catch {
                return NotFound("Organization not found");
            }
        }
        /// <summary>
        /// Get employee by a given Id
        /// </summary>
        /// <param name="employeeId">the id of the employee</param>
        /// <returns>An employee</returns>
        [HttpGet("{employeeId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Policy = "EmployeeAccess")]
        public ActionResult<Employee> GetEmployeeById(int employeeId) {
            Employee e = _employeeRepo.GetBy(employeeId);

            if (e == null) {
                return NotFound("Employee not found");
            }

            return e;
        }

        /// <summary>
        /// Get employee by a given email
        /// </summary>
        /// <param name="email">the email of the employee</param>
        /// <returns>employee obj</returns>
        [HttpGet("[action]/{email}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Policy = "EmployeeAccess")]
        public ActionResult<Employee> GetEmployeeByEmail(string email) {
            Employee e = _employeeRepo.GetByEmail(email);

            if (e == null) {
                return NotFound("Employee not found");
            }

            return e;
        }

        /// <summary>
        /// Create a new employee
        /// </summary>
        /// <param name="organizationId">id of the organization</param>
        /// <param name="dto">Information of the employee</param>
        /// <returns></returns>
        [HttpPost("{organizationId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Policy = "ChangeManagerAccess")]
        public IActionResult PostEmployee(int organizationId, EmployeeDTO dto) {
            try {
                Organization o = _organizationRepo.GetBy(organizationId);

                if (o == null) {
                    return NotFound("Organization not found");
                }

                Employee newE = new Employee(dto.FirstName, dto.LastName, dto.Email);

                o.Employees.Add(newE);

                _employeeRepo.SaveChanges();

                return CreatedAtAction(nameof(GetEmployeeById), new { employeeId = newE.Id }, newE);
            } catch (Exception e) {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Delete an employee
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        [HttpDelete("{employeeId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Policy = "ChangeManagerAccess")]
        public IActionResult DeleteEmployee(int employeeId) {
            Employee e = _employeeRepo.GetBy(employeeId);

            if (e == null) {
                return NotFound("Employee not found");
            }

            _employeeRepo.Delete(e);
            _employeeRepo.SaveChanges();

            return NoContent();
        }
    }
}
