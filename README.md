# Hotel Reservation System

## Database Systems Term Project - Fall 2025
**Group 12**

---

## Project Description

A comprehensive Hotel Reservation Management System built with a hybrid architecture using ASP.NET Core Blazor Server (.NET 8.0) for the UI layer and Entity Framework 6.5.1 with EDMX (.NET Framework 4.8) for the data access layer. The system allows hotel staff to manage rooms, customers, and bookings with real-time availability tracking.

### Key Features
- **Dashboard**: Real-time statistics with pie charts (ChartJs.Blazor), occupancy rates and booking summaries
- **Room Management**: Add, edit, delete rooms with availability tracking
- **Customer Management**: Customer registration and booking history
- **Booking System**: Create bookings, check-in/check-out with automatic room status updates
- **Staff Management**: Employee management with department assignments
- **Room Types**: Configure different room categories with pricing
- **Room Services**: Weak entity for additional services per booking

---

## Technology Stack

| Component | Technology | Version |
|-----------|------------|---------|
| **UI Framework** | ASP.NET Core Blazor Server | 8.0 |
| **Data Access** | .NET Framework | 4.8 |
| **ORM** | Entity Framework (EDMX) | 6.5.1 |
| **Database** | SQL Server | Express/LocalDB |
| **UI Components** | MudBlazor | 8.15.0 |
| **Charts** | ChartJs.Blazor.Fork | 2.0.2 |

### Architecture: Hybrid (.NET Framework 4.8 + .NET 8.0)

```
┌─────────────────────────────────────────────────────────────┐
│                    HotelSystem (.NET 8.0)                    │
│              Blazor Server + MudBlazor + ChartJs             │
├─────────────────────────────────────────────────────────────┤
│                  DataAccess (.NET Framework 4.8)             │
│              Entity Framework 6.5.1 + EDMX                   │
├─────────────────────────────────────────────────────────────┤
│                    SQL Server Database                       │
│          Tables, Views, Triggers, Indexes                    │
└─────────────────────────────────────────────────────────────┘
```

---

## Database Complex Structures

### 1. Concept Hierarchy (RoomType)
Room types form a concept hierarchy with different capacities and prices:

```
                    ROOM_TYPE (Concept Hierarchy)
                           │
        ┌──────────────────┼──────────────────┐
        │                  │                  │
    ┌───▼───┐        ┌────▼────┐       ┌────▼────┐
    │Single │        │ Double  │       │ Suite   │
    │Cap: 1 │        │ Cap: 2  │       │ Cap: 4  │
    │ $100  │        │  $150   │       │  $300   │
    └───────┘        └─────────┘       └─────────┘
```

### 2. Weak Entity (RoomService)
RoomService is a weak entity that depends on Booking:
- Cannot exist without a parent Booking
- Uses `ON DELETE CASCADE` - when booking is deleted, all room services are deleted
- Composite key dependency: ServiceID + BookingID

```sql
CREATE TABLE ROOM_SERVICE (
    ServiceID INT IDENTITY(1,1) PRIMARY KEY,
    BookingID INT NOT NULL,
    ServiceType NVARCHAR(100) NOT NULL,
    -- ON DELETE CASCADE ensures weak entity behavior
    CONSTRAINT FK_RoomService_Booking FOREIGN KEY (BookingID)
        REFERENCES BOOKING (BookingID) ON DELETE CASCADE
);
```

### 3. Ternary Relationship (Booking)
Booking connects three entities in a ternary relationship:

```
        ┌──────────┐
        │ CUSTOMER │
        └────┬─────┘
             │
             ▼
        ┌──────────┐         ┌──────────┐
        │ BOOKING  │────────▶│  STAFF   │
        └────┬─────┘         └──────────┘
             │
             ▼
        ┌──────────┐
        │   ROOM   │
        └──────────┘
```

---

## Prerequisites

Before running the application, ensure you have:

