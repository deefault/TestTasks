create database sample;
GO

Use sample;
GO
CREATE TABLE Employee
(
    EmployeeID Int PRIMARY KEY IDENTITY,
    Fullname   nvarchar(100) not null
);

CREATE TABLE EmployeeEntrance
(
    ID         Int PRIMARY KEY IDENTITY,
    EmployeeID int      not null references Employee (EmployeeID),
    DateEnter  datetime not NULL default (GETDATE()),
    DateExit   datetime null,
);

CREATE INDEX EmployeeEntrance_DateEnter_idx on EmployeeEntrance (DateEnter DESC, DateExit Desc);
CREATE INDEX EmployeeEntrance_UserID_idx on EmployeeEntrance (EmployeeID);

CREATE OR
ALTER PROCEDURE GetEmployeeSessions(@From datetime, @To datetime)
AS
BEGIN
    SELECT EmployeeID, SUM(d)
    from (
             SELECT EmployeeID, DateEnter, DateExit,
                    CASE
                        when DateEnter > @From and COALESCE(DateExit, @To) <= @To
                            THEN DATEDIFF(second , DateEnter, COALESCE(DateExit, @To))
                        when DateEnter < @From and COALESCE(DateExit, @To) > @From
                            THEN DATEDIFF(second, @From, COALESCE(DateExit, @To))
                        when COALESCE(DateExit, @To) >= @To and DateEnter < @To
                            THEN DATEDIFF(second, @From, COALESCE(DateExit, @To))
                        ELSE 0
                        END
                        as d
             FROM EmployeeEntrance
             WHERE DateEnter < @To
               and @From <= COALESCE(DateExit, @To)
         ) q1
    GROUP BY EmployeeID
END 
