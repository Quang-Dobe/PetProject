using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PetProject.StoreManagement.Application.Common.Queries;
using PetProject.StoreManagement.Application.Organisation.Commands.AddOrganisation;
using PetProject.StoreManagement.CrossCuttingConcerns.OS;
using PetProject.StoreManagement.Domain.ThirdPartyServices.DbConnectionClient;
using System.Diagnostics;

namespace PetProject.StoreManagement.Application.Organisation.Queries.GetOrganisationByID
{
    public class GetOrganisationByIDHandler : IQueryHandler<GetOrganisationByIDRequest, OrganisationDto>
    {
        private readonly IDbConnectionClient _connectionClient;

        private readonly IDateTimeProvider _dateTimeProvider;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly ILogger<GetOrganisationByIDHandler> _logger;

        private Stopwatch _stopwatch;


        public GetOrganisationByIDHandler(
            IDbConnectionClient connectionClient, 
            IDateTimeProvider dateTimeProvider,
            IHttpContextAccessor httpContextAccessor,
            ILogger<GetOrganisationByIDHandler> logger)
        {
            _connectionClient = connectionClient;
            _dateTimeProvider = dateTimeProvider;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<OrganisationDto> Handle(GetOrganisationByIDRequest request, CancellationToken cancellationToken)
        {
            _stopwatch = Stopwatch.StartNew();
            var ipAddress = GetIpAddress();

            try
            {
                var result = new OrganisationDto();

                using (var connection = _connectionClient.GetDbConnection())
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

                _stopwatch.Stop();
                return result;
            }
            catch (Exception ex)
            {
                LogTrace("", "", ipAddress, $"[Organisation - GetOrganisationById] {ex.Message}");
                throw new Exception(ex.Message);
            }
        }

        #region Private Methods

        private string GetIpAddress()
        {
            var remoteIpAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress;

            return remoteIpAddress != null ? remoteIpAddress.ToString() : "";
        }

        private void LogTrace(string? userName, string? clientId, string? ipAddress, string? message)
        {
            _stopwatch.Stop();
            _logger.LogInformation(string.Format(" At {0}. Time spent {1} ", _dateTimeProvider.Now, _stopwatch.Elapsed));
            _logger.LogInformation(string.Format(" UserName: {0} - ClientID: {1} - IpAddress: {2} ", userName, clientId, ipAddress));
            _logger.LogInformation(string.Format(" Message: {0} ", message));
        }

        #endregion
    }
}
