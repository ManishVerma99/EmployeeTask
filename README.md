1. Open the project in Visual Studio 2022.
2. Navigate to the "Tools" menu.
3. Click on "Manage NuGet Packages" to open the NuGet Package Manager.
4. In the NuGet Package Manager, select "Package Manager Console" from the options.
5. Once the Package Manager Console is open, execute the command update-database to apply any pending migrations and update the database schema.
6. To create admins using Postman, send a POST request to the endpoint https://localhost:7211/api/Authenticate/register-admin with the following JSON data:
{
    "Username": "example (unique)",
    "Email": "example@yopmail.com (unique)",
    "Password": "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one digit, and one special character",
    "FirstName": "example",
    "LastName": "example",
    "CreatedBy": ""
}
Ensure that the Username and Email fields are unique. The Password must be at least 8 characters long and include at least one uppercase letter, one lowercase letter, one digit, and one special character.
7. Your admin account will be created. Use the provided username and password to log in.
8. After creating admins, run the EmployeeTask.Server project.
9. Admins can create employees and their tasks by clicking on the action button. Employees can log in using the usernames and passwords created by the admin.
10. Employees can update the status of their tasks.