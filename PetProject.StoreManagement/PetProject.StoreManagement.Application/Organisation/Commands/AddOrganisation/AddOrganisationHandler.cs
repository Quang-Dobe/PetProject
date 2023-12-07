using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PetProject.StoreManagement.Domain.Repositories;
using PetProject.StoreManagement.Application.Common.Commands;
using PetProject.StoreManagement.CrossCuttingConcerns.Extensions;
using PetProject.StoreManagement.CrossCuttingConcerns.OS;

namespace PetProject.StoreManagement.Application.Organisation.Commands.AddOrganisation
{
    public class AddOrganisationHandler : ICommandHandler<AddOrganisationCommand, bool>
    {
        private readonly IOrganisationRepository _organisationRepository;

        private readonly IDateTimeProvider _dateTimeProvider;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly ILogger<AddOrganisationHandler> _logger;

        private Stopwatch _stopwatch;

        public AddOrganisationHandler(
            IOrganisationRepository organisationRepository, 
            IDateTimeProvider dateTimeProvider, 
            IHttpContextAccessor httpContextAccessor, 
            ILogger<AddOrganisationHandler> logger)
        {
            _organisationRepository = organisationRepository;
            _dateTimeProvider = dateTimeProvider;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<bool> Handle(AddOrganisationCommand request, CancellationToken cancellationToken)
        {
            _stopwatch = Stopwatch.StartNew();
            var ipAddress = GetIpAddress();

            try
            {
                var organisation = request.ToEntity();

                if (organisation == null || (organisation.IdCode.IsNullOrEmpty() && organisation.OrganisationName.IsNullOrEmpty() && organisation.Country.IsNullOrEmpty()))
                {
                    LogTrace("", "", ipAddress, $"[Organisation - AddOrganisationHandler] Invalid Organisation");
                    throw new HttpRequestException("Invalid Organisation");
                }

                await _organisationRepository.AddAsync(organisation);
                await _organisationRepository.SaveChangesAsync(cancellationToken);

                _stopwatch.Stop();
                return true;
            }
            catch (Exception ex)
            {
                LogTrace("", "", ipAddress, $"[Organisation - AddOrganisationHandler] {ex.Message}");
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
