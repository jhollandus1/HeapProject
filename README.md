This has only been tested in FireFox.

1. Clone the repository in Visual Studio
2. Build
3. From Package Manager Console, update the database
   
   a. update-database -Context HeapProjectContext
   
   b. update-database -Context ApplicationDBContext
   
5. Run the project (Ctrl-F5).  May have to add an exclusion in order to get past security restrictions in Windows 11.
6. Register a new user
7. Login
8. Click on "Create New" and add a heap item label and save.
9. Click "Edit" next to the heap item label, add node(s) to make a max heap, remove node(s), compute the sum of node(s), and/or Add bulk array.
10. Save to save the heap
11. Can click "Details" to see the heap details, "Delete" to delete the item.

Note: the application is not fully internationalized yet.
