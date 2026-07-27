using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using EmployeeManagement.BUSINESS.BusinessModels.RequestDTOs.DepartmentRequestDtos;
using EmployeeManagement.BUSINESS.BusinessModels.RequestDTOs.EmployeeRequestDtos;
using EmployeeManagement.BUSINESS.BusinessModels.ResponseDTOs.DepartmentResponseDtos;
using EmployeeManagement.BUSINESS.BusinessModels.ResponseDTOs.EmployeeResponseDtos;
using EmployeeManagement.DATA.DomainModels.DepartmentDATA;
using EmployeeManagement.DATA.DomainModels.EmployeeDATA;

namespace EmployeeManagement.BUSINESS.Mapping
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			// Employee
			CreateMap<CreateEmployeeDto, Employee>();
			CreateMap<UpdateEmployeeDto, Employee>();
			CreateMap<Employee, EmployeeResponseDto>()
				.ForMember(dest => dest.DepartmentName,
					opt => opt.MapFrom(src => src.Department.Name));

			// Department
			CreateMap<CreateDepartmentDto, Department>();
			CreateMap<UpdateDepartmentDto, Department>();
			CreateMap<Department, DepartmentResponseDto>()
				.ForMember(dest => dest.EmployeeCount, opt => opt.Ignore());
		}
	}
}
