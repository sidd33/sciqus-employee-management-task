namespace EmployeeManagement.BUSINESS.BusinessModels.RequestDTOs.CustomerRequestDtos;

public class CustomerQueryParameters
{
    public string? SearchTerm { get; set; }
    public string SortBy { get; set; } = "Name";
    public bool IsDescending { get; set; } = false;

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
