﻿using Microsoft.EntityFrameworkCore;
using P3Backend.Model;
using P3Backend.Model.RepoInterfaces;
using System.Collections.Generic;
using System.Linq;

namespace P3Backend.Data.Repositories {
    public class ChangeGroupRepository : IChangeGroupRepository {

        private readonly ApplicationDbContext _context;
        private readonly DbSet<ChangeGroup> _changeGroups;

        public ChangeGroupRepository(ApplicationDbContext context) {
            _context = context;
            _changeGroups = _context.ChangeGroups;
        }

        public void Add(ChangeGroup cg) {
            _changeGroups.Add(cg);
        }

        public void Delete(ChangeGroup cg) {
            _changeGroups.Remove(cg);
        }

        public IEnumerable<ChangeGroup> GetAll() {
            return _changeGroups
                .Include(cg => cg.EmployeeChangeGroups).ThenInclude(ecg => ecg.Employee);
        }

        public ChangeGroup GetBy(int id) {
            return _changeGroups
                .Include(cg => cg.EmployeeChangeGroups).ThenInclude(ecg => ecg.Employee).ThenInclude(e => e.EmployeeChangeGroups)
                .FirstOrDefault(cg => cg.Id == id);
        }

        public ChangeGroup GetByName(string name) {
            return _changeGroups
                .Include(cg => cg.EmployeeChangeGroups).ThenInclude(ecg => ecg.Employee).ThenInclude(e => e.EmployeeChangeGroups)
                .FirstOrDefault(cg => cg.Name == name);
        }

        public List<ChangeGroup> GetForUserId(int userId) {
            return _changeGroups
                .Include(cg => cg.EmployeeChangeGroups).ThenInclude(ecg => ecg.Employee).ThenInclude(e => e.EmployeeChangeGroups)
                .Where(cg => cg.EmployeeChangeGroups.Any(u => u.EmployeeId == userId)).ToList();
        }

        public void SaveChanges() {
            _context.SaveChanges();
        }

        public void Update(ChangeGroup cg) {
            _changeGroups.Update(cg);
        }
    }
}
