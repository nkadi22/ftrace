# ftrace

#FarmTrace Web Server

This is the implementation for FarmTrace Web Server -- an aplication to read and process farm data. The application requeriments are desbribed in TechScreening_dev.pdf.

It was implemented as a REST Api. To see the available endpoints, navigate to https://localhost:5001/swagger/index

# Data

To provide the application with data, edit the file 'FarmTraceRESTApi\Data\data.json' before running the application. That file is read while the application is loadind. It is currenly also being read on every request to the API as well. This is not actually an issue, but can be improved later.

It is possible to import data to a database, for persistance reasons. To do that, one should call the endpoint '/FarmTrace/import'. See swagger.

Almost every methods provide the ability to choose to look to the database data, or just to the current data on the input file.

# Business Rules

The validation has been implemented according to the requirements. If an animals breaks a validation rule, it is ignored, i.e, not loaded to memory nor database, if the user request to do so.

#Tests

Swagger documentation has been included to the project to serve as a test harness as well. Almost all methods can be exercised from there.

#More
Database operation are being handled using EF6
Dependency Injection pattern

# Limitations, Bugs

* Methods '/FarmTrace/ReportA' and '/FarmTrace/ReportD' are not working from swagger. Those are get methods that require a body. It seems that the browser is not providing the request with the correct parameters. To run those methods use Postman or a similar tool.