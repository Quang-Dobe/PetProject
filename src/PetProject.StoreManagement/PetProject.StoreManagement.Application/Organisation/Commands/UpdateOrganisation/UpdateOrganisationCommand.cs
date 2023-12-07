using PetProject.StoreManagement.Application.Common.Commands;

namespace PetProject.StoreManagement.Application.Organisation.Commands.UpdateOrganisation
{
    public class UpdateOrganisationCommand : ICommand<OrganisationDto>
    {
        public Guid Id { get; set; }

        public string IdCode { get; set; }

        public string OrganisationName { get; set; }

        public UpdateOrganisationCommand(Guid id, string idCode, string organisationName)
        {
            Id = id;
            IdCode = idCode;
            OrganisationName = organisationName;
        }

        public Domain.Entities.Organisation ToEntity()
        {
            return new Domain.Entities.Organisation()
            {
                Id = Id,
                IdCode = this.IdCode,
                OrganisationName = this.OrganisationName
            };
        }
    }
}
