USE DBNW;

SET SQL_SAFE_UPDATES = 0;

/*
-- Clear previously made tables
DELETE FROM `User-Login-Join`;
DELETE FROM UserInfo;
DELETE FROM Login;
*/

-- Generate and insert 13 users
INSERT INTO `UserInfo` (UserID, Surname, Fornames, Title, Position, Phone, Email, Location)
VALUES 
('User1','Smith','John William Michael','Mr', 'Student, 2nd Year, Computer Scientist', '+441233456789', 'user1@domain.com, user1@compsci.com', 'at Home' ),
('User2','Blogs','Joe','Ms', 'Lecturer, Staff, Computer Scientist', '+441482465222', 'User2@domain.com, User2@university.com,admin@university.com', 'in Univeristy' ),
('User3','Holmes','Sherlock','Mr', 'Detective, External', '+447259340152', 'user3@domain.com, sherlock@holmes.com, admin@university.com', 'at Baker Street' ),
('User4', 'Smith', 'Gerald Ethan', 'Mr.', 'Sales Manager', '+123456789022', 'john@gmail.com, john.smith@company1.com', 'in London'),
('User5', 'Johnson', 'Alice Lily', 'Mrs.', 'Marketing Specialist', '+987654321016', 'alice@yahoo.com', 'at New York Office'),
('User6', 'Brown', 'Robert', 'Dr.', 'Software Developer', '+112233445533', 'robert@hotmail.com, rbrown@techco.net', 'in Paris'),
('User7', 'Davis', 'Emily', 'Ms.', 'Accountant', '+987123456718', 'emily@financials.com', 'at Berlin Office'),
('User8', 'Wilson', 'David', 'Mr.', 'HR Manager', '+123098765434', 'david@hrgroup.org', 'in Sydney'),
('User9', 'Martinez', 'Sophia', 'Miss', 'Product Manager', '+554433221164', 'sophia@products.co', 'at Tokyo Office'),
('User10', 'Garcia', 'Oliver', 'Mr.', 'Graphic Designer', '+334455667764', 'oliver@designs.org, ogarcia@creative.net', 'in Rome'),
('User11', 'Lopez', 'Sophia', 'Mrs.', 'Data Analyst', '+112233445524', 'lily@dataanalytics.com', 'at Amsterdam Office'),
('User12', 'Perez', 'Aiden', 'Dr.', 'Project Manager', '+991122334498', 'aiden@projectsolutions.net', 'in Madrid'),
('User13', 'Adams', 'Emma', 'Ms.', 'Customer Support Specialist', '+456789123042', 'emma@customersupport.co, eadams@helpdesk.com', 'at Sydney Office');



-- Insert Login IDs for the 13 users
INSERT INTO `Login` (LoginID)
VALUES 
('User1log'),
('User2log'),
('User3log'),
('User4log'),
('User5log'),
('User6log'),
('User7log'),
('User8log'),
('User9log'),
('User10log'),
('User11log'),
('User12log'),
('User13log');

-- Join users with their login IDs
INSERT INTO `User-Login-Join` (UserID, LoginID)
VALUES 
('User1', 'User1log'),
('User2', 'User2log'),
('User3', 'User3log'),
('User3', 'admin'),
('User4', 'User4log'),
('User5', 'User5log'),
('User6', 'User6log'),
('User6', 'admin'),
('User7', 'User7log'),
('User8', 'User8log'),
('User9', 'User9log'),
('User10', 'User10log'),
('User11', 'User11log'),
('User12', 'User12log'),
('User13', 'User13log');


SELECT * FROM UserInfo;
SELECT * FROM `User-Login-Join`;
SELECT * FROM Login;




