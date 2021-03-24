# Warehouse Manager

This project is intended to be used to manage bulk ore stocks using MySql database through  
a web server running on a LAN.  
I'll try to design the application so that it can be used as well as a simple general purpose  
warehouse manager with a few modifications.

This project provides functionalities for a companie that stocks, processes and transports  
bulk ore owned by third parties and must register informations related to the incomings,  
shippings, processing and balance of the products of each client, tax data as well as other  
informations related to the transportation like vehicles and drivers.

## 1. Environment

- **IDE:** Visual Studio Code
- **OS:** Debian 11 (Bullseye)
- **Programming Language:** C#
- **Front-End:** HTML, Bootstrap CSS, Razor Views
- **Framework:** ASP.NET Core 3.1
- **Database:** MySql 8.0
- **Web Server:** NGINX 1.19

## 2. Features

The application should meet the following critirias:

1. Only registered users should access the application and restrictions are applied according to their roles.
2. Register and manipulate the following records:

- Users
- Clients
- Products
- Stocks
- Vehicles
- Drivers
- Incomings
- Shippings
- Product Enhancements

3. Provide a friendly user interface to collect and display informations stored in the DB.
4. Provide well-structured reports of activities and stocks.

**Further improvements:**

- Dockerize the project.
- Design a mobile front-end interface.
- Implement an SPA interface using the React framework.

## 3. Contributing

Pull requests are welcome. Feel free to open an issue to discuss what could be improved.

## 4. License

This project is made available under the ***[GNU GPLv3 License](./LICENSE)***.