1. **Visual Studio 2022** - With .NET Framework 4.8 and .NET 8.0 workloads
2. **.NET 8 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
3. **.NET Framework 4.8 Developer Pack** - [Download here](https://dotnet.microsoft.com/download/dotnet-framework/net48)
4. **SQL Server** - SQL Server Express or LocalDB
5. **SQL Server Management Studio (SSMS)** - For database setup

---

## Step-by-Step Installation Guide

### Step 1: Database Setup

1. Open **SQL Server Management Studio (SSMS)**
2. Connect to your SQL Server instance
3. Open the file `DatabaseSchema.sql` from the project root
4. Execute the script to create the database and sample data

```sql
-- The script will:
-- 1. Create HOTEL_MANAGEMENT database
-- 2. Create all tables (PERMISSION, DEPARTMENT, STAFF, ROOM_TYPE, ROOM, CUSTOMER, BOOKING, ROOM_SERVICE)
-- 3. Create triggers for automatic room availability and employee count updates
-- 4. Create views for reporting
-- 5. Insert sample data for testing
```

### Step 2: Configure Connection String

1. Open `DataAccess/App.config`
2. Update the connection string to match your SQL Server:

```xml
<connectionStrings>
    <!-- For SQL Server Express (default): -->
    <add name="HotelManagementEntities"
         connectionString="metadata=res://*/HotelModel.csdl|res://*/HotelModel.ssdl|res://*/HotelModel.msl;provider=System.Data.SqlClient;provider connection string='data source=localhost\SQLEXPRESS;initial catalog=HOTEL_MANAGEMENT;integrated security=True;encrypt=True;trustservercertificate=True;MultipleActiveResultSets=True;App=EntityFramework'"
         providerName="System.Data.EntityClient" />

    <!-- For LocalDB: -->
    <!-- data source=(localdb)\mssqllocaldb -->
</connectionStrings>
```

### Step 3: Build and Run

**Using Visual Studio:**
1. Open `HotelReservationSystem.sln` in Visual Studio 2022
2. Right-click solution → Restore NuGet Packages
3. Build Solution (Ctrl+Shift+B)
4. Set HotelSystem as Startup Project
5. Press F5 to run

**Using Command Line (Windows only):**
```bash
cd HotelSystem
dotnet restore
dotnet build
dotnet run
```

3. Open your browser and navigate to: `https://localhost:5001` or `http://localhost:5000`

---

## Project Structure

```
HotelReservationSystem/
├── DatabaseSchema.sql              # SQL script with tables, triggers, sample data
├── README.md                       # This file
├── HotelReservationSystem.sln      # Solution file
│
├── DataAccess/                     # Data Access Layer (.NET Framework 4.8)
│   ├── HotelModel.edmx             # Entity Data Model (Database-First)
│   ├── HotelModel.Context.cs       # DbContext for Entity Framework
│   ├── HotelModel.Designer.cs      # Designer code
│   ├── App.config                  # Connection string configuration
│   ├── packages.config             # NuGet packages
│   │
│   ├── Entities/                   # Entity classes (generated from EDMX)
│   │   ├── Permission.cs
│   │   ├── Department.cs
│   │   ├── Staff.cs
│   │   ├── RoomType.cs             # Concept Hierarchy
│   │   ├── Room.cs
│   │   ├── Customer.cs
│   │   ├── Booking.cs              # Ternary Relationship
│   │   └── RoomService.cs          # Weak Entity
│   │
│   └── CRUD/                       # CRUD operations with EF6
│       ├── RoomCRUD.cs
│       ├── RoomTypeCRUD.cs
│       ├── CustomerCRUD.cs
│       ├── BookingCRUD.cs          # Transaction management
│       ├── RoomServiceCRUD.cs      # Weak entity operations
│       ├── StaffCRUD.cs
│       ├── DepartmentCRUD.cs
│       ├── PermissionCRUD.cs
│       └── Models/                 # View models for grouping
│           ├── RoomOccupancyStat.cs
│           └── CustomerCountryStat.cs
│
└── HotelSystem/                    # Blazor Web Application (.NET 8.0)
    ├── Components/
    │   ├── Layout/
    │   │   ├── MainLayout.razor
    │   │   └── NavMenu.razor
    │   ├── App.razor               # ChartJs scripts included
    │   └── Pages/
    │       ├── Home.razor          # Dashboard with pie charts
    │       ├── Rooms/              # Room management pages
    │       ├── Customers/          # Customer management pages
    │       ├── Bookings/           # Booking management pages
    │       ├── Staff/              # Staff management pages
    │       └── RoomTypes/          # Room type management pages
    └── Program.cs                  # Application entry point
```

---

## EDMX (Entity Data Model)

The project uses **Database-First** approach with EDMX:

```xml
<!-- HotelModel.edmx contains: -->
<!-- SSDL: Storage Schema (database tables) -->
<!-- CSDL: Conceptual Schema (entity classes) -->
<!-- C-S Mapping: Maps database to entities -->
```

### Entities in EDMX:
1. **Permission** - Staff permission levels
2. **Department** - Hotel departments
3. **Staff** - Employees (Ternary: Department + Permission)
4. **RoomType** - Room categories (Concept Hierarchy)
5. **Room** - Hotel rooms
6. **Customer** - Hotel guests
7. **Booking** - Reservations (Ternary: Customer + Room + Staff)
8. **RoomService** - Additional services (Weak Entity)

---

## Database Features

### Triggers (5)
1. **trg_IncrementEmployeeCount**: Auto-increment department employee count on staff insert
2. **trg_DecrementEmployeeCount**: Auto-decrement count on staff delete
3. **trg_SetRoomUnavailable**: Sets room as unavailable when booking is created
4. **trg_SetRoomAvailable**: Sets room as available when booking is completed/cancelled
5. **trg_SetRoomAvailableOnDelete**: Handles room availability when booking is deleted

### Views (4)
1. **vw_BookingSummary**: JOIN of Booking, Customer, Room, RoomType, Staff
2. **vw_AvailableRooms**: List of currently available rooms
3. **vw_RoomOccupancyStats**: Grouped statistics by room type
4. **vw_CustomerBookingHistory**: Customer booking summary

### Indexes (9)
- Optimized indexes on frequently queried columns (CustomerID, RoomID, BookingStatus, Email, etc.)

---

## LINQ Query Examples (EF6 Syntax)

### 1. Data Grouping (LINQ GroupBy)
```csharp
// Room occupancy statistics by type
public List<RoomOccupancyStat> GetRoomOccupancyStats()
{
    var allRooms = db.Rooms.Include("RoomType").ToList();

    return allRooms
        .GroupBy(r => r.RoomType?.TypeName ?? "Unknown")
        .Select(g => new RoomOccupancyStat
        {
            TypeName = g.Key,
            TotalCount = g.Count(),
            AvailableCount = g.Count(r => r.IsAvailable == true),
            OccupiedCount = g.Count(r => r.IsAvailable == false)
        })
        .ToList();
}
```

### 2. Filtering (LINQ Where)
```csharp
// Get available rooms with filtering
public List<Room> GetAvailableRooms()
{
    return db.Rooms
        .Include("RoomType")
        .Where(r => r.IsAvailable == true)
        .OrderBy(r => r.RoomNumber)
        .ToList();
}
```

### 3. JOIN Operations (EF6 Include - String-based)
```csharp
// Get bookings with related data using EF6 Include syntax
public List<Booking> GetAllBookings()
{
    return db.Bookings
        .Include("Customer")           // JOIN with Customer
        .Include("Room")               // JOIN with Room
        .Include("Room.RoomType")      // Nested JOIN with RoomType
        .Include("Staff")              // JOIN with Staff
        .OrderByDescending(b => b.BookingDate)
        .ToList();
}
```

### 4. Transaction Management (EF6)
```csharp
public bool CreateBooking(Booking booking)
{
    using (var transaction = db.Database.BeginTransaction())
    {
        try
        {
            // 1. Add booking
            db.Bookings.Add(booking);
            db.SaveChanges();

            // 2. Update room availability (trigger also handles this)
            var room = db.Rooms.Find(booking.RoomID);
            if (room != null)
            {
                room.IsAvailable = false;
                db.SaveChanges();
            }

            transaction.Commit();
            return true;
        }
        catch
        {
            transaction.Rollback();
            return false;
        }
    }
}
```

### 5. Sorting (LINQ OrderBy)
```csharp
// Sort customers by last name then first name
public List<Customer> GetAllCustomers()
{
    return db.Customers
        .OrderBy(c => c.LastName)
        .ThenBy(c => c.FirstName)
        .ToList();
}
```

---

## Dashboard Charts (ChartJs.Blazor)

The dashboard includes interactive pie charts:

```csharp
// Home.razor - Pie Chart Configuration
private PieConfig roomOccupancyConfig = new PieConfig();
private PieConfig customerCountryConfig = new PieConfig();

private void ConfigureCharts()
{
    roomOccupancyConfig.Options = new PieOptions
    {
        Responsive = true,
        Title = new OptionsTitle
        {
            Display = true,
            Text = "Room Occupancy by Type"
        }
    };

    var roomDataset = new PieDataset<double>(roomData)
    {
        BackgroundColor = GenerateColors(roomStats.Count)
    };
    roomOccupancyConfig.Data.Datasets.Add(roomDataset);
}
```

```html
<!-- Pie Chart Component -->
<Chart Config="roomOccupancyConfig"></Chart>
```

---

## Q&A Topics Reference

| Topic | Implementation Location |
|-------|------------------------|
| **Data Grouping** | `BookingCRUD.GetRoomOccupancyStats()`, `CustomerCRUD.GetCustomerCountByCountry()` |
| **LINQ** | All CRUD classes - Where, Select, GroupBy, OrderBy, Include |
| **Database Connectivity** | `App.config`, `HotelModel.Context.cs` |
| **EDMX** | `HotelModel.edmx` - Database-First approach |
| **Filtering/Sorting** | All List pages with MudDataGrid |
| **CRUD Operations** | `CRUD/` folder - Create, Read, Update, Delete |
| **JOIN Operations** | `BookingCRUD.GetAllBookings()` - EF6 Include syntax |
| **Transaction Management** | `BookingCRUD.CreateBooking()`, `BookingCRUD.DeleteBooking()` |
| **Weak Entity** | `RoomService` table with ON DELETE CASCADE |
| **Concept Hierarchy** | `RoomType` with different categories |
| **Ternary Relationship** | `Booking` connecting Customer + Room + Staff |

---

## Troubleshooting

### Database Connection Issues
- Ensure SQL Server is running
- Verify the connection string in `DataAccess/App.config` matches your server
- Check if HOTEL_MANAGEMENT database exists

### Build Errors
- **Windows Required**: .NET Framework 4.8 requires Windows
- Run `dotnet restore` in HotelSystem folder
- Ensure both .NET 8 SDK and .NET Framework 4.8 Developer Pack are installed
- In Visual Studio: Right-click solution → Restore NuGet Packages

### EDMX Issues
- If EDMX doesn't update: Right-click .edmx → Update Model from Database
- Ensure EntityFramework 6.5.1 is installed in DataAccess project

### Port Already in Use
- Change the port in `Properties/launchSettings.json` or use `dotnet run --urls=http://localhost:5050`

---

## Authors
**Group 12** - Database Systems Term Project, Fall 2025

**Deadline**: 27.12.2025 09:00
