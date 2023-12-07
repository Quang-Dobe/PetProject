using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PetProject.StoreManagement.Domain.Repositories;
using PetProject.StoreManagement.Application.Common.Commands;
using PetProject.StoreManagement.CrossCuttingConcerns.Extensions;
using PetProject.StoreManagement.CrossCuttingConcerns.OS;

namespace PetProject.StoreManagement.Application.Organisation.Commands.UpdateOrganisation
{
    public class UpdateOrganisationHandler : ICommandHandler<UpdateOrganisationCommand, OrganisationDto>
    {
        private readonly IOrganisationRepository _organisationRepository;

        private readonly IDateTimeProvider _dateTimeProvider;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly ILogger<UpdateOrganisationHandler> _logger;

        private Stopwatch _stopwatch;

        public UpdateOrganisationHandler(
            IOrganisationRepository organisationRepository, 
            IDateTimeProvider dateTimeProvider, 
            IHttpContextAccessor httpContextAccessor, 
            ILogger<UpdateOrganisationHandler> logger)
        {
            _organisationRepository = organisationRepository;
            _dateTimeProvider = dateTimeProvider;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<OrganisationDto> Handle(UpdateOrganisationCommand request, CancellationToken cancellationToken)
        {
            _stopwatch = Stopwatch.StartNew();
            var ipAddress = GetIpAddress();

            var result = new OrganisationDto();

            try
            {
                var data = request.ToEntity();

                if (data == null || (data.IdCode.IsNullOrEmpty() && data.OrganisationName.IsNullOrEmpty() && data.Country.IsNullOrEmpty()))
                {
                    LogTrace("", "", ipAddress, $"[Organisation - UpdateOrganisationHandler] Invalid Organisation");
                    throw new HttpRequestException("Invalid Organisation");
                }

                var entity = _organisationRepository.GetAll().Where(x => x.Id == data.Id || x.IdCode == data.IdCode).FirstOrDefault();

                if (entity == null)
                {
                    LogTrace("", "", ipAddress, $"[Organisation - UpdateOrganisationHandler] Not exist Organisation with Id ({data.Id})");
                    throw new HttpRequestException($"Not exist Organisation with Id ({data.Id})");
                }
                else
                {
                    entity.OrganisationName = data.OrganisationName;

                    _organisationRepository.Update(entity);
                    await _organisationRepository.SaveChangesAsync(cancellationToken);
                    var organisation = _organisationRepository.GetAll().Where(x => x.Id == entity.Id).FirstOrDefault();

                    _stopwatch.Stop();
                    return result.FromEntity(organisation);
                }
            }
            catch (Exception ex)
            {
                LogTrace("", "", ipAddress, $"[Organisation - UpdateOrganisationHandler] {ex.Message}");
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
