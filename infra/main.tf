terraform {
  required_providers {
    kubernetes = {
      source  = "hashicorp/kubernetes"
      version = "~> 2.0"
    }
  }
}

provider "kubernetes" {
  config_path = "/home/vscode/.kube/config"
  host        = "https://host.docker.internal:6443"
  insecure    = true
}

# Secret for DB Credentials
resource "kubernetes_secret" "db_creds" {
  metadata {
    name = "db-creds"
  }
  data = {
    username = local.db_user
    password = local.db_pass
    database = local.db_name
  }
}

# Database Deployment
resource "kubernetes_deployment" "trade_db" {
  metadata {
    name   = "trade-db"
    labels = { app = "trade-db" }
  }
  spec {
    selector {
      match_labels = { app = "trade-db" }
    }
    template {
      metadata {
        labels = { app = "trade-db" }
      }
      spec {
        container {
          image = "postgres:16"
          name  = "postgres"
          env {
            name  = "POSTGRES_USER"
            value = local.db_user
          }
          env {
            name  = "POSTGRES_PASSWORD"
            value = local.db_pass
          }
          env {
            name  = "POSTGRES_DB"
            value = local.db_name
          }
          port {
            container_port = 5432
          }
        }
      }
    }
  }
}

# Database Service (DNS: 'trade-db')
resource "kubernetes_service" "trade_db_svc" {
  metadata {
    name = "trade-db"
  }
  spec {
    # NOTE: In Service, selector is an ATTRIBUTE, so it needs '='
    selector = {
      app = "trade-db"
    }
    port {
      port = 5432
    }
  }
}

# Engine Service (LoadBalancer)
resource "kubernetes_service" "trade_engine_svc" {
  metadata {
    name = "trade-engine-service"
  }
  spec {
    # NOTE: Needs '='
    selector = {
      app = "trade-engine"
    }
    type = "LoadBalancer"
    port {
      port        = 80
      target_port = 8080
    }
  }
}