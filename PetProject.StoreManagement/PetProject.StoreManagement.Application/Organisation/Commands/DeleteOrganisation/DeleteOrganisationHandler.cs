using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PetProject.StoreManagement.Domain.Repositories;
using PetProject.StoreManagement.Application.Common.Commands;
using PetProject.StoreManagement.CrossCuttingConcerns.Extensions;
using PetProject.StoreManagement.CrossCuttingConcerns.OS;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PetProject.StoreManagement.Application.Organisation.Commands.DeleteOrganisation
{
    public class DeleteOrganisationHandler : ICommandHandler<DeleteOrganisationCommand, bool>
    {
        private readonly IOrganisationRepository _organisationRepository;

        private readonly IDateTimeProvider _dateTimeProvider;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly ILogger<DeleteOrganisationHandler> _logger;

        private Stopwatch _stopwatch;

        public DeleteOrganisationHandler(
            IOrganisationRepository organisationRepository, 
            IDateTimeProvider dateTimeProvider, 
            IHttpContextAccessor httpContextAccessor, 
            ILogger<DeleteOrganisationHandler> logger)
        {
            _organisationRepository = organisationRepository;
            _dateTimeProvider = dateTimeProvider;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteOrganisationCommand request, CancellationToken cancellationToken)
        {
            _stopwatch = Stopwatch.StartNew();
            var ipAddress = GetIpAddress();

            try
            {
                var data = request.ToEntity();

                if (data == null || (data.Id == null && data.IdCode.IsNullOrEmpty()))
                {
                    LogTrace("", "", ipAddress, $"[Organisation - DeleteOrganisationHandler] Invalid Organisation");
                    return false;
                }

                var entity = _organisationRepository.GetAll().Where(x => x.Id == data.Id || x.IdCode == data.IdCode).FirstOrDefault();

                if (entity == null)
                {
                    LogTrace("", "", ipAddress, $"[Organisation - DeleteOrganisationHandler] Not exist Organisation with ID ({data.Id})");
                    return false;
                }
                else
                {
                    await _organisationRepository.DeleteAsync(entity);
                    await _organisationRepository.SaveChangesAsync(cancellationToken);

                    _stopwatch.Stop();
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogTrace("", "", ipAddress, $"[Organisation - DeleteOrganisationHandler] {ex.Message}");
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
