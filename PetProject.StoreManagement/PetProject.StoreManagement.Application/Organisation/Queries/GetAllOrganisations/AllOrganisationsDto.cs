namespace PetProject.StoreManagement.Application.Organisation.Queries.GetAllOrganisations
{
    public class AllOrganisationsDto
    {
        public IEnumerable<OrganisationDto> Organisations { get; set; }
    }

    public class OrganisationDto
    {
        public Guid Id { get; set; }

        public string IdCode { get; set; }

        public string OrganisationName { get; set; }

        public IEnumerable<UserDto> User { get; set; }
    }

    public class UserDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}
