﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using P3Backend.Model;
using P3Backend.Model.Questions;
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
    [Authorize(Policy = "ChangeManagerAccess")]
    public class DashboardController : ControllerBase {
        private readonly IRoadmapItemRepository _roadmapItemRepository;
        private readonly IChangeInitiativeRepository _changeInitiativeRepository;
        private readonly ISurveyRepository _surveyRepository;
        private readonly IChangeManagerRepository _changeManagerRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IUserRepository _userRepository;


        public DashboardController(IRoadmapItemRepository roadMapItemsRepository, IChangeInitiativeRepository changeInitiativeRepository, ISurveyRepository surveyRepository, IChangeManagerRepository changeManagerRepository, IProjectRepository projectRepository, IUserRepository userRepository) {
            _roadmapItemRepository = roadMapItemsRepository;
            _changeInitiativeRepository = changeInitiativeRepository;
            _surveyRepository = surveyRepository;
            _changeManagerRepository = changeManagerRepository;
            _projectRepository = projectRepository;
            _userRepository = userRepository;
        }

        /// <summary>
        ///	get the projects of an organization
        /// </summary>
        /// <returns></returns>
        [Route("[action]")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Policy = "ChangeManagerAccess")]
        public ActionResult<IEnumerable<Project>> GetProjectsChangeManager() {
            try {
                ChangeManager cm = _changeManagerRepository.GetByEmail(User.Identity.Name);
                IEnumerable<Project> projects = _projectRepository.GetByChangeManager(cm);
                if (projects == null) {
                    return NotFound("Project not found");
                }
                return new ActionResult<IEnumerable<Project>>(projects);
            } catch (Exception e) {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Get the change initiatives from a change manager, filters are possible
        /// </summary>
        /// <returns></returns>
        [Route("[action]")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Policy = "ChangeManagerAccess")]
        public ActionResult<IEnumerable<ChangeInitiative>> GetChangeInitiativesForChangeManager() {
            try {
                ChangeManager loggedInCm = _changeManagerRepository.GetByEmail(User.Identity.Name);
                return loggedInCm.CreatedChangeInitiatives.AsQueryable().ToList();
            } catch (Exception e) {
                return NotFound(e.Message);
            }
        }



        /// <summary>
        /// Get amount of Survey's filled in from ChangeInitiative by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("[action]/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Policy = "ChangeManagerAccess")]
        public ActionResult<double> GetFilledInSurveysOfChangeInitiative(int id) {
            ChangeInitiative ci = _changeInitiativeRepository.GetBy(id);

            if (ci == null) {
                return NotFound("Item not found");
            }

            List<IAssessment> surveys = new List<IAssessment>();
            foreach (var r in ci.RoadMap) {
                surveys.Add(r.Assessment);
            }

            var total = surveys.Count();
            double filledIn = 0;

            foreach (var s in surveys) {
                foreach (var q in s.Questions) {
                    filledIn += (q.QuestionRegistered.Keys.Count / Convert.ToDouble(ci.ChangeGroup.EmployeeChangeGroups.Count)) * 100 / total;
                }
            }


            return filledIn;
        }



        /// <summary>
        /// Get mood from ChangeInitiative by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("[action]/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize(Policy = "ChangeManagerAccess")]
        public ActionResult<Dictionary<int, int>> GetMoodFromChangeInitiative(int id) {
            ChangeInitiative ci = _changeInitiativeRepository.GetBy(id);

            Dictionary<int, int> moods = new Dictionary<int, int>();
            moods.Add(1, 0);
            moods.Add(2, 0);
            moods.Add(3, 0);
            moods.Add(4, 0);
            moods.Add(5, 0);

            if (ci == null) {
                return NotFound("Item not found");
            }

            List<IAssessment> surveys = new List<IAssessment>();
            foreach (var r in ci.RoadMap) {
                surveys.Add(r.Assessment);
            }

            List<RangedQuestion> feedback = new List<RangedQuestion>();

            foreach (var s in surveys) {
                if (s != null) {
                    feedback.Add(s.Feedback);
                }

            }

            foreach (var f in feedback) {
                if (f != null) {
                    foreach (var k in f.PossibleAnswers.Keys) {

                        if (k % 1 == 0 && k != 0) {
                            moods[Convert.ToInt32(k)] += f.PossibleAnswers[k];
                        }
                    }
                }
            }

            return moods;
        }
    }
}
