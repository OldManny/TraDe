FROM mcr.microsoft.com/devcontainers/dotnet:8.0

# Install only the system tool as root
USER root
RUN apt-get update && apt-get install -y postgresql-client && \
    apt-get clean && rm -rf /var/lib/apt/lists/*

# Switch back to the developer user
USER vscode