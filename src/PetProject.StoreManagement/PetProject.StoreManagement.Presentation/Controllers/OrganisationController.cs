using MediatR;
using Microsoft.AspNetCore.Mvc;
using PetProject.StoreManagement.Application.Organisation.Commands.AddOrganisation;
using PetProject.StoreManagement.Application.Organisation.Commands.DeleteOrganisation;
using PetProject.StoreManagement.Application.Organisation.Commands.UpdateOrganisation;
using PetProject.StoreManagement.Application.Organisation.Queries.GetAllOrganisations;
using PetProject.StoreManagement.Application.Organisation.Queries.GetOrganisationByID;
using PetProject.StoreManagement.Domain.DTOs.Common;

namespace PetProject.StoreManagement.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganisationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrganisationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("All")]
        [HttpGet]
        public async Task<IActionResult> GetAllOrganisations()
        {
            try
            {
                var organisationsDto = await _mediator.Send(new GetAllOrganisationsRequest());

                return Ok(new SuccessResultDto<Application.Organisation.Queries.GetAllOrganisations.AllOrganisationsDto>()
                {
                    Message = "",
                    Data = organisationsDto
                });
            }
            catch (HttpRequestException httpEx)
            {
                return BadRequest(new ErrorResultDto<Application.Organisation.Queries.GetAllOrganisations.AllOrganisationsDto>()
                {
                    Message = httpEx.Message,
                    Error = httpEx.Data.ToString(),
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResultDto<Application.Organisation.Queries.GetAllOrganisations.AllOrganisationsDto>()
                {
                    Message = ex.Message,
                    Error = ex.Data.ToString(),
                    Data = null
                });
            }
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<IActionResult> GetOrganisationByID([FromRoute] Guid? id)
        {
            try
            {
                var organisationDto = await _mediator.Send(new GetOrganisationByIDRequest(id));

                return Ok(new SuccessResultDto<Application.Organisation.Queries.GetOrganisationByID.OrganisationDto>()
                {
                    Message = "",
                    Data = organisationDto
                });
            }
            catch (HttpRequestException httpEx)
            {
                return BadRequest(new ErrorResultDto<Application.Organisation.Queries.GetOrganisationByID.OrganisationDto>()
                {
                    Message = httpEx.Message,
                    Error = httpEx.Data.ToString(),
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResultDto<Application.Organisation.Queries.GetOrganisationByID.OrganisationDto>()
                {
                    Message = ex.Message,
                    Error = ex.Data.ToString(),
                    Data = null
                });
            }
        }

        [Route("Add")]
        [HttpPost]
        public async Task<IActionResult> AddOrganisation([FromBody] Application.Organisation.Commands.AddOrganisation.OrganisationDto organisationDto)
        {
            try
            {
                var isAddedSuccessfully = await _mediator.Send(new AddOrganisationCommand(organisationDto.IdCode, organisationDto.OrganisationName, organisationDto.Address, organisationDto.Country));

                return Ok(new SuccessResultDto<bool>()
                {
                    Message = "",
                    Data = isAddedSuccessfully
                });
            }
            catch (HttpRequestException httpEx)
            {
                return BadRequest(new ErrorResultDto<bool>()
                {
                    Message = httpEx.Message,
                    Error = httpEx.Data.ToString(),
                    Data = false
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResultDto<bool>()
                {
                    Message = ex.Message,
                    Error = ex.Data.ToString(),
                    Data = false
                });
            }
        }

        [Route("Update")]
        [HttpPost]
        public async Task<IActionResult> UpdateOrganisation([FromBody] Application.Organisation.Commands.UpdateOrganisation.OrganisationDto organisationDto)
        {
            try
            {
                var updatedOrganisation = await _mediator.Send(new UpdateOrganisationCommand(organisationDto.Id, organisationDto.IdCode, organisationDto.OrganisationName));

                return Ok(new SuccessResultDto<Application.Organisation.Commands.UpdateOrganisation.OrganisationDto>()
                {
                    Message = "",
                    Data = updatedOrganisation
                });
            }
            catch (HttpRequestException httpEx)
            {
                return BadRequest(new ErrorResultDto<Application.Organisation.Commands.UpdateOrganisation.OrganisationDto>()
                {
                    Message = httpEx.Message,
                    Error = httpEx.Data.ToString(),
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResultDto<Application.Organisation.Commands.UpdateOrganisation.OrganisationDto>()
                {
                    Message = ex.Message,
                    Error = ex.Data.ToString(),
                    Data = null
                });
            }
        }

        [Route("Delete")]
        [HttpPost]
        public async Task<IActionResult> DeleteOrganisation([FromQuery] Guid id, [FromQuery] string? idCode)
        {
            try
            {
                var isDeletedSuccessfully = await _mediator.Send(new DeleteOrganisationCommand(id, idCode));

                return Ok(new SuccessResultDto<bool>()
                {
                    Message = "",
                    Data = isDeletedSuccessfully
                });
            }
            catch (HttpRequestException httpEx)
            {
                return BadRequest(new ErrorResultDto<bool>()
                {
                    Message = httpEx.Message,
                    Error = httpEx.Data.ToString(),
                    Data = false
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResultDto<bool>()
                {
                    Message = ex.Message,
                    Error = ex.Data.ToString(),
                    Data = false
                });
            }
        }
    }
}
