# ISSTechLogistics - orders tracking

This project consists on a Web API, that takes a .csv file and makes calculations for some statistics for a company.

Here are the main requirements:

1. Shipment data is uploaded via an API in CSV format;
2. A background worker processes the uploaded file, calculates aggregate statistics (e.g., totalshipments, average delivery time), and saves the results to a database;
3. An endpoint is available to fetch the calculated statistics;
4. An additional endpoint allows triggering a manual re-processing of the file if needed;

This is executable locally, so it's not needed any type of special setup, the only requirements needed are the following:
- having Visual Studio 2022 installed (this is to run the project);
- having the latest version of .NET 8 installed in your machine (you can get it here: https://dotnet.microsoft.com/en-us/download/dotnet/8.0);

To run this project, simply open the ISSTechLogistics.sln using VS 2022 and then press the F5 or Ctrl + F5 to run the project.
This will open a window in your default browser showing some endpoints for you to test.

NOTE: for this to work, you'll need to provide a instance for SQL Server. So to work with this, you'll have to do the following:
- go to apssettings.json, in the section "ConnectionStrings" > "DefaultConnection" you'll need to change the "Server"" to your server (open the SSMS and then just copy your express instance);
- After this is done, you just need to run the migrations so it can be created the database and the required tables, to do this, just open the terminal in the current localtion of the .sln and run this command: `dotnet ef database update`

## Endpoints:

NOTE: To test the endpoints, you just need to click in the endpoint you want to test and then click in the "Try it out" button. After that you just need to fill the data you want and you're good to go :)

- ProcessOrders -> To test this endpoint, you only need to provide a .csv file with the following requirements:
	1. Must have the following headers: ShipmentId, Origin, Destination, Weight, DeliveryTime (this is case sensitive, write the names exactly like this);
	
	This endpoint is used to provide the orders details so the program can calculate the data. This file is saved in your local machine and the path can be custom.
	To customize the path where the file is saved, you just need to go to the appsettings.json file and follow this:
		
	LogisticsTechSettings > FileSettings > FilesSavePath
	After you find the FilesSavePath, just change the content inside the string to your prefered path, this way you'll know where it is being saved.
- GetOrderStatistics -> this endpoint just retrieves the statistics data that are requested in the requirements
- RecalculateOrdersStatistics -> this endpoint just recalculates the statistics, to test this, you can go to the file that was saved in the ProcessOrders endpoint (go to the location in your file explorer where the file is saved). Open this file and insert the data you need in order to recalculate the data. This endpoint will return the data of the statistics required.