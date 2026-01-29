# trade-imports-gmr-journey-tests

Journey tests for Trade Imports GMR services.

- [trade-imports-gmr-finder](https://github.com/DEFRA/trade-imports-gmr-finder)
- [trade-imports-gmr-processor](https://github.com/DEFRA/trade-imports-gmr-processor)

## Prerequisites

### Dependencies

Install the following:
- [.NET 10 (SDK)](https://dotnet.microsoft.com/)
- [Docker](https://docs.docker.com/engine/) (optional)

### Environment variables

Create `.env` file in the root of the project and provide necessary secrets (copy `.env.example`).

### Services

See [trade-imports-gmr-local-env](https://github.com/DEFRA/trade-imports-gmr-local-env) for instructions.

## Tests

### Local

Build as follows:

```bash
dotnet build
```

Run as follows:

```bash
dotnet test
```

Run the tests and generate a report as follows:

```bash
./scripts/runner.sh
```

The HTML report is written to `test-report/index.html`.

### Docker

Build as follows:

```bash
docker build . -t trade-imports-gmr-journey-tests
```

Run as follows:

```bash
docker run -it --rm trade-imports-gmr-journey-tests
```

## Linting and formatting

[CSharpier](https://csharpier.com/) is used for linting and formatting.

Install .NET local tools as follows:

```bash
dotnet tool restore
```

Format all project files as follows:

```bash
dotnet csharpier format .
```

## Licence

THIS INFORMATION IS LICENSED UNDER THE CONDITIONS OF THE OPEN GOVERNMENT LICENCE found at:

<http://www.nationalarchives.gov.uk/doc/open-government-licence/version/3>

The following attribution statement MUST be cited in your products and applications when using this information.

> Contains public sector information licensed under the Open Government licence v3

### About the licence

The Open Government Licence (OGL) was developed by the Controller of Her Majesty's Stationery Office (HMSO) to enable
information providers in the public sector to license the use and re-use of their information under a common open
licence.

It is designed to encourage use and re-use of information freely and flexibly, with only a few conditions.
