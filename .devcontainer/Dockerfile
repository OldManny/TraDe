FROM mcr.microsoft.com/devcontainers/dotnet:8.0

USER root

# 1. Install prerequisites
RUN apt-get update && apt-get install -y \
    postgresql-client \
    gnupg \
    software-properties-common \
    curl \
    lsb-release \
    wget

# 2. Add HashiCorp GPG Key and Repo
RUN wget -O- https://apt.releases.hashicorp.com/gpg | gpg --dearmor -o /usr/share/keyrings/hashicorp-archive-keyring.gpg && \
    echo "deb [signed-by=/usr/share/keyrings/hashicorp-archive-keyring.gpg] https://apt.releases.hashicorp.com $(lsb_release -cs) main" | tee /etc/apt/sources.list.d/hashicorp.list

# 3. Install Terraform and kubectl
RUN apt-get update && apt-get install -y terraform

RUN curl -LO "https://dl.k8s.io/release/$(curl -L -s https://dl.k8s.io/release/stable.txt)/bin/linux/arm64/kubectl" && \
    install -o root -g root -m 0755 kubectl /usr/local/bin/kubectl

# Node.js 20
RUN curl -fsSL https://deb.nodesource.com/setup_20.x | bash - && \
    apt-get install -y nodejs

# 4. Clean up
RUN apt-get clean && rm -rf /var/lib/apt/lists/*

USER vscode
RUN dotnet tool restore