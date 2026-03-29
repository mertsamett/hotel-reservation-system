-- =============================================
-- HOTEL RESERVATION SYSTEM DATABASE SCHEMA
-- Database Systems Term Project - Fall 2025
-- Group 12
-- =============================================

USE master;
GO

-- Drop database if exists
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'HOTEL_MANAGEMENT')
BEGIN
    ALTER DATABASE HOTEL_MANAGEMENT SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE HOTEL_MANAGEMENT;
END
GO

CREATE DATABASE HOTEL_MANAGEMENT;
GO

USE HOTEL_MANAGEMENT;
GO

-- =============================================
-- TABLE 1: PERMISSION (Yetki Rolleri)
-- =============================================
CREATE TABLE PERMISSION (
    PerID VARCHAR(10) NOT NULL,
    PerRole CHAR(20) NOT NULL,
    PerName VARCHAR(30) NOT NULL,
    PRIMARY KEY (PerID),
    CONSTRAINT UQ_PerName UNIQUE(PerName)
);
GO

-- =============================================
-- TABLE 2: DEPARTMENT (Departmanlar)
-- =============================================
CREATE TABLE DEPARTMENT (
    DepartmentID INT IDENTITY(1,1) PRIMARY KEY,
    DepartmentName VARCHAR(50) NOT NULL,
    PerID VARCHAR(10),
    NumberOfEmployees INT DEFAULT 0,
    CreatedDate DATETIME DEFAULT GETDATE(),
    ModifiedDate DATETIME DEFAULT GETDATE(),
    CONSTRAINT UQ_DeptName UNIQUE(DepartmentName),
    CONSTRAINT FK_Dept_Perm FOREIGN KEY (PerID) REFERENCES PERMISSION (PerID)
);
GO

-- =============================================
-- TABLE 3: STAFF (Calisanlar)
-- =============================================
CREATE TABLE STAFF (
    StaffID INT IDENTITY(1,1) PRIMARY KEY,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    Email VARCHAR(100),
    Phone VARCHAR(20),
    DateOfBirth DATE,
    HireDate DATE DEFAULT GETDATE(),
    Salary DECIMAL(10,2),
    DepartmentID INT NOT NULL,
    PerID VARCHAR(10),
    CreatedDate DATETIME DEFAULT GETDATE(),
    ModifiedDate DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Staff_Dept FOREIGN KEY (DepartmentID) REFERENCES DEPARTMENT (DepartmentID),
    CONSTRAINT FK_Staff_Perm FOREIGN KEY (PerID) REFERENCES PERMISSION (PerID)
);
GO

-- =============================================
-- TABLE 4: ROOM_TYPE (Oda Tipleri)
-- =============================================
CREATE TABLE ROOM_TYPE (
    RoomTypeID INT IDENTITY(1,1) PRIMARY KEY,
    TypeName VARCHAR(50) NOT NULL,
    Capacity INT NOT NULL,
    PricePerNight DECIMAL(10,2) NOT NULL,
    Description VARCHAR(500),
    CreatedDate DATETIME DEFAULT GETDATE(),
    ModifiedDate DATETIME DEFAULT GETDATE(),
    CONSTRAINT UQ_TypeName UNIQUE(TypeName)
);
GO

-- =============================================
-- TABLE 5: ROOM (Odalar)
-- =============================================
CREATE TABLE ROOM (
    RoomID INT IDENTITY(1,1) PRIMARY KEY,
    RoomNumber VARCHAR(10) NOT NULL,
    RoomTypeID INT NOT NULL,
    Floor INT,
    IsAvailable BIT NOT NULL DEFAULT 1,
    Notes VARCHAR(500),
    CreatedDate DATETIME DEFAULT GETDATE(),
    ModifiedDate DATETIME DEFAULT GETDATE(),
    CONSTRAINT UQ_RoomNumber UNIQUE(RoomNumber),
    CONSTRAINT FK_Room_Type FOREIGN KEY (RoomTypeID) REFERENCES ROOM_TYPE (RoomTypeID)
);
GO

