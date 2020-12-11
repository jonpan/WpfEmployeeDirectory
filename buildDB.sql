-- by: Jon Pannaman
-- sql file to create SQL Server Database for Employee Directory test app




-- drop database
ALTER DATABASE EmployeeDB SET OFFLINE WITH ROLLBACK IMMEDIATE;
GO
DROP DATABASE IF EXISTS EmployeeDB;
GO


-- create database

CREATE DATABASE EmployeeDB;
GO

USE EmployeeDB;
GO
CREATE TABLE employeeInfo (
	employeeID INT IDENTITY (1,1) PRIMARY KEY,
	firstName VARCHAR (255) NOT NULL,
	lastName VARCHAR (255) NOT NULL,
	title VARCHAR (255) NOT NULL,
	startDate DATE NOT NULL
);

-- insert a few sample employees


INSERT INTO employeeInfo (firstName, lastName, title, startDate) VALUES ('John', 'Doe', 'Support Manager', '1/1/2010');
INSERT INTO employeeInfo (firstName, lastName, title, startDate) VALUES ('Jane', 'Smith', 'VP Marketing', '11/7/2007');
INSERT INTO employeeInfo (firstName, lastName, title, startDate) VALUES ('Sarah', 'Jones', 'Product Manager', '3/11/2019');


