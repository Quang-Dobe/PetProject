using PetProject.StoreManagement.Application.Common.Queries;

namespace PetProject.StoreManagement.Application.Organisation.Queries.GetOrganisationByID
{
    public class GetOrganisationByIDRequest : IQuery<OrganisationDto>
    {
        public Guid? OrganisationId { get; set; }

        public GetOrganisationByIDRequest(Guid? organisationId)
        {
            OrganisationId = organisationId;
        }
    }
}