-- =============================================
-- TABLE 6: CUSTOMER (Musteriler)
-- =============================================
CREATE TABLE CUSTOMER (
    CustomerID INT IDENTITY(1,1) PRIMARY KEY,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    Phone VARCHAR(20) NOT NULL,
    Address VARCHAR(200),
    City VARCHAR(50),
    Country VARCHAR(50) NOT NULL,
    DateOfBirth DATE,
    TotalBookings INT DEFAULT 0,
    CreatedDate DATETIME DEFAULT GETDATE(),
    ModifiedDate DATETIME DEFAULT GETDATE(),
    CONSTRAINT UQ_CustomerEmail UNIQUE(Email)
);
GO

-- =============================================
-- TABLE 7: BOOKING (Rezervasyonlar)
-- =============================================
CREATE TABLE BOOKING (
    BookingID INT IDENTITY(1,1) PRIMARY KEY,
    CustomerID INT NOT NULL,
    RoomID INT NOT NULL,
    StaffID INT,
    CheckInDate DATE NOT NULL,
    CheckOutDate DATE NOT NULL,
    BookingDate DATETIME DEFAULT GETDATE(),
    TotalPrice DECIMAL(10,2) NOT NULL,
    BookingStatus VARCHAR(20) NOT NULL DEFAULT 'Active',
    Notes VARCHAR(500),
    CreatedDate DATETIME DEFAULT GETDATE(),
    ModifiedDate DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Booking_Customer FOREIGN KEY (CustomerID) REFERENCES CUSTOMER (CustomerID),
    CONSTRAINT FK_Booking_Room FOREIGN KEY (RoomID) REFERENCES ROOM (RoomID),
    CONSTRAINT FK_Booking_Staff FOREIGN KEY (StaffID) REFERENCES STAFF (StaffID),
    CONSTRAINT CHK_BookingStatus CHECK (BookingStatus IN ('Active', 'Completed', 'Cancelled')),
    CONSTRAINT CHK_CheckOutAfterCheckIn CHECK (CheckOutDate > CheckInDate)
);
GO

-- =============================================
-- TABLE 8: ROOM_SERVICE (Oda Servisi - WEAK ENTITY)
-- Weak Entity: BOOKING'e bagimli, Booking silinirse servisler de silinir
-- ON DELETE CASCADE ile bagimlilik saglanir
-- =============================================
CREATE TABLE ROOM_SERVICE (
    ServiceID INT IDENTITY(1,1) PRIMARY KEY,
    BookingID INT NOT NULL,
    ServiceType NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    Price DECIMAL(10,2) NOT NULL,
    ServiceDate DATETIME DEFAULT GETDATE(),
    CreatedDate DATETIME DEFAULT GETDATE(),
    -- WEAK ENTITY: Booking silinirse tum servisler de silinir (CASCADE)
    CONSTRAINT FK_RoomService_Booking FOREIGN KEY (BookingID)
        REFERENCES BOOKING (BookingID) ON DELETE CASCADE
);
GO

-- =============================================
-- INDEXES FOR PERFORMANCE
-- =============================================
CREATE INDEX idx_roomservice_booking ON ROOM_SERVICE(BookingID);
CREATE INDEX idx_booking_customer ON BOOKING(CustomerID);
CREATE INDEX idx_booking_room ON BOOKING(RoomID);
CREATE INDEX idx_booking_status ON BOOKING(BookingStatus);
CREATE INDEX idx_booking_dates ON BOOKING(CheckInDate, CheckOutDate);
CREATE INDEX idx_room_available ON ROOM(IsAvailable);
CREATE INDEX idx_room_type ON ROOM(RoomTypeID);
CREATE INDEX idx_staff_department ON STAFF(DepartmentID);
CREATE INDEX idx_customer_email ON CUSTOMER(Email);
GO

-- =============================================
-- TRIGGER 1: Departmana calisan eklendiginde sayiyi artir
-- =============================================
CREATE TRIGGER trg_IncrementEmployeeCount
ON STAFF
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE d
    SET NumberOfEmployees = NumberOfEmployees + 1,
        ModifiedDate = GETDATE()
    FROM DEPARTMENT d
    INNER JOIN inserted i ON d.DepartmentID = i.DepartmentID;
END;
GO

