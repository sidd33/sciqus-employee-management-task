namespace EmployeeManagement.BUSINESS.BusinessModels.ResponseDTOs.AuthResponseDTOs
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
