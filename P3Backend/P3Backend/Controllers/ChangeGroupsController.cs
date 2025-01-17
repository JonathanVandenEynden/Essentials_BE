﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using P3Backend.Model;
using P3Backend.Model.RepoInterfaces;
using P3Backend.Model.Users;
using System;
using System.Collections.Generic;

namespace P3Backend.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ChangeGroupsController : ControllerBase {

        private readonly IOrganizationRepository _organizationRepository;
        private readonly IChangeInitiativeRepository _changeInitiativeRepo;
        private readonly IChangeGroupRepository _changeGroupRepo;
        private readonly IUserRepository _userRepo;


        public ChangeGroupsController(
            IOrganizationRepository organizationRepository,
            IChangeInitiativeRepository changeInitiativeRepo,
            IChangeGroupRepository changeGroupRepo,
            IUserRepository userRepo) {
            _organizationRepository = organizationRepository;
            _changeInitiativeRepo = changeInitiativeRepo;
            _changeGroupRepo = changeGroupRepo;
            _userRepo = userRepo;
        }

        /// <summary>
        /// Get all changegroups for the logged in user
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Policy = "EmployeeAccess")]
        public ActionResult<List<ChangeGroup>> GetChangeGroupForUser() {
            try {
                Employee loggedInUser = (Employee)_userRepo.GetByEmail(User.Identity.Name);

                return _changeGroupRepo.GetForUserId(loggedInUser.Id);
            } catch {
                return BadRequest("User not logged in or was an admin");
            }
        }

        /// <summary>
        /// Get all changegroups in an organization
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        [Route("[action]/{organizationId}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Policy = "ChangeManagerAccess")]
        public ActionResult<IList<ChangeGroup>> GetAllGhangeGroupsOfOrganization(int organizationId = 1) {

            try {
                Organization o = _organizationRepository.GetBy(organizationId);
                if (o == null) {
                    return NotFound("Organization does not exist");
                }
                List<ChangeInitiative> allCI = new List<ChangeInitiative>();

                o.ChangeManagers.ForEach(cm => {
                    allCI.AddRange(cm.CreatedChangeInitiatives);
                });
                List<ChangeGroup> allGroups = new List<ChangeGroup>();

                allCI.ForEach(ci => {
                    ChangeInitiative currentCi = _changeInitiativeRepo.GetBy(ci.Id);
                    allGroups.Add(currentCi.ChangeGroup);

                });

                return allGroups;
            } catch (Exception e) {
                return BadRequest(e.Message);
            }
        }


    }
}
