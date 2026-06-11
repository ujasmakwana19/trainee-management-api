# TraineeManagement.Api
 
## Project Overview
 
TraineeManagement.Api is an ASP.NET Core Web API developed for managing trainee records.
 
The project demonstrates:
 
- ASP.NET Core Web API
- Controllers
- DTOs
- Service Layer
- EF Core InMemory Database
- CRUD Operations
- Validation
- Swagger
 
## Phase 1
### Day 1:
#### Goal
- Create the Health endpoint to get the status of there server
- Create the Trainee class with the following fields
Id - Unique & AUTO Generate
FirstName 
LastName 
Email 
TechStack 
Status 
CreatedDate - AUTO Generate
UpdatedDate - AUTO Generate 
- Create the apis to (getall , getbyid , add) trainee 
- Get the swagger running and test apis

### Day 2:
#### Goal
- Add Data Validations to the fields
- Add Proper Error Messages and Error Codes
- Create the update API
- Create the Delete API
- Use the DTOs to send Response and the Data communication between controllers and services
- Use the services to write the modular code so that code remains losely coupled

### Day 3:
#### Goal
- Use the EF Core Framework & Connect the Inmemory storage
- Create the AppDbContext using DbContext and the Dbset
such that easy to move to the persistant DBMS
- Create the search api /api/trainees?search=value
- Complete the task , upload to github and documentation

## Phase 2
### Day 1:
- 


## Technology Stack
 
- C#
- .NET 9
- ASP.NET Core Web API
- Entity Framework Core
- EF Core InMemory Database
- Swagger

## Project Structure
 
├── Controllers
│   ├── HealthController.cs
│   └── TraineesController.cs
├── DTOs
│   └── TraineeDTO.cs
├── Data
│   └── AppDbContext.cs
├── Models
│   └── Traniee.cs
├── Program.cs
├── Readme.md
├── Services
│   ├── ITraineeService.cs
│   └── TraineeService.cs
├── TraineeManagement.Api.csproj
├── TraineeManagement.Api.http
├── appsettings.Development.json
├── appsettings.json 

## API ENDPOINTS
- GET /api/health 
- GET /api/trainees/getall  (get all trainees)

- GET /api/trainees/:id (get trainee by id)
    - /api/trainees/1

- POST /api/trainees  
    - Sample Json
    {
        "firstName": "ujas",
        "lastName": "makwana",
        "email": "ujas.makwana@zeuslearning.com",
        "techStack": "dotnet",
        "status": "Active"
    }

- PUT /api/trainees/:id
    - /api/trainees/1
    - Sample Json
    {
        "firstName": "ujas",
        "lastName": "makwana",
        "email": "ujas.makwana@zeuslearning.com",
        "techStack": "dotnet",
        "status": "Active"
    }
    
- DELETE /api/trainees/:id
    - /api/trainees/1

-  GET  /api/trainees?search=value

    
    
### To Run Project
- git clone https://github.com/ujasmakwana19/trainee-management-api
- dotnet restore
- dotnet clean
- dotnet build
- dotnet ef database update (if incase you have not tools use (dotnet tool install --global dotnet-ef) before this)
- dotnet run
- https://localhost:<port>/swagger

Day 1:
Created Dotnet project
Install Required Package
Completed the Swagger UI running for api testing
Create the following api endpoints
GET /api/health
GET /api/trainees
GET /api/trainees/:id
POST /api/trainees


Day 2:
Created DTOs
Created Services
Add Validations using Annoatations
Completed Day 2 Tasks
PUT /api/trainees/:id
DELETE /api/trainees/:id

Day 3:
Used the EF Core Framework , create the AppDbContext using DbContext and Dbset
Connected to inmemory store 
Created the search api /api/trainees?search=value
Completed the assignment & uploaded to github


## Sample Success Response
{
    "id": 1,
    "firstName": "Amit",
    "lastName": "Sharma",
    "email": "amit.sharma@training.com",
    "techStack": "HTML, CSS, JavaScript",
    "status": "Active"
  }
 
## Sample GET Response
 
[
  {
    "id": 1,
    "firstName": "Amit",
    "lastName": "Sharma",
    "email": "amit.sharma@training.com",
    "techStack": "HTML, CSS, JavaScript",
    "status": "Active"
  }
]

## Sample Validation Error
 {
  "firstName": "string",
  "lastName": "string",
  "email": "string",
  "techStack": "string",
  "status": 0
}

Error on post above body
"errors": {
    "Email": [
      "The Email field is not a valid e-mail address."
    ]
  },

### Limitation:
- We are using InMemory so its not persistant , and cleans when the server restarts 

### Future Scope
- Connect the DBMS to maintain persistant storage of the data
- Create the endpoint to handle the media files.