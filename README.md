# Quote Management API

A **.NET Web API** backend for managing quotes with role-based access using **Entity Framework**, **SQLite**, and **JWT tokens** for authentication. The API provides endpoints for managing quotes, user registration, and authentication.

## Frontend Repository

The frontend for this API is built using React and TypeScript with the Mantine component library. You can find the frontend repository here:

[Frontend Repository - quotes-manager-proj-react-FE](https://github.com/kal1nadam/quotes-manager-proj-react-FE)


## Features

- **CRUD Operations**: Create, read, update, and delete quotes.
- **Role-based Authorization**:
  - Admin: Full access to manage all quotes.
  - Regular User: Access to manage only their own quotes.
- **Authentication**: JWT tokens are used for secure user authentication and authorization.
- **User Management**: User registration, login, and role assignment (admin or regular user).
- **Database**: **SQLite** is used for data persistence.
- **Entity Framework**: Used as the ORM to interact with the SQLite database.

## Tech Stack

- **Backend**: .NET Web API
- **Database**: SQLite
- **ORM**: Entity Framework Core
- **Authentication**: JWT tokens
- **Authorization**: Identity module for role-based access

## Setup Instructions

### Prerequisites

Ensure you have the following installed on your local development environment:

- .NET 6+
- A code editor like Visual Studio or VS Code

### Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/kal1nadam/kal1nadam-quotes-manager-api-dotnet-BE.git
   ```

2. Restore dependencies:

   ```bash
   dotnet restore
   ```

3. Apply migrations to set up the SQLite database:
   
   ```bash
   dotnet ef database update
   ```

4. Run the API:

   ```bash
   dotnet run
   ```


The API should now be running on [http://localhost:7173](http://localhost:7173).

## JWT Authentication

The API uses JWT tokens for secure authentication. After registering or logging in, the API will return a JWT token, which must be included in the `Authorization` header for subsequent requests to protected endpoints.


## Endpoints

- **POST** `/Auth/Register`: Register a new user.
- **POST** `/Auth/Login`: Authenticate a user and receive a JWT tokens (access and refresh).
- **POST** `/Auth/GetAccessToken`: Generates a new access token when a valid refresh token provided.
- **GET** `/Quotes/random`: Pick and returns a random quote.
- **GET** `/Quotes`: Get a list of all quotes.
- **POST** `/Quotes`: Create a new quote with signed user as an owner.
- **GET** `/Quotes/tags`: Get a list of all quotes with tags provided in the query params.
- **GET** `/Quotes/{userId}`: Get all quotes owned by the user.
- **GET** `/Quotes/{userId}/tags`: Get all quotes owned by the user and with tags provided in the query params.
- **PUT** `/Quotes/{quoteId}`: Update an existing quote (user either has to own the quote or has to have the admin role).
- **DELETE** `/api/quotes/{id}`: Delete a quote user either has to own the quote or has to have the admin role).

## Database

The project uses **SQLite** as the database provider, and **Entity Framework Core** for ORM. Migrations are applied to set up the necessary tables.

## Usage

1. **Register** or **Login** to obtain a JWT tokens.
2. Admin users can manage all quotes, while regular users can only manage their own.
3. Add, update, or delete quotes through the provided API endpoints.
   

## Contributions

Feel free to fork the repository and submit pull requests. Contributions are welcome to improve the API!


   
