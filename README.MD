# otsom.fs

This repository contains various F# libraries and tools.

## Repository Structure

- `src/`: Contains the source code for all modules.
- `azure-pipelines.template.yml`: A shared Azure Pipelines template used by all modules.
- `Directory.Build.props`: Shared build properties for all projects.

## Pipelining

The project uses Azure Pipelines for Continuous Integration and Continuous Deployment (CI/CD). To reduce redundancy, all project-specific
pipelines extend a central template located at `azure-pipelines.template.yml`.

### Shared Pipeline Template

The template defines a standardized build and deployment process:

1. **Fantomas Check**: Ensures code formatting follows the project standards.
2. **Restore & Build**: Restores dependencies and builds the project in `Release` configuration using .NET 9.x.
3. **Pack**: Creates NuGet packages.
4. **Publish Artifacts**: Uploads the created packages as pipeline artifacts.
5. **Deployment**: On the `main` branch, the packages are automatically pushed to NuGet.org.

### Module Pipelines

Each module in the `src/` directory has its own `azure-pipelines.yml` file which:

- Defines triggers based on changes in its specific directory.
- Extends the shared template by passing its `projectName`.

## Versioning

Versioning is managed using [Nerdbank.GitVersioning](https://github.com/dotnet/Nerdbank.GitVersioning).

### Strategy

- Each module has its own `version.json` file defining its versioning rules.
- Versions are automatically calculated based on the git history.
- The `main` branch is configured for public releases.
- Versioning is path-filtered to ensure that changes to the module's directory (excluding the pipeline file itself) trigger version
  increments when appropriate.
