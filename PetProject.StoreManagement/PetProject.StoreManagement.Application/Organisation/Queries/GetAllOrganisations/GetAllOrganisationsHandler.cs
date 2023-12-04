using Dapper;
using PetProject.StoreManagement.Application.Common.Queries;
using PetProject.StoreManagement.Domain.ThirdPartyServices.DbConnectionClient;

namespace PetProject.StoreManagement.Application.Organisation.Queries.GetAllOrganisations
{
    public class GetAllOrganisationsHandler : IQueryHandler<GetAllOrganisationsRequest, AllOrganisationsDto>
    {
        private readonly IDbConnectionClient _connectionClient;

        public GetAllOrganisationsHandler(IDbConnectionClient dbConnectionClient)
        {
            _connectionClient = dbConnectionClient;
        }

        public async Task<AllOrganisationsDto> Handle(GetAllOrganisationsRequest request, CancellationToken cancellationToken)
        {
            var result = new AllOrganisationsDto();

            using (var connection = _connectionClient.GetDbConnection())
            {
                var sql = "SELECT " +
                          "[Organisation].[Id], " +
                          "[Organisation].[IdCode], " +
                          "[Organisation].[OrganisationName], " +
                          "[User].[Id] AS [UserId], " +
                          "[User].[Name] AS [UserName] " +
                          "FROM dbo.Organisation AS [Organisation] " +
                          "INNER JOIN dbo.User AS [User] ON [Organisation].Id = [User].OrganisationId";

                var resultQuery = await connection.QueryAsync(sql);

                result.Organisations = resultQuery.GroupBy(x => (Guid)x.Id).Select(x => new OrganisationDto
                {
                    Id = x.Key,
                    IdCode = x.FirstOrDefault().IdCode,
                    OrganisationName = x.FirstOrDefault().OrganisationName,
                    User = x.Select(y => new UserDto
                    {
                        Id = y.UserId,
                        Name = y.UserName,
                    })
                });
            }

            return result;
        }
    }
}
