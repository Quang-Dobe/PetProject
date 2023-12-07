
namespace PetProject.StoreManagement.Domain.DTOs.Common
{
    public class ErrorResultDto<T>
    {
        public string? Message { get; set; }

        public T? Data { get; set; }

        public string? Error { get; set; }

        public string? ErrorDescription { get; set; }
    }
}