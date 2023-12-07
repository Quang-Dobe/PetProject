namespace PetProject.StoreManagement.Application.Organisation.Commands.AddOrganisation
{
    public class OrganisationDto
    {
        public string IdCode { get; set; }

        public string OrganisationName { get; set; }

        public string? Address { get; set; }

        public string Country { get; set; }
    }
}
