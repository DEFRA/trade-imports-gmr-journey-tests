FROM mcr.microsoft.com/dotnet/sdk:10.0

WORKDIR /app

COPY . .

RUN apt-get update -qq \
    && apt-get install -y -qq --no-install-recommends curl unzip \
    && rm -rf /var/lib/apt/lists/* \
    && curl -sSL "https://awscli.amazonaws.com/awscli-exe-linux-x86_64.zip" -o /tmp/awscliv2.zip \
    && unzip -q /tmp/awscliv2.zip -d /tmp \
    && /tmp/aws/install > /dev/null \
    && rm -rf /tmp/aws /tmp/awscliv2.zip

RUN dotnet tool restore
RUN dotnet csharpier check .
RUN dotnet restore
ENTRYPOINT [ "./scripts/runner.sh" ]
