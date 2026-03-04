FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy và restore
COPY Compiler/*.csproj ./Compiler/
RUN dotnet restore Compiler/compiler.csproj

# Copy và build
COPY . .
RUN dotnet build -c Release

# Run
ENTRYPOINT ["dotnet", "run", "--project", "Compiler/compiler.csproj"]
