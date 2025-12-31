resource "kubernetes_deployment" "trade_engine" {
  metadata {
    name   = "trade-engine"
    labels = { app = "trade-engine" }
  }

  spec {
    replicas = 1
    selector {
      match_labels = { app = "trade-engine" }
    }
    template {
      metadata {
        labels = { app = "trade-engine" }
      }
      spec {
        # NOTE:
        # Resource limits and securityContext are intentionally omitted
        # to keep the deployment lightweight for local development.
        # Production workloads should define CPU/memory limits and
        # run as non-root.
        container {
          image             = "trade-engine:v1"
          name              = "engine"
          image_pull_policy = "Never"

          env {
            name  = "ConnectionStrings__DefaultConnection"
            value = "Host=trade-db;Database=${local.db_name};Username=${local.db_user};Password=${local.db_pass}"
          }
          port { container_port = 8080 }
        }
      }
    }
  }
}