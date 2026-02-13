# Smart Event Planning Web Platform

A web-based intelligent event planning system developed using ASP.NET Core MVC and React.  
This project was implemented as part of the Software Laboratory course.

## Project Overview

The Smart Event Planning Platform enables users to discover, create and manage events through a role-based web application.  
The system includes a rule-based recommendation engine, time conflict detection algorithm, messaging functionality and a gamification-based point system.

The architecture follows a layered structure with ASP.NET Core MVC as the backend and a React-based frontend integrated under the clientapp directory.

## Technologies Used

Backend:
- ASP.NET Core MVC
- C#
- Entity Framework Core
- SQL Server

Frontend:
- React
- JavaScript
- REST-based communication

Other:
- Role-based authorization
- Relational database design (normalized schema)

## Core Features

### User Management
- User registration and login
- Role-based authorization (Admin / User)
- Secure session handling

### Event Management
- Event creation, update and deletion
- Category-based organization
- Event participation system

### Recommendation System
The platform includes a rule-based recommendation engine that suggests events to users based on:

- User interests
- Participation history
- Event categories
- Additional rule constraints

### Time Conflict Detection
When a user attempts to join an event, the system checks for schedule conflicts with previously joined events.

Algorithm steps:
1. Retrieve user's existing event time ranges
2. Compare date and time intervals
3. Detect overlap
4. Prevent registration if conflict exists

### Messaging System
- User-to-user communication
- Admin interaction
- Message persistence in database

### Gamification
- Users earn points by participating in events
- Points contribute to engagement tracking

## Project Structure

