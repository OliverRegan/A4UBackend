FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Audit4You.Backend/Audit4You.Backend.csproj", "Audit4You.Backend/"]
RUN dotnet restore "Audit4You.Backend/Audit4You.Backend.csproj"
COPY . .
WORKDIR "/src/Audit4You.Backend"
RUN dotnet build "Audit4You.Backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Audit4You.Backend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Audit4You.Backend.dll"]
