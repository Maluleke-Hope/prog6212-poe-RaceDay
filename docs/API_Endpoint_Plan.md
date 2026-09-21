RaceDay System - API Plan

This document explains how the RaceDay system shares information between the website, mobile app, and database.

 API Endpoints List

| Action | Address | What It Does | Who Can Use It | What You Send | What You Get Back |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **GET** | `/api/v1/Events` | Shows a list of all upcoming events. | Everyone | Nothing (optional search filter like event type) | **200 OK**: List of all events |
| **GET** | `/api/v1/Events/{id}` | Shows details for one specific event. | Everyone | The Event ID number | **200 OK**: Event details<br>**404 Not Found**: Event does not exist |
| **POST** | `/api/v1/Events` | Creates a new race event. | Event Organisers | Event title, date, location, and description | **201 Created**: Saved event details<br>**400 Bad Request**: Missing required info |
| **PUT** | `/api/v1/Events/{id}` | Updates an existing event's details. | Event Organisers | Event ID and updated information | **204 No Content**: Update successful<br>**404 Not Found**: Event not found |
| **DELETE** | `/api/v1/Events/{id}` | Removes an event from the system. | Event Organisers | The Event ID number | **204 No Content**: Event deleted successfully<br>**404 Not Found**: Event not found |
| **POST** | `/api/v1/Events/{eventId}/Categories` | Adds race distances (e.g., 5km, 10km, 21km) to an event. | Event Organisers | Event ID, category name, distance, and ticket price | **201 Created**: Saved category details<br>**404 Not Found**: Event not found |
| **GET** | `/api/v1/Events/{eventId}/Categories` | Shows all race categories for an event. | Everyone | The Event ID number | **200 OK**: List of race categories for that event |
| **POST** | `/api/v1/Enrolments` | Signs up a runner or cyclist for an event category. | Participants | Event ID and selected Category ID | **201 Created**: Confirmation and assigned Bib Number<br>**400 Bad Request**: Category is full |
| **GET** | `/api/v1/Enrolments/me` | Shows a user all the events they have signed up for. | Logged-in Participants | User login token | **200 OK**: List of my event registrations<br>**401 Unauthorized**: Please log in first |
| **POST** | `/api/v1/Results` | Saves a runner's finish time and rank. | Event Organisers | Registration ID, finish time, and rank | **201 Created**: Result saved successfully<br>**400 Bad Request**: Incorrect time format |
| **GET** | `/api/v1/Events/{eventId}/Results` | Shows the final race leaderboard. | Everyone | The Event ID number | **200 OK**: List of finish times sorted by rank |
| **POST** | `/api/v1/Auth/register` | Creates a new account (Organiser or Participant). | New Users | Name, email address, password, and role | **201 Created**: Account created successfully<br>**400 Bad Request**: Email already taken or invalid password |
| **POST** | `/api/v1/Auth/login` | Logs a user into their account. | Existing Users | Email address and password | **200 OK**: Login token to access the system<br>**401 Unauthorized**: Wrong email or password |