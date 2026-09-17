using EmployeeManagementDataService;
using EmployeeManagementModels;

namespace EmployeeManagementAppService;

public class EmployeeAppService
{
    private readonly IEmployeeDataService _dataService;

    public EmployeeAppService(IEmployeeDataService dataService) => _dataService = dataService;

    public List<Employee> GetEmployees() => _dataService.GetAll();

    public Employee? GetEmployee(int id) => _dataService.GetById(id);

    public Employee AddEmployee(string name, string position, string email)
    {
        Validate(name, position, email);
        return _dataService.Add(new Employee { Name = name.Trim(), Position = position.Trim(), Email = email.Trim() });
    }

    public bool UpdateEmployee(int id, string name, string position, string email)
    {
        Validate(name, position, email);
        return _dataService.Update(new Employee { Id = id, Name = name.Trim(), Position = position.Trim(), Email = email.Trim() });
    }

    public bool DeleteEmployee(int id) => _dataService.Delete(id);

    public List<Employee> SearchEmployees(string keyword)
    {
        keyword = keyword.Trim();
        return GetEmployees().Where(e =>
            e.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
            e.Position.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
            e.Email.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    private static void Validate(string name, string position, string email)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.");
        if (string.IsNullOrWhiteSpace(position)) throw new ArgumentException("Position is required.");
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@')) throw new ArgumentException("A valid email is required.");
    }
}
