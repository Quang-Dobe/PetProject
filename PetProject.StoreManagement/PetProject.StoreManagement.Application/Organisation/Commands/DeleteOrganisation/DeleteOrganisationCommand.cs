using PetProject.StoreManagement.Application.Common.Commands;

namespace PetProject.StoreManagement.Application.Organisation.Commands.DeleteOrganisation
{
    public class DeleteOrganisationCommand : ICommand<bool>
    {
        public Guid Id { get; set; }

        public string? IdCode { get; set; }

        public DeleteOrganisationCommand(Guid id, string? idCode)
        {
            Id = id;
            IdCode = idCode;
        }

        public Domain.Entities.Organisation ToEntity()
        {
            return new Domain.Entities.Organisation()
            {
                Id = Id,
                IdCode = this.IdCode
            };
        }
    }
}
