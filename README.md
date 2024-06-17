# Net-exercise

## Description

This is a simple .Net Core app for demo purposes.

## Main stack

* .Net Core 8.0
* Entity Framework Core
* AutoMapper
* Swagger
* XUnit
* Moq
* FluentAssertions

## Folder structure

* Controllers
* Data
* Dtos
* Mapper
* Migrations
* Models
* Tests

## Instalation guide

1. Clone the repository.
2. Open the solution "CodeChallenge.sln" (Visual Studio recommended).
3. Edit the "DefaultConnection" in the appsettings.json to establish the database connection.
4. Open the Package Manage Console and run the command `Update-Database` to execute the migrations.
5. Run the project. It should open a browser in port 5149 with the Swagger documentation as default (http://localhost:5149/swagger/index.html).

## Considerations

Much of the application logic was included directly in the controllers. The main reason for this was simplicity, trying to save time, facilitate testing and minimize error, considering that it is a demo application. As improvements, this logic could be separated into different layers, such as validators, searchers, factories, etc. It was also decided to leave all the code in a single project, also for simplicity reasons. As an improvement, it could be separated into different projects.

