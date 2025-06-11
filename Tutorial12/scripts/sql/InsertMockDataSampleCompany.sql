INSERT INTO Departemnt (Name, Location) VALUES
('Human Resources', 'New York'),
('Finance', 'Chicago'),
('IT', 'San Francisco'),
('Sales', 'Los Angeles'),
('Engineering', 'Seattle');

INSERT INTO Job (Name) VALUES
('Manager'),
('Developer'),
('Analyst'),
('Accountant'),
('Designer'),
('Engineer'),
('Salesperson'),
('HR Specialist'),
('Data Scientist'),
('IT Support');

INSERT INTO Employee (Name, JobId, ManagerId, HireDate, Salary, Commission, DepartmentId) VALUES
('John Doe', 1, NULL, '2020-03-15', 85000.00, 5000.00, 1),
('Jane Smith', 2, 1, '2021-07-22', 95000.00, NULL, 3),
('Mike Johnson', 3, 2, '2018-04-05', 65000.00, 2000.00, 2),
('Sarah Lee', 4, NULL, '2019-11-14', 70000.00, NULL, 2),
('Chris Brown', 5, 3, '2022-01-18', 60000.00, 3000.00, 4),
('Emily Davis', 6, 1, '2020-09-07', 80000.00, NULL, 5),
('Robert Wilson', 7, 2, '2021-03-03', 55000.00, 1000.00, 4),
('Jessica Taylor', 8, 3, '2023-06-19', 50000.00, NULL, 1),
('David Martinez', 9, NULL, '2017-08-11', 75000.00, 4000.00, 3),
('Linda Anderson', 10, 6, '2022-12-28', 62000.00, NULL, 5);


