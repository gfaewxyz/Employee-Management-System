using System.Text.Json;
using EmployeeManagementModels;

namespace EmployeeManagementDataService;

public class EmployeeJsonDataService : IEmployeeDataService
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _options = new() { WriteIndented = true };

    public EmployeeJsonDataService(string filePath)
    {
        _filePath = filePath;
        EnsureFileExists();
    }

    public List<Employee> GetAll() => Load();

    public Employee? GetById(int id) => Load().FirstOrDefault(e => e.Id == id);

    public Employee Add(Employee employee)
    {
        var employees = Load();
        employee.Id = employees.Count == 0 ? 1 : employees.Max(e => e.Id) + 1;
        employees.Add(employee);
        Save(employees);
        return employee;
    }

    public bool Update(Employee employee)
    {
        var employees = Load();
        var index = employees.FindIndex(e => e.Id == employee.Id);
        if (index < 0) return false;
        employees[index] = employee;
        Save(employees);
        return true;
    }

    public bool Delete(int id)
    {
        var employees = Load();
        var removed = employees.RemoveAll(e => e.Id == id) > 0;
        if (removed) Save(employees);
        return removed;
    }

    private List<Employee> Load()
    {
        try
        {
            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<Employee>>(json) ?? new List<Employee>();
        }
        catch (JsonException)
        {
            return new List<Employee>();
        }
    }

    private void Save(List<Employee> employees)
    {
        File.WriteAllText(_filePath, JsonSerializer.Serialize(employees, _options));
    }

    private void EnsureFileExists()
    {
        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrWhiteSpace(directory)) Directory.CreateDirectory(directory);
        if (!File.Exists(_filePath)) Save(new List<Employee>());
    }
}
