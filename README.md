# ADO.NET and Crystal Reports for Book Management

## Overview

This project is a comprehensive Book Management System built using **ADO.NET** and **Crystal Reports**. It allows to manage book records, including their details, pricing, availability, and table of contents. The application also provides detailed reports generated using **Crystal Reports**, making it a practical solution for organizing and visualizing book-related data.

## Features

- **Book Management**: 
  - Add, update, delete, and view book details such as title, price, publish date, and eBook availability.
  - Store and display book-related images.

- **Table of Contents Management**:
  - Manage chapters and topics for each book.
  - Associate chapter details, including chapter numbers, topics, and total pages, with individual books.

- **Crystal Reports Integration**:
  - Generate detailed reports for books and their corresponding table of contents.
  - Support for dynamic data fetching and report customization.

## Technologies Used

- **Frontend**: Windows Forms (C#)
- **Backend**: SQL Server
- **Database Access**: ADO.NET
  - Used for data retrieval, manipulation, and transaction management.
- **Reporting Tool**: Crystal Reports
  - Used for creating visually appealing and customizable reports.
  
## Database Schema

The project uses a relational database with two primary tables:

1. **Books**:
   - Stores details about books, such as title, price, publish date, eBook availability, and associated picture.
   
   ```sql
   CREATE TABLE Books
   (
       BookId INT IDENTITY PRIMARY KEY,
       Title NVARCHAR(50) NOT NULL,
       Price MONEY NOT NULL,
       PublishDate DATE NOT NULL,
       IsEBookAvailable BIT,
       Picture NVARCHAR(50) NOT NULL
   )
   ```

2. **TableOfContents**:
   - Stores chapter details, such as chapter number, topic, total pages, and a foreign key linking to the `Books` table.
   
   ```sql
   CREATE TABLE TableOfContents
   (
       Id INT IDENTITY PRIMARY KEY,
       ChapterNo NVARCHAR(50) NOT NULL,
       Topic NVARCHAR(50) NOT NULL,
       TotalPages INT NOT NULL,
       BookId INT NOT NULL REFERENCES Books(BookId)
   )
   ```

## Key Functionalities

1. **Data Management**:
   - Perform CRUD (Create, Read, Update, Delete) operations for books and their table of contents.
   - Validate data before insertion to ensure accuracy.

2. **Dynamic Reporting**:
   - Generate real-time, data-driven reports using Crystal Reports.
   - Allow filtering, grouping, and sorting of report data.

3. **Ease of Use**:
   - Simple and intuitive user interface for managing records.
   - Error handling to ensure smooth user experience.

## How to Use

1. **Clone the Repository**:
   ```bash
   git clone [Book Record App](https://github.com/Ayesha9014/BookRecordsApp)
   ```

2. **Set Up the Database**:
   - Use the SQL scripts provided in the repository to create the required database and tables in SQL Server.

3. **Run the Application**:
   - Open the solution file in Visual Studio.
   - Update the database connection string in the configuration file to match your SQL Server setup.
   - Build and run the application.

4. **Generate Reports**:
   - Navigate to the reports section in the application to generate dynamic reports for books and their table of contents.
     