-- =============================================
-- TRIGGER 2: Departmandan calisan silindiginde sayiyi azalt
-- =============================================
CREATE TRIGGER trg_DecrementEmployeeCount
ON STAFF
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE d
    SET NumberOfEmployees = NumberOfEmployees - 1,
        ModifiedDate = GETDATE()
    FROM DEPARTMENT d
    INNER JOIN deleted del ON d.DepartmentID = del.DepartmentID;
END;
GO

-- =============================================
-- TRIGGER 3: Rezervasyon yapildiginda odayi DOLU yap
-- =============================================
CREATE TRIGGER trg_SetRoomUnavailable
ON BOOKING
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    -- Rezervasyon yapilan odayi dolu olarak isaretle
    UPDATE r
    SET IsAvailable = 0,
        ModifiedDate = GETDATE()
    FROM ROOM r
    INNER JOIN inserted i ON r.RoomID = i.RoomID
    WHERE i.BookingStatus = 'Active';

    -- Musterinin toplam rezervasyon sayisini artir
    UPDATE c
    SET TotalBookings = TotalBookings + 1,
        ModifiedDate = GETDATE()
    FROM CUSTOMER c
    INNER JOIN inserted i ON c.CustomerID = i.CustomerID;
END;
GO

-- =============================================
-- TRIGGER 4: Rezervasyon tamamlandiginda veya iptal edildiginde odayi MUSAIT yap
-- =============================================
CREATE TRIGGER trg_SetRoomAvailable
ON BOOKING
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Eger rezervasyon durumu 'Completed' veya 'Cancelled' olarak degistiyse
    UPDATE r
    SET IsAvailable = 1,
        ModifiedDate = GETDATE()
    FROM ROOM r
    INNER JOIN inserted i ON r.RoomID = i.RoomID
    INNER JOIN deleted d ON i.BookingID = d.BookingID
    WHERE d.BookingStatus = 'Active'
      AND (i.BookingStatus = 'Completed' OR i.BookingStatus = 'Cancelled')
      -- Ayni odada baska aktif rezervasyon yoksa musait yap
      AND NOT EXISTS (
          SELECT 1 FROM BOOKING b
          WHERE b.RoomID = i.RoomID
            AND b.BookingID != i.BookingID
            AND b.BookingStatus = 'Active'
      );
END;
GO

-- =============================================
-- TRIGGER 5: Rezervasyon silindiginde odayi MUSAIT yap
-- =============================================
CREATE TRIGGER trg_SetRoomAvailableOnDelete
ON BOOKING
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE r
    SET IsAvailable = 1,
        ModifiedDate = GETDATE()
    FROM ROOM r
    INNER JOIN deleted d ON r.RoomID = d.RoomID
    WHERE d.BookingStatus = 'Active'
      AND NOT EXISTS (
          SELECT 1 FROM BOOKING b
          WHERE b.RoomID = d.RoomID
            AND b.BookingStatus = 'Active'
      );
END;
GO

-- =============================================
-- VIEW 1: Otel Rezervasyon Ozeti (JOIN ornek)
-- =============================================
CREATE VIEW vw_BookingSummary AS
SELECT
    b.BookingID,
    c.FirstName + ' ' + c.LastName AS CustomerName,
    c.Email AS CustomerEmail,
    c.Phone AS CustomerPhone,
    r.RoomNumber,
    rt.TypeName AS RoomType,
    rt.PricePerNight,
    b.CheckInDate,
    b.CheckOutDate,
    DATEDIFF(DAY, b.CheckInDate, b.CheckOutDate) AS NightCount,
    b.TotalPrice,
    b.BookingStatus,
    s.FirstName + ' ' + s.LastName AS StaffName,
    b.BookingDate
FROM BOOKING b
INNER JOIN CUSTOMER c ON b.CustomerID = c.CustomerID
INNER JOIN ROOM r ON b.RoomID = r.RoomID
INNER JOIN ROOM_TYPE rt ON r.RoomTypeID = rt.RoomTypeID
LEFT JOIN STAFF s ON b.StaffID = s.StaffID;
GO

-- =============================================
-- VIEW 2: Musait Odalar
-- =============================================
CREATE VIEW vw_AvailableRooms AS
SELECT
    r.RoomID,
    r.RoomNumber,
    rt.TypeName,
    rt.Capacity,
    rt.PricePerNight,
    r.Floor,
    rt.Description
