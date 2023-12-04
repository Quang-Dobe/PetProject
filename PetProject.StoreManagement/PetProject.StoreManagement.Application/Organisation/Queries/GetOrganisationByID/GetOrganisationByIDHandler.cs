using Dapper;
using PetProject.StoreManagement.Application.Common.Queries;
using PetProject.StoreManagement.Domain.ThirdPartyServices.DbConnectionClient;

namespace PetProject.StoreManagement.Application.Organisation.Queries.GetOrganisationByID
{
    public class GetOrganisationByIDHandler : IQueryHandler<GetOrganisationByIDRequest, OrganisationDto>
    {
        private readonly IDbConnectionClient _connectionClient;

        public GetOrganisationByIDHandler(IDbConnectionClient connectionClient)
        {
            _connectionClient = connectionClient;
        }

        public async Task<OrganisationDto> Handle(GetOrganisationByIDRequest request, CancellationToken cancellationToken)
        {
            var result = new OrganisationDto();

            using(var connection = _connectionClient.GetDbConnection())
            {
                var sql = "SELECT " +
                          "[Organisation].[Id], " +
                          "[Organisation].[IdCode], " +
                          "[Organisation].[OrganisationName], " +
                          "[User].[Id] AS [UserId], " +
                          "[User].[Name] AS [UserName] " +
                          "FROM dbo.Organisation AS [Organisation] " +
                          "INNER JOIN dbo.User AS [User] ON [Organisation].Id = [User].OrganisationId" +
                         $"WHERE [Organisation].[ID] = {request.OrganisationId}";

                var resultQuery = await connection.QueryAsync(sql);

                result = resultQuery.GroupBy(x => (Guid)x.Id).Select(x => new OrganisationDto
                {
                    Id = x.Key,
                    IdCode = x.FirstOrDefault().IdCode,
                    OrganisationName = x.FirstOrDefault().OrganisationName,
                    User = x.Select(y => new UserDto
                    {
                        Id = y.UserId,
                        Name = y.UserName,
                    })
                }).FirstOrDefault();
            }

            return result;
        }
    }
}
