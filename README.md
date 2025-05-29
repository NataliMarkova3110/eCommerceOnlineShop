# eCommerce Online Shop

The goal of this repo is to learn course topics (.NET Advanced) on practical examples based on the e-commerce domain.

## Coding Standards and Style Guide

This project follows Microsoft's C# coding conventions and best practices. The standards are enforced through an `.editorconfig` file and should be followed by all contributors.

### Key Coding Standards

1. **Naming Conventions**

   - Use PascalCase for:
     - Public members
     - Types (classes, structs, interfaces, enums)
     - Methods
     - Properties
     - Constants
   - Use camelCase for:
     - Private fields (prefixed with `_`)
     - Parameters
     - Local variables
     - Local functions
   - Use `I` prefix for interfaces
   - Use `Async` suffix for async methods
   - Use `s_` prefix for private static fields
   - Use `_` prefix for private instance fields

2. **Code Organization**

   - One class per file
   - File name should match the class name
   - Group related functionality in namespaces
   - Use regions sparingly and only for large classes

3. **Formatting**

   - Use 4 spaces for indentation
   - Use spaces, not tabs
   - Maximum line length: 120 characters
   - Always use braces for control statements
   - Place opening brace on a new line
   - Add a blank line after closing brace

4. **Comments and Documentation**

   - Use XML documentation for public APIs
   - Use `///` for documentation comments
   - Use `//` for inline comments
   - Write comments in English
   - Keep comments up to date

5. **Best Practices**

   - Use async/await for I/O operations
   - Use dependency injection
   - Follow SOLID principles
   - Write unit tests for business logic
   - Use meaningful variable names
   - Keep methods small and focused
   - Use expression-bodied members when appropriate

6. **Error Handling**

   - Use try-catch blocks appropriately
   - Log exceptions with context
   - Use custom exceptions when needed
   - Don't catch and swallow exceptions

7. **Security**
   - Never store secrets in code
   - Use secure configuration management
   - Validate all input
   - Use parameterized queries
   - Follow the principle of least privilege

### IDE Setup

1. **Visual Studio**

   - Install the EditorConfig extension
   - Enable "Format document on save"
   - Enable "Trim trailing whitespace"

2. **VS Code**
   - Install the C# extension
   - Install the EditorConfig extension
   - Enable "Format on save"

### Code Review Guidelines

1. **What to Review**

   - Code style and formatting
   - Naming conventions
   - Error handling
   - Security considerations
   - Performance implications
   - Test coverage

2. **Review Process**
   - Review for functionality
   - Review for maintainability
   - Review for security
   - Provide constructive feedback
   - Be respectful and professional

### Getting Started

1. Clone the repository
2. Install required tools and extensions
3. Open the solution in your IDE
4. The `.editorconfig` file will automatically enforce coding standards

### Additional Resources

- [Microsoft C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [EditorConfig](https://editorconfig.org/)
- [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)
- [Clean Code](https://www.amazon.com/Clean-Code-Handbook-Software-Craftsmanship/dp/0132350882)
