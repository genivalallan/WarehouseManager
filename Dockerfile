FROM mcr.microsoft.com/dotnet/sdk:3.1 AS development
WORKDIR /app

ARG db_host=database
ARG db_port=3306
ARG db_user=root
ARG db_password
ARG db_name=warehouse

COPY *.csproj ./
RUN dotnet restore

COPY . ./

RUN dotnet user-secrets init
RUN dotnet user-secrets set "MySqlConnectionString" "server=${db_host};port=${db_port};database=${db_name};user=${db_user};password=${db_password}"

RUN dotnet add package MySql.EntityFrameworkCore --version 5.0.0+m8.0.23

CMD [ "dotnet", "run" ]
