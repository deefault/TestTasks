use sample;

INSERT INTO sample.dbo.Employee (Fullname) VALUES (N'qwe');
INSERT INTO sample.dbo.Employee (Fullname) VALUES (N'2');
INSERT INTO sample.dbo.Employee (Fullname) VALUES (N'3');

INSERT INTO sample.dbo.EmployeeEntrance (EmployeeID, DateEnter, DateExit) VALUES (1, N'2021-01-20 13:43:55.000', N'2021-01-20 15:43:55.000');
INSERT INTO sample.dbo.EmployeeEntrance (EmployeeID, DateEnter, DateExit) VALUES (1, N'2021-01-20 17:43:55.000', N'2021-01-20 18:43:55.000');
INSERT INTO sample.dbo.EmployeeEntrance (EmployeeID, DateEnter, DateExit) VALUES (2, N'2021-01-20 17:43:55.000', N'2021-01-20 18:43:55.000');
INSERT INTO sample.dbo.EmployeeEntrance (EmployeeID, DateEnter, DateExit) VALUES (2, N'2021-01-20 20:00:00.000', null);

EXEC dbo.GetEmployeeSessions @From = '2021-01-20 13:00:00.000',
     @To = '2021-01-20 23:00:00.000'