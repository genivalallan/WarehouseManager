# Warehouse Manager

### ***THIS PROJECT IS UNDER DEVELOPMENT***

This project is intended to be used to manage bulk ore stocks using MySql database through a web server running on a LAN.

This project provides functionalities for a companie that stocks, processes and transports bulk ore owned by third parties and must register informations related to the incomings, shippings, processing and balance of the products of each client, tax data as well as other informations related to the transportation like vehicles and drivers.

## 1. Development Environment

This project is being developed on the following environment:

- **IDE:** Visual Studio Code
- **OS:** Debian 11 (Bullseye)
- **Programming Language:** C#
- **Front-end:** HTML, Bootstrap CSS, Razor Views
- **Back-end Framework:** .NET Core 3.1
- **Database:** MySql 8.0
- **Web Server:** NGINX 1.19

## 2. Features

The application should meet the following critirias:

1. Manage enitites and informations related to storage and transportation of goods in a warehouse.
2. Only registered users should access the application and restrictions are applied according to their roles.
3. Provide a friendly web interface to collect and display informations stored in the DB.
4. Provide a web API to perfome CRUD operations over a LAN.
5. Create well-structured reports of activities and stocks.

**Further improvements:**

- Design a mobile front-end interface.
- Implement an SPA interface using the React framework.

## 3. Configuring and running

You can have the application running either using Docker or directly in Visual Studio Code.  
This project provides configuration files to support debugging using Docker out of the box.  
Following is how to configure and run the application and MySql Server.

### 3.1 Using Docker

Install [*Docker*](https://www.docker.com) and *docker-compose* if you haven't yet.  
You need the *Docker* extension `ms-azuretools.vscode-docker` as well.

#### 3.1.1 Running the application only

1. Open the Debugger panel.
2. Select `Docker .NET Core Launch` configuration.
3. Press `F5` to run **with** debugger or `Ctrl + F5` to run **without** debugger.

#### 3.1.2 Running the application and MySql server

You can run the application together with MySql server using *docker-compose*.  
Since Visual Studio Code doesn't support launching Compose files, you need to build the project manually.

1. Run `dotnet build` command in the project folder.
2. Rename the `.env-example` file int the project directory to `.env` and and set the `DB_PASSWORD` in the file.
3. In the Explorer panel right-click the `docker-compose.debug.yml` file and select the `Compose Up` menu.  
  Wait until Docker build the image and MySql and kestrel servers are running. The process may take some time.

If you want to run the debugger

1. Open the Debugger panel.
2. Select `Docker .NET Core Attach (Preview)` configuration and press `F5` to run the debugger.

### 3.2 Running the application in Visual Studio Code

1. Open the Debugger panel.
2. Select `.NET Core Launch` configuration.
3. Press `F5` to run **with** debugger or `Ctrl + F5` to run **without** debugger.

## 4. Contributing

Pull requests are welcome.  
Feel free to open an issue to discuss what could be improved.  
If you have any problem trying to run the application, explain step by step how to reproduce the error you are facing.

## 5. License

This project is made available under the ***[GNU GPLv3 License](./LICENSE)***.