FROM ROOM r
INNER JOIN ROOM_TYPE rt ON r.RoomTypeID = rt.RoomTypeID
WHERE r.IsAvailable = 1;
GO

-- =============================================
-- VIEW 3: Oda Doluluk Istatistikleri (GROUPING ornek)
-- =============================================
CREATE VIEW vw_RoomOccupancyStats AS
SELECT
    rt.TypeName,
    COUNT(r.RoomID) AS TotalRooms,
    SUM(CASE WHEN r.IsAvailable = 1 THEN 1 ELSE 0 END) AS AvailableRooms,
    SUM(CASE WHEN r.IsAvailable = 0 THEN 1 ELSE 0 END) AS OccupiedRooms
FROM ROOM r
INNER JOIN ROOM_TYPE rt ON r.RoomTypeID = rt.RoomTypeID
GROUP BY rt.TypeName;
GO

-- =============================================
-- SAMPLE DATA - PERMISSION
-- =============================================
INSERT INTO PERMISSION (PerID, PerRole, PerName) VALUES
('P01', 'Admin', 'General Manager'),
('P02', 'Manager', 'Front Desk Manager'),
('P03', 'Staff', 'Receptionist'),
('P04', 'Staff', 'Concierge'),
('P05', 'Staff', 'Housekeeper');
GO

-- =============================================
-- SAMPLE DATA - DEPARTMENT
-- =============================================
INSERT INTO DEPARTMENT (DepartmentName, PerID) VALUES
('Management', 'P01'),
('Front Desk', 'P02'),
('Housekeeping', 'P05'),
('Concierge', 'P04'),
('Maintenance', 'P03');
GO

-- =============================================
-- SAMPLE DATA - ROOM_TYPE
-- =============================================
INSERT INTO ROOM_TYPE (TypeName, Capacity, PricePerNight, Description) VALUES
('Single Room', 1, 75.00, 'Comfortable room with single bed, TV, and WiFi'),
('Double Room', 2, 120.00, 'Spacious room with double bed, TV, minibar, and WiFi'),
('Twin Room', 2, 110.00, 'Room with two single beds, TV, and WiFi'),
('Suite', 3, 250.00, 'Luxury suite with living area, king bed, jacuzzi, and city view'),
('Family Room', 4, 180.00, 'Large room with one double and two single beds, ideal for families'),
('Penthouse', 4, 500.00, 'Top floor luxury apartment with panoramic view and premium amenities');
GO

-- =============================================
-- SAMPLE DATA - ROOM
-- =============================================
INSERT INTO ROOM (RoomNumber, RoomTypeID, Floor, IsAvailable) VALUES
('101', 1, 1, 1),
('102', 1, 1, 1),
('103', 2, 1, 1),
('201', 2, 2, 1),
('202', 3, 2, 1),
('203', 3, 2, 1),
('301', 4, 3, 1),
('302', 4, 3, 1),
('401', 5, 4, 1),
('501', 6, 5, 1);
GO

-- =============================================
-- SAMPLE DATA - STAFF (Trigger otomatik departman sayisini gunceller)
-- =============================================
INSERT INTO STAFF (FirstName, LastName, Email, Phone, DepartmentID, PerID, Salary) VALUES
('Ahmet', 'Yilmaz', 'ahmet.yilmaz@hotel.com', '0532-111-0001', 1, 'P01', 15000.00),
('Ayse', 'Kaya', 'ayse.kaya@hotel.com', '0533-222-0002', 2, 'P02', 8000.00),
('Mehmet', 'Demir', 'mehmet.demir@hotel.com', '0534-333-0003', 2, 'P03', 5000.00),
('Fatma', 'Celik', 'fatma.celik@hotel.com', '0535-444-0004', 3, 'P05', 4500.00),
('Ali', 'Ozturk', 'ali.ozturk@hotel.com', '0536-555-0005', 4, 'P04', 5500.00);
GO

