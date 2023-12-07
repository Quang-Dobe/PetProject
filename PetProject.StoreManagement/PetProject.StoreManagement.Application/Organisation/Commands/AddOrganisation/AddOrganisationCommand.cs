using PetProject.StoreManagement.Application.Common.Commands;

namespace PetProject.StoreManagement.Application.Organisation.Commands.AddOrganisation
{
    public class AddOrganisationCommand : ICommand<bool>
    {
        public string IdCode { get; set; }

        public string OrganisationName { get; set; }

        public string? Address { get; set; }

        public string Country { get; set; }

        public AddOrganisationCommand(string idCode, string organisationName, string? address, string country)
        {
            IdCode = idCode;
            OrganisationName = organisationName;
            Address = address;
            Country = country;
        }

        public Domain.Entities.Organisation ToEntity()
        {
            return new Domain.Entities.Organisation()
            {
                IdCode = this.IdCode,
                OrganisationName = this.OrganisationName,
                Address = this.Address,
                Country = this.Country
            };
        }
    }
}
