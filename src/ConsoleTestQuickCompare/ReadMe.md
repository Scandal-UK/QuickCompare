## QuickCompare example

[Return to main project](/)

#### Instructions

Modify the two connection strings in ``appsettings.json`` and run the project. _There are many other option flags that you can toggle as required._

#### Sample output

```
QuickCompare schema comparison result

Database 1: [localhost\SQLEXPRESS].[Database1]
Database 2: [localhost\SQLEXPRESS].[Database2]


EXTENDED PROPERTY DIFFERENCES

Extended property: [Foo] - does not exist in database 1

PERMISSION DIFFERENCES

Permission: [CONNECT] for user: [LocalTestUser] does not exist in database 2

TABLE DIFFERENCES

Table: [dbo].[Table1] 
     [Column1] has different data type - Database 1 has type of INT and database 2 has type of NVARCHAR
     [Column2] 
      - is not allowed null in database 1 and is allowed null in database 2
      - has different data type - Database 1 has type of NVARCHAR and database 2 has type of INT
     [Column3] does not exist in database 2
     Permission: [INSERT] for user: [LocalTestUser] does not exist in database 2
     Permission: [SELECT] for user: [LocalTestUser] does not exist in database 2
     Permission: [UPDATE] for user: [LocalTestUser] does not exist in database 2

Table: [dbo].[Table2] 
     [Column3] does not exist in database 2
     [RelatedColumn] does not exist in database 2
     Index: [IX_Table2] does not exist in database 2
     Relation: [FK_Table2_Table4] does not exist in database 2
     Extended property: [Key1] does not exist in database 2

Table: [dbo].[Table3] 
     [Column1] has a unique constraint in database 1 and does not have a unique constraint in database 2
     [Column2] 
      - is allowed null in database 1 and is not allowed null in database 2
      - has different max length - Database 1 has max length of [10] and database 2 has max length of [12]
      - has different character octet length - Database 1 has an octet length of [20] and database 2 has an octet length of [24]
     [Column3] does not have a unique constraint in database 1 and has a unique constraint in database 2
     Primary key: [PK_Table3] 
      - [Column1] column does not exist in database 2 index
      - [Column3] column does not exist in database 1 index

Table: [dbo].[Table4] does not exist in database 2


STORED PROCEDURE DIFFERENCES

Stored procedure: [dbo].[sp_ReturnString]
     Extended property: [Foo] does not exist in database 2

```