# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the necessary project files
COPY ["Tasker/Tasker.csproj", "Tasker/"]
COPY ["Domain/Domain.csproj", "Domain/"]

# Restore packages for both projects
RUN dotnet restore "Tasker/Tasker.csproj"
RUN dotnet restore "Domain/Domain.csproj"

# Copy the entire solution
COPY . .

# Build the application
WORKDIR "/src/Tasker"
RUN dotnet build "Tasker.csproj" -c Release -o /app/build

# Stage 2: Publish the application
FROM build AS publish
RUN dotnet publish "Tasker.csproj" -c Release -o /app/publish

# Stage 3: Create the final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy the published output from the previous stage
COPY --from=publish /app/publish .

# Set the entry point for the application
ENTRYPOINT ["dotnet", "Tasker.dll"]
