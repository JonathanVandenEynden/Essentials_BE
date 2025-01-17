﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using P3Backend.Model.OrganizationParts;

namespace P3Backend.Data.Mapping.OrganizationPartsConfiguration {
    public class OrganizationalPartConfiguration : IEntityTypeConfiguration<OrganizationPart> {
        public void Configure(EntityTypeBuilder<OrganizationPart> builder) {

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Name).IsRequired();

            builder.HasMany(o => o.EmployeeOrganizationParts).WithOne(eo => eo.OrganizationPart);
        }
    }
}
