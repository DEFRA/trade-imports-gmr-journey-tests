FROM mcr.microsoft.com/dotnet/sdk:10.0 AS base

WORKDIR /app

RUN apt-get update -qq \
    && apt-get install -y -qq --no-install-recommends curl unzip \
    && rm -rf /var/lib/apt/lists/* \
    && curl -sSL "https://awscli.amazonaws.com/awscli-exe-linux-x86_64.zip" -o /tmp/awscliv2.zip \
    && unzip -q /tmp/awscliv2.zip -d /tmp \
    && /tmp/aws/install > /dev/null \
    && rm -rf /tmp/aws /tmp/awscliv2.zip

FROM base AS build

WORKDIR /app

ARG DEFRA_NUGET_PAT
ENV DEFRA_NUGET_PAT=${DEFRA_NUGET_PAT}

COPY TradeImportsGmrJourneyTests.slnx TradeImportsGmrJourneyTests.slnx
COPY .config/ .config/
COPY .editorconfig .editorconfig
COPY .csharpierrc .csharpierrc
COPY .csharpierignore .csharpierignore
COPY NuGet.config NuGet.config

COPY tests/TestFixtures/TestFixtures.csproj tests/TestFixtures/TestFixtures.csproj
COPY tests/TestHelpers/TestHelpers.csproj tests/TestHelpers/TestHelpers.csproj
COPY tests/TradeImportsGmr.JourneyTests/TradeImportsGmr.JourneyTests.csproj tests/TradeImportsGmr.JourneyTests/TradeImportsGmr.JourneyTests.csproj

RUN dotnet tool restore
RUN dotnet restore
RUN dotnet csharpier check .

COPY tests tests

RUN dotnet publish tests/TradeImportsGmr.JourneyTests -c Release -o /app/publish

FROM base AS publish

WORKDIR /app

COPY --from=build /app/publish .
COPY .config .config
COPY scripts scripts

RUN dotnet tool restore

ENTRYPOINT [ "./scripts/runner.sh" ]
