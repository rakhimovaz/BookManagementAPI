## Overview

The Book Management API is a RESTful API developed using ASP.NET Core that allows users to manage books. This API provides functionality to add, update, delete, and retrieve book information.

## Features

- **CRUD Operations**: Create, Read, Update, and Delete books.
- **Database Support**: Utilizes SQL Server for data storage.
- **RESTful Design**: Follows REST principles for API design.
- **Swagger Documentation**: Automatically generated API documentation for easy testing and exploration.

## Getting Started

### Prerequisites

-.NET SDK (8 or 9)
- SQL Server
- Visual Studio or any preferred IDE

 Installation

1. Clone the repository:
   ```bash
clone the repository 
cd BookManagementAPI
   ```

2. Restore the dependencies:
   ```bash
   dotnet restore
   ```

3. Update the `appsettings.json` file with your SQL Server connection string.

4. Run the migrations to set up the database:
   ```bash
   dotnet ef database update
   ```

5. Start the API:
   ```bash
   dotnet run
   ```

## API Endpoints

- **Add Book**: `POST /api/books`
- **Get All Books**: `GET /api/books`
- **Get Book by ID**: `GET /api/books/{id}`
- **Update Book**: `PUT /api/books/{id}`
- **Delete Book**: `DELETE /api/books/{id}`

