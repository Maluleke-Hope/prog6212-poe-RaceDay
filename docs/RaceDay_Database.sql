-- ============================================================
-- Project: RaceDay Event Management System
-- full schema creation and seed data
-- ============================================================

IF OBJECT_ID('dbo.Results', 'U') IS NOT NULL DROP TABLE dbo.Results;
IF OBJECT_ID('dbo.Enrolments', 'U') IS NOT NULL DROP TABLE dbo.Enrolments;
IF OBJECT_ID('dbo.Categories', 'U') IS NOT NULL DROP TABLE dbo.Categories;
IF OBJECT_ID('dbo.Events', 'U') IS NOT NULL DROP TABLE dbo.Events;
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL DROP TABLE dbo.Users;

-- 1. Users Table
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(150) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(256) NOT NULL,
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('Organiser', 'Participant')),
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- 2. Events Table (Includes Description, Distance and EventType (par 1 fix))
CREATE TABLE Events (
    EventId INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(150) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
    Date DATETIME NOT NULL,
    Location NVARCHAR(200) NOT NULL,
    Distance FLOAT NOT NULL CHECK (Distance > 0),
    EventType NVARCHAR(50) NOT NULL CHECK (EventType IN ('Run', 'Walk', 'Cycle')),
    OrganiserId INT NOT NULL,
    FOREIGN KEY (OrganiserId) REFERENCES Users(UserId) ON DELETE CASCADE
);

-- 3. Categories Table
CREATE TABLE Categories (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    EventId INT NOT NULL,
    FOREIGN KEY (EventId) REFERENCES Events(EventId) ON DELETE CASCADE
);

-- 4. Enrolments Table 
CREATE TABLE Enrolments (
    EnrolmentId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    EventId INT NOT NULL,
    CategoryId INT NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Pending' CHECK (Status IN ('Pending', 'Confirmed', 'Cancelled')),
    EnrolmentDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (EventId) REFERENCES Events(EventId) ON DELETE CASCADE,
    FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId)
);

-- 5. Results Table
CREATE TABLE Results (
    ResultId INT IDENTITY(1,1) PRIMARY KEY,
    EnrolmentId INT NOT NULL UNIQUE,
    FinishTime NVARCHAR(20) NOT NULL,
    FinishingPosition INT NOT NULL CHECK (FinishingPosition > 0),
    FOREIGN KEY (EnrolmentId) REFERENCES Enrolments(EnrolmentId) ON DELETE CASCADE
);

-- Seed Data (2 Organisers, 2 Participants, 3 Events, Categories, Enrolments)
INSERT INTO Users (FullName, Email, PasswordHash, Role) VALUES
('Sipho Ndlovu', 'sipho@raceday.co.za', 'AQAAAAEAACcQAAAAEH5...', 'Organiser'),
('Anika Botha', 'anika@raceday.co.za', 'AQAAAAEAACcQAAAAEH5...', 'Organiser'),
('David Malan', 'david@gmail.com', 'AQAAAAEAACcQAAAAEH5...', 'Participant'),
('Lethabo Khumalo', 'lethabo@gmail.com', 'AQAAAAEAACcQAAAAEH5...', 'Participant');

INSERT INTO Events (Title, Description, Date, Location, Distance, EventType, OrganiserId) VALUES
('Soweto Marathon', 'Annual township marathon celebrating South African culture and endurance.', '2026-11-01 06:00:00', 'Soweto, Johannesburg', 42.2, 'Run', 1),
('Cape Town Cycle Tour', 'Iconic coastal cycling route around the Cape Peninsula.', '2027-03-14 06:30:00', 'Cape Town', 109.0, 'Cycle', 2),
('Gauteng Spring Walk', 'Family-friendly morning fitness walk in Johannesburg.', '2026-09-15 08:00:00', 'Pretoria Botanical Gardens', 5.0, 'Walk', 1);

INSERT INTO Categories (Name, EventId) VALUES
('Open Male', 1), ('Open Female', 1),
('109km Elite', 2), ('109km Open', 2),
('5km Fun Walk', 3);

INSERT INTO Enrolments (UserId, EventId, CategoryId, Status) VALUES
(3, 1, 1, 'Confirmed'),
(4, 1, 2, 'Confirmed'),
(3, 2, 4, 'Pending');

INSERT INTO Results (EnrolmentId, FinishTime, FinishingPosition) VALUES
(1, '03:15:42', 142),
(2, '03:45:10', 88);