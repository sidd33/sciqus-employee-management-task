import api from './api';

export interface Employee {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  department?: string;
  role: number; // 1 for Employee, 2 for Admin
  isActive: boolean;
  createdAt: string;
}

export interface CreateEmployeeDto {
  firstName: string;
  lastName: string;
  email: string;
  department?: string;
  role?: number;
}

export interface UpdateEmployeeDto {
  firstName: string;
  lastName: string;
  email: string;
  department?: string;
  role?: number;
  isActive?: boolean;
}

const EmployeeService = {
  getAll: async (): Promise<Employee[]> => {
    const response = await api.get<Employee[]>('/Employees');
    return response.data;
  },

  getById: async (id: string): Promise<Employee> => {
    const response = await api.get<Employee>(`/Employees/${id}`);
    return response.data;
  },

  create: async (data: CreateEmployeeDto): Promise<Employee> => {
    const response = await api.post<Employee>('/Employees', data);
    return response.data;
  },

  update: async (id: string, data: UpdateEmployeeDto): Promise<Employee> => {
    const response = await api.put<Employee>(`/Employees/${id}`, data);
    return response.data;
  },

  delete: async (id: string): Promise<void> => {
    await api.delete(`/Employees/${id}`);
  },
};

export default EmployeeService;
