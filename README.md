This has only been tested in FireFox.

1. Clone the repository in Visual Studio
2. Build
3. Run migrations from Package Manager Console
   
   a. add-migration InitialCreate -Context HeapProjectContext
   
   b. update-database -Context HeapProjectContext
   
   c. add-migration InitialCreate -Context ApplicationDBContext
   
   d. update-database -Context ApplicationDBContext
   
5. Run the project (Ctrl-F5).  May have to add an exclusion in order to get past security restrictions in Windows 11.
6. Register a new user
7. Login
8. Click on "Create New"
9. Add node(s) to make a max heap, remove node(s), compute the sum of node(s).

Note: the application is not fully internationalized yet.
