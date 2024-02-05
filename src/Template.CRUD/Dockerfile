#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Template.Api/Template.Api.csproj", "Template.Api/"]
RUN dotnet restore "Template.Api/Template.Api.csproj"
COPY . .
WORKDIR "/src/Template.Api"
RUN dotnet build "Template.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Template.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
ARG TEMPLATE_VERSION="v1.0"
ENV TEMPLATE_VERSION=${TEMPLATE_VERSION}
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Template.Api.dll"]