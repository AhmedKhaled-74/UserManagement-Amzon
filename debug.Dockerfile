FROM mcr.microsoft.com/dotnet/sdk:9.0
WORKDIR /src
COPY . .
RUN ls -la && echo "=== Contents of UserManagement.Presentation ===" && ls -la UserManagement.Presentation/
