
# TaskFlow — Task Management System

TaskFlow is a web-based task management application built with ASP.NET Core MVC. It helps users organize their daily work, manage tasks, track progress, and monitor deadlines.

## Features

- **User Authentication:** Register, log in, and log out securely.
- **Personal Task Management:** Each user can manage their own tasks.
- **Create, Read, Update, Delete (CRUD):** Create new tasks, view details, edit tasks, and delete them.
- **Search Tasks:** Find tasks by title.
- **Filter Tasks:** Filter tasks by status and priority.
- **Quick Status Update:** Change a task's status directly from the task list.
- **Dashboard:** View total, pending, in-progress, and completed tasks.
- **Progress Bar:** Track task completion progress.
- **Priority Management:** Organize tasks by priority.
- **Due Date Tracking:** View task deadlines and identify overdue tasks.

## Technologies Used

- C#
- ASP.NET Core MVC
- .NET 10
- Entity Framework Core
- SQLite
- ASP.NET Core Identity
- HTML
- CSS
- JavaScript
- Git and GitHub

## Getting Started

### Prerequisites

Install the following tools:

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Visual Studio Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/)

### Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/Ashiqulislam171/TaskFlow.git
   ```

2. Open the project folder:

   ```bash
   cd TaskFlow/TaskFlow
   ```

3. Restore the required packages:

   ```bash
   dotnet restore
   ```

4. Apply the database migrations if they have not already been applied:

   ```bash
   dotnet ef database update
   ```

5. Build the application:

   ```bash
   dotnet build
   ```

6. Run the application:

   ```bash
   dotnet run
   ```

7. Open the local URL displayed in the terminal in your browser.

## How to Use

1. Register a new account.
2. Log in to access your dashboard.
3. Create tasks with a title, description, status, priority, and optional due date.
4. Search for tasks or filter them by status and priority.
5. Update task statuses as your work progresses.
6. Edit or delete tasks when needed.
7. Use the dashboard to monitor your overall progress.

## Database

TaskFlow uses SQLite to store application data and Entity Framework Core to manage database operations. ASP.NET Core Identity handles user authentication.

## Security

Task operations are restricted to authenticated users. The application checks task ownership so users can access and manage only their own tasks.

## Future Improvements

- Task categories and tags
- Task sorting options
- Notifications for approaching deadlines
- Improved responsive UI
- Unit and integration testing

## Author

**Ashiqul Islam**

GitHub: [Ashiqulislam171](https://github.com/Ashiqulislam171)

---

*TaskFlow — Organize your work. Track your progress. Get things done.*

## Application Screenshot

### Dashboard

![TaskFlow Dashboard](wwwroot/images/dashboard.png)
