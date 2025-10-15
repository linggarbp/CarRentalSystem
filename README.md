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

-- INSERT INTO Rentals (UserId, CarId, StartDate, EndDate, TotalDays, TotalPrice, Status, CreatedDate, IsDeleted)
VALUES 

-- Sample completed rental

-- (@AdminUserId, 1, '2025-10-01', '2025-10-05', 4, 180.00, 1, '2025-10-01', 0),

-- Sample active rental  

-- (@AdminUserId, 2, '2025-10-08', '2025-10-12', 4, 168.00, 0, '2025-10-08', 0);

-- Insert sample cars data

INSERT INTO Cars (Brand, Model, Year, LicensePlate, PricePerDay, Status, ImageUrl, CreatedDate, IsDeleted)

VALUES 

-- Economy Cars

('Toyota', 'Corolla', 2023, 'B-1234-ABC', 45000, 0, 'https://images.unsplash.com/photo-1621007947382-bb3c3994e3fb?w=500', GETDATE(), 0),

('Honda', 'Civic', 2022, 'B-5678-DEF', 42000, 0, 'https://images.unsplash.com/photo-1606664515524-ed2f786a0bd6?w=500', GETDATE(), 0),

('Nissan', 'Sentra', 2023, 'B-9101-GHI', 40000, 0, 'https://images.unsplash.com/photo-1552519507-da3b142c6e3d?w=500', GETDATE(), 0),

('Hyundai', 'Elantra', 2022, 'B-1121-JKL', 38000, 0, 'https://images.unsplash.com/photo-1563720223185-11003d516935?w=500', GETDATE(), 0),

-- Mid-Size Cars

('Toyota', 'Camry', 2023, 'B-3141-MNO', 55000, 0, 'https://images.unsplash.com/photo-1621135802920-52663ed5a85d?w=500', GETDATE(), 0),

('Honda', 'Accord', 2022, 'B-5161-PQR', 52000, 0, 'https://images.unsplash.com/photo-1603584173870-7f23fdae1b7a?w=500', GETDATE(), 0),

('Volkswagen', 'Jetta', 2023, 'B-7181-STU', 48000, 0, 'https://images.unsplash.com/photo-1549399742-d8882f2cb6d6?w=500', GETDATE(), 0),

-- SUVs

('Toyota', 'RAV4', 2023, 'B-9202-VWX', 70000, 0, 'https://images.unsplash.com/photo-1566473965997-3de9c817e938?w=500', GETDATE(), 0),

('Honda', 'CR-V', 2022, 'B-1222-YZA', 68000, 0, 'https://images.unsplash.com/photo-1571088775291-d128d3c4ce67?w=500', GETDATE(), 0),

('Hyundai', 'Tucson', 2023, 'B-3242-BCD', 65000, 0, 'https://images.unsplash.com/photo-1605559424843-9e4c228bf1c2?w=500', GETDATE(), 0),

('Subaru', 'Outback', 2022, 'B-5262-EFG', 72000, 0, 'https://images.unsplash.com/photo-1544636331-e26879cd4d9b?w=500', GETDATE(), 0),

('Ford', 'Escape', 2023, 'B-7282-HIJ', 66000, 0, 'https://images.unsplash.com/photo-1552519507-da3b142c6e3d?w=500', GETDATE(), 0),

-- Luxury Cars

('BMW', '3 Series', 2023, 'B-9303-KLM', 95000, 0, 'https://images.unsplash.com/photo-1555215695-3004980ad54e?w=500', GETDATE(), 0),

('Mercedes-Benz', 'C-Class', 2022, 'B-1323-NOP', 98000, 0, 'https://images.unsplash.com/photo-1618843479313-40f8afb4b4d8?w=500', GETDATE(), 0),

('Audi', 'A4', 2023, 'B-3343-QRS', 9200, 0, 'https://images.unsplash.com/photo-1606664515524-ed2f786a0bd6?w=500', GETDATE(), 0),

-- Electric/Hybrid

('Tesla', 'Model 3', 2023, 'B-5363-TUV', 85000, 0, 'https://images.unsplash.com/photo-1560958089-b8a1929cea89?w=500', GETDATE(), 0),

('Toyota', 'Prius', 2022, 'B-7383-WXY', 50000, 0, 'https://images.unsplash.com/photo-1553440569-bcc63803a83d?w=500', GETDATE(), 0),

-- Large SUVs

('Chevrolet', 'Tahoe', 2023, 'B-9404-ZAB', 105000, 0, 'https://images.unsplash.com/photo-1544636331-e26879cd4d9b?w=500', GETDATE(), 0),

('Ford', 'Expedition', 2022, 'B-1424-CDE', 102000, 0, 'https://images.unsplash.com/photo-1566473965997-3de9c817e938?w=500', GETDATE(), 0),

-- Vans/Minivans

('Honda', 'Odyssey', 2023, 'B-3444-FGH', 80000, 0, 'https://images.unsplash.com/photo-1544636331-e26879cd4d9b?w=500', GETDATE(), 0),

('Toyota', 'Sienna', 2022, 'B-5464-IJK', 78000, 0, 'https://images.unsplash.com/photo-1566473965997-3de9c817e938?w=500', GETDATE(), 0);

-- Set beberapa mobil sebagai sedang disewa untuk testing

UPDATE Cars 

SET Status = 1 -- CarStatus.Rented

WHERE Id IN (2, 8, 13); -- Honda Civic, Toyota RAV4, BMW 3 Series

-- Sisanya tetap Available (Status = 0)

-- Check cars data

SELECT Id, Brand, Model, Year, LicensePlate, PricePerDay, CASE WHEN Status = 0 THEN 'Available' ELSE 'Rented' END as Status

FROM Cars 

WHERE IsDeleted = 0

ORDER BY Brand, Model;

-- Check total cars by status

SELECT CASE WHEN Status = 0 THEN 'Available' ELSE 'Rented' END as Status, COUNT(*) as Count

FROM Cars 

WHERE IsDeleted = 0

GROUP BY Status;

-- Check if admin user exists

SELECT Id, Email, FirstName, LastName FROM AspNetUsers WHERE Email = 'admin@carrental.com';

4. Build & Run

Credential admin

Username = admin@carrental.com
Password = Admin123!
