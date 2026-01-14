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

COPY . .

RUN dotnet tool restore
RUN dotnet restore
RUN dotnet csharpier check .

FROM base AS publish

WORKDIR /app

COPY --from=build /app .

ENTRYPOINT [ "./scripts/runner.sh" ]
