namespace PetProject.StoreManagement.Domain.DTOs.Common
{
    public class SuccessResultDto<T>
    {
        public string? Message { get; set; }

        public T? Data { get; set; }
    }
}