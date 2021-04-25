# Warehouse Manager

### ***THIS PROJECT IS UNDER DEVELOPMENT***

This project is intended to be used to manage bulk ore stocks using MySql database through a web server running on a LAN.  
I'll try to design the application so that it can be used as well as a simple general purpose warehouse manager with a few modifications.

This project provides functionalities for a companie that stocks, processes and transports bulk ore owned by third parties and must register informations related to the incomings, shippings, processing and balance of the products of each client, tax data as well as other informations related to the transportation like vehicles and drivers.

## 1. Development Environment

This project is being developed on the following environment:

- **IDE:** Visual Studio Code
- **OS:** Debian 11 (Bullseye)
- **Programming Language:** C#
- **Front-End:** HTML, Bootstrap CSS, Razor Views
- **Framework:** .NET Core 3.1
- **Database:** MySql 8.0
- **Web Server:** NGINX 1.19

## 2. Features

The application should meet the following critirias:

1. Manage enitites and informations related to storage and transportation of goods in a warehouse.
2. Only registered users should access the application and restrictions are applied according to their roles.
3. Create, Read, Update and Delete the following records:

- Users
- Clients
- Products
- Stocks
- Vehicles
- Drivers
- Incomings
- Shippings
- Product Enhancements

4. Provide a friendly user interface to collect and display informations stored in the DB.
5. Provide well-structured reports of activities and stocks.

**Further improvements:**

- Design a mobile front-end interface.
- Implement an SPA interface using the React framework.

## 3. Configuring and running

You can have the application running either using Docker or setting each component individually.  
Following is how to configure and run MySql Server and the application.  

### 3.1 Using Docker

Rename the `.env-example` file to `.env` and and set the `DB_PASSWORD` in the file.

Install [*Docker*](https://www.docker.com) and *docker-compose* if you haven't yet and run the command in the project directory:

```
docker-compose up
```

Wait until Docker build the image and MySql and kestrel servers are running.  
The process may take some time.  
Use `Ctrl + C` to stop the container.

### 3.2 Setting The Environment Manually

1. Install [.NET Core SDK 3.1](https://dotnet.microsoft.com/download)
2. Install [MySql 8 Server](https://dev.mysql.com/downloads/mysql/)
3. Set database connection string either editing `appsettings.json` or setting the `environment variable`.  
   Use the string template and change `<ROOT-PASSWORD>`:
   ```
   "ASPNETCORE_DB_CONN_STRING": "server=localhost;port=3306;database=warehouse;user=root;password=<ROOT-PASSWORD>"
   ```

Using `.NET Core CLI`, run the following commands in the project directory:
```
dotnet run
```

### 3.3 Known Issues

#### 3.3.1 Dependencies Issues

- Visual Studio Code may report issues related to dependencies with **Microsoft.EntityFrameworkCore** package.  
  Try to run the following command in the project directory:

```
dotnet restore
```

## 4. Contributing

Pull requests are welcome. Feel free to open an issue to discuss what could be improved.

## 5. License

This project is made available under the ***[GNU GPLv3 License](./LICENSE)***.
