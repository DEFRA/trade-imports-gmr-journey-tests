FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build

RUN apk add --no-cache aws-cli

WORKDIR /app

COPY . .

RUN dotnet tool restore

RUN --mount=type=secret,id=DEFRA_NUGET_PAT \
  DEFRA_NUGET_PAT="$(cat /run/secrets/DEFRA_NUGET_PAT)" \
  dotnet restore

RUN dotnet csharpier check .

RUN dotnet publish tests/TradeImportsGmr.JourneyTests -c Release -o /app/publish

FROM build AS publish

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT [ "./scripts/entrypoint.sh" ]
