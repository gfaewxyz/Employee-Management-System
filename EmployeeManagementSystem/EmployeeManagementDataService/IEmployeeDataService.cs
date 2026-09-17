using EmployeeManagementModels;

namespace EmployeeManagementDataService;

public interface IEmployeeDataService
{
    List<Employee> GetAll();
    Employee? GetById(int id);
    Employee Add(Employee employee);
    bool Update(Employee employee);
    bool Delete(int id);
}
