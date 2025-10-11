How to use this web app?
1. Open appsettings.json -> Edit Server to yours
"DefaultConnection": "Server=LAPTOP-NSUAR96P;Database=CarRentalSystemDb;Integrated Security=true;MultipleActiveResultSets=true;Encrypt=true;TrustServerCertificate=true"

2. Open Package Manager Console
Enter -> Update-Database 

3. Open SSMS and Run Query
-- First, get the admin user ID (jika ingin buat sample rental)
DECLARE @AdminUserId NVARCHAR(450)
SELECT @AdminUserId = Id FROM AspNetUsers WHERE Email = 'admin@carrental.com'

-- Insert sample rental data (jika ada user yang sudah register)
-- Uncomment dan modifikasi jika sudah ada user yang register


INSERT INTO Rentals (UserId, CarId, StartDate, EndDate, TotalDays, TotalPrice, Status, CreatedDate, IsDeleted)
VALUES 
-- Sample completed rental
(@AdminUserId, 1, '2025-10-01', '2025-10-05', 4, 180.00, 1, '2025-10-01', 0),
-- Sample active rental  
(@AdminUserId, 2, '2025-10-08', '2025-10-12', 4, 168.00, 0, '2025-10-08', 0);


-- Set beberapa mobil sebagai sedang disewa untuk testing
UPDATE Cars 
SET Status = 1 -- CarStatus.Rented
WHERE Id IN (2, 8, 13); -- Honda Civic, Toyota RAV4, BMW 3 Series

-- Sisanya tetap Available (Status = 0)

-- Check cars data
SELECT Id, Brand, Model, Year, LicensePlate, PricePerDay, 
       CASE WHEN Status = 0 THEN 'Available' ELSE 'Rented' END as Status
FROM Cars 
WHERE IsDeleted = 0
ORDER BY Brand, Model;

-- Check total cars by status
SELECT 
    CASE WHEN Status = 0 THEN 'Available' ELSE 'Rented' END as Status,
    COUNT(*) as Count
FROM Cars 
WHERE IsDeleted = 0
GROUP BY Status;

-- Check if admin user exists
SELECT Id, Email, FirstName, LastName FROM AspNetUsers WHERE Email = 'admin@carrental.com';

4. Build & Run
Credential admin
Username = admin@carrental.com
Password = Admin123!
