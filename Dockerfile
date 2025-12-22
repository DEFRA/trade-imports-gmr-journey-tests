FROM mcr.microsoft.com/dotnet/sdk:10.0

WORKDIR /app

COPY . .

RUN dotnet tool restore
RUN dotnet csharpier check .
RUN dotnet restore

ENTRYPOINT [ "dotnet", "test" ]
