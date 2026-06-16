# TraineeManagement.Api

## Project Overview

TraineeManagement.Api is an ASP.NET Core Web API developed for managing trainee records.

The project demonstrates:

- ASP.NET Core Web API
- Controllers
- DTOs
- Service Layer
- EF Core with MySQL (Code First)
- CRUD Operations
- Validation
- JWT Authentication
- Swagger

## Technology Stack

- C#
- .NET 9
- ASP.NET Core Web API
- Entity Framework Core
- MySQL
- JWT Authentication
- Swagger

## Project Structure
```text
├── Controllers/
│   ├── HealthController.cs
│   ├── TraineesController.cs
│   ├── UsersController.cs
│   ├── MentorsController.cs
│   ├── LearningTasksController.cs
│   ├── TaskAssignmentsController.cs
│   ├── SubmissionsController.cs
│   └── ReviewsController.cs
├── DTOs/
│   ├── TraineeDTO.cs
│   ├── UserDTO.cs
│   ├── MentorDTO.cs
│   ├── LearningTaskDTO.cs
│   ├── TaskAssignmentDTO.cs
│   ├── SubmissionDTO.cs
│   └── ReviewDTO.cs
├── Data/
│   └── AppDbContext.cs
├── Interfaces/
│   ├── IDateTimeAuto.cs
│   ├── ITraineeService.cs
│   ├── IUserService.cs
│   ├── IMentorService.cs
│   ├── ILearningTaskService.cs
│   ├── ITaskAssignmentService.cs
│   ├── ISubmissionService.cs
│   └── IReviewService.cs
├── Middlewares/
│   └── GlobalExceptionMiddleware.cs
├── Migrations/
├── Models/
│   ├── Trainee.cs
│   ├── User.cs
│   ├── Mentor.cs
│   ├── LearningTask.cs
│   ├── TaskAssignment.cs
│   ├── Submission.cs
│   └── Review.cs
├── Properties/
│   └── launchSettings.json
├── utils/
│   ├── AppException.cs
│   ├── JwtService.cs
│   └── SeedService.cs
├── Program.cs
├── TraineeManagement.Api.csproj
├── TraineeManagement.Api.http
├── appsettings.json
└── appsettings.Development.json
```

---

## Phase 1
### Day 1:
#### Goal
- Create the Health endpoint to get the status of the server
- Create the Trainee class with the following fields
  - Id - Unique & AUTO Generate
  - FirstName
  - LastName
  - Email
  - TechStack
  - Status
  - CreatedDate - AUTO Generate
  - UpdatedDate - AUTO Generate
- Create the apis to (getall, getbyid, add) trainee
- Get the swagger running and test apis

### Day 2:
#### Goal
- Add Data Validations to the fields
- Add Proper Error Messages and Error Codes
- Create the update API
- Create the Delete API
- Use the DTOs to send Response and the Data communication between controllers and services
- Use the services to write the modular code so that code remains loosely coupled

### Day 3:
#### Goal
- Use the EF Core Framework & Connect the Inmemory storage
- Create the AppDbContext using DbContext and the Dbset such that easy to move to the persistent DBMS
- Create the search api /api/trainees?search=value
- Complete the task, upload to github and documentation

---

## Phase 2
### Day 1: MySQL Database with EF Core Code First
#### Goal
- Replace EF Core InMemory DB with MySQL
- Read connection string from appsettings.json (no hardcoding)
- Create and apply EF Core migration for the Trainee table
- Verify all existing trainee CRUD APIs work with MySQL
- Update README with MySQL setup and migration commands

### Day 2: User/Auth, Password Hashing, and JWT
#### Goal
- Create User entity with fields: Id, Username, Email, PasswordHash, Role, CreatedDate, UpdatedDate
- Never store plain text passwords - only store PasswordHash
- Create migration for Users table
- Seed at least one Admin user for testing
- Create Login API (POST /api/auth/login)
- Generate JWT token with claims: UserId, Username, Role
- JWT config (Issuer, Audience, ExpiryMinutes) read from appsettings.json

### Day 3: Protected APIs, Pagination, CORS, Logging
#### Goal
- Protect all trainee APIs with JWT (no token = 401)
- Add pagination + search + status filter to GET /api/trainees
- Use Skip() and Take() for pagination, never return large datasets blindly
- Configure CORS for React frontend origins (localhost:3000, localhost:5173)
- Add structured logging for login, CRUD events, not-found cases
- Never log passwords, JWT tokens, or sensitive data

