Server:

Technologies:
 
ASP .NET Core 8 REST API
PostgreSQL

Projects:
FileConverter.Api - API project. Settings are stored in appsettings.json. You need to configure a connection string to the database and specify the path to the folder in which the downloaded and converted files will be stored. Settings name are "PathToWorkDir" and "DefaultConnection". This project contains a background service that will perform the conversion

FileConverter.Bll - The project stores converter services and classes. The services will perform the functionality of working with the file system and transferring data to the repository. The converter class converts html to pdf

FileConverter.DataLayer - The project stores the logic for working with the database. The Repository pattern has been implemented.


Client:

Technologies:
 
TypeScript
React/Redux
SCSS/bootstrap

On the client, all added files are stored in the LocalStore. When open it for the first time a client identifier is created, all files are linked to this identifier. When adding a file the client is not blocked. File readiness is polled at intervals. There is an operation indicator. Once ready a button appears to download and delete the file. After restarting the browser all information is saved, and if necessary, the poll is launched to check file readiness