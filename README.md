# Task Management System

A full-stack web application built to demonstrate Clean Architecture, Test-Driven Development (TDD), and modern containerized deployment. 

## Informal User Story
**Title:** Secure Task Management Dashboard

**User Story:**
> *"As an user, I need a simple, secure way to track my daily tasks with clear due dates and statuses so that I can never miss a deadline."*

### Acceptance Criteria

* **AC1: Secure Access**
  * **Given** an unauthenticated user, **when** they navigate to the application, **then** they must be prompted to log in.
  * **Given** an authenticated user, **when** they log in, **then** they should only see their own tasks.
* **AC2: Task Creation**
  * **Given** a logged-in user, **when** they click "Create Task", **then** they can input a Title, Description, Due Date, and Status.
  * **Constraint:** The system must reject the creation if the Title or Due Date is missing.
  * **Constraint:** The system should not allow due dates in the past.
* **AC3: Task Dashboard (Read)**
  * **Given** a logged-in user with existing tasks, **when** they view the dashboard, **then** they should see a responsive grid/list of their tasks clearly displaying the status and due date.
* **AC4: Task Modification (Update)**
  * **Given** an existing task, **when** the user clicks "Edit", **then** they can modify the details or toggle the status between "Pending" and "Completed".
* **AC5: Task Removal (Delete)**
  * **Given** an existing task, **when** the user clicks "Delete", **then** the task is permanently removed from their dashboard and the database.

### Technical Details:
* **Architecture:** Must adhere to Clean Architecture principles (API, Application, Domain, Infrastructure).
* **Data Access:** Must use raw ADO.NET ('SqliteConnection', 'SqlCommand'). The use of ORMs like Entity Framework or Dapper is not allowed.
* **Testing:** All layers (Data, Business Logic, API) must be covered by automated unit tests using xUnit and Moq.
* **Quality:** Zero warnings or errors in the browser console. Code must be structured and cleanly separated.

##  Architecture & Tech Stack

This project adheres to **Clean Architecture** principles, separating concerns into Domain, Application, Infrastructure, and API layers. 

**Backend:**
* **Framework:** .NET 9.0 ASP.NET Core Web API
* **Database:** SQLite
* **Data Access:** Raw ADO.NET (`SqliteConnection`, `SqlCommand`).
* **Testing:** xUnit and Moq 
* **Security:** JWT Authentication.

**Frontend:**
* React with TypeScript (bootstrapped via Vite)
* Material-UI
* Axios

**Infrastructure:**
* Docker & Docker Compose (Multi-stage builds, Nginx for the frontend).

---

##  Quick Start

The easiest way to review and run this application is via Docker.

1. Ensure [Docker Desktop](https://www.docker.com/products/docker-desktop/) is installed and running.
2. Clone this repository and open a terminal in the root directory.
3. Run the following command to build and start the containers:

```bash
docker-compose up -d --build
```

Alternatively, you can manually run the API and frontend using visual studio and npm run dev