### Day 4: Mentor and Learning Task APIs
#### Goal
- Create Mentor module (Id, FirstName, LastName, Email, Expertise, Status, CreatedDate, UpdatedDate)
- Create CRUD APIs for mentors
- Create Learning Task module (Id, Title, Description, ExpectedTechStack, DueDate, Status, CreatedDate, UpdatedDate)
- Create CRUD APIs for learning tasks
- DTOs and validations for both modules
- All APIs protected with JWT
- Migrations updated for both tables

### Day 5: Task Assignment, Submission, Review APIs
#### Goal
- Create Task Assignment module - links Trainee, Mentor, LearningTask with AssignedDate, DueDate, Status, Remarks
- Validate that TraineeId, MentorId, LearningTaskId exist before creating assignment
- DueDate must not be before AssignedDate
- Create Submission module - links to TaskAssignment with SubmissionUrl, Notes, SubmittedDate, Status
- Create Review module - links to Submission with Feedback, Score, ReviewStatus, ReviewedDate
- Add Global Exception Handling Middleware - return safe error messages, never expose stack traces
- OWASP API Security checklist completed

---

## API Endpoints

### Public
- GET /api/health
- POST /api/auth/login

### Trainees (JWT Protected)
- GET /api/trainees?pageNumber=1&pageSize=10&search=value&status=Active
- GET /api/trainees/:id
- POST /api/trainees
- PUT /api/trainees/:id
- DELETE /api/trainees/:id

### Mentors (JWT Protected)
- GET /api/mentors
- GET /api/mentors/:id
- POST /api/mentors
- PUT /api/mentors/:id
- DELETE /api/mentors/:id

### Learning Tasks (JWT Protected)
- GET /api/learning-tasks
- GET /api/learning-tasks/:id
- POST /api/learning-tasks
- PUT /api/learning-tasks/:id
- DELETE /api/learning-tasks/:id

### Task Assignments (JWT Protected)
- POST /api/task-assignments
- GET /api/task-assignments
- GET /api/task-assignments/:id
- PUT /api/task-assignments/:id/status

### Submissions (JWT Protected)
- POST /api/submissions
- GET /api/submissions
- GET /api/submissions/:id

### Reviews (JWT Protected)
- POST /api/reviews
- GET /api/reviews
- GET /api/reviews/:id

---

## To Run Project
```bash
git clone https://github.com/ujasmakwana19/trainee-management-api
dotnet restore
dotnet clean
dotnet build
dotnet ef database update
# if you dont have ef tools installed
dotnet tool install --global dotnet-ef
dotnet run
# open swagger
https://localhost:<port>/swagger
```

---

## MySQL Setup

```json
// appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;port=3306;database=trainee_management_db;user=root;password=your_password;"
  },
  "Jwt": {
    "Issuer": "TraineeManagementApi",
    "Audience": "TraineeManagementClient",
    "ExpiryMinutes": 60
  }
}
```

```bash
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

---

## JWT Usage

**Login Request**
```json
POST /api/auth/login
{
  "username": "admin",
  "password": "Admin@123"
}
```

**Login Response**
```json
{
  "token": "jwt-token-value",
  "expiresIn": 3600,
  "user": {
    "id": 1,
    "username": "admin",
    "role": "Admin"
  }
}
```

**Using the token**
```
Authorization: Bearer <token>
```

---

## Sample Responses

**Trainee Response**
```json
{
  "id": 1,
  "firstName": "Amit",
  "lastName": "Sharma",
  "email": "amit.sharma@training.com",
  "techStack": "HTML, CSS, JavaScript",
  "status": "Active"
}
```

**Paginated GET Response**
```json
{
  "pageNumber": 1,
  "pageSize": 10,
  "totalRecords": 25,
  "data": []
}
```

---

## Security Checklist
- JWT authentication enabled
- All non-public APIs require valid token
- Passwords stored as hash only (BCrypt)
- DTOs used - PasswordHash never returned in responses
- EF Core used - no unsafe raw SQL
- CORS restricted to expected origins
- Secrets not hardcoded in controllers
- Stack traces not returned in error responses
- Passwords and tokens not logged
- Global exception handling middleware added