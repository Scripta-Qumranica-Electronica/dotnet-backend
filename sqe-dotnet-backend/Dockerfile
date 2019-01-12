FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["sqe-dotnet-backend/sqe-dotnet-backend.csproj", "sqe-dotnet-backend/"]
RUN dotnet restore "sqe-dotnet-backend/sqe-dotnet-backend.csproj"
COPY . .
WORKDIR "/src/sqe-dotnet-backend"
RUN dotnet build "sqe-dotnet-backend.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "sqe-dotnet-backend.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "sqe-dotnet-backend.dll"]