-- =============================================
-- SAMPLE DATA - CUSTOMER
-- =============================================
INSERT INTO CUSTOMER (FirstName, LastName, Email, Phone, Address, City, Country) VALUES
('John', 'Doe', 'john.doe@email.com', '555-0101', '123 Main St', 'New York', 'USA'),
('Jane', 'Smith', 'jane.smith@email.com', '555-0102', '456 High St', 'London', 'UK'),
('Hans', 'Muller', 'hans.muller@email.com', '555-0103', 'Berliner Str 1', 'Berlin', 'Germany'),
('Marie', 'Curie', 'marie.curie@email.com', '555-0104', 'Rue de Paris 10', 'Paris', 'France'),
('Kemal', 'Yildiz', 'kemal.yildiz@email.com', '555-0105', 'Ataturk Bulvari 50', 'Ankara', 'Turkey'),
('Elena', 'Petrova', 'elena.petrova@email.com', '555-0106', 'Lenin St 15', 'Moscow', 'Russia'),
('Carlos', 'Garcia', 'carlos.garcia@email.com', '555-0107', 'Gran Via 20', 'Madrid', 'Spain'),
('Yuki', 'Tanaka', 'yuki.tanaka@email.com', '555-0108', 'Shibuya 1-1', 'Tokyo', 'Japan');
GO

-- =============================================
-- SAMPLE DATA - BOOKING (Trigger otomatik oda durumunu ve musteri sayisini gunceller)
-- =============================================
INSERT INTO BOOKING (CustomerID, RoomID, StaffID, CheckInDate, CheckOutDate, TotalPrice, BookingStatus) VALUES
(1, 1, 3, '2025-01-15', '2025-01-18', 225.00, 'Active'),
(2, 3, 3, '2025-01-16', '2025-01-20', 480.00, 'Active'),
(3, 5, 2, '2025-01-17', '2025-01-19', 220.00, 'Active'),
(4, 7, 3, '2025-01-20', '2025-01-25', 1250.00, 'Active'),
(5, 9, 2, '2025-01-10', '2025-01-12', 360.00, 'Completed');
GO

-- =============================================
-- SAMPLE DATA - ROOM_SERVICE (Weak Entity ornegi)
-- =============================================
INSERT INTO ROOM_SERVICE (BookingID, ServiceType, Description, Price) VALUES
(1, 'Room Service', 'Breakfast in room', 25.00),
(1, 'Laundry', 'Clothes washing service', 15.00),
(2, 'Mini Bar', 'Mini bar consumption', 45.00),
(2, 'Spa', 'Massage service', 80.00),
(3, 'Room Service', 'Dinner in room', 55.00),
(4, 'Airport Transfer', 'Airport pickup service', 100.00),
(4, 'Room Service', 'Late night snack', 20.00);
GO

-- =============================================
-- VIEW 4: Room Service Summary (Weak Entity example)
-- =============================================
CREATE VIEW vw_RoomServiceSummary AS
SELECT
    rs.ServiceID,
    b.BookingID,
    c.FirstName + ' ' + c.LastName AS CustomerName,
    r.RoomNumber,
    rs.ServiceType,
    rs.Description,
    rs.Price,
    rs.ServiceDate
FROM ROOM_SERVICE rs
INNER JOIN BOOKING b ON rs.BookingID = b.BookingID
INNER JOIN CUSTOMER c ON b.CustomerID = c.CustomerID
INNER JOIN ROOM r ON b.RoomID = r.RoomID;
GO

-- =============================================
-- VERIFICATION QUERIES
-- =============================================
PRINT '=== DATABASE CREATED SUCCESSFULLY ===';
PRINT '';

PRINT '=== ROOM STATUS ===';
SELECT RoomNumber,
       CASE WHEN IsAvailable = 1 THEN 'Available' ELSE 'Occupied' END AS Status
FROM ROOM ORDER BY RoomNumber;

PRINT '';
PRINT '=== DEPARTMENT EMPLOYEE COUNTS ===';
SELECT DepartmentName, NumberOfEmployees FROM DEPARTMENT;

PRINT '';
PRINT '=== ACTIVE BOOKINGS ===';
SELECT * FROM vw_BookingSummary WHERE BookingStatus = 'Active';

PRINT '';
PRINT '=== ROOM OCCUPANCY STATS ===';
SELECT * FROM vw_RoomOccupancyStats;
GO
