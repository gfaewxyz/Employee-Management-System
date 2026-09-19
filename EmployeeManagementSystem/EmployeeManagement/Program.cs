using System.Text.Json;
using EmployeeManagementAppService;
using EmployeeManagementDataService;
using EmployeeManagementModels;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
var dataPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "data", "Employees.json"));
services.AddSingleton<IEmployeeDataService>(_ => new EmployeeJsonDataService(dataPath));
services.AddSingleton<EmployeeAppService>();

var mailtrap = LoadMailtrapSettings();
services.AddSingleton(_ => new EmailService(
    mailtrap.Host, mailtrap.Port, mailtrap.Username, mailtrap.Password, mailtrap.FromAddress, mailtrap.FromName));

using var provider = services.BuildServiceProvider();
var app = provider.GetRequiredService<EmployeeAppService>();
var emailService = provider.GetRequiredService<EmailService>();

while (true)
{
    Console.Clear();
    Console.WriteLine("====================================");
    Console.WriteLine("       EMPLOYEE MANAGEMENT SYSTEM   ");
    Console.WriteLine("====================================");
    Console.WriteLine("1. View Employees");
    Console.WriteLine("2. Add Employee");
    Console.WriteLine("3. Edit Employee");
    Console.WriteLine("4. Delete Employee");
    Console.WriteLine("5. Search Employee");
    Console.WriteLine("6. Send Email to Employee");
    Console.WriteLine("7. Exit");
    Console.Write("\nSelect option: ");

    switch (Console.ReadLine()?.Trim())
    {
        case "1": View(app); break;
        case "2": Add(app); break;
        case "3": Edit(app); break;
        case "4": Delete(app); break;
        case "5": Search(app); break;
        case "6": SendEmail(app, emailService); break;
        case "7": return;
        default: Pause("Invalid option."); break;
    }
}

static void SendEmail(EmployeeAppService app, EmailService emailService)
{
    Console.Clear(); Console.WriteLine("SEND EMAIL TO EMPLOYEE\n");
    if (!int.TryParse(Read("Employee ID"), out var id) || app.GetEmployee(id) is not { } employee)
    {
        Pause("Employee not found.");
        return;
    }

    var subject = Read("Subject");
    var body = Read("Message");

    try
    {
        emailService.SendEmail(employee.Email, subject, body);
        Pause($"Email sent to {employee.Name} ({employee.Email}) successfully.");
    }
    catch (Exception ex)
    {
        Pause($"Failed to send email: {ex.Message}");
    }
}

static MailtrapSettings LoadMailtrapSettings()
{
    var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
    var json = File.ReadAllText(path);
    using var doc = JsonDocument.Parse(json);
    var m = doc.RootElement.GetProperty("Mailtrap");
    return new MailtrapSettings(
        m.GetProperty("Host").GetString()!,
        m.GetProperty("Port").GetInt32(),
        m.GetProperty("Username").GetString()!,
        m.GetProperty("Password").GetString()!,
        m.GetProperty("FromAddress").GetString()!,
        m.GetProperty("FromName").GetString()!
    );
}

static void View(EmployeeAppService app)
{
    Console.Clear(); Console.WriteLine("EMPLOYEES\n");
    var employees = app.GetEmployees();
    if (employees.Count == 0) Console.WriteLine("No employees found.");
    foreach (var e in employees) Print(e);
    Pause();
}

static void Add(EmployeeAppService app)
{
    Console.Clear(); Console.WriteLine("ADD EMPLOYEE\n");
    try
    {
        var employee = app.AddEmployee(Read("Name"), Read("Position"), Read("Email"));
        Console.WriteLine($"\nEmployee #{employee.Id} added successfully.");
    }
    catch (ArgumentException ex) { Console.WriteLine($"\nError: {ex.Message}"); }
    Pause();
}

static void Edit(EmployeeAppService app)
{
    Console.Clear(); Console.WriteLine("EDIT EMPLOYEE\n");
    if (!int.TryParse(Read("Employee ID"), out var id) || app.GetEmployee(id) is null) { Pause("Employee not found."); return; }
    try
    {
        var ok = app.UpdateEmployee(id, Read("Name"), Read("Position"), Read("Email"));
        Pause(ok ? "Employee updated successfully." : "Employee not found.");
    }
    catch (ArgumentException ex) { Pause($"Error: {ex.Message}"); }
}

static void Delete(EmployeeAppService app)
{
    Console.Clear(); Console.WriteLine("DELETE EMPLOYEE\n");
    if (!int.TryParse(Read("Employee ID"), out var id)) { Pause("Invalid ID."); return; }
    var employee = app.GetEmployee(id);
    if (employee is null) { Pause("Employee not found."); return; }
    Console.Write($"Delete {employee.Name}? (y/n): ");
    if (Console.ReadLine()?.Trim().Equals("y", StringComparison.OrdinalIgnoreCase) == true)
        Pause(app.DeleteEmployee(id) ? "Employee deleted successfully." : "Employee not found.");
}

static void Search(EmployeeAppService app)
{
    Console.Clear(); Console.WriteLine("SEARCH EMPLOYEE\n");
    var results = app.SearchEmployees(Read("Keyword"));
    if (results.Count == 0) Console.WriteLine("No matching employees found.");
    foreach (var e in results) Print(e);
    Pause();
}

static string Read(string label) { Console.Write($"{label}: "); return Console.ReadLine() ?? ""; }
static void Print(Employee e) => Console.WriteLine($"ID: {e.Id}\nName: {e.Name}\nPosition: {e.Position}\nEmail: {e.Email}\n------------------------------------");
static void Pause(string message = "Press Enter to continue...") { Console.WriteLine($"\n{message}"); Console.ReadLine(); }

record MailtrapSettings(string Host, int Port, string Username, string Password, string FromAddress, string FromName);
