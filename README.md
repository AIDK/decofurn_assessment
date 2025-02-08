# Programmer Exercise 1

## Description

Create a C# console application that imports data from the provided `data.csv` file into a Microsoft SQL Server database. The database should contain two tables: `InvoiceHeader` and `InvoiceLines`, as defined in the accompanying `tables.sql` script.

## Guidelines

When implementing the solution, please adhere to the following guidelines:

- **Static Classes**: Use static classes only when defining functions.
- **Complex Data Types**: Utilize classes and/or structs to define complex data types.
- **Function Logic**: Avoid placing all logic within a single function.
- **Global State**: Refrain from referencing any global state except where appropriate. Global state includes:
  - Variables outside a function's scope.
  - Data referenced by a pointer passed to a function as a parameter.
  - Data in external resources (e.g., files, databases, web APIs).

- **Exception Handling and Logging**: Implement exception handling and console logging.
- **Invoice Summary**: For each imported invoice, output the invoice number and the total quantity (sum of `InvoiceLines.Quantity`) to the console.
- **Data Duplication**: Ensure that invoices are not duplicated in the `InvoiceHeader` table and that invoice lines are not duplicated in the `InvoiceLines` table. Prevent duplicate header or line imports if the CSV file is imported multiple times.
- **NuGet Libraries**: Feel free to use any NuGet libraries to assist with completing this task.
- **Specific Requirements**:
  - Use Entity Framework Code First.
  - Use LINQ for manipulating and summarizing the imported data.

- **Data Validation**: After the import is complete, verify that the sum of `InvoiceLines.Quantity * InvoiceLines.UnitSellingPriceExVAT` matches the sum of all `InvoiceHeader.InvoiceTotal` (i.e., 21,860.71). Print a message indicating the outcome of this check.
