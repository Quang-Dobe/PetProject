using System.ComponentModel.DataAnnotations;

namespace PetProject.OrderManagement.Domain.DTOs.User.Request
{
    public class UserDto
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Required field")]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Required field")]
        public string FirstName { get; set; }

        public string? MiddleName { get; set; }

        [Required(ErrorMessage = "Required field")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Required field")]
        public string PhoneNumber { get; set; }

        public bool IsChangingPassword { get; set; } = false;

        public string? OldPassword { get; set; }

        public string? NewPassword { get; set; }
    }

    public static partial class Mapper
    {
        public static Entities.User ToUser(this UserDto user)
        {
            return new Entities.User
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber
            };
        }

        public static UserDto ToUserDto(this Entities.User user)
        {
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber
            };
        }
    }
}
