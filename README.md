# Customer Register

This API was made as part of Ilia's challenge. About this solution:

* This solutions uses .NET 5 and C# 9.0.
* The swagger implementation was made with the package Swashbuckle, therefore, you need to run the application in order to see the Swagger UI.
* It uses unit tests and integration tests with xUnit.
* The architecture used is base on the Clean Architecture, proposed by Robert Martin.
* In order to accomplish customers living together, it was used a many-to-many relationship between customers and addresses.
* In order to share local phones between customers that live together, the local phones are stored in a separeted table with a many-to-one relationship with the addresses.
