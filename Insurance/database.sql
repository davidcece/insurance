create table Clients(
	Id NVARCHAR(50) PRIMARY KEY,
	Name NVARCHAR(300),
	Dob NVARCHAR(20),
	DateOfIssue NVARCHAR(20),
	ExcelFile NVARCHAR(200),
	CarFile NVARCHAR(200),
	HasPartner BIT
	)

