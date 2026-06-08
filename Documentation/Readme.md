3-Day Task
## Day 1:
### Goal
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
## Day 2:
### Goal
- Add Data Validations to the fields
- Add Proper Error Messages and Error Codes
- Create the update API
- Create the Delete API
- Use the DTOs to send Response and the Data communication between controllers and services
- Use the services to write the modular code so that code remains losely coupled

## API ENDPOINTS
- GET /api/health 
- GET /api/trainees  (get all trainees)

- GET /api/trainees/:id (get trainee by id)
    - /api/trainees/1

- POST /api/trainees  
    - Sample Json
    {
        "firstName": "ujas",
        "lastName": "makwana",
        "email": "ujas.makwana@zeuslearning.com",
        "techStack": "dotnet",
        "status": 0
    }

- PUT /api/trainees/:id
    - /api/trainees/1
    - Sample Json
    {
        "firstName": "ujas",
        "lastName": "makwana",
        "email": "ujas.makwana@zeuslearning.com",
        "techStack": "dotnet",
        "status": 2
    }
    
- DELETE /api/trainees/:id
    - /api/trainees/1

    
### To Run Project
- dotnet restore
- dotnet run

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

