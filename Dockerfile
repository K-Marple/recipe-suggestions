# Dockerfile for deployment
# Use .NET SDK image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all, restore dependencies, build and publish
COPY . .
RUN dotnet restore "recipe-suggestions.csproj"
RUN dotnet publish "recipe-suggestions.csproj" -c Release -o /app/publish

# Use image for container
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Port
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}
EXPOSE 5000

# Run app
ENTRYPOINT ["dotnet", "recipe-suggestions.dll"]