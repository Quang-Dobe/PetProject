namespace PetProject.StoreManagement.Application.Organisation.Commands.UpdateOrganisation
{
    public class OrganisationDto
    {
        public Guid Id { get; set; }

        public string IdCode { get; set; }

        public string OrganisationName { get; set; }

        public IEnumerable<UserDto> Users { get; set; }

        public Domain.Entities.Organisation ToEntity()
        {
            return new Domain.Entities.Organisation()
            {
                Id = Id,
                IdCode = IdCode,
                OrganisationName = OrganisationName,
                Users = Users.Select(x => new Domain.Entities.User()
                {
                    Id = x.Id,
                    UserName = x.Name
                })
            };
        }

        public OrganisationDto FromEntity(Domain.Entities.Organisation organisation)
        {
            return new OrganisationDto()
            {
                Id = organisation.Id,
                IdCode = organisation.IdCode,
                OrganisationName = organisation.OrganisationName,
                Users = organisation.Users.Select(x => new UserDto()
                {
                    Id = x.Id,
                    Name = x.UserName
                })
            };
        }
    }

    public class UserDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}
