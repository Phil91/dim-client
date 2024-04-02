# DIM Middle Layer

This repository contains the backend code for the DIM Middle Layer written in C#.

## Installation

To install the chart with the release name `dim`:

```shell
$ helm repo add dim-repo https://phil91.github.io/dim-client
$ helm install dim dim-repo/dim
```

To install the helm chart into your cluster with your values:

```shell
$ helm install -f your-values.yaml dim dim-repo/dim
```

To use the helm chart as a dependency:

```yaml
dependencies:
  - name: dim
    repository: https://phil91.github.io/dim-client
    version: 0.0.1
```

## How to build and run

Install the [.NET 7.0 SDK](https://www.microsoft.com/net/download).

Run the following command from the CLI:

```console
dotnet build src
```

Make sure the necessary config is added to the settings of the service you want to run.
Run the following command from the CLI in the directory of the service you want to run:

```console
dotnet run
```

## License

Distributed under the Apache 2.0 License.
See [LICENSE](./LICENSE) for more information.
