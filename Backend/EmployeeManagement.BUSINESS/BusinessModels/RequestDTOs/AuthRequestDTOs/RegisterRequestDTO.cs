namespace EmployeeManagement.BUSINESS.BusinessModels.RequestDTOs.AuthRequestDTOs
{
    public class RegisterRequestDto
    {
        public string Name { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
    }
}