CREATE TABLE Clients
(
    ClientId INT PRIMARY KEY IDENTITY(1, 1),
    FirstLastName NVARCHAR(255) NOT NULL,
	PhoneNumber VARCHAR(20) NOT NULL,
	EMail VARCHAR(60),
	Address VARCHAR(90), 
	BirthDay  DATE
);