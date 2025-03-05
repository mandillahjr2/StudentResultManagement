This is a simple console application for managing student records using a SQLite database. The application allows you to fetch, add, update, and delete student records.

## Features

- Add new student records
- Fetch and display student records
- Update student marks
- Delete student records

## Prerequisites

- .NET SDK 6.0 or later
- SQLite

## Project Structure

```
StudentResultManagement/
    Program.cs
    StudentResultManagement.csproj
```

## Getting Started

1. **Clone the repository:**

    ```sh
    git clone <repository-url>
    cd StudentResultManagement
    ```

2. **Build the project:**

    ```sh
    dotnet build
    ```

3. **Run the application:**

    ```sh
    dotnet run
    ```

## Usage

1. **Initialize the database:**

    The database is automatically initialized when the application starts. It creates the necessary tables if they do not exist.

2. **Enter Student ID:**

    When prompted, enter the Student ID to fetch, add, or delete a student record.

3. **Fetch Student Record:**

    If the student record exists, it will be displayed along with the option to update marks or delete the record.

4. **Add New Student Record:**

    If the student record does not exist, you will be prompted to enter the student's name and marks for a unit.

5. **Update Marks:**

    If you choose to update marks, enter the unit name and the new marks.

6. **Delete Student Record:**

    If you choose to delete the student record, confirm the deletion when prompted.

## Code Overview

### `Student` Class

Represents a student with properties for Student ID, Name, and a dictionary of Units and Marks. It includes methods to add subjects, calculate total and average marks, get grades, and print the result slip.

### `DatabaseManager` Class

Handles database operations including initializing the database, inserting or updating student records, fetching student records, and deleting student records.

### `Program` Class

The main entry point of the application. It initializes the database and handles user input for fetching, adding, updating, and deleting student records.
