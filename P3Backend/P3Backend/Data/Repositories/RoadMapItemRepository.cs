﻿using Microsoft.EntityFrameworkCore;
using P3Backend.Model;
using P3Backend.Model.RepoInterfaces;
using System.Collections.Generic;
using System.Linq;

namespace P3Backend.Data.Repositories {
    public class RoadMapItemRepository : IRoadmapItemRepository {

        private readonly ApplicationDbContext _context;
        private readonly DbSet<RoadMapItem> _roadMapItems;

        public RoadMapItemRepository(ApplicationDbContext context) {
            _context = context;
            _roadMapItems = _context.RoadMapItems;
        }

        public void Add(RoadMapItem rmi) {
            _roadMapItems.Add(rmi);
        }

        public void Delete(RoadMapItem rmi) {
            _roadMapItems.Remove(rmi);
        }

        public IEnumerable<RoadMapItem> GetAll() {
            return _roadMapItems
                .Include(rmi => rmi.Assessment).ThenInclude(a => a.Questions)
                .Include(rmi => rmi.Assessment).ThenInclude(a => a.Feedback);
        }

        public RoadMapItem GetBy(int id) {
            return _roadMapItems
                .Include(rmi => rmi.Assessment).ThenInclude(a => a.Questions)
                .Include(rmi => rmi.Assessment).ThenInclude(a => a.Feedback)
                .FirstOrDefault(rmi => rmi.Id == id);
        }

        public RoadMapItem GetByTitle(string title) {
            return _roadMapItems
                .Include(rmi => rmi.Assessment).ThenInclude(a => a.Questions)
                .Include(rmi => rmi.Assessment).ThenInclude(a => a.Feedback)
                .FirstOrDefault(rmi => rmi.Title == title);
        }

        public void SaveChanges() {
            _context.SaveChanges();
        }

        public void Update(RoadMapItem rmi) {
            _roadMapItems.Update(rmi);
        }
    }
}
