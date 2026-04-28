# 🏟️ Sports Facility Reservation Management System

Web application built with **ASP.NET MVC** that allows managing users, sports facilities, and reservations, including email notifications.

---

## 🚀 Technologies Used

* ASP.NET MVC (.NET)
* Entity Framework Core
* PostgreSQL
* Docker & Docker Compose
* MailKit (email sending)
* LINQ
* C#

---

## 📦 Project Architecture

The project follows the **MVC (Model-View-Controller)** pattern:

* **Models:** Represent entities (User, Spaces, Reservations).
* **Views:** User interface components.
* **Controllers:** Handle request flow.
* **Services:** Handle business logic

---

## 🐳 Docker Setup

The project includes a `docker-compose.yml` file to run PostgreSQL.

### ▶️ Start services

```bash
docker-compose up -d
```

This will start:

* PostgreSQL container
* Default exposed port: 5432

### ⚙️ Example environment variables

```yaml
POSTGRES_DB=reservas_db
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres
```

---

## 🛠️ Project Setup

### 1. Clone repository

```bash
git clone https://github.com/Choklitos21/Prueba-Desempe-o-CSharp.git
cd <project-folder>
```

### 2. Configure connection string

In `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=reservas_db;Username=postgres;Password=postgres"
}
```

### 3. Apply migrations

```bash
dotnet ef database update
```

### 4. Run the project

```bash
dotnet run
```

---

## 📧 MailKit Configuration

In `appsettings.json`:

```json
"EmailSettings": {
  "SmtpServer": "smtp.gmail.com",
  "Port": 587,
  "SenderName": "Reservations App",
  "SenderEmail": "youremail@gmail.com",
  "Username": "youremail@gmail.com",
  "Password": "your_password"
}
```

The system automatically sends an email when a reservation is created.

---

## 👥 Features

### 1. User Management

* Register users with:

    * Name
    * Identity document
    * Phone number
    * Email
* Edit user information
* Validations:

    * Unique identity document
    * Unique email
* List all users

---

### 2. Sports Facility Management

* Register spaces with:

    * Name
    * Type (soccer, basketball, etc.)
    * Capacity
* Edit facility information
* Prevent duplicate entries
* List all facilities
* Filter by type

---

### 3. Reservation Management

* Create reservations with:

    * User
    * Sports facility
    * Date
    * Start time / End time

#### ✅ Validations:

* Prevent overlapping reservations
* A user cannot have multiple reservations at the same time
* End time must be greater than start time
* No reservations in the past

#### 🔄 Reservation statuses:

* Active
* Cancelled
* Completed

#### Features:

* Cancel reservations
* List reservations by:

    * User
    * Sports facility

---

### 4. Email Notifications

* Automatic email sent when a reservation is created using **MailKit**

---

### 5. Data Persistence

* Use of:

    * `List<>`
    * `Dictionary<TKey, TValue>`
* LINQ queries
* Persistence with **Entity Framework Core**

---

### 6. Error Handling

* `try-catch` blocks
* Validation on every operation
* Clear and user-friendly error messages

---

***Diego Alejandro Morales Montoya***
