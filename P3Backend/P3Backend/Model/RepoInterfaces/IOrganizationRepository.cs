﻿using System.Collections.Generic;

namespace P3Backend.Model.RepoInterfaces {
    public interface IOrganizationRepository {

        IEnumerable<Organization> GetAll();

        Organization GetBy(int id);

        void Add(Organization o);

        void Update(Organization o);

        void Delete(Organization o);

        void SaveChanges();

        Organization GetByName(string name);
    }
}
